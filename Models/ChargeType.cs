using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    [Serializable]
    public class ChargeTypeList
    {
        public String ChargeTypeCode { get; set; }
        public String ChargeType { get; set; }
    }

    public class ChargeTypes
    {
        public string LabTrav { get; set; }
        public bool New { get; set; }
        [Required]

        [MaxLength(5, ErrorMessage = "Maximum length of field is 5 characters...")]
        public String ChargeTypeCode { get; set; }
        [Required]
        [MaxLength(50, ErrorMessage = "Maximum length of field is 50 characters...")]
        public String ChargeTypeDesc { get; set; }

        [Required]
        public decimal Rate { get; set; }

    }

}