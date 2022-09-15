using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.Extentions;
using WebAPI.Middleweres;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Default")));
            services.AddControllers();
            services.AddCors();
            var SecretKey = Configuration.GetSection("AppSetting:SecretKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        IssuerSigningKey = key,

                    };
                }
                );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseStaticFiles(new StaticFileOptions()
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Resources\Images")),
                RequestPath = new PathString("/Resources/Images")
            }
                );
             app.ConfigureExceptionHandler(env);
            //app.UseMiddleware<ExceptionMeddleware>();
            app.UseRouting();
            app.UseCors(m => m.AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin());
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
