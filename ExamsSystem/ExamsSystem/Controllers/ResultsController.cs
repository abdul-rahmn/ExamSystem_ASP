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
    public class ResultsController : Controller
    {
        private readonly ExamsSystemContext _context;

        public ResultsController(ExamsSystemContext context)
        {
            _context = context;
        }

        // GET: Results
        public async Task<IActionResult> Index()
        {
            var examsSystemContext = _context.Results.Include(r => r.Course).Include(r => r.User);
            return View(await examsSystemContext.ToListAsync());
        }

        // GET: Results/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Results == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Course)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // GET: Results/Create
        public IActionResult Create()
        {
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id");
            return View();
        }

        // POST: Results/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CountTrue,CountFalse,Average,UserId,CourseId")] Result result)
        {
            if (ModelState.IsValid)
            {
                _context.Add(result);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", result.CourseId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", result.UserId);
            return View(result);
        }

        // GET: Results/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Results == null)
            {
                return NotFound();
            }

            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", result.CourseId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", result.UserId);
            return View(result);
        }

        // POST: Results/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CountTrue,CountFalse,Average,UserId,CourseId")] Result result)
        {
            if (id != result.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(result);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResultExists(result.Id))
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
            ViewData["CourseId"] = new SelectList(_context.Courses, "Id", "Id", result.CourseId);
            ViewData["UserId"] = new SelectList(_context.AspNetUsers, "Id", "Id", result.UserId);
            return View(result);
        }

        // GET: Results/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Results == null)
            {
                return NotFound();
            }

            var result = await _context.Results
                .Include(r => r.Course)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }

            return View(result);
        }

        // POST: Results/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Results == null)
            {
                return Problem("Entity set 'ExamsSystemContext.Results'  is null.");
            }
            var result = await _context.Results.FindAsync(id);
            if (result != null)
            {
                _context.Results.Remove(result);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResultExists(int id)
        {
          return _context.Results.Any(e => e.Id == id);
        }
    }
}
