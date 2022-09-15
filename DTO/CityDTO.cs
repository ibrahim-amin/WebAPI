using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTO
{
    public class CityDTO
    {
        public int Id { set; get; }
        [Required,StringLength(50,MinimumLength =2)]
        [RegularExpression(".*[a-zA-Z]+.*",ErrorMessage ="Only Numeric are not allowed")]
        public string Name { set; get; }
        [Required]
        public string Country { set; get; }

    }
}
