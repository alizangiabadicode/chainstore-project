using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ChainStore.Models
{
    public class CreditCard
    {

        public int Id { get; set; }


        [Required]
        public string AccountSerialNum { get; set; }



        [NotMapped]
        [Required]
        public string Pin { get; set; }



        [NotMapped]
        [Required]
        public DateTime CardExpirationDate { get; set; }


        [Required]
        public decimal SalePrice { get; set; }

        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public Appointments Appointments { get; set; }
    }
}
