using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class AuditSelect
    {

        public List<DB.Branch> branches { get; set; }

        public List<string> users { get; set; }
    }
}