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

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FurnishingTypeController : ControllerBase
    {

        private readonly DataContext dc;

        public FurnishingTypeController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetFurnishingType()
        {
          


          var data = await Task.FromResult(dc.KeyValuePairDTO.FromSqlRaw(
                 "select * from FurnishingTypes").ToList());

            return Ok(data);
        }
    }
}
