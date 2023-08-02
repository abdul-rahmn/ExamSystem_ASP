using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace ExamsSystem.Controllers
{
    [Authorize(Roles = "Admin,Teacher")]
    public class CoursesController : Controller
    {
        private readonly ExamsSystemContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        // To Get Access To Current User
        private readonly IHttpContextAccessor httpContextAccessor;

        // Constructor
        public CoursesController(ExamsSystemContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            // Get Id Current User
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.IsInRole("Admin"))
            {
                var courses = _context.Courses.Include(u => u.User);
                ViewBag.us = userId;
                return View(courses);
            }
            else
            {
                var courses = _context.Courses.Where(c => c.UserId == userId).Include(u => u.User);
                ViewBag.us = userId;
                return View(courses);
            }

        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            Response.HttpContext.Session.SetString("CourseId", id.ToString());
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }
            List<Question> questions = await _context.Questions.Where(q => q.CourseId == id).ToListAsync();
            ViewBag.Qs = questions;
            AspNetUser user = await _context.AspNetUsers.SingleAsync(u => u.Id == course.UserId);
            ViewBag.un = user;
            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseName")] Course course)
        {
            if (ModelState.IsValid)
            {
                string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                course.UserId = userId;
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Edit/5        
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,CourseName")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CourseExists(course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Courses == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Courses == null)
            {
                return Problem("Entity set 'ExamsSystemContext.Courses'  is null.");
            }
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                List<Question> questions = await _context.Questions.Where(q => q.CourseId == id).ToListAsync();
                List<Exam> exams = await _context.Exams.Where(e => e.CourseId == id).ToListAsync();
                List<Result> results = await _context.Results.Where(r => r.CourseId == id).ToListAsync();
                foreach (var item in questions)
                {
                    List<UserAnswer> userAnswers = await _context.UserAnswers.Where(e => e.QuestionId == item.Id).ToListAsync();
                    foreach (var i in userAnswers)
                    {
                        _context.UserAnswers.Remove(i);
                    }
                }
                foreach (var re in results)
                {
                    _context.Results.Remove(re);
                }
                foreach (var e in exams)
                {
                    _context.Exams.Remove(e);
                    await _context.SaveChangesAsync();
                }
                foreach (var q in questions)
                {
                    _context.Questions.Remove(q);
                    await _context.SaveChangesAsync();
                }
                _context.Courses.Remove(course);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CourseExists(int id)
        {
            return _context.Courses.Any(e => e.Id == id);
        }
    }
}
