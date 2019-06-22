using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Models;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chain_Store.Areas.Administrator.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser)]
    [Area("Administrator")]
    public class ORMController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ORMController(ApplicationDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int number = -1)

        {
            orm o = new orm();
            o.i = _db.WitchOrm.First().i;
            if (number != -1)
            {
                _db.WitchOrm.First().i = number;
                _db.SaveChanges();
                ViewBag.changed = true;
                ViewBag.text = number == 1 ? "Queries" : "EntityFramework";
                TempData["orm"] = number;
                return RedirectToAction(nameof(Index), "Home",new { @Area="Customer"});
            }
            return View(o);
        }
    }
}