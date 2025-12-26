using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Controllers;

[Route("reviews")]
public class ReviewsController : Controller
{
    private readonly BookClubContext _db;

    public ReviewsController(BookClubContext db) => _db = db;

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Review review)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        review.DatePosted = DateTime.Now;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Books", new { id = review.BookId });
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id)
    {
        var review = await _db.Reviews.Include(r => r.Book).Include(r => r.Member).FirstOrDefaultAsync(r => r.ReviewId == id);
        if (review == null) return NotFound();
        return View(review);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Review updated)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        review.Rating = updated.Rating;
        review.Comment = updated.Comment;

        if (!ModelState.IsValid)
            return View(updated);

        _db.Reviews.Update(review);
        await _db.SaveChangesAsync();
        return RedirectToAction("Details", "Books", new { id = review.BookId });
    }

    [HttpGet("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _db.Reviews.Include(r => r.Book).FirstOrDefaultAsync(r => r.ReviewId == id);
        if (review == null) return NotFound();
        return View(review);
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var review = await _db.Reviews.FindAsync(id);
        if (review == null) return NotFound();

        var bookId = review.BookId;
        _db.Reviews.Remove(review);
        await _db.SaveChangesAsync();
        return RedirectToAction("Details", "Books", new { id = bookId });
    }
}
