using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController] 
    [Authorize]
    public class CityController : ControllerBase
    {
        private readonly DataContext dc;

        public CityController(DataContext dc)
        {
            this.dc = dc;
        }

   [HttpGet]
   [AllowAnonymous]
        public async Task<IActionResult> Get()
        {
           //throw new UnauthorizedAccessException();
            var cities = await Task.FromResult(dc.City.ToList());
            var citydto = from c in cities
                          select new CityDTO()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              Country=c.Country
                          };
            return Ok(citydto);
        }
    

        [HttpPost("AddCity")]
        [HttpPost("AddCity/{CityName}")]
        [HttpPost("post")]
        [AllowAnonymous]
       //public async Task<IActionResult> AddCity(string CityName)
        public async Task<IActionResult> AddCity(CityDTO citydto)
        {
            /*City city = new City();
            city.Name = CityName;*/

            var city = new City();
            city.Id = citydto.Id;
            city.Name = citydto.Name;
            city.Country = citydto.Country;
            city.LastUpdatedBy = 1;
            city.LastUpdatedOn = DateTime.Now;

            await dc.City.AddAsync(city);
            await dc.SaveChangesAsync();
            return Ok(city);
        }


        [HttpPut("update/{Id}")]
        public async Task<IActionResult> UpdateCity(CityDTO citydto,int Id)
        {
           // try {
                var city = await dc.City.FindAsync(Id);
               /* if (city == null)
                {

                    return BadRequest("Update not allowed");
                }*/
                city.Name = citydto.Name;
                city.Country = citydto.Country;
                city.LastUpdatedBy = 1;
                city.LastUpdatedOn = DateTime.Now;
                //throw new Exception("some error occured");
                dc.Entry(city).State = EntityState.Modified;
                await dc.SaveChangesAsync();
                return StatusCode(200);
         //   }
           /* catch
            {
                return StatusCode(500, "some unknown error occured");
            }
*/
        }

        [HttpDelete("DeleteCity/{Id}")]
        public async Task<IActionResult> DeleteCity(int Id)
        {
            var city = await dc.City.FindAsync(Id);
             dc.City.Remove(city);
            await dc.SaveChangesAsync();
            return Ok(Id);
        }

        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "Atlanta";
        }
    

    }
}
