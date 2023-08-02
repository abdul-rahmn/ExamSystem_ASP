using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamsSystem.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ExamsSystem.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly ExamsSystemContext _context;
        // To Get Access To Current User
        private readonly IHttpContextAccessor httpContextAccessor;

        public QuestionsController(ExamsSystemContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            this.httpContextAccessor = httpContextAccessor;
        }

        // GET: Page Of The Exam Quetions
        public async Task<IActionResult> Test(int? id)
        {
            List<Result> results = await _context.Results.ToListAsync();
            string userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            foreach (var item in results)
            {
                if (item.UserId == userId && item.CourseId == id)
                {
                    return RedirectToAction("StudentResults", "Exams");
                }
            }
            Response.HttpContext.Session.SetString("cId", id.ToString());
            var examsSystemContext = _context.Questions.Include(q => q.Course);
            var questions = examsSystemContext.Where(q => q.CourseId == id).ToList();
            return View(questions);
        }

        // POST: Page Of The Exam Quetions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Test(IFormCollection form)
        {
            int id = Convert.ToInt32(Response.HttpContext.Session.GetString("cId"));

            List<Question> questions = _context.Questions.Where(q => q.CourseId == id).ToList();

            var userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;

            int countTrue = 0;
            int countFalse = 0;
            double Average = 0.0;
            foreach (var item in questions)
            {
                if (form[$"{item.Id}"].ToString() == item.Answer)
                {
                    countTrue++;
                }
                else
                {
                    countFalse++;
                }
                UserAnswer ua = new UserAnswer()
                {
                    QuestionId = item.Id,
                    UserId = userId,
                    UserAnswer1 = form[$"{item.Id}"].ToString(),
                    StateAnswer = (form[$"{item.Id}"].ToString() == item.Answer)
                };
                await _context.UserAnswers.AddAsync(ua);
                await _context.SaveChangesAsync();
            }
            Average = ((double)countTrue / (double)questions.Count()) * 100;
            Result result = new Result()
            {
                CountTrue = countTrue,
                CountFalse = countFalse,
                Average = Average,
                UserId = userId,
                CourseId = id
            };
            await _context.Results.AddAsync(result);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Exams");
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var examsSystemContext = _context.Questions.Include(q => q.Course);
            return View(await examsSystemContext.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            int courseId = Convert.ToInt32(Response.HttpContext.Session.GetString("CourseId"));
            ViewBag.cId = courseId;
            //ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName");
            return View();
        }

        // POST: Questions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Question1,Option1,Option2,Option3,Option4,Answer")] Question question)
        {
            int courseId = Convert.ToInt32(Response.HttpContext.Session.GetString("CourseId"));
            question.CourseId = courseId;
            if (ModelState.IsValid)
            {
                _context.Add(question);
                await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Details", "Courses", new { id = courseId });
            }
            //ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", question.CourseId);
            return View(question);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions.FindAsync(id);
            if (question == null)
            {
                return NotFound();
            }
            //ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", question.CourseId);
            return View(question);
        }

        // POST: Questions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Question1,Option1,Option2,Option3,Option4,Answer")] Question question)
        {
            int courseId = Convert.ToInt32(Response.HttpContext.Session.GetString("CourseId"));
            question.CourseId = courseId;
            if (id != question.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(question);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionExists(question.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Courses", new { id = courseId });
            }
            //ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "CourseName", question.CourseId);
            return View(question);
        }


        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Questions == null)
            {
                return NotFound();
            }

            var question = await _context.Questions
                .Include(q => q.Course)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            int courseId = Convert.ToInt32(Response.HttpContext.Session.GetString("CourseId"));
            if (_context.Questions == null)
            {
                return Problem("Entity set 'ExamsSystemContext.Questions'  is null.");
            }
            List<UserAnswer> userAnswers = await _context.UserAnswers.Where(r => r.QuestionId == id).ToListAsync();
            foreach (var re in userAnswers)
            {
                _context.UserAnswers.Remove(re);
            }
            var question = await _context.Questions.FindAsync(id);
            if (question != null)
            {
                _context.Questions.Remove(question);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        private bool QuestionExists(int id)
        {
            return _context.Questions.Any(e => e.Id == id);
        }
    }
}
