using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipmentModel
    {
        public string ManufactureId { get; set; }
        public string Equiptype { get; set; }
        public string ModelContain { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<ManufactureList> ManufactureList { get; set; }
        public List<EquipmentModelList> EquipmentModelList { get; set; } 
    }
}