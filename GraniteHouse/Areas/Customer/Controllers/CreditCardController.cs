using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Migrations;
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

        [BindProperty]
        public CreditCard CreditCard { get; set; }
        public CreditCardController(ApplicationDbContext db)
        {
            _db = db;
            CreditCard = new CreditCard();
        }
        public async Task<IActionResult> Index(int appointmentId)
        {
            CreditCard.AppointmentId = appointmentId;
            var idd = HttpContext.Session.Get<Dictionary<int, int>>("ls")[int.MaxValue];
            if (CreditCard.AppointmentId != idd)
            {
                return NotFound();
            }
            CreditCard.Appointments = await _db.Appointments.FirstAsync(i => i.Id == appointmentId);
            var psd = _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == appointmentId)
                .Include(e => e.Products);
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
            _db.CreditCards.Add(CreditCard);
            await _db.SaveChangesAsync();


            var psd = await _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == id)
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


            Dictionary<int, int> dic = new Dictionary<int, int>();
            HttpContext.Session.Set("ls", dic);
            return RedirectToAction("Index", "Home");
        }
    }
}