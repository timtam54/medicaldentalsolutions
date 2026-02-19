using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipManufacturerSearch
    {
        public string EquipManufacturerName { get; set; }
        public List<EquipManufacturerList> Manufacturer { get; set; }
        public bool PopUp { get; set; }
    }
}