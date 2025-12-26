using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksApiController : ControllerBase
    {
        private readonly BookClubContext _db;
        public BooksApiController(BookClubContext db) => _db = db;

        [HttpGet]
        public async Task<IActionResult> GetAll(string? search = null, int? genreId = null, string? sortBy = "title")
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

            // Calculate ratings
            foreach (var book in books)
            {
                if (book.Reviews.Count > 0)
                {
                    book.AverageRating = book.Reviews.Average(r => r.Rating);
                }
            }

            // Sort
            books = sortBy switch
            {
                "rating" => books.OrderByDescending(b => b.AverageRating).ToList(),
                "year" => books.OrderByDescending(b => b.PublicationYear).ToList(),
                _ => books.OrderBy(b => b.Title).ToList()
            };

            return Ok(books);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var book = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.Reviews)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.BookId == id);
            if (book == null) return NotFound();

            if (book.Reviews.Count > 0)
            {
                book.AverageRating = book.Reviews.Average(r => r.Rating);
            }

            return Ok(book);
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRated(int? count = 10)
        {
            var books = await _db.Books
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .ToListAsync();

            foreach (var book in books)
            {
                if (book.Reviews.Count > 0)
                {
                    book.AverageRating = book.Reviews.Average(r => r.Rating);
                }
            }

            return Ok(books.OrderByDescending(b => b.AverageRating).Take(count ?? 10));
        }

        [HttpGet("by-genre/{genreId}")]
        public async Task<IActionResult> GetByGenre(int genreId)
        {
            var books = await _db.Books
                .Where(b => b.GenreId == genreId)
                .Include(b => b.Genre)
                .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
                .Include(b => b.Reviews)
                .ToListAsync();

            foreach (var book in books)
            {
                if (book.Reviews.Count > 0)
                {
                    book.AverageRating = book.Reviews.Average(r => r.Rating);
                }
            }

            return Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Book book)
        {
            _db.Books.Add(book);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = book.BookId }, book);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Book updated)
        {
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

            _db.Books.Update(book);
            await _db.SaveChangesAsync();
            return Ok(book);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null) return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
