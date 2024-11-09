using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class Branch
    {
        public int BranchID { get; set; }
        public string BranchName { get; set; }
        public string BranchLocation { get; set; }

        public string Reason { get; set; }
    }
}