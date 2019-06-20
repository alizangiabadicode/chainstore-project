using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Traditional;
using ChainStore.Extension;
using Microsoft.AspNetCore.Mvc;
using ChainStore.Models;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ChainStore.Models.ViewModel;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;

namespace ChainStore.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static Qdatabase qdb;
        private static int orm;
        public HomeController(ApplicationDbContext db)
        {
            _db = db;
            orm = _db.WitchOrm.First().i;
            qdb = new Qdatabase();
        }
        //[BindProperty]
        //public bool Error { get; set; }
        public IActionResult Index(ProductViewModelForList vm)
        {
            if(vm.tag == "All")
            {
                vm.tag = null;
                TempData["tag"] = null;
            }
            if(vm.type == "All")
            {
                vm.type = null;
                TempData["type"] = null;
            }
            if (TempData["tag"] != null)
            {
                vm.tag = TempData["tag"].ToString();
                TempData.Keep();
            }
            else
            {
                if (vm.tag != null)
                {
                    TempData["tag"] = vm.tag;
                    TempData.Keep();
                }
            }
            if (TempData["type"] != null)
            {
                vm.type = TempData["type"].ToString();
                TempData.Keep();
            }
            else
            {
                if (vm.type != null)
                {
                    TempData["type"] = vm.type;
                    TempData.Keep();
                }
            }

            //ProductViewModelForList vm = new ProductViewModelForList();
            //vm.tag = tag;
            //vm.type = type;
            if (TempData["congrats"] != null && Convert.ToInt32(TempData["congrats"]) == 1)
            {
                ViewBag.congrats = true;
            }
            if (TempData["orm"] != null)
            {
                int o = Convert.ToInt32(TempData["orm"]);
                if (o == 1)
                {
                    ViewBag.orm = 1;
                }
                else
                {
                    ViewBag.orm = 0;
                }
            }
            bool tagg = true, typee = true;
            if (vm.tag == null)
                tagg = false;
            if (vm.type == null)
                typee = false;

            List<Products> temp = null;
            if (orm == 1)
            {
                temp = qdb.retProduct();
                qdb.include_pt_st(temp);
            }
            else
            {
                temp = _db.Products.Include(e => e.SpecialTags).Include(e => e.ProductTypes).ToList();
            }

            if (tagg)
            {
                temp = temp.Where(e => e.SpecialTags.Name == vm.tag).ToList();
            }

            if (typee)
            {
                temp = temp.Where(e => e.ProductTypes.Name == vm.type).ToList();
            }



            vm.Products = temp;
            vm.SpecialTags = _db.SpecialTags.ToList();
            vm.ProductTypes = _db.ProductTypes.ToList();
            
            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> EditTheCount(int id,int value)
        {
            Products p = null;
            if (orm == 1)
            {
                p = qdb.retProduct(id);
            }
            else
            {
                p = await _db.Products.FirstAsync(i => i.Id == id);
            }

            if (p.Count < value)
            {
                Products product = null;
                if (orm == 1)
                {
                    product = qdb.retProduct(id);
                    List<Products> ls = new List<Products>(); ls.Add(product);
                    qdb.include_pt_st(ls);
                }
                else
                {
                    product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags)
                        .FirstOrDefaultAsync(m => m.Id == id);
                }

                ViewBag.error = true;
                return View("Details", product);
            }
            Dictionary<int, int> lst = HttpContext.Session.Get<Dictionary<int, int>>("ls") ?? new Dictionary<int, int>();
            lst[id] = value;
            HttpContext.Session.Set<Dictionary<int, int>>("ls", lst);
            TempData.Keep();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            Products product = null;
            if (orm == 1)
            {
                product = qdb.retProduct(id);
                var ls = new List<Products>();ls.Add(product);
                qdb.include_pt_st(ls);
            }
            else
            {
                product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags)
                    .FirstOrDefaultAsync(m => m.Id == id);
            }
            TempData.Keep();
            return View(product);
        }
        //[Authorize(Roles = SD.AdminEndUser + ","+ SD.SuperAdminEndUser)]
        [HttpPost, ActionName("Details")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DetailsPost(int id,int value)
        {
            Dictionary<int,int> lst = HttpContext.Session.Get<Dictionary<int,int>>("ls") ?? new Dictionary<int, int>();

            Products p = null;
            if (orm == 1)
            {
                p = qdb.retProduct(id);
            }
            else
            {
                p = await _db.Products.FirstAsync(i => i.Id == id);
            }

            if (p.Count < value)
            {
                Products product = null;
                if (orm == 1)
                {
                    product = qdb.retProduct(id);
                    List<Products> ls = new List<Products>();ls.Add(product);
                    qdb.include_pt_st(ls);
                }
                else
                {
                    product = await _db.Products.Include(m => m.ProductTypes).Include(m => m.SpecialTags)
                        .FirstOrDefaultAsync(m => m.Id == id);
                }

                ViewBag.error = true;
                return View("Details", product);
            }
            lst.Add(id, value);
            //lst[id] = value;
            HttpContext.Session.Set<Dictionary<int,int>>("ls", lst);
            TempData.Keep();
            return RedirectToAction(nameof(Index), "Home", new { area = "Customer" });
        }

        public IActionResult Remove(int id)
        {
            var temp = HttpContext.Session.Get<Dictionary<int,int>>("ls");
            if (temp.Count > 0)
            {
                if (temp.ContainsKey(id))
                {
                    temp.Remove(id);
                }
            }
            HttpContext.Session.Set("ls", temp);
            TempData.Keep();
            return RedirectToAction(nameof(Index));
        }
    }
}
