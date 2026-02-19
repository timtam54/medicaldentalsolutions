using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class Retirement
    {
        public Int32 EquipID { get; set; }
        public string RetirementComment { get; set; }
        public bool BERDisposal { get; set; }
        public bool Obselete { get; set; }
        public bool PartsNotAvailable { get; set; }
        public bool NoLongerRequired { get; set; }
        public DateTime? DateRetired { get; set; }
    }
}