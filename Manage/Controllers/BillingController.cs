using AutoMapper;
using BioTech.Helpers;
using BioTech.Models;
using BioTech.Services;
using BioTech.ViewModels;
using BioTech.ViewModels.Reports;
using CsvHelper;
using IronPdf;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Controllers
{
    [Authorize(Roles = "Billing")]
    public class BillingController : Controller
    {
        private readonly IProposalService _proposalService;
        private readonly IInvoiceProjectService _invoiceProjectService;
        private readonly IPartialPaymentService _partialPaymentService;
        private readonly ITimesheetEntryService _timesheetEntryService;
        private readonly IInvoiceTimesheetEntryService _invoiceTimesheetEntryService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITaskExtensionService _taskExtensionService;
        private readonly IUserService _userService;
        private readonly ILogger<HomeController> _log;
        private readonly IContactService _contactService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IMapper _mapper;
        public BillingController(ILogger<HomeController> log, IProposalService proposalService, IInvoiceProjectService invoiceProjectService
            , ITimesheetEntryService timesheetEntryService, UserManager<ApplicationUser> userManager, IUserService userService, IContactService contactService
            , IHostingEnvironment hostingEnvironment, IInvoiceTimesheetEntryService invoiceTimesheetEntryService, IMapper mapper, IPartialPaymentService partialPaymentService
            , ITaskExtensionService taskExtensionService)
        {
            _proposalService = proposalService;
            _invoiceProjectService = invoiceProjectService;
            _partialPaymentService = partialPaymentService;
            _timesheetEntryService = timesheetEntryService;
            _userManager = userManager;
            _userService = userService;
            _log = log;
            _contactService = contactService;
            _hostingEnvironment = hostingEnvironment;
            _invoiceTimesheetEntryService = invoiceTimesheetEntryService;
            _mapper = mapper;
            _taskExtensionService = taskExtensionService;
        }

        public IActionResult Index()
        {
            var allInvoices = _invoiceProjectService.GetAll().ToList();
            return View(allInvoices);
        }

        public IActionResult Invoices_Read([DataSourceRequest] DataSourceRequest request)
        {
            IEnumerable<InvoiceProject> invoices = _invoiceProjectService.GetAll();
            List<InvoiceProjectVM> invoicesVM = new List<InvoiceProjectVM>();
            foreach (var invoice in invoices)
            {
                var invoiceVMToAdd = _mapper.Map<InvoiceProjectVM>(invoice);
                invoiceVMToAdd.PartialPaymentsTotal = invoice.PartialPayments.Sum(pp => pp.Amount);
                invoiceVMToAdd.RemainingAmountToBePaid = invoice.CurrentCharges - invoiceVMToAdd.PartialPaymentsTotal;
                invoiceVMToAdd.LastPaymentReceivedDate = invoice.PartialPayments.OrderByDescending(i => i.PaymentDate).FirstOrDefault()?.PaymentDate;
                invoicesVM.Add(invoiceVMToAdd);
            }
            var obj = invoicesVM.ToDataSourceResult(request);
            return Json(obj);
        }
        public IActionResult CreateInvoiceBatch()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AutocompleteActiveProjectName(string prefix)
        {
            var projectByPrefix = _proposalService.GetAll().Where(p => p.ProjectName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)
            && p.IsActive).Select(obj => new { label = obj.ProjectName, val = obj.Id });
            return Json(projectByPrefix);
        }

        [HttpPost]
        public JsonResult UpdateInvoiceStatus(int invoiceId, string status)
        {
            try
            {
                var invoiceToUpdate = _invoiceProjectService.GetById(invoiceId);
                invoiceToUpdate.Status = status;
                _invoiceProjectService.Update(invoiceToUpdate);
                return Json(new { success = "success" });
            }
            catch (Exception ex)
            {
                _log.LogError("Exception trying to update invoice status: " + ex.Message);
                throw;
            }

        }

        [HttpPost]
        public JsonResult UpdateInvoiceEmailed(int invoiceId, bool emailed)
        {
            try
            {
                var invoiceToUpdate = _invoiceProjectService.GetById(invoiceId);
                invoiceToUpdate.Emailed = emailed;
                _invoiceProjectService.Update(invoiceToUpdate);
                return Json(new { success = "success" });
            }
            catch (Exception ex)
            {
                _log.LogError("Exception trying to update invoice status: " + ex.Message);
                throw;
            }
        }

        [HttpPost]
        public JsonResult UpdateInvoiceFinalized(int invoiceId, bool finalized, string status)
        {
            try
            {
                var invoiceToUpdate = _invoiceProjectService.GetById(invoiceId);
                invoiceToUpdate.Finalized = finalized;
                invoiceToUpdate.Status = status;
                _invoiceProjectService.Update(invoiceToUpdate);
                return Json(new { success = "success" });
            }
            catch (Exception ex)
            {
                _log.LogError("Exception trying to update invoice Finalized: " + ex.Message);
                throw;
            }
        }

        [HttpPost]
        public IActionResult CreateInvoiceBatch(InvoiceProject invoiceProject)
        {
            List<TimesheetEntry> timesheetsApprovedByRange = new List<TimesheetEntry>();
            if (String.Equals(invoiceProject.InvoiceType, "general"))
            {
                timesheetsApprovedByRange = _timesheetEntryService.GetAllByDateRange(invoiceProject.FromDate, invoiceProject.ToDate)
                      .Where(ts => ts.IsApproved && !ts.InvoiceGenerated && ts.HoursWorked > 0 && String.Equals(ts.TimesheetEntryType, "billed")).ToList();
            }

            if (String.Equals(invoiceProject.InvoiceType, "client"))
            {
                var allTimesheetsApprovedByRange = _timesheetEntryService.GetAllByDateRange(invoiceProject.FromDate, invoiceProject.ToDate)
                      .Where(ts => ts.IsApproved && !ts.InvoiceGenerated && ts.HoursWorked > 0 && String.Equals(ts.TimesheetEntryType, "billed")).ToList();
                foreach (var timesheetApproved in allTimesheetsApprovedByRange)
                {
                    if (String.Equals(timesheetApproved.TaskUser.TaskExtension.Proposal.Contact.Company.Name, invoiceProject.Filter, StringComparison.OrdinalIgnoreCase))
                    {
                        timesheetsApprovedByRange.Add(timesheetApproved);
                    }
                }
            }

            if (String.Equals(invoiceProject.InvoiceType, "employee"))
            {
                var allTimesheetsApprovedByRange = _timesheetEntryService.GetAllByDateRange(invoiceProject.FromDate, invoiceProject.ToDate)
                      .Where(ts => ts.IsApproved && !ts.InvoiceGenerated && ts.HoursWorked > 0 && String.Equals(ts.TimesheetEntryType, "billed")).ToList();
                foreach (var timesheetApproved in allTimesheetsApprovedByRange)
                {
                    var username = _userService.GetById(timesheetApproved.TaskUser.UserId).UserName;
                    if (String.Equals(username, invoiceProject.Filter, StringComparison.OrdinalIgnoreCase))
                    {
                        timesheetsApprovedByRange.Add(timesheetApproved);
                    }
                }
            }

            if (String.Equals(invoiceProject.InvoiceType, "projectNumber"))
            {
                var allTimesheetsApprovedByRange = _timesheetEntryService.GetAllByDateRange(invoiceProject.FromDate, invoiceProject.ToDate)
                      .Where(ts => ts.IsApproved && !ts.InvoiceGenerated && ts.HoursWorked > 0 && String.Equals(ts.TimesheetEntryType, "billed")).ToList();
                foreach (var timesheetApproved in allTimesheetsApprovedByRange)
                {

                    if (String.Equals(timesheetApproved.TaskUser.TaskExtension.Proposal.ProjectNumber, invoiceProject.Filter, StringComparison.OrdinalIgnoreCase))
                    {
                        timesheetsApprovedByRange.Add(timesheetApproved);
                    }
                }
            }

            if (String.Equals(invoiceProject.InvoiceType, "contractNumber"))
            {
                var allTimesheetsApprovedByRange = _timesheetEntryService.GetAllByDateRange(invoiceProject.FromDate, invoiceProject.ToDate)
                      .Where(ts => ts.IsApproved && !ts.InvoiceGenerated && ts.HoursWorked > 0 && String.Equals(ts.TimesheetEntryType, "billed")).ToList();
                foreach (var timesheetApproved in allTimesheetsApprovedByRange)
                {
                    if (String.Equals(timesheetApproved.TaskUser.TaskExtension.Proposal.ContractNumber, invoiceProject.Filter, StringComparison.OrdinalIgnoreCase))
                    {
                        timesheetsApprovedByRange.Add(timesheetApproved);
                    }
                }
            }

            var timesheetsApprovedByRangeList = new List<TimesheetEntry>();
            //Validating if all tasks are Flat Tasks and that are least one is Complete, otherwise they shouldn't be included in an invoice           
            foreach (var timesheetApproved in timesheetsApprovedByRange)
            {
                if (!(String.Equals(timesheetApproved.TaskUser.TaskExtension.FeeStructure, "flat fee", StringComparison.OrdinalIgnoreCase)
                    && !timesheetApproved.TaskUser.TaskExtension.TaskCompleted))
                {
                    timesheetsApprovedByRangeList.Add(timesheetApproved);
                }
            }

            if (timesheetsApprovedByRangeList.Count() == 0)
            {
                ModelState.AddModelError("", "There are no approved timesheet entries for the selected date range...");
            }
           
            if (ModelState.IsValid)
            {
                try
                {                    
                    var timesheetsApprovedGroupedByContractNumber = timesheetsApprovedByRangeList.GroupBy(ts => ts.TaskUser.TaskExtension.Proposal.ContractNumber);
                  
                    foreach (var timesheetApprovedEntryGroup in timesheetsApprovedGroupedByContractNumber)
                    {
                        InvoiceProject invoiceProjectTemp = new InvoiceProject();
                        invoiceProjectTemp.InvoiceType = invoiceProject.InvoiceType;
                        invoiceProjectTemp.Status = invoiceProject.Status;
                        invoiceProjectTemp.FromDate = invoiceProject.FromDate;
                        invoiceProjectTemp.ToDate = invoiceProject.ToDate;
                        invoiceProjectTemp.RecordCount = timesheetApprovedEntryGroup.Count();                        
                        var lastInvoiceNumber = _invoiceProjectService.GetLastInvoiceNumber();
                        //Making sure that Invoice Number starts at 200 000
                        if (lastInvoiceNumber < 200000)
                        {
                            lastInvoiceNumber = 200000;
                        }                       
                        invoiceProjectTemp.InvoiceNumber = lastInvoiceNumber + 1;
                        decimal HNTEAmountSpentCurrentInvoice = 0;
                        foreach (var timesheetApprovedEntry in timesheetApprovedEntryGroup)
                        {
                            {
                                var currentProject = _proposalService.GetProposalById(timesheetApprovedEntry.TaskUser.TaskExtension.ProposalId);
                                var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;                             
                                invoiceProjectTemp.Contact = currentProject.Contact;
                                invoiceProjectTemp.ProjectNumber = currentProject.ProjectNumber;
                                invoiceProjectTemp.ProjectTitle = currentProject.ProjectName;
                                invoiceProjectTemp.InvoiceDate = DateTime.Now;
                                invoiceProjectTemp.ContractNumber = currentProject.ContractNumber;
                                var authorProject = _userService.GetById(currentProject.AuthorId);
                                invoiceProjectTemp.ProjectManager = $"{authorProject.FirstName} {authorProject.LastName}";
                                var currentTaskFeeStructure = timesheetApprovedEntry.TaskUser.TaskExtension.FeeStructure;
                                var rateUsed = timesheetApprovedEntry.HoursWorked * timesheetApprovedEntry.HourlyRate;
                                var currentTaskTotalPrice = timesheetApprovedEntry.TaskUser.TaskExtension.TotalPrice;
                                var prev = _invoiceTimesheetEntryService.GetAll().Where(ite => ite.TaskExtensionId == timesheetApprovedEntry.TaskUser.TaskExtensionId).Sum(ite => ite.RateUsed);
                                bool isFlatFeeTaskCompleted = false;
                                bool isFlatFee = false;
                                if (String.Equals(currentTaskFeeStructure, "Flat Fee", StringComparison.CurrentCultureIgnoreCase))
                                {
                                    isFlatFee = true;
                                    var taskExtensionFlatFeeID = timesheetApprovedEntry.TaskUser.TaskExtension.Id;
                                    isFlatFeeTaskCompleted = _taskExtensionService.GetById(taskExtensionFlatFeeID).TaskCompleted;
                                }
                                //If the Task Fee Structure is Flat Fee it should only appear in the Invoice once is Completed.                              
                                if ((isFlatFee && isFlatFeeTaskCompleted && timesheetApprovedEntry.HoursWorked > 0
                                  && !invoiceProjectTemp.InvoiceTimesheetEntries.Select(t => t.TaskExtensionId).Contains(timesheetApprovedEntry.TaskUser.TaskExtensionId))
                                  || !isFlatFee)
                                {
                                    if (String.Equals(timesheetApprovedEntry.TaskUser.TaskExtension.FeeStructure, "Hourly Not To Exceed", StringComparison.OrdinalIgnoreCase))
                                    {
                                        HNTEAmountSpentCurrentInvoice += BillingHelper.GetAmount(currentTaskFeeStructure, timesheetApprovedEntry);
                                    }
                                 
                                    invoiceProjectTemp.InvoiceTimesheetEntries.Add(new InvoiceTimesheetEntry
                                    {
                                        TaskExtensionId = timesheetApprovedEntry.TaskUser.TaskExtensionId,
                                        TaskNumber = $"{timesheetApprovedEntry.TaskUser.TaskExtension.TaskCodeParent}-{timesheetApprovedEntry.TaskUser.TaskExtension.TaskCodeSub}",
                                        TaskName = timesheetApprovedEntry.TaskUser.TaskExtension.TaskTitle,
                                        EmployeeId = timesheetApprovedEntry.TaskUser.UserId,
                                        Contract = BillingHelper.GetContractAmt(currentTaskFeeStructure, timesheetApprovedEntry),
                                        RateUsed = BillingHelper.GetRate(currentTaskFeeStructure, timesheetApprovedEntry),
                                        Prev = prev,
                                        Qty = BillingHelper.GetQty(currentTaskFeeStructure, timesheetApprovedEntry),
                                        TotalPercentage = !String.Equals(timesheetApprovedEntry.TaskUser.TaskExtension.FeeStructure, "Hourly Not To Exceed"
                                        , StringComparison.OrdinalIgnoreCase) ? BillingHelper.GetPercentage(currentTaskFeeStructure, timesheetApprovedEntry)
                                        : BillingHelper.GetPercentageHNTE(timesheetApprovedEntry, HNTEAmountSpentCurrentInvoice),
                                        Amount = BillingHelper.GetAmount(currentTaskFeeStructure, timesheetApprovedEntry),
                                        FeeStructure = currentTaskFeeStructure,
                                        DateModified = timesheetApprovedEntry.DateModified
                                    });
                                }
                            }
                        }
                      
                        invoiceProjectTemp.TotalInvoiced = invoiceProjectTemp.InvoiceTimesheetEntries.Select(e => e.Amount).Sum();
                        invoiceProjectTemp.CurrentCharges = invoiceProjectTemp.InvoiceTimesheetEntries.Select(e => e.Amount).Sum();
                        _invoiceProjectService.Create(invoiceProjectTemp);
                        //Updating TimesheetEntry records, marking them as already Invoiced so they are not included again in new invoices unless invoice is removed.
                        foreach (var timesheetEntry in timesheetApprovedEntryGroup)
                        {
                            timesheetEntry.InvoiceGenerated = true;
                            timesheetEntry.InvoiceId = invoiceProjectTemp.Id;
                            _timesheetEntryService.Update(timesheetEntry);
                        }
                        ViewBag.InvoiceCreated = "created";

                    }
                   
                    return RedirectToAction("Index"); ;
                }
                catch (Exception ex)
                {
                    var message = ex.Message;
                    throw;
                }

            }
            return View(invoiceProject);
        }

        public IActionResult TimeSheetEntriesByProject(int id)
        {
            var invoiceProject = _invoiceProjectService.GetById(id);
            return View(invoiceProject);
        }

        public IActionResult CreateTimesheetReport()
        {
            return View();
        }       

        public IActionResult EditInvoice(int id)
        {
            var invoice = _invoiceProjectService.GetById(id);
            if (TempData["InvoiceUpdated"] != null)
            {
                ViewBag.InvoiceUpdated = "Invoice has been updated";
            }
            return View(invoice);
        }
        [HttpPost]
        public IActionResult EditInvoice(List<InvoiceTimesheetEntryVM> fieldsPossibleUpdate, string note, int invoiceId)
        {
            //Reading invoice previously to test/prevent issue when retrieving TimesheetEntries when GetById() method is called again..
            var currentInvoiceRead = _invoiceProjectService.GetById(invoiceId);
            foreach (var field in fieldsPossibleUpdate)
            {
                var currentInvoiceTimesheetEntry = _invoiceTimesheetEntryService.GetById(field.InvoiceTimesheetEntryId);
                if (String.Equals(field.Type, "rate"))
                {
                    if (currentInvoiceTimesheetEntry.RateUsed != field.Value)
                    {
                        currentInvoiceTimesheetEntry.RateUsed = field.Value;
                        currentInvoiceTimesheetEntry.Amount = field.Value;
                        _invoiceTimesheetEntryService.Update(currentInvoiceTimesheetEntry);
                    }
                }

                if (String.Equals(field.Type, "qty"))
                {
                    if (currentInvoiceTimesheetEntry.Qty != field.Value)
                    {
                        currentInvoiceTimesheetEntry.Qty = Convert.ToInt32(field.Value);
                        currentInvoiceTimesheetEntry.Amount = Convert.ToInt32(field.Value) * currentInvoiceTimesheetEntry.RateUsed;
                        _invoiceTimesheetEntryService.Update(currentInvoiceTimesheetEntry);
                    }
                }

            }
            var currentInvoice = _invoiceProjectService.GetById(invoiceId);
            currentInvoice.TotalInvoiced = currentInvoice.InvoiceTimesheetEntries.Select(e => e.Amount).Sum();
            currentInvoice.CurrentCharges = currentInvoice.InvoiceTimesheetEntries.Select(e => e.Amount).Sum();
            if (!String.Equals(currentInvoice.Note, note))
            {
                currentInvoice.Note = note;                
            }
            _invoiceProjectService.Update(currentInvoice);
            TempData["InvoiceUpdated"] = "Invoice has been updated";
            return Json(new { success = "success" });
        }

        [HttpPost]
        public IActionResult AutocompleteBilling(string prefix, string typeSearch)
        {
            var allProjects = _proposalService.GetAllAndInclude().Where(p => p.IsActive);
            if (typeSearch == "client")
            {
                var projectCompanies = allProjects.Where(p => p.Contact.Company.Name.Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                    .Select(obj => new
                    {
                        label = obj.Contact.Company.Name
                    });
                return Json(projectCompanies);
            }

            if (typeSearch == "employee")
            {
                var projectEmployees = _userService.GetAll().Where(u => u.UserName.Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                    .Select(obj => new
                    {
                        label = obj.UserName
                    });
                return Json(projectEmployees);
            }

            if (typeSearch == "projectNumber")
            {
                var projectNumbers = allProjects.Where(p => p.ProjectNumber.Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                    .Select(obj => new
                    {
                        label = obj.ProjectNumber
                    });
                return Json(projectNumbers);
            }

            if (typeSearch == "contractNumber")
            {
                var contractNumbers = allProjects.Where(p => p.ContractNumber.Contains(prefix, StringComparison.InvariantCultureIgnoreCase))
                    .Select(obj => new
                    {
                        label = obj.ContractNumber
                    });
                return Json(contractNumbers);
            }

            //When prefix doesn't match one of the above conditions then just return empty Json
            return Json("");
        }

        [HttpPost]
        public IActionResult PostPayment(int invoiceId, decimal partialPaymentAmount)
        {
            if (partialPaymentAmount <= 0)
            {
                return Json(new { error = "Partial Payment Amount needs to be more than cero." });
            }
            var invoiceToUpdate = _invoiceProjectService.GetById(invoiceId);
            invoiceToUpdate.PartialPayments.Add(
                new PartialPayment
                {
                    PaymentDate = DateTime.Now,
                    Amount = partialPaymentAmount
                }
                );
            var totalPartiallyPaid = invoiceToUpdate.PartialPayments.Sum(p => p.Amount);
            if (totalPartiallyPaid > invoiceToUpdate.TotalInvoiced)
            {
                return Json(new { error = "Total of Partial Payments exceeded the Total Invoiced price." });
            }
            invoiceToUpdate.TotalInvoiced -= totalPartiallyPaid;
            _invoiceProjectService.Update(invoiceToUpdate);
            return Json(new { success = "Partial Payment has been posted" });
        }


        [AcceptVerbs("Post")]
        public IActionResult Invoice_Destroy([DataSourceRequest] DataSourceRequest request, InvoiceProjectVM invoice)
        {
            var invoiceToDelete = _invoiceProjectService.GetById(invoice.Id);
            if (invoiceToDelete != null)
            {
                foreach (var partialPayment in invoiceToDelete.PartialPayments.ToList())
                {
                    _partialPaymentService.Destroy(partialPayment);
                }
                _invoiceProjectService.Destroy(invoiceToDelete);
            }
            var relatedTimesheetEntries = _timesheetEntryService.GetAllByDateRange(invoice.FromDate, invoice.ToDate).Where(t => t.InvoiceId == invoice.Id).ToList();
            foreach (var timesheetEntry in relatedTimesheetEntries)
            {
                timesheetEntry.InvoiceGenerated = false;
                timesheetEntry.InvoiceId = 0;
                _timesheetEntryService.Update(timesheetEntry);
            }

            return Json(new[] { invoice }.ToDataSourceResult(request, null));
        }

        [HttpPost]
        public JsonResult RemovePartialPayment(int partialPaymentId, int invoiceId)
        {
            if (partialPaymentId == 0)
            {
                return Json(new { error = "There was an error trying to remove the Partial Payment. Please contact developer." });
            }
            else
            {
                var partialPaymentToRemove = _invoiceProjectService.GetPartialPaymentById(partialPaymentId);
                var invoice = _invoiceProjectService.GetById(invoiceId);
                _invoiceProjectService.RemovePartialPayment(partialPaymentToRemove);
                invoice.TotalInvoiced = invoice.TotalInvoiced + partialPaymentToRemove.Amount;
                _invoiceProjectService.Update(invoice);
                return Json(new { success = "Partial Payment has been removed" });
            }
        }

        public ActionResult AgingReport()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AgingReportData(InvoiceAgingReport agingReport)
        {
            Dictionary<string, List<InvoiceAgingReport>> invoicesByAgingDaysDictionary = new Dictionary<string, List<InvoiceAgingReport>>();
            List<InvoiceAgingReport> current = new List<InvoiceAgingReport>();
            List<InvoiceAgingReport> oneToThirty = new List<InvoiceAgingReport>();
            List<InvoiceAgingReport> thirtyOneToSixty = new List<InvoiceAgingReport>();
            List<InvoiceAgingReport> sixtyOneToNinety = new List<InvoiceAgingReport>();
            List<InvoiceAgingReport> moreThanNinety = new List<InvoiceAgingReport>();
            var invoicesPendingFiltered = _invoiceProjectService.GetAll().Where(i => String.Equals(i.Status, "Pending")
            && DateTime.Compare(i.InvoiceDate, agingReport.Date) <= 0);
            invoicesPendingFiltered = BillingHelper.FilterReport(invoicesPendingFiltered, agingReport.FilterType, agingReport.FilterValue);
            foreach (var invoice in invoicesPendingFiltered)
            {
                var relatedAuthorId = _proposalService.GetProposalByProjectNumber(invoice.ProjectNumber).AuthorId;
                var relatedAuthor = _userService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == relatedAuthorId);
                var relatedAuthorName = $"{relatedAuthor.FirstName} {relatedAuthor.LastName}";
                var repInitials = Helper.FormatInitials(relatedAuthorName);
                var currentInvoiceAging = (DateTime.Now - invoice.InvoiceDate.AddDays(30)).Days;
                var currentInvoiceReport = new InvoiceAgingReport
                {
                    Aging = currentInvoiceAging,
                    Rep = repInitials,
                    Date = invoice.InvoiceDate,
                    Balance = invoice.TotalInvoiced,
                    DueDate = invoice.InvoiceDate.AddDays(30),
                    Num = invoice.InvoiceNumber,
                    Type = "Invoice",
                    Name = $"{invoice.ProjectNumber}-{invoice.ProjectTitle}",
                    CompanyName = invoice.Contact.Company.Name
                };
                if (currentInvoiceAging < 0)
                {
                    current.Add(currentInvoiceReport);
                }
                if (currentInvoiceAging > 0 && currentInvoiceAging <= 30)
                {
                    oneToThirty.Add(currentInvoiceReport);
                }
                if (currentInvoiceAging > 30 && currentInvoiceAging <= 60)
                {
                    thirtyOneToSixty.Add(currentInvoiceReport);
                }
                if (currentInvoiceAging > 60 && currentInvoiceAging <= 90)
                {
                    sixtyOneToNinety.Add(currentInvoiceReport);
                }
                if (currentInvoiceAging > 90)
                {
                    moreThanNinety.Add(currentInvoiceReport);
                }
            }
            invoicesByAgingDaysDictionary.Add("Current", agingReport.Sorting == "aging" ? current.OrderBy(c => c.Aging).ToList() : current.OrderBy(c => c.Name).ToList());
            invoicesByAgingDaysDictionary.Add("1 - 30", agingReport.Sorting == "aging" ? oneToThirty.OrderBy(c => c.Aging).ToList() : oneToThirty.OrderBy(c => c.Name).ToList());
            invoicesByAgingDaysDictionary.Add("31 - 60", agingReport.Sorting == "aging" ? thirtyOneToSixty.OrderBy(c => c.Aging).ToList() : thirtyOneToSixty.OrderBy(c => c.Name).ToList());
            invoicesByAgingDaysDictionary.Add("61 - 90", agingReport.Sorting == "aging" ? sixtyOneToNinety.OrderBy(c => c.Aging).ToList() : sixtyOneToNinety.OrderBy(c => c.Name).ToList());
            invoicesByAgingDaysDictionary.Add("> 90", agingReport.Sorting == "aging" ? moreThanNinety.OrderBy(c => c.Aging).ToList() : moreThanNinety.OrderBy(c => c.Name).ToList());

            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();

            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block;font-weight:bold;margin-left:10px;'><span>{DateTime.Now.ToString("hh:mm tt")}</span><br />" +
    $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span></div><div style='display:inline-block; margin-left:400px;font-weight:bold;text-align:center;'>" +
    $"<span>Bio-Tech Consulting, Inc.</span><br /><span style='font-size:20px;'>A/R Aging Detail</span><br /><span>As of {DateTime.Now.ToString("MMMM d, yyyy")}</span>" +
    $"</div><hr style='color:black;background-color:black;border:2px solid black;' /><span style='margin-right:100px'></span>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/AgingReportData", invoicesByAgingDaysDictionary));
            var pdfName = $"/Pdfs/Aging/Aging-Report-{agingReport.Date.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Aging-Report-{agingReport.Date.ToString("MM-dd-yyyy")}.pdf"
            };
        }

        public async Task<IActionResult> GeneratePdf(int id)
        {
            var invoiceProject = _invoiceProjectService.GetById(id);
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/TimeSheetEntriesByProjectPDF", invoiceProject));
            var pdfName = $"/Pdfs/Invoices/{invoiceProject.ContractNumber} {invoiceProject.ProjectTitle} {invoiceProject.InvoiceDate.ToString("MM.dd.yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"{invoiceProject.ContractNumber} {invoiceProject.ProjectTitle}" +
                $" {invoiceProject.InvoiceDate.ToString("MM/dd/yyyy")}.pdf"
            };
        }

        [HttpGet]
        public IActionResult OpenInvoices()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> OpenInvoices(BaseReport baseReport)
        {
            var invoicesPaidInFull = _invoiceProjectService.GetAll().Where(i => !String.Equals(i.Status, "Paid in Full", StringComparison.CurrentCultureIgnoreCase));
            var filteredInvoices = BillingHelper.FilterReport(invoicesPaidInFull, baseReport.FilterType, baseReport.FilterValue);
            var openInvoicesGroupedByContractNrTitle = filteredInvoices.GroupBy(i => $"{i.ProjectNumber}-{i.ProjectTitle}").ToList();
            Dictionary<string, List<OpenInvoice>> openInvoicesDictionary = new Dictionary<string, List<OpenInvoice>>();
            foreach (var invoiceList in openInvoicesGroupedByContractNrTitle)
            {
                List<OpenInvoice> openInvoicesList = new List<OpenInvoice>();
                foreach (var invoice in invoiceList)
                {
                    var relatedAuthorId = _proposalService.GetProposalByProjectNumber(invoice.ProjectNumber).AuthorId;
                    var relatedAuthor = _userService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == relatedAuthorId);
                    var relatedAuthorName = $"{relatedAuthor.FirstName} {relatedAuthor.LastName}";
                    var repInitials = Helper.FormatInitials(relatedAuthorName);
                    var currentInvoiceAging = (DateTime.Now - invoice.InvoiceDate.AddDays(30)).Days;
                    var currentInvoiceReport = new OpenInvoice
                    {
                        Aging = currentInvoiceAging,
                        Date = invoice.InvoiceDate,
                        Num = invoice.InvoiceNumber,
                        DueDate = invoice.InvoiceDate.AddDays(30),
                        Balance = invoice.TotalInvoiced,
                        ContractNumber = invoice.ContractNumber,
                        CompanyName = invoice.Contact.Company.Name
                    };
                    openInvoicesList.Add(currentInvoiceReport);
                }
                openInvoicesDictionary.Add(invoiceList.Key, openInvoicesList);
            }

            //PDF Generation
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block; font-weight:bold;'>" +
                 $"<span>{DateTime.Now.ToString("h:mm tt")}</span>" +
                 $"<br />" +
                 $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span>" +
                 $"</div>" +
                 $"<div style='display:inline-block; font-weight:bold;text-align:center;margin-left:290px;font-size:20px;'>" +
                 $"<span>Bio-Tech Consulting, Inc.</span>" +
                 $"<br />" +
                 $"<span>Open Invoices</span>" +
                 $"<br />" +
                 $"<span>As of {DateTime.Now.ToString("MMMM dd, yyyy")}</span>" +
                 $"</div>" +
                 $"<hr style='color:black;background-color:black;border:2px solid black;'/>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/OpenInvoices", openInvoicesDictionary));
            var pdfName = $"/Pdfs/Open Invoices/Open Invoices-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Open Invoices-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf"
            };
        }

        public IActionResult PendingInvoices()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> PendingInvoices(BaseReport baseReport)
        {
            var pendingInvoices = _invoiceProjectService.GetAll().Where(i => String.Equals(i.Status, "Pending", StringComparison.CurrentCultureIgnoreCase));
            var pendingInvoicesFiltered = BillingHelper.FilterReport(pendingInvoices, baseReport.FilterType, baseReport.FilterValue);
            List<PendingInvoice> model = new List<PendingInvoice>();
            foreach (var invoice in pendingInvoicesFiltered)
            {
                var relatedAuthorId = _proposalService.GetProposalByProjectNumber(invoice.ProjectNumber).AuthorId;
                var relatedAuthor = _userService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == relatedAuthorId);
                var relatedAuthorName = $"{relatedAuthor.FirstName} {relatedAuthor.LastName}";
                var repInitials = Helper.FormatInitials(relatedAuthorName);
                model.Add(new PendingInvoice
                {
                    Date = invoice.InvoiceDate,
                    Num = invoice.InvoiceNumber,
                    Name = $"{invoice.ProjectNumber}-{invoice.ProjectTitle}",
                    Rep = repInitials,
                    Balance = invoice.TotalInvoiced,
                    CompanyName = invoice.Contact.Company.Name
                });
            }

            //PDF Generation
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block; font-weight:bold;'>" +
                 $"<span>{DateTime.Now.ToString("h:mm tt")}</span>" +
                 $"<br />" +
                 $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span>" +
                 $"<br />" +
                 $"<span>Accrual Basis</span>" +
                 $"</div>" +
                 $"<div style='display:inline-block; font-weight:bold;text-align:center;margin-left:290px;font-size:20px;'>" +
                 $"<span>Bio-Tech Consulting, Inc.</span>" +
                 $"<br />" +
                 $"<span>Pending Invoices</span>" +
                 $"<br />" +
                 $"<span>As of {DateTime.Now.ToString("MMMM dd, yyyy")}</span>" +
                 $"</div>" +
                 $"<hr style='color:black;background-color:black;border:2px solid black;'/>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/PendingInvoices", model));
            var pdfName = $"/Pdfs/Pending Invoices/Pending Invoices-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Pending Invoices-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf"
            };
        }
        public IActionResult CustomerBalanceSummary()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> CustomerBalanceSummary(BaseReport baseReport)
        {
            var pendingInvoices = _invoiceProjectService.GetAll().Where(i => String.Equals(i.Status, "Pending", StringComparison.CurrentCultureIgnoreCase));
            var filteredInvoices = BillingHelper.FilterReport(pendingInvoices, baseReport.FilterType, baseReport.FilterValue);
            var pendingInvoicesGroupedByProject = filteredInvoices.GroupBy(i => $"{i.ProjectNumber}-{i.ProjectTitle}");
            List<CustomerBalanceSummary> model = new List<CustomerBalanceSummary>();
            foreach (var invoice in pendingInvoicesGroupedByProject)
            {
                List<EntryDetail> entryDetails = new List<EntryDetail>();
                foreach (var invoiceDetails in invoice)
                {
                    if (entryDetails.Select(ed => ed.ContractNumber).Contains(invoiceDetails.ContractNumber))
                    {
                        entryDetails.First(ed => String.Equals(ed.ContractNumber, invoiceDetails.ContractNumber, StringComparison.CurrentCultureIgnoreCase))
                            .SummaryAmount += invoiceDetails.TotalInvoiced;
                    }
                    else
                    {
                        entryDetails.Add(new EntryDetail
                        {
                            ContractNumber = invoiceDetails.ContractNumber,
                            SummaryAmount = invoiceDetails.TotalInvoiced
                        });
                    }
                }
                model.Add(new CustomerBalanceSummary
                {
                    ProjectTitle = invoice.Key,
                    EntryDetails = entryDetails,
                    Total = entryDetails.Sum(ed => ed.SummaryAmount)
                });
            }

            //PDF Generation
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block; font-weight:bold;'>" +
                 $"<span>{DateTime.Now.ToString("h:mm tt")}</span>" +
                 $"<br />" +
                 $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span>" +
                 $"</div>" +
                 $"<div style='display:inline-block; font-weight:bold;text-align:center;margin-left:310px;font-size:20px;'>" +
                 $"<span>Bio-Tech Consulting, Inc.</span>" +
                 $"<br />" +
                 $"<span>Customer Balance Summary</span>" +
                 $"<br />" +
                 $"<span>All Transactions</span>" +
                 $"</div>" +
                 $"<hr style='color:black;background-color:black;border:2px solid black;'/>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/CustomerBalanceSummary", model));
            var pdfName = $"/Pdfs/Customer Balance Summary/Customer Balance Summary-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Customer Balance Summary-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf"
            };
        }

        public IActionResult CustomerOpenBalance()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CustomerOpenBalanceData(BaseReport baseReport)
        {
            var pendingInvoices = _invoiceProjectService.GetAll().Where(i => String.Equals(i.Status, "Pending", StringComparison.OrdinalIgnoreCase));
            var filteredInvoices = BillingHelper.FilterReport(pendingInvoices, baseReport.FilterType, baseReport.FilterValue); 
            if (filteredInvoices.Count() == 0)
            {
                TempData["CustomerOpenBalanceEmpty"] = $"There are no invoices with Open Balance for the Search Criteria";
                return View("CustomerOpenBalance");
            }

            //PDF Generation
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block; font-weight:bold;'>" +
                 $"<span>{DateTime.Now.ToString("h:mm tt")}</span>" +
                 $"<br />" +
                 $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span>" +
                 $"<br/>" +
                 $"<span>Accrual Basis</span>" +
                 $"</div>" +
                 $"<div style='display:inline-block; font-weight:bold;text-align:center;margin-left:290px;font-size:20px;'>" +
                 $"<span>Bio-Tech Consulting, Inc.</span>" +
                 $"<br />" +
                 $"<span>Customer Open Balance</span>" +
                 $"<br />" +
                 $"<span>As of {DateTime.Now.ToString("MMMM dd, yyyy")}</span>" +
                 $"</div>" +
                 $"<hr style='color:black;background-color:black;border:2px solid black;'/>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/CustomerOpenBalanceData", filteredInvoices));
            var pdfName = $"/Pdfs/Customer Open Balance/Customer Open Balance-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Customer Open Balance-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf"
            };
        }

        public IActionResult AutocompleteProjectNumberTitle(string prefix)
        {
            var projectByPrefix = _proposalService.GetAll().Where(p => !String.IsNullOrEmpty(p.ProjectNumber) &&  p.ProjectName.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)
            || (!String.IsNullOrEmpty(p.ProjectNumber) && p.ProjectNumber.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase))
              && p.IsActive).Select(obj => new { label = $"{obj.ProjectNumber}-{obj.ProjectName}", val = obj.ProjectNumber });
            return Json(projectByPrefix);
        }

        public IActionResult JobProgressInvVsEstimates()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> JobProgressInvVsEstimates(BaseReport baseReport)
        {
            var pendingInvoices = _invoiceProjectService.GetAll().Where(i => String.Equals(i.Status, "Pending", StringComparison.CurrentCultureIgnoreCase));
            var filteredInvoices = BillingHelper.FilterReport(pendingInvoices, baseReport.FilterType, baseReport.FilterValue);
            var pendingInvoicesGroupedByProject = filteredInvoices.GroupBy(i => $"{i.ProjectNumber}-{i.ProjectTitle}");
            List<JobProgressVsEstimate> model = new List<JobProgressVsEstimate>();
            List<InvoicesRelated> invoicesRelated = new List<InvoicesRelated>();
            foreach (var groupedByProject in pendingInvoicesGroupedByProject)
            {
                foreach (var pendingInvoice in groupedByProject)
                {
                    var relatedAuthorId = _proposalService.GetProposalByProjectNumber(pendingInvoice.ProjectNumber).AuthorId;
                    var relatedAuthor = _userService.GetAllApplicationUsers().FirstOrDefault(u => u.Id == relatedAuthorId);
                    var relatedAuthorName = $"{relatedAuthor.FirstName} {relatedAuthor.LastName}";
                    var repInitials = Helper.FormatInitials(relatedAuthorName);
                    if (invoicesRelated.Select(i => i.ContractNumber).Contains(pendingInvoice.ContractNumber))
                    {
                        var invoiceToUpdate = invoicesRelated.FirstOrDefault(i => String.Equals(i.ContractNumber, pendingInvoice.ContractNumber));
                        invoiceToUpdate.EstimateTotal += pendingInvoice.CurrentCharges;
                        invoiceToUpdate.ProgressInvoice += pendingInvoice.CurrentCharges - pendingInvoice.TotalInvoiced;
                        invoiceToUpdate.PercentageProgress = invoiceToUpdate.ProgressInvoice * 100 / invoiceToUpdate.EstimateTotal;
                    }
                    else
                    {
                        invoicesRelated.Add(new InvoicesRelated
                        {
                            ContractNumber = pendingInvoice.ContractNumber,
                            Type = "Estimate",
                            Date = pendingInvoice.InvoiceDate,
                            Num = pendingInvoice.ContractNumber,
                            Rep = repInitials,
                            CompanyName = pendingInvoice.Contact.Company.Name,
                            EstimateTotal = pendingInvoice.CurrentCharges,
                            ProgressInvoice = pendingInvoice.CurrentCharges - pendingInvoice.TotalInvoiced,
                            PercentageProgress = ((pendingInvoice.CurrentCharges - pendingInvoice.TotalInvoiced) * 100) / pendingInvoice.CurrentCharges
                        });
                    }

                }
                var invoiceProject = new JobProgressVsEstimate
                {
                    ProjectFormattedTitle = groupedByProject.Key,
                    InvoicesRelated = invoicesRelated
                };
                model.Add(invoiceProject);
            }
            //PDF Generation
            var root = _hostingEnvironment.ContentRootPath;
            var rendererFormat = PdfHelper.GeneratePdfRendererFormat();
            rendererFormat.PrintOptions.Header = new HtmlHeaderFooter()
            {
                Height = 40,
                HtmlFragment = $"<div style='display:inline-block; font-weight:bold;'>" +
                 $"<span>{DateTime.Now.ToString("h:mm tt")}</span>" +
                 $"<br />" +
                 $"<span>{DateTime.Now.ToString("MM/dd/yy")}</span>" +
                 $"</div>" +
                 $"<div style='display:inline-block; font-weight:bold;text-align:center;margin-left:290px;font-size:20px;'>" +
                 $"<span>Bio-Tech Consulting, Inc.</span>" +
                 $"<br />" +
                 $"<span>Job Progress Invoices vs. Estimates</span>" +
                 $"<br />" +
                 $"<span>All Transactions</span>" +
                 $"</div>" +
                 $"<hr style='color:black;background-color:black;border:2px solid black;'/>"
            };

            var pdf = rendererFormat.RenderHtmlAsPdf(await this.RenderViewAsync("Pdf/JobProgressVsEstimates", model));
            var pdfName = $"/Pdfs/Job Progress Invoices vs. Estimates/Job Progress Invoices vs. Estimates-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf";
            var pdfRoot = $"{root}{pdfName}";
            pdf.SaveAs(pdfRoot);
            var stream = new FileStream(pdfRoot, FileMode.Open);
            return new FileStreamResult(stream, "application/pdf")
            {
                FileDownloadName = $"Job Progress Invoices vs. Estimates-{DateTime.Now.ToString("MM-dd-yyyy")}.pdf"
            };
        }

        public IActionResult CreditMemoByProject()
        {
            return View();
        }

        public IActionResult CreditMemoByProjectData(string projectNumber)
        {
            var invoicesByProjectNumber = _invoiceProjectService.GetByProjectNumber(projectNumber);


            return View("Pdf/CreditMemoByProjectData");
        }

        public IActionResult ExportInvoicesToQuickBooks()
        {
            var finalizedInvoices = _invoiceProjectService.GetAll().Where(i => i.Finalized);
            if (finalizedInvoices.Count() == 0)
            {
                TempData["ExportToQuickBookNoFiles"] = "There are no finalized invoices to import";
                return View("Index");            
            }
            if (ModelState.IsValid)
            {
                byte[] fileData = null;
                var invoicesFormatted = BiotechCsvHelper.FormatInvoiceDataToCSV(finalizedInvoices);
                var resultsToByteArray = BiotechCsvHelper.WriteCsvToMemory(invoicesFormatted);    
                var memoryStream = new MemoryStream(resultsToByteArray);
                return new FileStreamResult(memoryStream, "text/csv") { FileDownloadName = $"{DateTime.Now.ToString("MM-dd-yyyy")} Invoices to import to Quickbooks.csv" };           
            }
            return RedirectToAction("Index");
    }
        //Creating Spreadsheet format for Quickbooks to be able to read it
        public DataTable GetDataTable(IEnumerable<InvoiceProject> finalizedInvoices)
        {
            DataTable dt = new DataTable();
            dt.TableName = "Finalized Invoices";
            dt.Clear();
            
            dt.Columns.Add("Client Number");
            dt.Columns.Add("Contract Number");
            dt.Columns.Add("Invoice Number");
            dt.Columns.Add("Invoice Date");
            dt.Columns.Add("Invoice Total");
            dt.Columns.Add("Terms");          
            
            foreach (var invoice in finalizedInvoices)
            {
                dt.Rows.Add(invoice.ProjectNumber, invoice.ContractNumber, invoice.InvoiceNumber, invoice.InvoiceDate, invoice.CurrentCharges, "Net 30");                           
            }
            return dt;
        }
    }
}
