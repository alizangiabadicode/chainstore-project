using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChainStore.Models.ViewModel
{
    public class ProductsViewModel
    {
        public Products Products { get; set; }
        public IEnumerable<Special_Tags> SpecialTags { get; set; }
        public IEnumerable<ProductTypes> ProductTypes { get; set; }
    }
}
