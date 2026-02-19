using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class EngineerSearch
    {
        public string EngineerName { get; set; }
        public bool PopUp { get; set; }
        public List<EngineerList> Engineers { get; set; }
    }
}