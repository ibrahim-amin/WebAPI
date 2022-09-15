using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DTO
{
    public class PropertyListDTO

    {
        public int SellRent { get; set; }
        public string Name { get; set; }
        public int PropertyTypeId { get; set; }
        public int FurnishingTypeId { get; set; }
        public int Price { get; set; }
        public int BHK { get; set; }
        public int BuiltArea { get; set; }
        public int CityId { get; set; }
        public bool ReadyToMove { get; set; }
        public int CarpetArea { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public int FloorNo { get; set; }
        public int TotalFloors { get; set; }
        public string MainEntrance { get; set; }
        public int Security { get; set; }
        public bool Gated { get; set; }
        public int Maintenance { get; set; }
        public DateTime EstPossessionOn { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }

        public DateTime PostedOn { get; set; }

        public int PostedBy { get; set; }
        public string PropertyType { get; set; }
        public string CityName { get; set; }
        public string FurnishType { get; set; }
        public int Id { get; set; }
        public DateTime LastUpdatedOn { get; set; }
        public int LastUpdatedBy { get; set; }
        public string photo { get; set; }
     

    }
}

