using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class ServiceworkorderList
    {     
        public string CompanyName { get; set; }
        public string ServiceArea { get; set; }
        public string Location { get; set; }
        public string Equipment { get; set; }
        public string WorkDone { get; set; }
        public DateTime? DateServiced { get; set; }
        public string EngineerName { get; set; }
        public int ServiceId { get; set; }
        public string NotesForNextService { get; set; }
        public string NotesForThisService { get; set; }
        public decimal? PartsCost { get; set; }
        public DateTime? WarrantyExpirationDate { get; set; }
    }
}