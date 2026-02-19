using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class Signature
    {
       public string SignatureDataUrl { get; set; }
        public int BranchID { get; set; }
        public int ID { get; set; }
        public string RepairOrService { get; set; }

    }
}