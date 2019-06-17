using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Traditional;
using ChainStore.Extension;
using ChainStore.Models;
using ChainStore.Models.ViewModel;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.Areas.Customer.Controllers
{

    //[Authorize(Roles = SD.AdminEndUser + "," + SD.SuperAdminEndUser)]
    [Area("Customer")]
    public class ShoppingCardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static int orm;
        private static Qdatabase qdb;
        [BindProperty]
        public ShoppingCardViewModel ShoppingCardvm { get; set; }
        public ShoppingCardController(ApplicationDbContext db)
        {
            _db = db;
            ShoppingCardvm = new ShoppingCardViewModel()
            {
                Products = new List<Products>()
            };
            orm = _db.WitchOrm.First().i;
            qdb = new Qdatabase();
        }
        public IActionResult Index()
        {
            Dictionary<int, int> lst = HttpContext.Session.Get<Dictionary<int, int>>("ls");
            if (lst!=null&&lst.TryGetValue(int.MinValue,out int v) && v==1)
            {
                HttpContext.Session.Set("ls", new Dictionary<int, int>());
                lst = new Dictionary<int, int>();
            }
            if (lst != null)
            {

                if (orm == 1)
                {
                    List<Products> ls = new List<Products>();
                    foreach (int i in lst.Keys)
                    {
                        Products p = qdb.retProduct(i);ls.Add(p);qdb.include_pt_st(ls);
                        p.Price = lst[i] * p.Price;
                        ShoppingCardvm.Products.Add(p);
                        ls.Clear();
                    }
                }
                else
                {
                    foreach (int i in lst.Keys)
                    {
                        Products p = _db.Products.Include(e => e.ProductTypes).Include(e => e.SpecialTags)
                            .FirstOrDefault(e => e.Id == i);
                        p.Price = lst[i] * p.Price;
                        ShoppingCardvm.Products.Add(p);
                    }
                }

                //foreach (int i in lst.Keys)
                //{
                //    Products p = _db.Products.Include(e => e.ProductTypes).Include(e => e.SpecialTags)
                //        .FirstOrDefault(e => e.Id == i);
                //    p.Price = lst[i] * p.Price;
                //    ShoppingCardvm.Products.Add(p);
                //}
            }

            return View(ShoppingCardvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> ScheduleAppointment()
        {
            Dictionary<int, int> items = HttpContext.Session.Get<Dictionary<int, int>>("ls");

            if(items.Keys.Count == 0)
            {
                return RedirectToAction(nameof(Index), "Home");
            }

            ShoppingCardvm.Appointments.AppointmentDate = ShoppingCardvm.Appointments.AppointmentDate
                .AddHours(ShoppingCardvm.Appointments.AppointmentTime.Hour)
                .AddMinutes(ShoppingCardvm.Appointments.AppointmentTime.Minute);

            if (orm == 1)
            {
                ShoppingCardvm.Appointments.Id = qdb.inappointment(ShoppingCardvm.Appointments);
            }
            else
            {
                _db.Appointments.Add(ShoppingCardvm.Appointments);
                await _db.SaveChangesAsync();
            }


            int idAppointment = ShoppingCardvm.Appointments.Id;

            if (orm == 1)
            {
                foreach (var i in items.Keys)
                {
                    ProductsSelectedForAppointment psa = new ProductsSelectedForAppointment()
                    {
                        AppointmentId = idAppointment,
                        ProductId = i,
                        Count = items[i]
                    };
                    //qdb.include_pi_ai(psa);
                    qdb.inpsa(psa);
                }
            }
            else
            {
                foreach (var i in items.Keys)
                {
                    ProductsSelectedForAppointment psa = new ProductsSelectedForAppointment()
                    {
                        AppointmentId = idAppointment,
                        ProductId = i,
                        Count = items[i]
                    };
                    _db.ProductsSelectedForAppointments.Add(psa);
                }
                await _db.SaveChangesAsync();
            }

            //items = new Dictionary<int, int>();
            items[int.MaxValue] = idAppointment;
            HttpContext.Session.Set("ls", items);
            return RedirectToAction("AppointmentConfirmation", "ShoppingCard",
                new { id = ShoppingCardvm.Appointments.Id });
        }

        public IActionResult Remove(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Dictionary<int, int> lst = HttpContext.Session.Get<Dictionary<int, int>>("ls");
            if (lst.Count > 0)
            {
                if (lst.ContainsKey((int)id))
                {
                    lst.Remove((int)id);
                }
            }

            HttpContext.Session.Set("ls", lst);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> RemoveFromSessionInConfirmation(int? id, int appoinmentId)
        {
            if (id == null)
            {
                return NotFound();
            }
            Dictionary<int, int> lst = HttpContext.Session.Get<Dictionary<int, int>>("ls");
            if (lst[int.MaxValue] != appoinmentId)
            {
                return NotFound();
            }


            //Dictionary<int, int> lst = HttpContext.Session.Get<Dictionary<int, int>>("ssShoppingCard");
            //if (lst.Count > 0)
            //{
            //    if (lst.ContainsKey((int)id))
            //    {
            //        lst.Remove((int)id);
            //    }
            //}
            ProductsSelectedForAppointment psd = null;
            if (orm == 1)
            {
                qdb.rmpsa(appoinmentId, (int)id);
            }
            else
            {
                psd = await _db.ProductsSelectedForAppointments.FirstAsync(e => e.AppointmentId == appoinmentId && e.ProductId == id);
                _db.ProductsSelectedForAppointments.Remove(psd);
                await _db.SaveChangesAsync();
            }

            var count = _db.ProductsSelectedForAppointments.Count(e => e.AppointmentId == appoinmentId);
            if (count == 0)
            {
                lst[int.MinValue] = 1;
                HttpContext.Session.Set("ls", lst);
                return RedirectToAction(nameof(Index));
            }

            HttpContext.Session.Set("ls", lst);
            return RedirectToAction("AppointmentConfirmation", "ShoppingCard", new {id=appoinmentId});
        }

        public async Task<IActionResult> AppointmentConfirmation(int id)
        {
            List<ProductsSelectedForAppointment> lst = null;
            if (orm == 1)
            {
                ShoppingCardvm.Appointments = qdb.retAppointment(id);
                lst = qdb.retpsaall();
            }
            else
            {
                ShoppingCardvm.Appointments = await _db.Appointments.FirstOrDefaultAsync(e => e.Id == id);
                lst = await _db.ProductsSelectedForAppointments.ToListAsync();
            }

            Dictionary<int, int> ls = HttpContext.Session.Get<Dictionary<int, int>>("ls");
            if (ls[int.MaxValue] != ShoppingCardvm.Appointments.Id)
            {
                return NotFound();
            }

            lst = lst.Where(e => e.AppointmentId == id).ToList();

            if (orm == 1)
            {
                
                foreach (ProductsSelectedForAppointment psa in lst)
                {
                    Products p = qdb.retProduct(psa.ProductId);
                    List<Products> products = new List<Products>();
                    products.Add(p);
                    qdb.include_pt_st(products);
                    ShoppingCardvm.Products.Add(p);
                }

            }
            else
            {
                foreach (ProductsSelectedForAppointment psa in lst)
                {
                    ShoppingCardvm.Products.Add(_db.Products.Include(e => e.ProductTypes).Include(e => e.SpecialTags)
                        .FirstOrDefault(e => e.Id == psa.ProductId));
                }
            }

            //foreach (ProductsSelectedForAppointment psa in lst)
            //{
            //    ShoppingCardvm.Products.Add(_db.Products.Include(e => e.ProductTypes).Include(e => e.SpecialTags)
            //       .FirstOrDefault(e => e.Id == psa.ProductId));
            //}

            ls[int.MinValue] = 1;
            HttpContext.Session.Set("ls", ls);
            return View(ShoppingCardvm);
        }
    }
}