using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;

namespace BookClub.WebApi.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly BookClubContext _db;
    public HomeController(BookClubContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var books = await _db.Books
            .Include(b => b.Genre)
            .Include(b => b.BookAuthors).ThenInclude(ba => ba.Author)
            .Include(b => b.Reviews)
            .ToListAsync();

        // Calculate average rating for each book
        foreach (var book in books)
        {
            if (book.Reviews.Count > 0)
            {
                book.AverageRating = book.Reviews.Average(r => r.Rating);
            }
        }

        books = books.OrderByDescending(b => b.AverageRating).ToList();
        ViewBag.FeaturedBooks = books;

        // Dashboard statistics
        var stats = new Dictionary<string, int>
        {
            { "TotalBooks", await _db.Books.CountAsync() },
            { "TotalMembers", await _db.Members.CountAsync(m => m.IsActive) },
            { "TotalMeetings", await _db.Meetings.CountAsync() },
            { "TotalReviews", await _db.Reviews.CountAsync() }
        };

        ViewBag.Statistics = stats;
        return View(books);
    }

    [HttpGet("book/{id}")]
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

        return View("Details", book);
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard()
    {
        var topBooks = await _db.Books
            .Include(b => b.Genre)
            .Include(b => b.Reviews)
            .ToListAsync();
        foreach (var book in topBooks)
        {
            if (book.Reviews.Count > 0)
                book.AverageRating = book.Reviews.Average(r => r.Rating);
        }
        topBooks = topBooks.OrderByDescending(b => b.AverageRating).Take(5).ToList();

        var stats = new Dictionary<string, object>
        {
            { "TotalBooks", await _db.Books.CountAsync() },
            { "TotalMembers", await _db.Members.CountAsync(m => m.IsActive) },
            { "TotalMeetings", await _db.Meetings.CountAsync() },
            { "TotalReviews", await _db.Reviews.CountAsync() },
            { "TopRatedBooks", topBooks },
            { "ActiveMembers", await _db.Members
                .Where(m => m.IsActive)
                .OrderByDescending(m => m.Reviews.Count)
                .Take(5)
                .ToListAsync() }
        };

        ViewBag.Statistics = stats;
        return View();
    }
}
