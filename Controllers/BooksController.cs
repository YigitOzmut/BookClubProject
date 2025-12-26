using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Controllers
{
    [Route("books")]
    public class BooksController : Controller
    {
        private readonly BookClubContext _db;

        public BooksController(BookClubContext db)
        {
            _db = db;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search = null, int? genreId = null, string? sortBy = "title")
        {
            var query = _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(b => b.Title.Contains(search) ||
                    b.BookAuthors.Any(ba => ba.Author.Name.Contains(search)));
            }

            if (genreId.HasValue)
            {
                query = query.Where(b => b.GenreId == genreId.Value);
            }

            var books = await query.ToListAsync();
            foreach (var book in books)
            {
                if (book.Reviews.Count > 0)
                {
                    book.AverageRating = book.Reviews.Average(r => r.Rating);
                }
            }

            books = sortBy switch
            {
                "rating" => books.OrderByDescending(b => b.AverageRating).ToList(),
                "year" => books.OrderByDescending(b => b.PublicationYear).ToList(),
                "newest" => books.OrderByDescending(b => b.BookId).ToList(),
                _ => books.OrderBy(b => b.Title).ToList()
            };

            ViewBag.Genres = await _db.Genres.ToListAsync();
            ViewBag.Search = search;
            ViewBag.SelectedGenre = genreId;
            ViewBag.SortBy = sortBy;

            return View(books);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var book = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews).ThenInclude(r => r.Member)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();

            if (book.Reviews.Count > 0)
            {
                book.AverageRating = book.Reviews.Average(r => r.Rating);
            }

            ViewBag.Members = await _db.Members.ToListAsync();
            return View(book);
        }

        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "GenreId", "Name");
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "GenreId", "Name", book.GenreId);
                return View(book);
            }
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = book.BookId });
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();
            ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "GenreId", "Name", book.GenreId);
            return View(book);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book updated)
        {
            if (id != updated.BookId) return BadRequest();

            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            book.Title = updated.Title;
            book.PublicationYear = updated.PublicationYear;
            book.PageCount = updated.PageCount;
            book.ISBN = updated.ISBN;
            book.GenreId = updated.GenreId;
            book.Description = updated.Description;
            book.CoverImageUrl = updated.CoverImageUrl;
            book.IsAvailable = updated.IsAvailable;

            if (!ModelState.IsValid)
            {
                ViewBag.Genres = new SelectList(await _db.Genres.ToListAsync(), "GenreId", "Name", book.GenreId);
                return View(updated);
            }

            _db.Entry(book).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = book.BookId });
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.Include(b => b.Genre).FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost("delete/{id:int}")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _db.Books.AsNoTracking()
                .Include(b => b.Reviews)
                .Include(b => b.BookMeetings)
                .Include(b => b.BookAuthors)
                .FirstOrDefaultAsync(b => b.BookId == id);

            if (book == null) return NotFound();

            var bookAuthorsToDelete = await _db.BookAuthors
                .Where(ba => ba.BookId == id)
                .ToListAsync();

            if (bookAuthorsToDelete.Any())
            {
                _db.BookAuthors.RemoveRange(bookAuthorsToDelete);
            }

            var reviewsToDelete = await _db.Reviews
                .Where(r => r.BookId == id)
                .ToListAsync();

            if (reviewsToDelete.Any())
            {
                _db.Reviews.RemoveRange(reviewsToDelete);
            }

            var bookMeetingsToDelete = await _db.BookMeetings
                .Where(bm => bm.BookId == id)
                .ToListAsync();

            if (bookMeetingsToDelete.Any())
            {
                _db.BookMeetings.RemoveRange(bookMeetingsToDelete);
            }

            await _db.SaveChangesAsync();

            var bookToDelete = await _db.Books.FindAsync(id);
            if (bookToDelete != null)
            {
                _db.Books.Remove(bookToDelete);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}