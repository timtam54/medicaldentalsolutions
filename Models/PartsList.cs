using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class PartsList
    {
        [DisplayName("Part Name")]
        [UIHint("Parts")]
        public string PartName { get; set; }
        public Int16 NoOfParts { get; set; }
        public decimal Cost { get; set; }

    }

    [Serializable]
    public class PartsUsedList
    {      
        public int Id { get; set; }
        public int PartId { get; set; }
        [DisplayName("Stocked Part")]
        public bool Stocked_Part { get; set; }
        [DisplayName("Part Description")]
        [UIHint("Parts")]
        public string PartName { get; set; }
        [DisplayName("Part Number")]
        public string PartNumber { get; set; }
        [DisplayName("# of units used")]
        public Int32 NoOfParts { get; set; }
        [DisplayName("$ Sell Price Ex Tax")]
       [UIHintAttribute("new { style = 'width: 240px' }")]
        public decimal Price { get; set; }
       
    }

}