using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class Allocation
    {
        public int AllocationID { get; set; }
        public int PrinterID { get; set; }
        public int BranchID { get; set; }
        public Nullable<System.DateTime> AllocatedDate { get; set; }
        public string Remark { get; set; }

        public string Reason { get; set; }


        public List<int> PrinterIDs { get; set; }
        public List<int> BranchList { get; set; }
    }
}