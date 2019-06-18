using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Migrations;
using ChainStore.Data.Traditional;
using ChainStore.Extension;
using ChainStore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chain_Store.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class CreditCardController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static Qdatabase qdb;
        private static int orm;
        [BindProperty]
        public CreditCard CreditCard { get; set; }
        public CreditCardController(ApplicationDbContext db)
        {
            _db = db;
            CreditCard = new CreditCard();
            orm = _db.WitchOrm.First().i;
            qdb = new Qdatabase();
        }
        public async Task<IActionResult> Index(int appointmentId)
        {
            CreditCard.AppointmentId = appointmentId;
            var idd = HttpContext.Session.Get<Dictionary<int, int>>("ls")[int.MaxValue];
            if (CreditCard.AppointmentId != idd)
            {
                return NotFound();
            }

            List<ProductsSelectedForAppointment> psd = new List<ProductsSelectedForAppointment>();
            if (orm == 1)
            {
                CreditCard.Appointments = qdb.retAppointment(appointmentId);
                psd = qdb.retpsa_with_ai(appointmentId);
                foreach (ProductsSelectedForAppointment i in psd)
                {
                    qdb.include_pi_ai(i);
                }
            }
            else
            {
                CreditCard.Appointments = await _db.Appointments.FirstAsync(i => i.Id == appointmentId);
                psd = _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == appointmentId).Include(e=>e.Products).ToList();
            }

            double price = 0;
            foreach (ProductsSelectedForAppointment i in psd)
            {
                price += i.Products.Price * i.Count;
            }

            CreditCard.SalePrice = Convert.ToDecimal(price);
            return View(CreditCard);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> Pay()
        {
            var idd = HttpContext.Session.Get<Dictionary<int, int>>("ls")[int.MaxValue];
            if (CreditCard.AppointmentId != idd)
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return View(CreditCard);
            }
            var id = CreditCard.AppointmentId;
            CreditCard.Appointments = null;

            List<ProductsSelectedForAppointment> psd = null;
            if (orm == 1)
            {
                qdb.incredit(CreditCard);
                psd = qdb.retpsa_with_ai(id);

                foreach (var item in psd)
                {
                    var product = qdb.retProduct(item.ProductId);
                    product.Count -= item.Count;
                    if (product.Count == 0)
                    {
                        product.Available = false;
                    }

                    qdb.upProduct(product);
                }

                var appointment = qdb.retAppointment(id);
                appointment.IsConfirmed = true;
                qdb.upAppointment(appointment);
            }
            else
            {
                _db.CreditCards.Add(CreditCard);
                await _db.SaveChangesAsync();
                psd = await _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == id)
                    .ToListAsync();
                foreach (var item in psd)
                {
                    var product = _db.Products.First(e => e.Id == item.ProductId);
                    product.Count -= item.Count;
                    if (product.Count == 0)
                    {
                        product.Available = false;
                    }

                }
                _db.Appointments.First(e => e.Id == id).IsConfirmed = true;
                await _db.SaveChangesAsync();
            }

            Dictionary<int, int> dic = new Dictionary<int, int>();
            HttpContext.Session.Set("ls", dic);
            TempData["congrats"] = 1;
            return RedirectToAction("Index", "Home");
        }
    }
}