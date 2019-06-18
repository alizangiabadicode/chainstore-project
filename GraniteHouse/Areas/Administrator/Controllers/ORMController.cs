﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
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
            if (number != -1)
            {
                _db.WitchOrm.First().i = number;
                _db.SaveChanges();
                ViewBag.changed = true;
                ViewBag.text = number == 1 ? "Queries" : "EntityFramework";
            }
            return View();
        }
    }
}