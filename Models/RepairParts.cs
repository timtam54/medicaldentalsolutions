using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class RepairParts
    {
        public Int16 PartID { get; set; }
        public String PartName { get; set; }
        public Int16 NoOfParts { get; set; }
        public Decimal Cost { get; set; }
        public String FaultDetail { get; set; }
        public String WorkDone { get; set; }
    }

    public class RepairPhotos
    {
        public Int32 Id { get; set; }
        public List<DB.Photo> Photos { get; set; }
    }


}