using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class Printer
    {
        public int PrinterID { get; set; }
        public string PrinterCode { get; set; }
        public string BrandName { get; set; }
        public Nullable<System.DateTime> PurchasedDate { get; set; }
        public string VendorName { get; set; }
        public string VendorContact { get; set; }
        public Nullable<int> WarrantyPeriod { get; set; }

        public string Reason { get; set; }
    }
}