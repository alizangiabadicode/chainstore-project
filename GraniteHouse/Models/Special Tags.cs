using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Cache;
using System.Threading.Tasks;

namespace ChainStore.Models
{
    public class Special_Tags
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
    }
}
