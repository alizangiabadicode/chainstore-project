using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ChainStore.Models
{
    public class Products
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public double Price { get; set; }
        public bool Available { get; set; }
        //public string ShadeColor { get; set; }
        [Range(minimum:0,maximum:100)]
        public int Count { get; set; }
        [Display(Name = "Product Type")]
        public int ProductTypeId { get; set; }


        [ForeignKey("ProductTypeId")]
        public virtual ProductTypes ProductTypes { get; set; }

        [Display(Name = "Special Tag")]
        public int SpecialTagId { get; set; }

        [ForeignKey("SpecialTagId")]
        public virtual Special_Tags SpecialTags { get; set; }
    }
}
