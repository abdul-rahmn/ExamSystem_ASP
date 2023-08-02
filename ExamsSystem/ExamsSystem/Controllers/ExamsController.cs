using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamsSystem.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ExamsSystem.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ExamsSystemContext _context;
        // To Get Access To Current User
        private readonly IHttpContextAccessor httpContextAccessor;

        public ExamsController(ExamsSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            // Get Id Current User
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.IsInRole("Admin") || User.IsInRole("Student"))
            {
                ViewBag.idu = userId;
                var examsSystemContext = _context.Exams.Include(e => e.Course).Include(e => e.Course.User);
                return View(await examsSystemContext.ToListAsync());
            }
            else
            {
                var examsSystemContext = _context.Exams.Include(e => e.Course).Where(e => e.Course.UserId == userId);
                ViewBag.uid = userId;
                return View(await examsSystemContext.ToListAsync());
            }
        }

        // Get All Students Complete the Exam And thier Marks 
        // For The Teacher 
        public async Task<IActionResult> ExamResults(int? id)
        {
            List<Result> results = await _context.Results.Where(r => r.CourseId == id).Include(r => r.Course).Include(r => r.User).ToListAsync();
            Course course = await _context.Courses.FirstAsync(c => c.Id == id);
            ViewBag.coursee = course.CourseName;
            return View(results);

        }

        // Get marks of Exams 
        // For The Student
        public async Task<IActionResult> StudentResults()
        {
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<Result> results = await _context.Results.Include(r => r.Course).Include(r => r.User).Where(r => r.UserId == userId).ToListAsync();
            return View(results);

        }

        // Get Review of Exam  
        // For The Student
        public async Task<IActionResult> ExamDetails(int? id)
        {
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            //List<Result> results = await _context.Results.Include(r => r.Course).Include(r => r.User).Where(r => r.UserId == userId).ToListAsync();
            //return View(results);
            Result results = await _context.Results.Include(r => r.Course).Include(r => r.User).SingleOrDefaultAsync(r => r.Id == id);
            List<UserAnswer> userAnswers = await _context.UserAnswers.Include(ua => ua.Question).Include(ua => ua.User).Where(ua => ua.Question.CourseId == results.CourseId && ua.UserId == userId).ToListAsync();
            ViewBag.resu = results;
            return View(userAnswers);
        }

        // Search About Exams
        // For All Users
        public async Task<IActionResult> Search(IFormCollection form)
        {
            if (form["search"].ToString() == "" || form["search"].ToString() == null)
            {
                return RedirectToAction("Index");
            }
            string srch = form["search"].ToString();
            //string sdsd = "sadsadsa";
            //bool sss = sdsd.Contains(srch,, RegexOptions.);
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (User.IsInRole("Admin") || User.IsInRole("Student"))
            {
                ViewBag.idu = userId;
                var examsSystemContext = _context.Exams.Include(e => e.Course).Include(e => e.Course.User).Where(e => e.Course.CourseName.Contains(srch));
                return View(await examsSystemContext.ToListAsync());
            }
            else
            {
                var examsSystemContext = _context.Exams.Include(e => e.Course).Where(e => (e.Course.UserId == userId) && (e.Course.CourseName.Contains(srch)));
                ViewBag.uid = userId;
                return View(await examsSystemContext.ToListAsync());
            }
        }

        // Ensure The NO Is True to Enter To Exam If not Complete it 
        // Or View the Marks If Complete it
        // For The Students
        public async Task<IActionResult> ConfirmNo(IFormCollection form)
        {
            if (form["no"].ToString() == null || form["no"].ToString() == "")
            {
                return RedirectToAction("Index");
            }
            List<Exam> exams = await _context.Exams.ToListAsync();
            bool noIsTrue = false;
            foreach (var item in exams)
            {
                if (item.RegNo == Convert.ToInt32(form["no"].ToString()) && item.CourseId == Convert.ToInt32(form["courseId"].ToString()))
                {
                    noIsTrue = true;
                    break;
                }
            }
            if (noIsTrue)
            {
                DateTime t = Convert.ToDateTime(form["Date"]);
                if (t > DateTime.Now)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Test", "Questions", new { id = Convert.ToInt32(form["courseId"].ToString()) });
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Exams == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exams/Create
        public IActionResult Create()
        {
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            ViewData["CourseId"] = new SelectList(_context.Courses.Where(c => c.UserId == userId), "Id", "CourseName");
            return View();
        }

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CourseId,Date,RegNo")] Exam exam)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exam);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", exam.CourseId);
            return View(exam);
        }

        // GET: Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Exams == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", exam.CourseId);
            return View(exam);
        }

        // POST: Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CourseId,Date,RegNo")] Exam exam)
        {
            if (id != exam.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", exam.CourseId);
            return View(exam);
        }

        // GET: Exams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Exams == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Exams == null)
            {
                return Problem("Entity set 'ExamsSystemContext.Exams'  is null.");
            }
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.Id == id);
        }
    }
}
