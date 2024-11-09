using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class Ticket
    {
 

        public int TicketID { get; set; }
        public string TicketDescription { get; set; }
        public Nullable<int> PrinterID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string urgency { get; set; }
        public Nullable<System.DateTime> OpenedDate { get; set; }
        public Nullable<System.DateTime> ClosedDate { get; set; }
        public string OpenedBy { get; set; }
        public Nullable<bool> States { get; set; }
        public string CreatedDate { get; set; }
        public string Reason { get; set; }

        public List<int> PrinterIDs { get; set; }
        public List<int> BranchList { get; set; }
    }
}