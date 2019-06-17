using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChainStore.Models.ViewModel
{
    public class AppointmentDetailViewModel
    {
        public IEnumerable<Products> Products { get; set; }
        public IEnumerable<ApplicationUser> ApplicationUsers { get; set; }
        public Appointments Appointments { get; set; }
    }
}
