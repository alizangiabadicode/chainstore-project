using System;
using System.Collections.Generic;
using System.Text;
using ChainStore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChainStore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ProductTypes> ProductTypes { get; set; }

        public DbSet<Special_Tags> SpecialTags { get; set; }

        public DbSet<Products> Products { get; set; }

        public DbSet<Appointments> Appointments { get; set; }

        public DbSet<ProductsSelectedForAppointment> ProductsSelectedForAppointments { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        public DbSet<CreditCard> CreditCards { get; set; }

        public DbSet<orm> WitchOrm { get; set; }

        //public DbSet<SuccessfullAppointmentModel> SuccessfullAppointments { get; set; }

       // public DbSet<SuccessfullAppointments> SuccessfullAppointments { get; set; }
    }
}
