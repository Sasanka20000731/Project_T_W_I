using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Printer_And_Ticket_Management_System.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public Nullable<System.DateTime> DateOfBirth { get; set; }
        public Nullable<int> BranchID { get; set; }
        public string Passowrd { get; set; }

        public string EmpID { get; set; }

        public string UserRole { get; set; }
        public string Reason { get; set; }

        public string Status { get; set; }

        public List<int> BranchList { get; set; }

    }
}