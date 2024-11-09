using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class AuditTrailRpt
    {
        public int selectedReport { get; set; }
        public string fromDate { get; set; }
        public string toDate { get; set; }

    }
}