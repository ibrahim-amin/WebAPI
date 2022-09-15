using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Middleweres
{
    public class ExceptionMeddleware
    {
        private readonly RequestDelegate next;

        public ExceptionMeddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch(Exception ex)
            {
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync(ex.Message);
            }
        }

    }

}