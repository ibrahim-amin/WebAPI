using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Data;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {
        private readonly DataContext dc;

        public PropertyTypeController(DataContext dc)
        {
            this.dc = dc;
        }

        [HttpGet("list")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyType()
        {
       
            var data = await Task.FromResult(dc.KeyValuePairDTO.FromSqlRaw(
                 "select * from PropertyTypes").ToList());

            return Ok(data);
        }

    }
}
