using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Traditional;
using ChainStore.Models;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

namespace ChainStore.Areas.Administrator.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Administrator")]
    public class SpecialTagsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static Qdatabase db;
        private static int orm;
        public IEnumerable<Special_Tags> SpecialTags { get; set; }
        public SpecialTagsController(ApplicationDbContext db)
        {
            _db = db;
            orm = _db.WitchOrm.First().i;
        }
        public async Task<IActionResult> Index()
        {
            if (orm == 1)
            {
                db = new Qdatabase();
                SpecialTags = db.retSpecialTag();
            }
            else
            {
                SpecialTags = await _db.SpecialTags.ToListAsync();
            }
            return View(SpecialTags);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Special_Tags st)
        {
            if (ModelState.IsValid)
            {
                if (orm == 1)
                {
                    db = new Qdatabase();
                    db.InSpecialTagses(st);
                }
                else
                {
                    _db.Add(st);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(st);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Special_Tags st = await _db.SpecialTags.FindAsync(id);
            if (st == null)
            {
                return NotFound();
            }

            return View(st);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Special_Tags st)
        {
            if (id != st.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (orm == 1)
                {
                    db.upSpecialTag(st);
                }
                else
                {
                    _db.Update(st);
                    await _db.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(st);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Special_Tags st = await _db.SpecialTags.FindAsync(id);
            return View(st);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Special_Tags st = null;
            if (orm == 1)
            {
                st = db.retSpecialTag((int)id);
            }
            else
            {
                st = await _db.SpecialTags.FindAsync(id);
            }

            if (st == null)
            {
                return NotFound();
            }

            return View(st);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DelteConfirmed(int id)
        {
            if (orm == 1)
            {
                db.rmSpecialTag(id);
            }
            else
            {
                Special_Tags st = await _db.SpecialTags.FindAsync(id);
                _db.Remove(st);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index");
        }
    }
}