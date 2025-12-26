using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookClub.WebApi.Data;
using BookClub.WebApi.Models;

namespace BookClub.WebApi.Controllers
{
    [Route("members")]
    public class MembersController : Controller
    {
        private readonly BookClubContext _db;

        public MembersController(BookClubContext db) => _db = db;

        [HttpGet("")]
        public async Task<IActionResult> Index(string? search = null, string? role = null)
        {
            var query = _db.Members.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m => m.Name.Contains(search) || m.Email.Contains(search));
            }

            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(m => m.Role == role);
            }

            var members = await query
                .Include(m => m.Reviews)
                .Include(m => m.MemberMeetings)
                .OrderByDescending(m => m.JoinDate)
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.SelectedRole = role;
            ViewBag.Roles = new[] { "Member", "Moderator", "Admin" };

            return View(members);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Details(int id)
        {
            var member = await _db.Members
                .Include(m => m.Reviews).ThenInclude(r => r.Book)
                .Include(m => m.MemberMeetings).ThenInclude(mm => mm.Meeting)
                .FirstOrDefaultAsync(m => m.MemberId == id);

            if (member == null) return NotFound();
            return View(member);
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Member member)
        {
            member.JoinDate = DateTime.Now;

            if (!ModelState.IsValid)
                return View(member);

            _db.Members.Add(member);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = member.MemberId });
        }

        [HttpGet("edit/{id:int}")]
        public async Task<IActionResult> Edit(int id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();
            return View(member);
        }

        [HttpPost("edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Member updated)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();

            member.Name = updated.Name;
            member.Email = updated.Email;
            member.Phone = updated.Phone;
            member.Role = updated.Role;
            member.Bio = updated.Bio;
            member.IsActive = updated.IsActive;

            if (!ModelState.IsValid)
                return View(updated);

            _db.Members.Update(member);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = member.MemberId });
        }

        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null) return NotFound();
            return View("Delete", member);
        }

        [HttpPost("delete/{id:int}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Review WHERE Member_ID = {0}", id);
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM MemberMeeting WHERE Member_ID = {0}", id);
            await _db.Database.ExecuteSqlRawAsync("DELETE FROM Member WHERE Member_ID = {0}", id);

            return RedirectToAction(nameof(Index));
        }
    }
}