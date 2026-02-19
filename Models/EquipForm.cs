using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EquipForm
    {
        public List<TestScirpt> Tests { get; set; }
        public string Code { get; set; }
        public string ECode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool PopUp { get; set; }
    }
}