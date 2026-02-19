using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipModelForm
    {
        public string EquipTypeCode { get; set; }
        public string Manufacturer { get; set; }
        public string EquipModel { get; set; }
        public string Notes { get; set; }
        public int ModelUID { get; set; }
        public List<EquipmentTypeList> EquipTypeList { get; set; }
        public List<ManufactureList> ManufactureList { get; set; }
    }
}