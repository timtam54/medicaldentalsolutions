using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class RepairEquip
    {

        public TechClockOnOffStatus TechClockOnPrompt { get; set; }

        public DB.Repair Repair { get; set; }
        public DB.Equipment Equipment { get; set; }
        public string Status { get; set; }

        public string EquipTypeCode { get; set; }
        public List<IdText> MakeModels { get; set; }

        public string Command { get; set; }

    }

    public class ServiceEquip
    {
        public DB.Service Service { get; set; }
        //public DB.Equipment Equipment{get; set;}

        public List<IdText> CustomerEquipment { get; set; }
        public bool ServiceFunction { get; set; }
        //public List<IdText> MakeModels { get; set; }
//        public bool ItemNotSeen { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Serviced Date")]
        public DateTime? DateServiced { get; set; }
    }

    public enum TechClockOnOffStatus
    {
        ClockOn_NoActiveShifts=0,
        NoClockOn_ActiveShiftExists = 1,
        NoClock_JobCompleted=2
    }
    public class ServiceJobService
    {

        public TechClockOnOffStatus TechClockOnPrompt { get; set; }
        public string Command { get; set; }
        public DB.ServiceJob ServiceJob { get; set; }
        public List<ServiceEquip> Services { get; set; }
        public List<DB.Engineer> Engineers { get; set; }

        public string ServicesIDs { get; set; }
        public DB.Customer Customer { get; set; }
        public string Status { get; set; }

        public int EquipUID { get; set; }
    }

}