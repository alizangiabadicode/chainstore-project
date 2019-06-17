using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Models;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.Areas.Administrator.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Administrator")]
    public class AdminUsersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public AdminUsersController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }

        //Get : Edit
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id.Trim() == string.Empty)
            {
                return NotFound();
            }

            ApplicationUser au = await _db.ApplicationUsers.FirstAsync(e => e.Id == id);
            if (au == null)
            {
                return NotFound();
            }

            return View(au);
        }

        //Post : Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ApplicationUser au)
        {
            if (au.Id != id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                ApplicationUser obj = await _db.ApplicationUsers.FirstAsync(e => e.Id == id);
                obj.Name = au.Name;
                obj.PhoneNumber = au.PhoneNumber;
                await _db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(au);
        }

        //Get : Delete
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null || id.Trim() == string.Empty)
            {
                return NotFound();
            }

            ApplicationUser au = await _db.ApplicationUsers.FirstAsync(e => e.Id == id);
            if (au == null)
            {
                return NotFound();
            }

            return View(au);

        }


        //Post : Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName(name: "Delete")]
        public async Task<IActionResult> DeletePost(string id)
        {
            ApplicationUser au = await _db.ApplicationUsers.FirstAsync(e => e.Id == id);
            au.LockoutEnd=DateTime.Now.AddYears(1000);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}