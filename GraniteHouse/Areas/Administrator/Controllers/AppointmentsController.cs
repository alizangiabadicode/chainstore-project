using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ChainStore.Data;
using ChainStore.Data.Traditional;
using ChainStore.Models;
using ChainStore.Models.ViewModel;
using ChainStore.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Products = ChainStore.Models.Products;

namespace ChainStore.Areas.Administrator.Controllers
{
    [Authorize(Roles = SD.SuperAdminEndUser + "," + SD.AdminEndUser)]
    [Area("Administrator")]
    public class AppointmentsController : Controller
    {
        private readonly ApplicationDbContext _db;
        private static Qdatabase qdb;
        private static int orm;
        public AppointmentsController(ApplicationDbContext db)
        {
            qdb = new Qdatabase();
            _db = db;
            orm = _db.WitchOrm.First().i;
        }

        public async Task<IActionResult> Index(string searchName = null, string searchEmail = null,
            string searchPhone = null, string searchDate = null)
        {
            if (Convert.ToInt32(TempData["edit"]) == 1)
            {
                ViewBag.edit = true;
            }
            else if (Convert.ToInt32(TempData["delete"]) == 1)
            {
                ViewBag.delete = true;
            }
            else if (Convert.ToInt32(TempData["create"]) == 1)
            {
                ViewBag.create = true;
            }

            var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVm = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };

            if (orm == 1)
            {
                appointmentVm.Appointments = qdb.retAppointment();
            }
            else
            {
                appointmentVm.Appointments = await _db.Appointments.ToListAsync();
            }

            if (searchName != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (searchEmail != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }

            if (searchPhone != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }

            if (searchDate != null)
            {
                try
                {
                    DateTime date = Convert.ToDateTime(searchDate);
                    appointmentVm.Appointments = appointmentVm.Appointments
                        .Where(e => e.AppointmentDate.ToShortDateString().Equals(date.ToShortDateString())).ToList();
                }
                catch (Exception e)
                {
                }
            }

            return View(appointmentVm);
        }


        // Get : Edit
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            List<Products> products = null;
            Appointments appointments = null;
            if (orm == 1)
            {
                products = qdb.p_join_psa((int) id);
                qdb.include_pt_st(products);
                appointments = qdb.retAppointment((int) id);
            }
            else
            {
                products = (from i in _db.Products
                        join j in _db.ProductsSelectedForAppointments on i.Id equals j.ProductId
                        where j.AppointmentId == id
                        select i
                    ).Include(e => e.ProductTypes).ToList();
                appointments = (from i in _db.Appointments where i.Id == (int) id select i).FirstOrDefault();
            }
            
            //var appointment = (from i in _db.Appointments where i.Id == (int) id select i).FirstOrDefault();
            var applicationUsers = (from i in _db.ApplicationUsers select i).ToList();
            AppointmentDetailViewModel avm = new AppointmentDetailViewModel()
            {
                Appointments = appointments,
                ApplicationUsers = applicationUsers,
                Products = products
            };
            return View(avm);
        }

        // Post : Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, AppointmentDetailViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            Appointments obj = null;
            if (orm == 1)
            {
                obj = qdb.retAppointment(id);
            }
            else
            {
                obj = await _db.Appointments.FirstAsync(e => e.Id == id);
            }

            obj.CustomerName = vm.Appointments.CustomerName;
            obj.CustomerEmail = vm.Appointments.CustomerEmail;
            obj.IsConfirmed = vm.Appointments.IsConfirmed;
            obj.CustomerNumber = vm.Appointments.CustomerNumber;
            obj.AppointmentDate = vm.Appointments.AppointmentDate.AddHours(vm.Appointments.AppointmentTime.Hour)
                .AddMinutes(vm.Appointments.AppointmentTime.Minute);

            if (orm == 1)
            {
                qdb.upAppointment(obj);
            }
            else
            {
                await _db.SaveChangesAsync();
            }

            TempData["edit"] = 1;
            return RedirectToAction(nameof(Index));
        }

    
        
        // Get : Details
        public async Task<IActionResult>Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointments apppointment = null;
            if (orm == 1)
            {
                apppointment = qdb.retAppointment((int) id);
            }
            else
            {
                apppointment = await (from i in _db.Appointments where i.Id == id select i).FirstAsync();
            }

            if (apppointment == null)
            {
                return NotFound();
            }

            List<Products> products = null;
            List<ProductsSelectedForAppointment> psd = null;
            if (orm == 1)
            {
                products = qdb.p_join_psa((int) id);
                qdb.include_pt_st(products);
                psd = qdb.retpsa((int) id);
            }
            else
            {
                products = await (from i in _db.Products
                    join j in _db.ProductsSelectedForAppointments on i.Id equals j.ProductId
                    where j.AppointmentId == id
                    select i).Include(e => e.ProductTypes).ToListAsync();
                psd = _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == id).ToList();
            }

            foreach (Products i in products)
            {
                i.Count = _db.ProductsSelectedForAppointments.First(e => e.ProductId == i.Id).Count;
            }

            var applicationUsers = (from i in _db.ApplicationUsers select i);

            AppointmentDetailViewModel vm = new AppointmentDetailViewModel()
            {
                ApplicationUsers = applicationUsers,
                Appointments = apppointment,
                Products = products
            };
            return View(vm);
        }

        // Get : Delete
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Appointments apppointment = null;
            if (orm == 1)
            {
                apppointment = qdb.retAppointment((int)id);
            }
            else
            {
                apppointment = await (from i in _db.Appointments where i.Id == id select i).FirstAsync();
            }
            if (apppointment == null)
            {
                return NotFound();
            }

            var applicationUsers = (from i in _db.ApplicationUsers select i);

            List<Products> products = null;
            List<ProductsSelectedForAppointment> psd = null;
            if (orm == 1)
            {
                products = qdb.p_join_psa((int)id);
                qdb.include_pt_st(products);
                psd = qdb.retpsa((int)id);
            }
            else
            {
                products = await (from i in _db.Products
                    join j in _db.ProductsSelectedForAppointments on i.Id equals j.ProductId
                    where j.AppointmentId == id
                    select i).Include(e => e.ProductTypes).ToListAsync();
                psd = _db.ProductsSelectedForAppointments.Where(e => e.AppointmentId == id).ToList();
            }
            AppointmentDetailViewModel vm = new AppointmentDetailViewModel()
            {
                ApplicationUsers = applicationUsers,
                Appointments = apppointment,
                Products = products
            };
            return View(vm);
        }

        // Post : Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int id)
        {
            Appointments appointment = null;
            if (orm == 1)
            {
                qdb.rmappointment(id);
            }
            else
            {
                appointment = await _db.Appointments.FindAsync(id);
                _db.Appointments.Remove(appointment);
                await _db.SaveChangesAsync();
            }

            TempData["delete"] = 1;
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> SuccessFullAppointments(string searchName = null, string searchEmail = null,
            string searchPhone = null, string searchDate = null)
        {
            //var claimsIdentity = (ClaimsIdentity)this.User.Identity;
            //var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            AppointmentViewModel appointmentVm = new AppointmentViewModel()
            {
                Appointments = new List<Appointments>()
            };

            if (orm == 1)
            {
                appointmentVm.Appointments = qdb.SucAppointments();
            }
            else
            {
                appointmentVm.Appointments = await _db.Appointments.Where(e => e.IsConfirmed == true).ToListAsync();
            }

            //if (User.IsInRole(SD.AdminEndUser))
            //{
            //    appointmentVm.Appointments =
            //        appointmentVm.Appointments.Where(e => e.SalesPersonId == claim.Value).ToList();
            //}

            if (searchName != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerName.ToLower().Contains(searchName.ToLower())).ToList();
            }

            if (searchEmail != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerEmail.ToLower().Contains(searchEmail.ToLower())).ToList();
            }

            if (searchPhone != null)
            {
                appointmentVm.Appointments = appointmentVm.Appointments
                    .Where(e => e.CustomerNumber.ToLower().Contains(searchPhone.ToLower())).ToList();
            }

            if (searchDate != null)
            {
                try
                {
                    DateTime date = Convert.ToDateTime(searchDate);
                    appointmentVm.Appointments = appointmentVm.Appointments
                        .Where(e => e.AppointmentDate.ToShortDateString().Equals(date.ToShortDateString())).ToList();
                }
                catch (Exception e)
                {
                }
            }

            return View(appointmentVm);
        }
    }

}