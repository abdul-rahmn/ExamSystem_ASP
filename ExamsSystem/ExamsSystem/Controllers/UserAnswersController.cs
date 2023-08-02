using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamsSystem.Models;

namespace ExamsSystem.Controllers
{
    public class UserAnswersController : Controller
    {
        private readonly ExamsSystemContext _context;

        public UserAnswersController(ExamsSystemContext context)
        {
            _context = context;
        }

        // GET: UserAnswers
        public async Task<IActionResult> Index()
        {
            var examsSystemContext = _context.UserAnswers.Include(u => u.Question).Include(u => u.User);
            return View(await examsSystemContext.ToListAsync());
        }

        // GET: UserAnswers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.UserAnswers == null)
            {
                return NotFound();
            }

            var userAnswer = await _context.UserAnswers
                .Include(u => u.Question)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAnswer == null)
            {
                return NotFound();
            }

            return View(userAnswer);
        }

        // GET: UserAnswers/Create
        public IActionResult Create()
        {
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: UserAnswers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionId,UserId,UserAnswer1,StateAnswer")] UserAnswer userAnswer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userAnswer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", userAnswer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userAnswer.UserId);
            return View(userAnswer);
        }

        // GET: UserAnswers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.UserAnswers == null)
            {
                return NotFound();
            }

            var userAnswer = await _context.UserAnswers.FindAsync(id);
            if (userAnswer == null)
            {
                return NotFound();
            }
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", userAnswer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userAnswer.UserId);
            return View(userAnswer);
        }

        // POST: UserAnswers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionId,UserId,UserAnswer1,StateAnswer")] UserAnswer userAnswer)
        {
            if (id != userAnswer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userAnswer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserAnswerExists(userAnswer.Id))
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
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Id", userAnswer.QuestionId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", userAnswer.UserId);
            return View(userAnswer);
        }

        // GET: UserAnswers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.UserAnswers == null)
            {
                return NotFound();
            }

            var userAnswer = await _context.UserAnswers
                .Include(u => u.Question)
                .Include(u => u.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (userAnswer == null)
            {
                return NotFound();
            }

            return View(userAnswer);
        }

        // POST: UserAnswers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.UserAnswers == null)
            {
                return Problem("Entity set 'ExamsSystemContext.UserAnswers'  is null.");
            }
            var userAnswer = await _context.UserAnswers.FindAsync(id);
            if (userAnswer != null)
            {
                _context.UserAnswers.Remove(userAnswer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserAnswerExists(int id)
        {
            return _context.UserAnswers.Any(e => e.Id == id);
        }
    }
}
