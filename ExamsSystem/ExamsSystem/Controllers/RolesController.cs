using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using ExamsSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ExamsSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly ExamsSystemContext _context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        // Constructor
        public RolesController(ExamsSystemContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        // GET: Roles
        public async Task<IActionResult> Index()
        {
            //roleManager.Roles.ToList();
            return View(await _context.AspNetRoles.ToListAsync());
        }

        // GET: Roles/Details
        public async Task<IActionResult> Details(string id)
        {
            Response.HttpContext.Session.SetString("role", id.ToString());
            if (id == null || _context.AspNetRoles == null)
            {
                return NotFound();
            }
            //var role = await _context.AspNetRoles.SingleOrDefaultAsync(r => r.Id == id);
            var usersInRole = await userManager.GetUsersInRoleAsync(id);
            List<AspNetUser> us = new List<AspNetUser>();
            foreach (var item in usersInRole)
            {
                us.Add(await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Id == item.Id));
            }
            ViewBag.users = us;
            var aspNetRole = await _context.AspNetRoles
                .FirstOrDefaultAsync(m => m.Name == id);
            if (aspNetRole == null)
            {
                return NotFound();
            }

            return View(aspNetRole);
        }

        // GET: Roles/Create
        public async Task<IActionResult> AddToRole()
        {
            string? role = Response.HttpContext.Session.GetString("role");
            ViewBag.role = role;
            List<IdentityUser> allUsers = userManager.Users.ToList();
            List<AspNetUser> users = new List<AspNetUser>();
            foreach (var user in allUsers)
            {
                if (!(await userManager.IsInRoleAsync(user, role)))
                {
                    users.Add(await _context.AspNetUsers.SingleOrDefaultAsync(u => u.Id == user.Id));
                }
            }
            return View(users);
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToRole(IFormCollection form)
        {
            string? role = Response.HttpContext.Session.GetString("role");
            if (form["selectedItems"].ToString() == "")
            {
                return RedirectToAction("Details", "Roles", new { id = role });
            }
            string? selectedItems = form["selectedItems"];
            string[] userId = selectedItems.Split(",");
            foreach (var item in userId)
            {
                IdentityUser user = await userManager.FindByIdAsync(item);
                await userManager.AddToRoleAsync(user, role);
            }
            return RedirectToAction("Details", "Roles", new { id = role });
        }

        // GET: Roles/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] RoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                AspNetRole role = new AspNetRole()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = roleModel.Name,
                    NormalizedName = roleModel.Name,
                    ConcurrencyStamp = roleModel.Name
                };
                _context.Add(role);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(roleModel);
        }

        // GET: Roles/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || _context.AspNetRoles == null)
            {
                return NotFound();
            }

            var aspNetRole = await _context.AspNetRoles.FindAsync(id);
            if (aspNetRole == null)
            {
                return NotFound();
            }
            return View(aspNetRole);
        }

        // POST: Roles/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name")] AspNetRole aspNetRole)
        {
            if (id != aspNetRole.Id)
            {
                return NotFound();
            }
            AspNetRole role = _context.AspNetRoles.SingleOrDefault(r => r.Id == id);
            role.Name = aspNetRole.Name;
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(role);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetRoleExists(role.Id))
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
            return View(role);
        }

        public async Task<IActionResult> RemoveFromRole(string id)
        {
            var role = Response.HttpContext.Session.GetString("role");
            IdentityUser user = await userManager.FindByIdAsync(id);
            await userManager.RemoveFromRoleAsync(user, role);
            return RedirectToAction("Details", "Roles", new
            {
                id = role
            });
        }

        // GET: Roles/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || _context.AspNetRoles == null)
            {
                return NotFound();
            }

            var aspNetRole = await _context.AspNetRoles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetRole == null)
            {
                return NotFound();
            }

            return View(aspNetRole);
        }

        // POST: Roles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (_context.AspNetRoles == null)
            {
                return Problem("Entity set 'OrderIndivisualContext.AspNetRoles'  is null.");
            }
            var aspNetRole = await _context.AspNetRoles.FindAsync(id);
            if (aspNetRole != null)
            {
                _context.AspNetRoles.Remove(aspNetRole);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AspNetRoleExists(string id)
        {
            return _context.AspNetRoles.Any(e => e.Id == id);
        }
    }
}
