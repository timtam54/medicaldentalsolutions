using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipTypeSearch
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<EquipTypeList> EquipTypes { get; set; }
        public bool PopUp { get; set; }
    }
}