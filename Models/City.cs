using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class City:BaseEntity
    {
 
        [Required]
        public string Name { set; get; }

        [Required]
        public string Country { set; get; }

    }
}
