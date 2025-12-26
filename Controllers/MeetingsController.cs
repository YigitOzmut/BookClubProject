using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Controllers
{
    [Route("meetings")]
    public class MeetingsController : Controller
    {
        private readonly BookClubContext _db;

        public MeetingsController(BookClubContext db) => _db = db;

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var meetings = await _db.Meetings
                .Include(m => m.BookMeetings).ThenInclude(bm => bm.Book)
                .Include(m => m.MemberMeetings).ThenInclude(mm => mm.Member)
                .OrderByDescending(m => m.Date)
                .ToListAsync();

            return View(meetings);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var meeting = await _db.Meetings
                .Include(m => m.BookMeetings)
                .ThenInclude(bm => bm.Book)   // ← THIS was missing
                .Include(m => m.MemberMeetings)
                .ThenInclude(mm => mm.Member)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null)
                return NotFound();

            return View(meeting);
        }


        [HttpGet("create")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Members = await _db.Members.Where(m => m.IsActive).ToListAsync();
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Meeting meeting, int[]? bookIds, int[]? memberIds)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Books = await _db.Books.ToListAsync();
                ViewBag.Members = await _db.Members.Where(m => m.IsActive).ToListAsync();
                return View(meeting);
            }

            _db.Meetings.Add(meeting);
            await _db.SaveChangesAsync();

            if (bookIds != null && bookIds.Length > 0)
            {
                var bookMeetings = bookIds.Select(bid => new BookMeeting
                {
                    MeetingId = meeting.MeetingId,
                    BookId = bid
                }).ToList();
                _db.AddRange(bookMeetings);
            }

            if (memberIds != null && memberIds.Length > 0)
            {
                var memberMeetings = memberIds.Select(mid => new MemberMeeting
                {
                    MeetingId = meeting.MeetingId,
                    MemberId = mid
                }).ToList();
                _db.AddRange(memberMeetings);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = meeting.MeetingId });
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var meeting = await _db.Meetings
                .Include(m => m.BookMeetings)
                .Include(m => m.MemberMeetings)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null) return NotFound();

            ViewBag.Books = await _db.Books.ToListAsync();
            ViewBag.Members = await _db.Members.Where(m => m.IsActive).ToListAsync();
            ViewBag.SelectedBooks = meeting.BookMeetings.Select(bm => bm.BookId).ToArray();
            ViewBag.SelectedMembers = meeting.MemberMeetings.Select(mm => mm.MemberId).ToArray();

            return View(meeting);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Meeting updated, int[]? bookIds, int[]? memberIds)
        {
            var meeting = await _db.Meetings
                .Include(m => m.BookMeetings)
                .Include(m => m.MemberMeetings)
                .FirstOrDefaultAsync(m => m.MeetingId == id);

            if (meeting == null) return NotFound();

            meeting.Date = updated.Date;
            meeting.Location = updated.Location;
            meeting.Notes = updated.Notes;

            if (!ModelState.IsValid)
            {
                ViewBag.Books = await _db.Books.ToListAsync();
                ViewBag.Members = await _db.Members.Where(m => m.IsActive).ToListAsync();
                return View(updated);
            }

            _db.RemoveRange(meeting.BookMeetings);
            if (bookIds != null && bookIds.Length > 0)
            {
                var bookMeetings = bookIds.Select(bid => new BookMeeting
                {
                    MeetingId = meeting.MeetingId,
                    BookId = bid
                }).ToList();
                _db.AddRange(bookMeetings);
            }

            _db.RemoveRange(meeting.MemberMeetings);
            if (memberIds != null && memberIds.Length > 0)
            {
                var memberMeetings = memberIds.Select(mid => new MemberMeeting
                {
                    MeetingId = meeting.MeetingId,
                    MemberId = mid
                }).ToList();
                _db.AddRange(memberMeetings);
            }

            _db.Meetings.Update(meeting);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = meeting.MeetingId });
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var meeting = await _db.Meetings
                .Include(m => m.BookMeetings)
                .Include(m => m.MemberMeetings)
                .FirstOrDefaultAsync(m => m.MeetingId == id);
            if (meeting == null) return NotFound();
            return View("Delete", meeting);
        }

        [HttpPost("delete/{id:int}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM BookMeeting WHERE Meeting_ID = {0}", id);
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM MemberMeeting WHERE Meeting_ID = {0}", id);
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Meeting WHERE Meeting_ID = {0}", id);

            return RedirectToAction(nameof(Index));
        }
    }
}