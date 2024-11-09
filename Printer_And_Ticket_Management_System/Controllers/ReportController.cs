using CrystalDecisions.CrystalReports.Engine;
using Printer_And_Ticket_Management_System.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Printer_And_Ticket_Management_System.Models;
using System.Globalization;
using System.Data.SqlClient;
using System.Data;

namespace Printer_And_Ticket_Management_System.Controllers
{
    public class ReportController : Controller
    {
        //Database object
        ProjectDBEntities db = new ProjectDBEntities();

        // GET: Audit trail report view page
        public ActionResult AuditTrailFunction()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate Audit trail report
        [HttpPost]
        public ActionResult AuditTrailFunction(AuditTrailRpt Ar)
        {

            int reportType = Ar.selectedReport;
            string FromDate = Ar.fromDate;
            string ToDate = Ar.toDate;

            using (var context = new ProjectDBEntities())
            {
                
                var dataTable = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GenerateAuditTrailRpt] @@ReportType, @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@ReportType", reportType));
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        dataTable.Load(reader);
                    }
                }

                var dataset = new DataSet();
                dataset.Tables.Add(dataTable);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/AuditTrailReport.rpt")));
                rd.SetDataSource(dataTable);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "AuditrailReport.pdf");
            }

        }

        //Get : printer list report view page
        public ActionResult GenerateReportPrinterList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate printer list report
        [HttpPost]
        public ActionResult GenerateReportPrinterList(CommonReport CR)
        {
            string FromDate = CR.fromDate;
            string ToDate = CR.toDate;
 
            using (var context = new ProjectDBEntities())
            {

                var DataTable1 = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GeneratePrinterListReport] @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable1.Load(reader);
                    }
                }

                var DataSet1 = new DataSet();
                DataSet1.Tables.Add(DataTable1);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/PrinterListReport.rpt")));
                rd.SetDataSource(DataTable1);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "PrinterListReport.pdf");
            }
        }

        //Get : Allocation list report view
        public ActionResult GenerateReportAllocationList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate allocation list report 
        [HttpPost]
        public ActionResult GenerateReportAllocationList(CommonReport CR)
        {
            string FromDate = CR.fromDate;
            string ToDate = CR.toDate;

            using (var context = new ProjectDBEntities())
            {

                var DataTable2 = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GenerateAllocationListReport] @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable2.Load(reader);
                    }
                }

                var DataSet2 = new DataSet();
                DataSet2.Tables.Add(DataTable2);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/AllocationListReport.rpt")));
                rd.SetDataSource(DataTable2);
         
                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "AllocationListReport.pdf");
            }

        }

        //Get : User management report view
        public ActionResult GenerateReportUserManagementList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate User Management Report
        [HttpPost]
        public ActionResult GenerateReportUserManagementList(CommonReport CR)
        {
            string FromDate = CR.fromDate;
            string ToDate = CR.toDate;

            using (var context = new ProjectDBEntities())
            {

                var DataTable3 = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GenerateUserManagementReport] @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable3.Load(reader);
                    }
                }

                var DataSet3 = new DataSet();
                DataSet3.Tables.Add(DataTable3);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/UserManagementReport.rpt")));
                rd.SetDataSource(DataTable3);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "UserManagementReport.pdf");
            }

        }

        //Get : Branch list report view
        public ActionResult GenerateReportBranchList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate branch list report 
        [HttpPost]
        public ActionResult GenerateReportBranchList(CommonReport CR)
        {
            string FromDate = CR.fromDate;
            string ToDate = CR.toDate;

            using (var context = new ProjectDBEntities())
            {

                var DataTable4 = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GenerateBranchListReport] @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable4.Load(reader);
                    }
                }

                var DataSet4 = new DataSet();
                DataSet4.Tables.Add(DataTable4);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/BranchListReport.rpt")));
                rd.SetDataSource(DataTable4);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "BranchListReport.pdf");
            }
        }

        //Get ticket report view
        public ActionResult GenerateReportTicketList()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginFunction", "Home");
            }
            return View();
        }

        //Generate ticket report
        [HttpPost]
        public ActionResult GenerateReportTicketList(CommonReport CR)
        {
            string FromDate = CR.fromDate;
            string ToDate = CR.toDate;

            using (var context = new ProjectDBEntities())
            {

                var DataTable5 = new DataTable();
                using (var command = context.Database.Connection.CreateCommand())
                {
                    command.CommandText = "exec [SP_GenerateTicketsReport] @@FromDate, @@ToDate";
                    command.Parameters.Add(new SqlParameter("@@FromDate", FromDate));
                    command.Parameters.Add(new SqlParameter("@@ToDate", ToDate));

                    context.Database.Connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        DataTable5.Load(reader);
                    }
                }

                var DataSet5 = new DataSet();
                DataSet5.Tables.Add(DataTable5);

                ReportDocument rd = new ReportDocument();
                rd.Load(Path.Combine(Server.MapPath("~/Reports/TicketListReport.rpt")));
                rd.SetDataSource(DataTable5);

                Response.Buffer = false;
                Response.ClearContent();
                Response.ClearHeaders();

                Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);
                stream.Seek(0, SeekOrigin.Begin);
                return File(stream, "application/pdf", "TicketListReport.pdf");
            }
        }



    }

}