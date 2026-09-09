using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolApp.Models;

namespace SchoolApp.Controllers
{
    // [Route("[controller]")] <-- REMOVED THIS LINE
    [Authorize]
    public class AppUserController : Controller
    {
        private readonly IAppUserRepository _repo;
        public AppUserController(IAppUserRepository repo) => _repo = repo;

        public async Task<IActionResult> Index() => View(await _repo.GetAllAsync());

        [Authorize(Roles = "Admin")] 
        public IActionResult Create() => View();

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppUser user) {
            if (ModelState.IsValid) { 
                await _repo.InsertAsync(user); 
                return RedirectToAction(nameof(Index)); 
            }
            return View(user);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id) => View(await _repo.GetByIdAsync(id));

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppUser user) {
            if (ModelState.IsValid) { 
                await _repo.UpdateAsync(user); 
                return RedirectToAction(nameof(Index)); 
            }
            return View(user);
        }

        [HttpPost, Authorize(Roles = "Admin"), ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) { 
            await _repo.DeleteAsync(id); 
            return RedirectToAction(nameof(Index)); 
        }
    }
}