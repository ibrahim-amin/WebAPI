using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Data;
using WebAPI.DTO;
using WebAPI.Models;
using static System.Net.Mime.MediaTypeNames;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    [Authorize]
    public class PropertyController : ControllerBase
    {
        private readonly DataContext dc;

        public PropertyController(DataContext dc)
        {
            this.dc = dc;
        }

        protected int GetUserID()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        }


        [HttpGet("list/{SellRent}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyList(int SellRent)
        {
            /*  var data = await Task.FromResult(dc.Properties.Where(p => p.SellRent == SellRent).
                  Include(p => p.PropertyType).
                  Include(p => p.City).Include(p => p.FurnishingType).ToList());
  */
            var data = await Task.FromResult(dc.PropertyListDTO.FromSqlRaw(
                 @" select P.*,Pt.Name as PropertyType,C.Name CityName, FT.Name FurnishType,Pho.ImageUrl as photo from
                    properties P inner join PropertyTypes PT on PT.Id = P.PropertyTypeId
                    inner join City C on C.Id = P.CityId inner join FurnishingTypes
                    FT on Ft.Id = P.FurnishingTypeId
                    left join
                    (select PropertyId, ImageUrl from Photos where IsPrimary = 1) as Pho on pho.PropertyId = p.Id where P.SellRent={0}", SellRent).ToList());

            return Ok(data);
        }

        [HttpGet("GetPropertyDetail/{propertyId}")]
        [AllowAnonymous]

        public async Task<IActionResult> GetPropertyDetail(int propertyId)
        {


            var data1 = await Task.FromResult(dc.VMProperties.FromSqlRaw(
                 @"select P.*,
                  Pt.Name as PropertyType,C.Name CityName, FT.Name FurnishType from 
                 properties P inner join PropertyTypes PT on PT.Id = P.PropertyTypeId 
            inner join City C on C.Id = P.CityId inner join FurnishingTypes FT
            on Ft.Id = P.FurnishingTypeId  where P.Id={0}", propertyId).First());

            var Photos = await Task.FromResult(
               dc.PhotoDTO.FromSqlRaw("select PublicId,ImageUrl,IsPrimary,PropertyId from Photos WHERE PropertyId ={0}", propertyId

               ).ToList()

               );
            data1.Photos = Photos;
            return Ok(data1);
        }






        [HttpPost("AddProperty")]
        [Authorize]
        public async Task<IActionResult> AddProperty([FromForm]  PropertyDTO prop)
        {
            try
            {
                var property = new Property();
                property.SellRent = prop.SellRent;
                property.BHK = prop.BHK;
                property.PropertyTypeId = prop.FurnishingTypeId;
                property.Name = prop.Name;
                property.CityId = prop.CityId;
                property.FurnishingTypeId = prop.FurnishingTypeId;
                property.Price = prop.Price;
                property.Security = prop.Security;
                property.Maintenance = prop.Maintenance;
                property.BuiltArea = prop.BuiltArea;
                property.CarpetArea = prop.CarpetArea;
                property.FloorNo = prop.FloorNo;
                property.TotalFloors = prop.TotalFloors;
                property.Address = prop.Address;
                property.Address2 = prop.Address2;
                property.ReadyToMove = prop.ReadyToMove;
                property.Gated = prop.Gated;
                property.MainEntrance = prop.MainEntrance;
                property.EstPossessionOn = prop.EstPossessionOn;
                property.Description = prop.Description;
                property.PostedBy = GetUserID();
                property.LastUpdatedBy = GetUserID();
                //property.Id = 90;
                await dc.Properties.AddAsync(property);
                await dc.SaveChangesAsync();
                //return StatusCode(201);

                //file upload 
                var datas = Request.Form.Files;
                var file = datas[0];
                try
                {
                    //var file = Request.Form.Files[0];
                    var folderName = Path.Combine("Resources", "Images");
                    var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                    if (datas[0].Length > 0)
                    {


                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                        string TimeStamp = DateTime.Now.ToString("yyyMMddHmmss");
                        string imageExtention = System.IO.Path.GetExtension(file.FileName);
                        string CompletePath = TimeStamp + imageExtention;
                        fileName = CompletePath.Replace(" ", "_");

                        var fullPath = Path.Combine(pathToSave, fileName);
                        // var dbPath = Path.Combine(folderName, fileName);
                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }

                        var photo = new Photo();
                        photo.LastUpdatedOn = DateTime.Now;
                        photo.LastUpdatedBy = GetUserID();
                        photo.PublicId = fullPath;
                        photo.ImageUrl = fileName;
                        int propid = property.Id;

                        int data = dc.Photos.Where(e => e.PropertyId == propid).Count();
                        //var data = dc.Database.ExecuteSqlCommand("select count(*) from Photos where PropertyId ="+ propid);
                        if (data > 0)
                        {
                            photo.IsPrimary = false;

                        }
                        else
                        {
                            photo.IsPrimary = true;
                        }
                        photo.PropertyId = propid;

                       await dc.Photos.AddAsync(photo);
                        await dc.SaveChangesAsync();
                        // return StatusCode(201);


                        return Ok(201);
                    }
                    else
                    {
                        return BadRequest(500);
                    }

                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Internal server error: { ex}");
                }


            }
            catch (Exception ex)
            {
                return Ok(ex.Message);
            }
        }


        [HttpPost("Add/Photo/{propid}")]
        [AllowAnonymous]
        //[Authorize]
        public IActionResult AddPhoto(int propid)
        {
            var datas = Request.Form.Files;
            var file = datas[0];
            try
            {
                //var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (datas[0].Length > 0)
                {


                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    string TimeStamp = DateTime.Now.ToString("yyyMMddHmmss");
                    string imageExtention = System.IO.Path.GetExtension(file.FileName);
                    string CompletePath = TimeStamp + imageExtention;
                    fileName = CompletePath.Replace(" ", "_");
                    
                    var fullPath = Path.Combine(pathToSave, fileName);
                    // var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }

                    var photo = new Photo();
                    photo.LastUpdatedOn = DateTime.Now;
                    photo.LastUpdatedBy = GetUserID();
                    photo.PublicId = fullPath;
                    photo.ImageUrl = fileName;
                    int data = dc.Photos.Where(e => e.PropertyId == propid).Count();
                    //var data = dc.Database.ExecuteSqlCommand("select count(*) from Photos where PropertyId ="+ propid);
                    if (data > 0)
                    {
                        photo.IsPrimary = false;

                    }
                    else
                    {
                        photo.IsPrimary = true;
                    }
                    photo.PropertyId = propid;

                    dc.Photos.AddAsync(photo);
                    dc.SaveChangesAsync();
                    // return StatusCode(201);


                    return Ok(201);
                }
                else
                {
                    return BadRequest(500);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: { ex}");
            }

        }


        [HttpPost("set-primary-photo/{propid}/{ImgUrl}")]
       [Authorize]
        public async Task<IActionResult> SetPrimaryPhoto(int propid,string ImgUrl) {
            var trans = dc.Database.BeginTransaction();
            try
            {
                var data = dc.Properties.Where(e => e.Id == propid).ToList();
                if (data is null)
                {
                    return BadRequest("Property does not exists");
                }

                var photo = await Task.FromResult(dc.Photos.Where(e => e.ImageUrl == ImgUrl).SingleOrDefault());
                if (photo == null)
                {
                    return BadRequest("Photo Deos not exist");
                }
                if (photo.IsPrimary == true)
                {
                    return BadRequest("Photo is already Primary");
                }
                if (photo.LastUpdatedBy != GetUserID())
                {
                    return BadRequest("you are not authorrized");
                }
                dc.Database.ExecuteSqlRaw("Update photos set IsPrimary =0 where PropertyId={0}", propid);
                photo.IsPrimary = true;
                dc.Entry(photo).State = EntityState.Modified;
                await dc.SaveChangesAsync();
                trans.Commit();
                return NoContent();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return BadRequest("errro occured");
            }
        }




        [HttpDelete("delete-photo/{propid}/{ImgUrl}")]
        [Authorize]
        public async Task<IActionResult> DeletePrimaryPhoto(int propid, string ImgUrl)
        {
            var trans = dc.Database.BeginTransaction();
            try
            {
                var data = dc.Properties.Where(e => e.Id == propid).ToList();
                if (data is null)
                {
                    return BadRequest("Property does not exists");
                }

                var photo = await Task.FromResult(dc.Photos.Where(e => e.ImageUrl == ImgUrl).SingleOrDefault());
                if (photo == null)
                {
                    return BadRequest("Photo Deos not exist");
                }
                if (photo.IsPrimary)
                {
                    return BadRequest("You can not delete Primary Photo");
                }
                if (photo.LastUpdatedBy != GetUserID())
                {
                    return BadRequest("you are not authorrized");
                }
                

                string file_name = photo.ImageUrl;
                
                var folderName = Path.Combine("Resources", "Images",file_name);

                string path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                FileInfo file = new FileInfo(path);
                if(file.Exists)//check file exsit or not  
                {
                //return Ok(path);
            
                      file.Delete();
                }
                dc.Photos.Remove(photo);
                await dc.SaveChangesAsync();
                trans.Commit();

                
                return NoContent();
            }
            catch (Exception ex)
            {
                trans.Rollback();
                return BadRequest("errro occured");
            }
        }




    }


}



