using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;


namespace WebAPI.DTO
{
    
    public class PhotoDTO
    {
        //public int Id { get; set; }
        public string PublicId { get; set; }
       // [ForeignKey("PropertyId")]
        public int PropertyId { get; set; }
        public string ImageUrl { get; set; }
        public  bool IsPrimary{get; set;}
    }
}
