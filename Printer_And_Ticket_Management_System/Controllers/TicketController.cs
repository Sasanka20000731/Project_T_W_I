using Printer_And_Ticket_Management_System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Printer_And_Ticket_Management_System.Models;
using System.Threading.Tasks;
using System.Collections;

namespace Printer_And_Ticket_Management_System.Controllers
{
    public class TicketController : Controller
    {
        ProjectDBEntities db = new ProjectDBEntities();
        // View Ticket dashboard
        public ActionResult TicketDashBoard()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }
        
        //View add ticket form
        public ActionResult addTicket(Ticket ST)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            int lastID = 0; 
            lastID = db.Table_Ticket.Max(t => t.TicketID);
            ST.TicketID = lastID + 1;

            ArrayList printers = new ArrayList();

            using (var context = new ProjectDBEntities()) 
            {


                var loggeduser = Session["Username"];
                var branchID = context.Table_UserDetail.Where(u => u.FirstName == loggeduser.ToString()).Select(u => u.BranchID).FirstOrDefault();
             
                ArrayList Plist = new ArrayList(context.Table_PrinterAllocation.Where(a => a.BranchID == branchID).Select(a => a.PrinterID).ToList());
                List<int> printerIDs = Plist.OfType<int>().ToList();
                ST.PrinterIDs = printerIDs;

                ST.BranchID = Convert.ToInt32(branchID);

            }




            return View("addTicket", ST);
        }

        //Add ticket to the database
        [HttpPost]
        public ActionResult AddTicket(Ticket TT)
        {
            int TicketNum = TT.TicketID;
            string RR = "Add Ticket :" + TicketNum.ToString() + " Ticket Added by : " + Session["Username"].ToString();
            string Action = "Add ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

           
         
            Table_Ticket TKT =  new Table_Ticket();
            TKT.TicketID = TT.TicketID;
            TKT.TicketDescription = TT.TicketDescription;
            TKT.BranchID = TT.BranchID;
            TKT.PrinterID = Convert.ToInt32(TT.PrinterID);
            TKT.urgency = TT.urgency;
            TKT.CreatedDate = DateTime.Now;
            db.Table_Ticket.Add(TKT);
            db.SaveChanges();

            ModelState.Clear();
            TT = new Ticket();

            using (var context = new ProjectDBEntities())
            {
     
                TT.PrinterIDs = context.Table_Printer.Select(p => p.PrinterID).ToList();
                TT.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();
   
            }

            return View("addTicket",TT);
        }

        //View ViewTicket page
        public ActionResult viewTicket()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var tickets = db.Table_Ticket;

            ViewBag.UserRole = Session["UserRole"].ToString();

            return View(tickets);
        }

        //Edit ticket form
        public ActionResult editTicket(Ticket TT)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var id = Request["Tid"];
            int A = Convert.ToInt32(id);
            Table_Ticket TblT = db.Table_Ticket.SingleOrDefault(x => x.TicketID == A);
            TT.TicketID = TblT.TicketID;
            TT.TicketDescription = TblT.TicketDescription;
            TT.PrinterID = TblT.PrinterID;
            TT.urgency = TblT.urgency;
            TT.CreatedDate = TblT.CreatedDate.ToString();
            TT.BranchID = TblT.BranchID;

            using (var context = new ProjectDBEntities())
            {

                var loggeduser = Session["Username"];
                var branchID = context.Table_UserDetail.Where(u => u.FirstName == loggeduser.ToString()).Select(u => u.BranchID).FirstOrDefault();

                ArrayList Plist = new ArrayList(context.Table_PrinterAllocation.Where(a => a.BranchID == branchID).Select(a => a.PrinterID).ToList());
                List<int> printerIDs = Plist.OfType<int>().ToList();
                TT.PrinterIDs = printerIDs;

                TT.BranchID = Convert.ToInt32(branchID);

            }
            return View(TT);
        }

        //Update ticket in the database
        [HttpPost]
        public async Task<ActionResult> UpdateTicket(Ticket ticket)
        {
            int TicketNum = ticket.TicketID;
            string reason = ticket.Reason.ToString();
            string RR = "Edit Ticket :"+reason+" " + TicketNum.ToString()+" Updated by : " + Session["Username"].ToString();
            string Action = "Update ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            try
            {
                var entity = await db.Table_Ticket.FindAsync(ticket.TicketID);
                if (entity != null)
                {
                    entity.TicketDescription = ticket.TicketDescription;
                    entity.PrinterID = ticket.PrinterID;
                    entity.urgency = ticket.urgency;
                    entity.BranchID = ticket.BranchID;
                    await db.SaveChangesAsync();
                    return RedirectToAction("viewTicket");
                }
                ViewBag.Error = "Ticket not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating Ticket: " + ex.Message;
            }
            return View();
        }

        //Delete ticket form
        public ActionResult DeleteTicket(Ticket TD)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var TID_D = Request["Tid_d"];
            int TID = Convert.ToInt32(TID_D);
            Table_Ticket tkt = db.Table_Ticket.SingleOrDefault(x => x.TicketID == TID);
            TD.TicketID = tkt.TicketID;
            TD.TicketDescription = tkt.TicketDescription;
            TD.CreatedDate = tkt.CreatedDate.ToString();
            return View("DeleteTicket", TD);
        }

        //Delete ticket from the database
        public async Task<ActionResult> Delete_Ticket()
        {
            var tid_D = Request["Tid"];
            int TID = Convert.ToInt32(tid_D);


            var TicketNum = tid_D;
            string RR = "Delete Ticket :" + TicketNum.ToString() + " Deleted by : " + Session["Username"].ToString();
            string Action = "Delete ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            var entity = await db.Table_Ticket.FindAsync(TID);
            if (entity != null)
            {
                db.Table_Ticket.Remove(entity);
                db.SaveChanges();
                await db.SaveChangesAsync();
                return RedirectToAction("viewTicket");
            }
            ViewBag.Error = "Somthing went wrong";



            return View(ViewBag.Error);
        }

        //View Ticket status form  
        public ActionResult TicketStatus()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var tickets = db.Table_Ticket;


            ViewBag.UserRole = Session["UserRole"].ToString();


            return View(tickets);
        }
        
        //Ticket open 
        public async Task<ActionResult> openTicket(Ticket ticket)
        {
            var tktID = Request["otid"];
            int TID = Convert.ToInt32(tktID);
            var openBY = Session["LoggedUser"].ToString();
            int OB = Convert.ToInt32(openBY);

            try
            {
                var entity = await db.Table_Ticket.FindAsync(TID);
                if (entity != null)
                {
                    var comment = "Open" ;
                    entity.States = comment;
                    entity.OpenedBy = OB;
                    entity.OpenedDate = DateTime.Now;
                 
                    await db.SaveChangesAsync();

                    string RR = "Open Ticket :" + TID.ToString() + " Ticket Opened by : " + Session["Username"].ToString();
                    string Action = "Open Ticket ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    return RedirectToAction("TicketStatus");
                }
                ViewBag.Error = "Ticket not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating Ticket: " + ex.Message;
            }

          

            return View("TicketStatus");
        }

        //Ticket close
        public async Task<ActionResult> closeTicket(Ticket ticket)
        {
            var tktID = Request["ctid"];
            int TID = Convert.ToInt32(tktID);
            var ClosedBY = Session["LoggedUser"].ToString();
            
            try
            {
                var entity = await db.Table_Ticket.FindAsync(TID);
                if (entity != null)
                {
                    DateTime dt = DateTime.Now;
                    var comment = "Close";
                    entity.ClosedBy = ClosedBY.ToString();
                    entity.States = comment;
                    entity.ClosedDate = dt;

                    string RR = "Close Ticket :" + TID.ToString() + " Ticket Closed by : " + Session["Username"].ToString();
                    string Action = "Close Ticket ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    await db.SaveChangesAsync();
                    return RedirectToAction("TicketStatus");
                }
                ViewBag.Error = "Ticket not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating Ticket: " + ex.Message;
            }

           

            return View();
        }

        //View ticket search page
        public ActionResult TicketSeearch()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Search ticket from the databse
        [HttpPost]
        public ActionResult TicketSeearch(Ticket TT)
        {
           var TID =  TT.TicketID;

            Table_Ticket TblT = db.Table_Ticket.SingleOrDefault(x => x.TicketID == TID);
            if (TblT != null)
            {
            TT.TicketID = TblT.TicketID;
            TT.TicketDescription = TblT.TicketDescription;
            TT.PrinterID = TblT.PrinterID;
            TT.urgency = TblT.urgency;
            TT.CreatedDate = TblT.CreatedDate.ToString();
            TT.BranchID = TblT.BranchID;
            TT.OpenedDate = TblT.OpenedDate;
            TT.ClosedDate = TblT.ClosedDate;
            TT.OpenedBy = TblT.OpenedBy.ToString();

            return View("TicketSeearch", TT);
            }
            else
            {
                ViewBag.ErrorMessage = "Enter a valid Ticket ID.";
                return View ();
            }
        }

        //Insert data to audit trail
        public void InsertDataToSPAuditTrail(string Case, string Reason, DateTime date)
        {

            using (var context = new ProjectDBEntities())
            {
                var result = context.SP_InsertDataToTableAuditTrail(Case, Reason, date);

            }

        }


    }
}