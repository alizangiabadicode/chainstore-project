using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChainStore.Models
{
    public class Appointments
    {
        public int Id { get; set; }
        

        public DateTime AppointmentDate { get; set; }

        [NotMapped]
        public DateTime AppointmentTime { get; set; }

        public string CustomerName { get; set; }
        public string CustomerNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsConfirmed { get; set; }

        
    }
}
