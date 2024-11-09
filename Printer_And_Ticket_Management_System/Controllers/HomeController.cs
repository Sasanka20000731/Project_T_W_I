using Printer_And_Ticket_Management_System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Printer_And_Ticket_Management_System.Models;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Data.Entity.Core.Objects;
using QuickMailer;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using CrystalDecisions.CrystalReports.Engine;
using System.IO;

namespace Printer_And_Ticket_Management_System.Controllers
{
    public class HomeController : Controller
    {
        //Database object
        ProjectDBEntities db = new ProjectDBEntities();

        //View dashboard 
        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
   
            {
                int ticketCount = db.Table_Ticket.Count();
                int branchCount = db.Table_Branch.Count();
                int printerCount = db.Table_Printer.Count();
                int userCount = db.Table_UserDetail.Count();
                int displayPendingCount = db.Table_Ticket.Count(t => t.States == null);
                int displayOpenedCount = db.Table_Ticket.Count(t => t.States == "Open" && t.States != "Close");


                ViewBag.TicketCount = ticketCount;
                ViewBag.BranchCount = branchCount;
                ViewBag.PrinterCount = printerCount;
                ViewBag.UserCount = userCount;
                ViewBag.DisTickets = displayPendingCount;
                ViewBag.DisOpened = displayOpenedCount;

                var lastEmployee = db.Table_Ticket.OrderByDescending(e => e.TicketDescription).FirstOrDefault();
                ViewBag.LastTicket = lastEmployee.ToString();

                // Retrieve ticket count for each BranchID
                var ticketData = db.Table_Ticket
                    .GroupBy(t => t.BranchID)
                    .Select(g => new { BranchID = g.Key, Count = g.Count() })
                    .ToList();

                // Get the unique BranchIDs and ticket counts
                var branchIDs = ticketData.Select(d => d.BranchID).ToArray();
                var counts = ticketData.Select(d => d.Count).ToArray();

                ViewBag.ChartBranchIDs = branchIDs;
                ViewBag.ChartCounts = counts;


            }

            ViewBag.UserRole = Session["UserRole"].ToString();
            return View();
        }

        //View Login function
        public ActionResult LoginFunction()
        {
            if (Session["Username"] != null)
            {
                return RedirectToAction("Index", "Home", new { Username = Session["Username"].ToString() });
            }
            else 
            {
                return View();
            } 
        }

        //Check user credentials and log in to systemm
        [HttpPost]
        public ActionResult LoginFunction(User US)
        {
            if (ModelState.IsValid)
            {
                var user = db.Table_UserDetail.FirstOrDefault(u => u.FirstName == US.FirstName && u.Passowrd == US.Passowrd);
          

                if (user != null && user.Status == true)
                {
                    var logedUser = db.Table_UserDetail.SingleOrDefault(x => x.FirstName == US.FirstName && x.Passowrd == US.Passowrd);
                    var Role = logedUser?.UserRole?.ToString();
                    Session["Username"] = US.FirstName;
                    Session["LoggedUser"] = logedUser.EmpID ;
                    Session["UserRole"] = Role;
                    return RedirectToAction("Index", "Home"); // Redirect to the desired page after successful login
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            // If login failed, display the error message
            ViewBag.ErrorMessage = "Invalid username or password";

            return View(US);
        }


        //Log out from the system
        public ActionResult LogOut()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("LoginFunction", "Home"); // Redirect to the home page after logout
        }

        //View about
        public ActionResult About()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            ViewBag.Message = "Your application description page.";
            return View();
        }

        //View contact page
        public ActionResult Contact()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            ViewBag.Message = "Your contact page.";
            return View();
        }

        //View add user form
        public ActionResult AddUser(User Ud)
        {
            //if (Session["Username"] == null)
            //{
            //    return RedirectToAction("LoginFunction", "Home");
            //}

            //int lastID = 0;
            //lastID = db.Table_UserDetail.Max(t => t.UserID);
            //Ud.UserID = lastID + 1;


            //using (var context = new ProjectDBEntities())
            //{
       

            //    Ud.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();

            //}

            return View("AddUser", Ud);
        }

        //Add user to system and save to database
        [HttpPost]
        public ActionResult addUser(User u)
        {
            string UserName = u.UserID.ToString();
            string RR = "Create New User : ";
            string Action = "Add ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);


            Table_UserDetail tu = new Table_UserDetail();
            tu.BranchID = u.BranchID;
            tu.EmpID = u.EmpID;
            tu.ContactNumber = u.ContactNumber;
            tu.DateOfBirth = u.DateOfBirth;
            tu.FirstName = u.FirstName;
            tu.LastName = u.LastName;
            tu.Passowrd = u.Passowrd;
            tu.BranchID = u.BranchID;
            tu.UserID = u.UserID;
            tu.UserRole = u.UserRole;
            tu.Status = true;
            tu.CreatedDate = DateTime.Now;
            db.Table_UserDetail.Add(tu);
            db.SaveChanges();

            ModelState.Clear();
            u = new User();
            using (var context = new ProjectDBEntities())
            {


                u.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();

            }
            int lastID = 0;
            lastID = db.Table_UserDetail.Max(t => t.UserID);
            u.UserID = lastID + 1;

            return View("AddUser",u);
        }

        //Deactivate user view page
        public ActionResult DeactivateUser(User Udisplay)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var uid_D = Request["uid"];
            int UID = Convert.ToInt32(uid_D);
     
            Table_UserDetail TBU = db.Table_UserDetail.SingleOrDefault(x => x.UserID == UID);
            Udisplay.UserID = TBU.UserID;
            Udisplay.FirstName = TBU.FirstName;
            Udisplay.LastName = TBU.LastName;
            Udisplay.UserRole = TBU.UserRole;
            Udisplay.Status = TBU.Status.ToString();
            Udisplay.ContactNumber = TBU.ContactNumber;

            return View("DeactivateUser",Udisplay);
        }

        //Deactivate user in the system
        public async Task<ActionResult> DeactivateUserAsync(User UD)
        {

            string UserName = UD.UserID.ToString();
            string RR = "Deactivated User";
            string Action = "Deactivate ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);



            var uid_Dea = Request["UID_Dea"];
            int UID_D = Convert.ToInt32(uid_Dea);
            try
            {
                var entity = await db.Table_UserDetail.FindAsync(UID_D);
                if (entity != null)
                {
                    entity.UserID = UID_D;
                    entity.Status = false;
                    await db.SaveChangesAsync();
                    return RedirectToAction("user");
                }
                ViewBag.Error = "User not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating user: " + ex.Message;
            }
            return View("user");
        }

        //View activate user form 
        public ActionResult ActivateUser(User Udisplay)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var uid_D = Request["uid"];
            int UID = Convert.ToInt32(uid_D);

            Table_UserDetail TBU = db.Table_UserDetail.SingleOrDefault(x => x.UserID == UID);
            Udisplay.UserID = TBU.UserID;
            Udisplay.FirstName = TBU.FirstName;
            Udisplay.LastName = TBU.LastName;
            Udisplay.UserRole = TBU.UserRole;
            Udisplay.Status = TBU.Status.ToString();
            Udisplay.ContactNumber = TBU.ContactNumber;

            return View("ActivateUser", Udisplay);
        }

        //Activate user in the system
        public async Task<ActionResult> ActivateUserAsync(User UD)
        {

            string UserName = UD.UserID.ToString();
            string RR = "Activated User";
            string Action = "Activated ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);



            var uid_Dea = Request["UID_Dea"];
            int UID_D = Convert.ToInt32(uid_Dea);
            try
            {
                var entity = await db.Table_UserDetail.FindAsync(UID_D);
                if (entity != null)
                {
                    entity.UserID = UID_D;
                    entity.Status = true;
                    await db.SaveChangesAsync();
                    return RedirectToAction("user");
                }
                ViewBag.Error = "User not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating user: " + ex.Message;
            }
            return View("user");
        }

        //View Add Allocation form
        public ActionResult AddAllocation(Allocation Ad)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            int lastID = 0;
            lastID = db.Table_PrinterAllocation.Max(t => t.AllocationID);
            Ad.AllocationID = lastID + 1;

            using (var context = new ProjectDBEntities()) 
            {
                Ad.PrinterIDs = context.Table_Printer
                    .Where(p => !context.Table_PrinterAllocation.Any(pa => pa.PrinterID == p.PrinterID))
                    .Select(p => p.PrinterID)
                    .ToList();

                Ad.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();

            }




            return View("AddAllocation", Ad);
        }

        //Save add allocation to the database
        [HttpPost]
        public ActionResult addAllocation(Allocation a)
        {

            Table_PrinterAllocation TPA = new Table_PrinterAllocation();
            TPA.AllocatedDate = a.AllocatedDate;
            TPA.BranchID = a.BranchID;
            TPA.PrinterID = a.PrinterID;
            TPA.Remark = a.Remark;
            db.Table_PrinterAllocation.Add(TPA);
            db.SaveChanges();

            string RR = "Add Allocation : Printer ID " + a.PrinterID.ToString() + " Allocated by : " + Session["Username"].ToString()+" to branch id "+ a.BranchID.ToString();
            string Action = "Add Allocation ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            ModelState.Clear();
            a = new Allocation();
            
             int lastID = 0;
             lastID = db.Table_PrinterAllocation.Max(t => t.AllocationID);
             a.AllocationID = lastID + 1;

            using (var context = new ProjectDBEntities())
            {
                a.PrinterIDs = context.Table_Printer
                    .Where(p => !context.Table_PrinterAllocation.Any(pa => pa.PrinterID == p.PrinterID))
                    .Select(p => p.PrinterID)
                    .ToList();

                a.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();

            }

            return View("AddAllocation",a);
        }

        //add Printer form
        public ActionResult AddPrinter(Printer Pd)
        {

            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            int lastID = 0;
            lastID = db.Table_Printer.Max(t => t.PrinterID);
            Pd.PrinterID = lastID + 1;
            return View("AddPrinter", Pd);
        }

        //save add printer to the database
        [HttpPost]
        public ActionResult addPrinter(Printer p)
        {
            string Printer = p.PrinterCode.ToString();
            DateTime DD;
           
            if (p.PurchasedDate > DateTime.Now)
            {
                DD = DateTime.Now;

            }
            else {
                DD = (DateTime)p.PurchasedDate;
            }

            Table_Printer TP = new Table_Printer();
            TP.PrinterID = p.PrinterID;
            TP.PrinterCode = p.PrinterCode;
            TP.BrandName = p.BrandName;
            TP.PurchasedDate = DD;
            TP.VendorName = p.VendorName;
            TP.VendorContact = p.VendorContact;
            TP.WarrantyPeriod = p.WarrantyPeriod;
            db.Table_Printer.Add(TP);
            db.SaveChanges();


            string RR = "Add Printer : Printer ID " + p.PrinterID.ToString() + " Added by : " + Session["Username"].ToString();
            string Action = "Add Printer ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            ModelState.Clear();
            p = new Printer();
            int lastID = 0;
            lastID = db.Table_Printer.Max(t => t.PrinterID);
            p.PrinterID = lastID + 1;

            return View("AddPrinter",p);
        }

        //add branch form
        public ActionResult AddBranch(Branch Bd)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            int lastID = 0;
            lastID = db.Table_Branch.Max(t => t.BranchID);
            Bd.BranchID = lastID + 1;
            return View("AddBranch", Bd);
        }

        //Add branch save to the database
        [HttpPost]
        public ActionResult addBranch(Branch b)
        {
            string BranchName = b.BranchName.ToString();

            Table_Branch TB = new Table_Branch();
            TB.BranchID = b.BranchID;
            TB.BranchName = b.BranchName;
            TB.BranchLocation = b.BranchLocation;
            db.Table_Branch.Add(TB);
            db.SaveChanges();

            string RR = "Create new  Branch : Branch ID " + b.BranchID.ToString() + " Added by : " + Session["Username"].ToString();
            string Action = "Add branch ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);


            ModelState.Clear();
            b = new Branch();
            int lastID = 0;
            lastID = db.Table_Branch.Max(t => t.BranchID);
            b.BranchID = lastID + 1;

            return View("AddBranch",b);
        }

        //View ViewAllocation page
        public ActionResult ViewAllocations()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var Allocation = db.Table_PrinterAllocation;
            return View(Allocation);
        }

        //View Users page
        public ActionResult user()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            ViewBag.UserRole = Session["UserRole"].ToString();
            var user = db.Table_UserDetail;
            return View(user);
        }

        //View Printers page
        public ActionResult Printer()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            ViewBag.UserRole = Session["UserRole"].ToString();
            var printer = db.Table_Printer;
            return View(printer);
        }

        //View Branches page
        public ActionResult Branch()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var branch = db.Table_Branch;
            return View(branch);
        }

        //View Update branch page
        public ActionResult EditBranch(Branch BB)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var id = Request["Bid"];
            int A = Convert.ToInt32(id);
            Table_Branch TB = db.Table_Branch.SingleOrDefault(x => x.BranchID == A);
            BB.BranchID = TB.BranchID;
            BB.BranchLocation = TB.BranchLocation;
            BB.BranchName = TB.BranchName;

            return View("EditBranch", BB);
        }

        //Update branch in the database
        public async Task<ActionResult> UpdateBranch(Branch branch)
        {

            string BranchName = branch.BranchName.ToString();



            try
            {
                var entity = await db.Table_Branch.FindAsync(branch.BranchID);

                if (entity != null)
                {
                    entity.BranchID = branch.BranchID;
                    entity.BranchLocation = branch.BranchLocation;
                    entity.BranchName = branch.BranchName;
                    await db.SaveChangesAsync();

                    string RR = "Update Branch : Branch ID " + branch.BranchID.ToString() + " (" + BranchName.ToString() +" )" + " Updated by : " + Session["Username"].ToString();
                    string Action = "Update branch ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    return RedirectToAction("Branch");
                }

                ViewBag.Error = "Branch not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating Branch: " + ex.Message;
            }
            return View();
        }

        //View Update printer page
        public ActionResult EditPrinter(Printer PP)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var id = Request["pid"];
            int A = Convert.ToInt32(id);
            Table_Printer TP = db.Table_Printer.SingleOrDefault(x => x.PrinterID == A);

            PP.PrinterID = TP.PrinterID;
            PP.PrinterCode = TP.PrinterCode;
            PP.PurchasedDate = TP.PurchasedDate;
            PP.BrandName = TP.BrandName;
            PP.VendorName = TP.VendorName;
            PP.VendorContact = TP.VendorContact;
            PP.WarrantyPeriod = TP.WarrantyPeriod;
            return View("EditPrinter", PP);
        }

        //Update printer in the database
        public async Task<ActionResult> UpdateDis(Printer mfep)
        {
          

            try
            {
                var entity = await db.Table_Printer.FindAsync(mfep.PrinterID);
                if (entity != null)
                {
                    entity.BrandName = mfep.BrandName;
                    entity.PrinterCode = mfep.PrinterCode;
                    entity.VendorName = mfep.VendorName;
                    entity.VendorContact = mfep.VendorContact;
                    entity.WarrantyPeriod = mfep.WarrantyPeriod;
                    await db.SaveChangesAsync();


                    string RR = "Update printer : Printer ID " + mfep.PrinterID.ToString() + " (" + mfep.PrinterCode.ToString() + " )" + " Updated by : " + Session["Username"].ToString();
                    string Action = "Update branch ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    return RedirectToAction("Printer");
                }

                ViewBag.Error = "Printer not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating printer: " + ex.Message;
            }
            return View("Printer");
        }

        //View Edit user form
        public ActionResult EditUser(User UU)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }


            var id = Request["uid"];
            int UID = Convert.ToInt32(id);
            Table_UserDetail TU = db.Table_UserDetail.SingleOrDefault(x => x.UserID == UID);

            UU.UserID = TU.UserID;
            UU.FirstName = TU.FirstName;
            UU.LastName = TU.LastName;
            UU.Passowrd = TU.Passowrd;
            UU.DateOfBirth = TU.DateOfBirth;
            UU.ContactNumber = TU.ContactNumber;
            UU.BranchID = TU.BranchID;

            using (var context = new ProjectDBEntities())
            {


                UU.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();

            }


            return View("EditUser", UU);
        }

        //Update user in the database
        public async Task<ActionResult> UpdateUser(User Us)
        {


            try
            {
                var entity = await db.Table_UserDetail.FindAsync(Us.UserID);
                if (entity != null)
                {
                    entity.UserID = Us.UserID;
                    entity.FirstName = Us.FirstName;
                    entity.LastName = Us.LastName;
                    entity.ContactNumber = Us.ContactNumber;
                    entity.DateOfBirth = Us.DateOfBirth;
                    entity.BranchID = Convert.ToInt32(Us.BranchID);

                    string RR = "Update User : Employee ID " + Us.EmpID.ToString() + " (" + Us.FirstName.ToString() + " )" + " Updated by : " + Session["Username"].ToString();
                    string Action = "Update User ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    await db.SaveChangesAsync();
                    return RedirectToAction("user");
                }
                ViewBag.Error = "User not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating user: " + ex.Message;
            }
            return View();
        }

        //View edit allocation form
        public ActionResult EditAllocation(Allocation AA)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var id = Request["Aid"];
            int AID = Convert.ToInt32(id);
            Table_PrinterAllocation TA = db.Table_PrinterAllocation.SingleOrDefault(x => x.AllocationID == AID);


            AA.AllocationID = TA.AllocationID;
            AA.BranchID = (int)TA.BranchID;
            AA.AllocatedDate = TA.AllocatedDate;
            AA.PrinterID = (int)TA.PrinterID;
            AA.Remark = TA.Remark;

            using (var context = new ProjectDBEntities())
            {


                AA.BranchList = context.Table_Branch.Select(b => b.BranchID).ToList();
               // AA.PrinterIDs = context.Table_Printer.Select(b => b.PrinterID).ToList();
                AA.PrinterIDs = context.Table_Printer
                .Where(p => !context.Table_PrinterAllocation.Any(pa => pa.PrinterID == p.PrinterID))
                .Select(p => p.PrinterID)
                .ToList();

            }

            return View("EditAllocation", AA);
        }

        //Update allocation in the database
        [HttpPost]
        public async Task<ActionResult> UpdateAllocation(Allocation alo)
        {
        

            try
            {
                var entity = await db.Table_PrinterAllocation.FindAsync(alo.AllocationID);
                if (entity != null)
                {
                    entity.AllocationID = alo.AllocationID;
                    entity.BranchID = Convert.ToInt32(alo.BranchID);
                    entity.AllocatedDate = alo.AllocatedDate;
                    entity.PrinterID = Convert.ToInt32(alo.PrinterID);
                    entity.Remark = alo.Remark;
                    await db.SaveChangesAsync();

                    string RR = "Update Allocation : Allocated ID " + alo.AllocationID.ToString()  + " Updated by : " + Session["Username"].ToString();
                    string Action = "Update Allocation ";
                    InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

                    return RedirectToAction("ViewAllocations");
                }
                ViewBag.Error = "Allocation not found";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating allocation: " + ex.Message;
            }
            return RedirectToAction("ViewAllocations");
        }

        //View delete printer form
        public ActionResult DeletePrinter(Printer pd)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            } 

            var pid_D = Request["pid_d"];
            int PID = Convert.ToInt32(pid_D);
            Table_Printer Printer = db.Table_Printer.SingleOrDefault(x => x.PrinterID == PID);

            pd.PrinterID = Printer.PrinterID;
            pd.PrinterCode = Printer.PrinterCode;
            pd.PurchasedDate = Printer.PurchasedDate;
            pd.BrandName = Printer.BrandName;
            pd.VendorName = Printer.VendorName;
            pd.VendorContact = Printer.VendorContact;
            pd.WarrantyPeriod = Printer.WarrantyPeriod;
            
            return View("DeletePrinter", pd);
        }
        
        //Delete printer from the databse
        public async Task<ActionResult> Delete_Printer(Printer ppp)
        {
            var pid_D = Request["pid"];
            int PID = Convert.ToInt32(pid_D);
            var entity = await db.Table_Printer.FindAsync(PID);

            string PrinterIDAudit = ppp.PrinterID.ToString();

            string RR = "Delete printer :" + PID.ToString() + " Deleted by : " + Session["Username"].ToString();
            string Action = "Delete Printer ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            if (entity != null)
            {
                db.Table_Printer.Remove(entity);
                db.SaveChanges();
                await db.SaveChangesAsync();
                return RedirectToAction("Printer");
            }

            ViewBag.Error = "Somthing went wrong";
            return View(ViewBag.Error);
        }

        //View Delete Allocation
        public ActionResult DeleteAllocation(Allocation Al)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var id = Request["Aid"];
            int AID = Convert.ToInt32(id);
            Table_PrinterAllocation TA = db.Table_PrinterAllocation.SingleOrDefault(x => x.AllocationID == AID);

            Al.AllocationID = TA.AllocationID;
            Al.BranchID = (int)TA.BranchID;
            Al.AllocatedDate = TA.AllocatedDate;
            Al.PrinterID = (int)TA.PrinterID;
            Al.Remark = TA.Remark;

            return View("DeleteAllocation", Al);
        }

        //Delete allocation from the database
        public async Task<ActionResult> Delete_Allocation(Allocation aaa)
        {
            var aid_D = Request["Aid"];
            int AID = Convert.ToInt32(aid_D);
            var entity = await db.Table_PrinterAllocation.FindAsync(AID);

            string RR = "Delete Allocation :" + aid_D.ToString() + " Deleted by : " + Session["Username"].ToString();
            string Action = "Delete Allocation ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            if (entity != null)
            {
                db.Table_PrinterAllocation.Remove(entity);
                db.SaveChanges();

                await db.SaveChangesAsync();

                return RedirectToAction("ViewAllocations");
            }

            ViewBag.Error = "Somthing went wrong";

            return View(ViewBag.Error);
        }

        //View delete branch page
        public ActionResult DeleteBranch(Branch brc)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            var id = Request["Bid"];
            int BID = Convert.ToInt32(id);
            Table_Branch TB = db.Table_Branch.SingleOrDefault(x => x.BranchID == BID);

            brc.BranchID = TB.BranchID;
            brc.BranchName = TB.BranchName;
            brc.BranchLocation = TB.BranchLocation;
            return View("DeleteBranch", brc);
        }

        //Delete branch from the database
        public async Task<ActionResult> Delete_Branch(Branch BBB)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var id = Request["Bid"];
            int BID = Convert.ToInt32(id);
            var entity = await db.Table_Branch.FindAsync(BID);

            string BranchName = entity.BranchName;


            string RR = "Delete branch :" + BBB.BranchID.ToString() + " Deleted by : " + Session["Username"].ToString()+" "+BranchName.ToString();
            string Action = "Delete branch ";
            InsertDataToSPAuditTrail(Action, RR, DateTime.Now);

            if (entity != null)
            {
                db.Table_Branch.Remove(entity);
                db.SaveChanges();
                await db.SaveChangesAsync();
                return RedirectToAction("Branch");
            }
            ViewBag.Error = "Somthing went wrong";
            return View(ViewBag.Error);
        }

        //View contact us page
        public ActionResult ContactUs()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            return View();
        }

        //Sent email 
        public ActionResult sentEmail(ModelMails em)
        {

            //From mail Password "pwuzbzvjpyjjduat"
            //HO@vfplc
            //From Mail = testingbranchuser@gmail.com

            var toMail = "sasankabuddhi@gmail.com";
            Email email = new Email();
            bool isEmailSent = false;

            try
            {
                isEmailSent = email.SendEmail(toMail, em.Email, em.Password, em.Subject, em.Body);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it in an appropriate way
                ViewBag.ErrorMessage = "An error occurred while sending the email: " + ex.Message;
            }

            if (isEmailSent)
            {
                ModelState.Clear();
                em = new ModelMails();
                TempData["SuccessMessage"] = "Email sent successfully.";
            }
            else
            {
                ViewBag.ErrorMessage = "Failed to send the email. Please check the recipient address and try again.";
            }

            return View("ContactUs", em);




        }

        //View audit trail 
        public ActionResult ViewAudiTrail()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            var AuditTrailTable = db.Table_AuditTrail;
            return View(AuditTrailTable);
        }

        //Insert data in to audit trail
        public void InsertDataToSPAuditTrail(string Case, string Reason, DateTime date) {

            using (var context = new ProjectDBEntities())
            {
                var result = context.SP_InsertDataToTableAuditTrail(Case, Reason, date);

            }
        
        }

        //View profile page
        public ActionResult ViewProfile(User US)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            string UN = Session["LoggedUser"].ToString();
            var logedUser = db.Table_UserDetail.SingleOrDefault(x => x.EmpID == UN);


            US.UserID = logedUser.UserID;
            US.FirstName = logedUser.FirstName;
            US.LastName = logedUser.LastName;
            US.DateOfBirth = logedUser.DateOfBirth;
            US.ContactNumber = logedUser.ContactNumber;
            US.BranchID = logedUser.BranchID;
            US.EmpID = logedUser.EmpID;
            return View("ViewProfile", US);
        }

        //View change password form
        public ActionResult ChangePassword()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }

            return View("ChangePassword");
        }

        //Change password in the databse
        [HttpPost]
        public ActionResult ChangePassword(User USP)
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            string UN = Session["LoggedUser"].ToString();
            var logedUser = db.Table_UserDetail.SingleOrDefault(x => x.EmpID == UN);

            var CurrentPwd = Request["currentPassword"];

            var NewPWD = Request["newPassword"];

            if (CurrentPwd == logedUser.Passowrd)
            {
                logedUser.Passowrd = NewPWD;
                db.SaveChanges();
                ViewBag.SuccessMessage = "Password Changed";
            }
            else
            {
                ViewBag.ErrorMessage = "Incorrect current password. Please try again.";
                return View("ChangePassword");
            }

            return View("ChangePassword");
        }



    }
}