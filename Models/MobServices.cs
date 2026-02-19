using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MDS.Models
{
    public class MobServices
    {
        public int EquipmentID { get; set; }
        public string EquipmentSerialNo { get; set; }
        public DateTime? DateServiced { get; set; }
        public string EquipmentType { get; set; }
        public string EquipmentModel { get; set; }
        public string WorkDone { get; set; }
        public string JobCode { get; set; }
        public string Customer { get; set; }
    }

    public class MobRepairs
    {
        public string JobNo { get; set; }
        public int RepairID { get; set; }
        public int EquipmentID { get; set; }
        public string Equipment { get; set; }
        public DateTime? DateServiced { get; set; }
        public string Fault { get; set; }
        public decimal? Amount { get; set; }
        public string Customer { get; set; }
    }

    public class MobServiceJobs
    {
        public string JobNo { get; set; }
        public int ServiceJobID { get; set; }
        //public int EquipmentID { get; set; }
        //public string Equipment { get; set; }
        public DateTime? DateStart { get; set; }
        //public string Fault { get; set; }
        //public decimal? Amount { get; set; }
        public string Customer { get; set; }
    }

    public class MobServiceJob
    {
        public string ID { get; set; }
        public string Typ { get; set; }
        public List<MobServiceJobs> sjs { get; set; }
    }

    public class MobRepair
    {
        public string ID { get; set; }
        public string Typ { get; set; }
        public List<MobRepairs> reps { get; set; }
    }

    public class MobEquip
    {
        public MDS.DB.Equipment Equip { get; set; }
        public List<MDS.DB.Repair> Repairs { get; set; }
        public string BranchPrefix { get; set; }
        public List<MDS.DB.ServiceJob> Services { get; set; }

    }

    public class MobRep
    {
        public MDS.DB.Repair rep { get; set; }
        public List<MDS.DB.Part> parts { get; set; }


    }


    //public class MobEquip
    //{
    //    public MDS.DB.Equipment reps { get; set; }

    //}
}