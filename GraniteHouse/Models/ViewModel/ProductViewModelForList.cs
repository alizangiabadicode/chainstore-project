using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChainStore.Models.ViewModel
{
    public class ProductViewModelForList
    {
        public List<Products> Products { get; set; }
        public IEnumerable<Special_Tags> SpecialTags { get; set; }
        public IEnumerable<ProductTypes> ProductTypes { get; set; }

        public string tag { get; set; }
        public string type { get; set; }

    }
}
