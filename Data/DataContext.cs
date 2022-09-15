using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.DTO;
using WebAPI.Models;
using WebAPI.ViewModels;

namespace WebAPI.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        public DbSet<City> City { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Property> Properties { get; set; }

        public DbSet<PropertyType> PropertyTypes { get; set; }

        public DbSet<FurnishingType> FurnishingTypes { get; set; }
        public DbSet<VMProperties> VMProperties { get; set; }
        public DbSet<PropertyListDTO> PropertyListDTO { get; set; }
        public DbSet<KeyValuePairDTO> KeyValuePairDTO { get; set; }
        public DbSet<PhotoDTO> PhotoDTO { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PhotoDTO>()
                .HasNoKey()
                .ToView("PhotoDTOView");
        }
    }
}
