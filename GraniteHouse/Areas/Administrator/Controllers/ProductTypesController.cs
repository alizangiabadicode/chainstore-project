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
using Microsoft.AspNetCore.Razor.Language.Extensions;

namespace ChainStore.Areas.Administrator.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Administrator")]
    public class ProductTypesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static Qdatabase qdb;
        private static int orm;

        public ProductTypesController(ApplicationDbContext db)
        {
            _db = db;
            qdb = new Qdatabase();
            orm = 0;
        }

        public IActionResult Index()
        {
            if(Convert.ToInt32(TempData["edit"]) == 1)
            {
                ViewBag.edit = true;
            }
            else if(Convert.ToInt32(TempData["delete"]) == 1)
            {
                ViewBag.delete = true;
            }
            else if(Convert.ToInt32(TempData["create"]) == 1)
            {
                ViewBag.create = true;
            }
          
            if (orm == 1)
            {
                return View(qdb.retProductType());
            }
            else
            {
                return View(_db.ProductTypes.ToList());
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductTypes pt)
        {
            if (ModelState.IsValid)
            {
                if (orm == 1)
                {
                    qdb.Inproducttype(pt);
                }
                else
                {

                _db.Add(pt);
                await _db.SaveChangesAsync();
                }
                TempData["create"] = 1;
                return RedirectToAction(nameof(Index));
            }

            return View(pt);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductTypes pt = null;
            if (orm == 1)
            {
                pt = qdb.retProductType((int)id);
            }
            else
            {
                pt = await _db.ProductTypes.FindAsync(id);
            }

            if (pt == null)
            {
                return NotFound();
            }

            return View(pt);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductTypes pt)
        {
            if (id != pt.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (orm == 1)
                {
                    qdb.upproducttype(pt);
                }
                else
                {
                    _db.Update(pt);
                    await _db.SaveChangesAsync();
                }
                TempData["edit"] = 1;
                return RedirectToAction(nameof(Index));
            }

            return View(pt);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductTypes pt;
            if (orm == 1)
            {
                pt = qdb.retProductType((int) id);
            }
            else
            {
                pt = await _db.ProductTypes.FindAsync(id);
            }
            return View(pt);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ProductTypes pt;
            if (orm == 1)
            {
                pt = qdb.retProductType((int) id);
            }
            else
            {
                pt = await _db.ProductTypes.FindAsync(id);
            }
            return View(pt);
        }

        [HttpPost,ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (orm == 1)
            {
                qdb.rmproducttype(id);
            }
            else
            {
                ProductTypes pt = await _db.ProductTypes.FindAsync(id);
                _db.ProductTypes.Remove(pt);
                await _db.SaveChangesAsync();
            }

            TempData["delete"] = 1;
            return RedirectToAction(nameof(Index));
        }
    }
}