﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ChainStore.Models
{
    public class ApplicationUser:IdentityUser
    {

        [Display(Name = "Sales Person")]
        public string Name { get; set; }

        [NotMapped]
        public bool IsConfirmed { get; set; }
    }
}
