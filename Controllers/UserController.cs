using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext dc;
        private readonly IConfiguration configuration;

        public UserController(DataContext dc, IConfiguration configuration)
        {
            this.dc = dc;
            this.configuration = configuration;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginReqdto LoginReq)
        {
           // var User = await Task.firstor( dc.City.ToList());
           var user= await dc.Users.FirstOrDefaultAsync(X => X.Username == LoginReq.Username 
           //&& X.Password==LoginReq.Password
           );

            if (user == null)
            {
                return Unauthorized("Invalid Username or Password");
            }

            using (var hmac = new HMACSHA512(user.PasswordKey))
            {
               var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(LoginReq.Password));
                for(int i=0; i<passwordHash.Length; i++)
                {
                    if(passwordHash[i] != user.Password[i])
                    {
                        return BadRequest("Password not matched");
                    }
                }
            
            }

            var loginres = new LoginRes();
            loginres.Username = user.Username;
            // loginres.Token = "Token to be Generated";
            loginres.Token = CreateJWT(user);
            return Ok(loginres);
          
        }


        [HttpPost("Register")]
        public async Task<IActionResult> Register(LoginReqdto LoginReq)
        {
            if (string.IsNullOrEmpty(LoginReq.Username.Trim()) || string.IsNullOrEmpty(LoginReq.Password.Trim()))
            {
                return BadRequest("User name or password Empty");

            }

            if (await UserAlreadyExist(LoginReq.Username))
            {
                return BadRequest("user Already Exist");
            }

            byte[] passwordHash, passwordKey;
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(LoginReq.Password));
            }
            User user = new User();
            user.Username = LoginReq.Username ;
            user.Password = passwordHash;
            user.PasswordKey = passwordKey;
            dc.Users.Add(user);
            await dc.SaveChangesAsync();
            return StatusCode(200);

        }

        public async Task<bool> UserAlreadyExist(string username)
        {
            return await dc.Users.AnyAsync(x => x.Username == username);

          

        }
    /*    public void Register(string username, string password)
        {
            byte[] passwordHash, passwordKey;
            using (var hmac = new HMACSHA512())
            {
                passwordKey = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            User user = new User();
            user.Username = username;
            user.Password = passwordHash;
            user.PasswordKey = passwordKey;
        }*/

            public string CreateJWT(User user)
        {
            var SecretKey = configuration.GetSection("AppSetting:SecretKey").Value;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var claims = new Claim[]
                { 
                  new Claim(ClaimTypes.Name,user.Username),
                  new Claim(ClaimTypes.NameIdentifier,user.Id.ToString())
                };
            var signingCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(1),
                SigningCredentials = signingCredential
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
           


        }


    }
}
