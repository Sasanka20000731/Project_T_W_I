//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Printer_And_Ticket_Management_System.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Table_PrinterAllocation
    {
        public int AllocationID { get; set; }
        public Nullable<int> PrinterID { get; set; }
        public Nullable<int> BranchID { get; set; }
        public Nullable<System.DateTime> AllocatedDate { get; set; }
        public string Remark { get; set; }
    
        public virtual Table_Branch Table_Branch { get; set; }
        public virtual Table_Printer Table_Printer { get; set; }
    }
}
