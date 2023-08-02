using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ExamsSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Microsoft.AspNetCore.Authorization;

namespace ExamsSystem.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly ExamsSystemContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        
        // Constructor
        public UsersController(ExamsSystemContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            return View(_context.AspNetUsers.ToList());
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            IdentityUser us = await userManager.FindByIdAsync(id);
            var rolesUser = await userManager.GetRolesAsync(us);
            ViewBag.rol = rolesUser;
            return View(aspNetUser);
        }

        // GET: Users/Create
        public IActionResult CreateUser()
        {
            return View();
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(UserModel um)
        {
            if (ModelState.IsValid)
            {
                IdentityUser iu = new IdentityUser()
                {
                    Email = um.Email,
                    UserName = um.Email,
                    NormalizedEmail = um.Email,
                    NormalizedUserName = um.Email,
                    EmailConfirmed = true

                };
                var status = await userManager.CreateAsync(iu, um.Password);

                if (status.Succeeded)
                {


                    AspNetUser uu = new AspNetUser()
                    {
                        Id = iu.Id,
                        FirstName = um.FirstName,
                        LastName = um.LastName,
                        UserName = iu.UserName,
                        NormalizedUserName = iu.NormalizedUserName,
                        Email = iu.Email,
                        NormalizedEmail = iu.NormalizedEmail,
                        EmailConfirmed = iu.EmailConfirmed,
                        PasswordHash = iu.PasswordHash,
                        SecurityStamp = iu.SecurityStamp,
                        ConcurrencyStamp = iu.ConcurrencyStamp,
                        PhoneNumber = um.PhoneNumber,
                        PhoneNumberConfirmed = iu.PhoneNumberConfirmed,
                        TwoFactorEnabled = iu.TwoFactorEnabled,
                        LockoutEnd = iu.LockoutEnd,
                        LockoutEnabled = iu.LockoutEnabled,
                        AccessFailedCount = iu.AccessFailedCount,
                    };
                    _context.AspNetUsers.Update(uu);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    return View(um);
                }

            }
            return View(um);
        }
        
        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,FirstName,LastName,PhoneNumber")] AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return NotFound();
            }
            AspNetUser user = _context.AspNetUsers.SingleOrDefault(u => u.Id == id);
            user.FirstName = aspNetUser.FirstName;
            user.LastName = aspNetUser.LastName;
            user.PhoneNumber = aspNetUser.PhoneNumber;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserExists(user.Id))
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
            return View(aspNetUser);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AspNetUsers == null)
            {
                return NotFound();
            }

            var aspNetUser = await _context.AspNetUsers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AspNetUsers == null)
            {
                return Problem("Entity set 'ExamsSystemContext.AspNetUsers'  is null.");
            }
            IdentityUser us = await userManager.FindByIdAsync(id);
            var userRoles = await userManager.GetRolesAsync(us);
            foreach (var role in userRoles)
            {
                if (!(us.Email=="a123@gmail.com"))
                {
                    await userManager.RemoveFromRoleAsync(us, role);
                }                
            }
            List<Result> results = await _context.Results.Where(r => r.UserId == id).ToListAsync();
            List<UserAnswer> userAnswers = await _context.UserAnswers.Where(r => r.UserId == id).ToListAsync();
            List<Course> courses = await _context.Courses.Where(r => r.UserId == id).ToListAsync();
            foreach (var re in results)
            {
                _context.Results.Remove(re);
            }
            foreach (var ua in userAnswers)
            {
                _context.UserAnswers.Remove(ua);
            }
            foreach (var co in courses)
            {
                _context.Courses.Remove(co);
            }
            var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            if (aspNetUser != null && !(aspNetUser.Email=="a123@gmail.com"))
            {
                _context.AspNetUsers.Remove(aspNetUser);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetUserExists(string id)
        {
            return _context.AspNetUsers.Any(e => e.Id == id);
        }
    }
}
