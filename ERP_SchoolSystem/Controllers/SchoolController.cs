using ERP_SchoolSystem.Filters;
using ERP_SchoolSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Controllers
{
    public class SchoolController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities();
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }



        [Authorize]
        [CustomAuthorizeAttribute(Roles = "Admin")]
        [RestrictRoute]
        public ActionResult AddNewSchool()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public async System.Threading.Tasks.Task<ActionResult> AddNewSchool(string SchoolName, string SRPersonName, int Country, int City, string RegistrationNo, string PtclNo, string CellNo2, int IsActive, DateTime RegistrationDate, string SROffiersContactNo, int Province, string Address, string NTNNo, string CellNo1, string Email, string Website, HttpPostedFileBase SchoolLogo, string DefaultUserName, string DefaultPassword, string SchoolShortCode, int PackageID)
        {
            try
            {
                TblSchool s = new TblSchool();
                s.SchoolName = SchoolName;
                s.SROfficerName = SRPersonName;
                s.CountryId = Country;
                s.CityId = City;
                s.RegistrationNo = RegistrationNo;
                s.PtclNo = PtclNo;
                s.CellNo1 = CellNo2;
                if (IsActive == 1)
                {
                    s.IsActive = true;
                }
                else
                {
                    s.IsActive = false;
                }
                s.RegistrationDate = RegistrationDate;
                s.SROffiersContactNo = SROffiersContactNo;
                s.ProvinceId = Province;
                s.Address = Address;
                s.NTNNo = NTNNo;
                s.CellNo = CellNo1;
                s.Email = Email;
                s.Website = Website;
                s.AdminUserName = DefaultUserName;
                s.SchoolShortCode = SchoolShortCode;

                if (SchoolLogo != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SchoolLogo.InputStream.CopyTo(ms);
                        s.SchoolLogo = ms.GetBuffer();
                    }
                    string FilePath = SchoolName + System.IO.Path.GetExtension(SchoolLogo.FileName);
                    var path = Path.Combine(Server.MapPath("~/SchoolLogo"), FilePath);
                    SchoolLogo.SaveAs(path);
                    s.SchoolLogoPath = "/SchoolLogo/" + FilePath;
                }
                s.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                s.CreatedDateTime = DateTime.Now;
                db.TblSchools.Add(s);
                db.SaveChanges();


                TblSchoolFee sf = new TblSchoolFee();
                sf.SchoolID = s.SchoolId;
                sf.PackageID = PackageID;
                sf.MonthFee = db.TblSchoolPackages.Where(x => x.PackageID == PackageID).Select(x => x.Price).FirstOrDefault();
                sf.IsActive = true;
                sf.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                sf.AddedOn = DateTime.Now;
                db.TblSchoolFees.Add(sf);
                db.SaveChanges();


                TblSchoolBranch B = new TblSchoolBranch();
                B.BranchTitle = "Head Office";
                B.SchoolID = s.SchoolId;
                B.IsActive = true;
                B.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                B.CreatedOn = DateTime.Now;
                db.TblSchoolBranches.Add(B);
                db.SaveChanges();

                TblSchoolBranchesFee SBF = new TblSchoolBranchesFee();
                SBF.SchoolID = s.SchoolId;
                SBF.BranchID = B.BranchID;
                SBF.PackageID = PackageID;
                SBF.MonthlyFee = db.TblSchoolPackages.Where(x => x.PackageID == PackageID).Select(x => x.Price).FirstOrDefault();
                SBF.IsActive = true;
                SBF.Percentage = "100";
                SBF.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                SBF.AddedOn = DateTime.Now;
                db.TblSchoolBranchesFees.Add(SBF);
                db.SaveChanges();

                CboDepartment Dp = new CboDepartment();
                Dp.DepartmentName = "Administrator";
                Dp.IsActive = true;
                Dp.SchoolId = s.SchoolId;
                Dp.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                Dp.CreatedOn = DateTime.Now;
                db.CboDepartments.Add(Dp);
                db.SaveChanges();

                CboDesignation D = new CboDesignation();
                D.DesignationName = "Administrator Head";
                D.IsActive = true;
                D.SchoolId = s.SchoolId;
                D.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                D.CreatedOn = DateTime.Now;
                db.CboDesignations.Add(D);
                db.SaveChanges();

                CboEmpStatu ES = new CboEmpStatu();
                ES.EmpStatusName = "Permanent";
                ES.IsActive = true;
                ES.SchoolId = s.SchoolId;
                ES.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                ES.CreatedOn = DateTime.Now;
                db.CboEmpStatus.Add(ES);
                db.SaveChanges();

                var user = new ApplicationUser { UserName = DefaultUserName, Email = DefaultUserName };
                var result = await UserManager.CreateAsync(user, DefaultPassword);
                if (result.Succeeded)
                {
                    AspNetUser obj = db.AspNetUsers.Where(x => x.Email == DefaultUserName).FirstOrDefault();
                    obj.SchoolID = s.SchoolId;
                    obj.BranchID = B.BranchID;
                    obj.UserName = DefaultUserName;
                    obj.Password = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(DefaultPassword);
                    obj.UserTypeId = 2;
                    obj.IsActive = true;
                    obj.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    obj.AddedOn = DateTime.Now;
                    db.SaveChanges();

                    AspNetUserRole r = new AspNetUserRole();
                    r.RoleId = "0630bc37-63bf-4880-936c-ab01bb42607a";
                    r.UserId = obj.Id;
                    db.AspNetUserRoles.Add(r);
                    db.SaveChanges();

                    AspNetUserRole r1 = new AspNetUserRole();
                    r1.RoleId = "64daf2a4-c239-4300-b63c-5265868ac8bb";
                    r1.UserId = obj.Id;
                    db.AspNetUserRoles.Add(r1);
                    db.SaveChanges();

                }
                ViewBag.SuccessMessage = "New School Successfully Added";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            return View();
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult GetProvincesList(int CountryID)
        {
            try
            {
                var ProvincesList = db.TblProvinces.Where(x => x.CountryID == CountryID).ToList();
                return PartialView("GetProvincesList", ProvincesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult GetProvincesList_Update(int CountryID, int ProvinceID)
        {
            try
            {
                ViewBag.ProvinceID = ProvinceID;
                var ProvincesList = db.TblProvinces.Where(x => x.CountryID == CountryID && x.ProvincesID != ProvinceID).ToList();
                return PartialView("GetProvincesList_Update", ProvincesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult GetCitiesList(int ProvinceID)
        {
            try
            {
                var CitiesList = db.TblCities.Where(x => x.ProvincesID == ProvinceID).ToList();
                return PartialView("GetCitiesList", CitiesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult GetCitiesList_Update(int ProvinceID, int CityID)
        {
            try
            {
                ViewBag.CityID = CityID;
                var CitiesList = db.TblCities.Where(x => x.ProvincesID == ProvinceID && x.CityID != CityID).ToList();
                return PartialView("GetCitiesList_Update", CitiesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult ManageSchool()
        {
            var Schools = db.TblSchools.ToList();
            return View(Schools);
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult ManageSchool(int Schoolid, int StatusID)
        {
            try
            {
                TblSchool obj = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
                obj.IsActive = Convert.ToBoolean(StatusID);
                obj.ModifiedBY = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                obj.ModifeidDateTime = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception ex)
            {

            }
            var Schools = db.TblSchools.ToList();
            return View(Schools);
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult UpdateSchool(int Schoolid)
        {
            var Schools = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
            return View(Schools);
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult UpdateSchool(int Schoolid, string SchoolName, string SRPersonName, int Country, int City, string RegistrationNo, string PtclNo, string CellNo2, int IsActive, DateTime RegistrationDate, string SROffiersContactNo, int Province, string Address, string NTNNo, string CellNo1, string Email, string Website, HttpPostedFileBase SchoolLogo, string DefaultUserName, string DefaultPassword, string SchoolShortCode)
        {
            try
            {
                TblSchool s = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
                s.SchoolName = SchoolName;
                s.SROfficerName = SRPersonName;
                s.CountryId = Country;
                s.CityId = City;
                s.RegistrationNo = RegistrationNo;
                s.PtclNo = PtclNo;
                s.CellNo1 = CellNo2;
                if (IsActive == 1)
                {
                    s.IsActive = true;
                }
                else
                {
                    s.IsActive = false;
                }
                s.RegistrationDate = RegistrationDate;
                s.SROffiersContactNo = SROffiersContactNo;
                s.ProvinceId = Province;
                s.Address = Address;
                s.NTNNo = NTNNo;
                s.CellNo = CellNo1;
                s.Email = Email;
                s.Website = Website;
                s.AdminUserName = DefaultUserName;
                s.SchoolShortCode = SchoolShortCode;
                if (SchoolLogo != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SchoolLogo.InputStream.CopyTo(ms);
                        s.SchoolLogo = ms.GetBuffer();
                    }
                    string FilePath = SchoolName + System.IO.Path.GetExtension(SchoolLogo.FileName);
                    var path = Path.Combine(Server.MapPath("~/SchoolLogo"), FilePath);
                    SchoolLogo.SaveAs(path);
                    s.SchoolLogoPath = "/SchoolLogo/" + FilePath;
                }
                s.ModifiedBY = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                s.ModifeidDateTime = DateTime.Now;
                db.SaveChanges();
                ViewBag.SuccessMessage = "School info Successfully Update.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            var Schools = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
            return View(Schools);
        }



        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult ManageBranch(int Schoolid, int BranchID = 0, string StatusID = null)
        {
            ViewBag.SchoolID = Schoolid;
            if (BranchID != 0)
            {
                if (StatusID != null)
                {
                    TblSchoolBranch B = db.TblSchoolBranches.Where(x => x.BranchID == BranchID).FirstOrDefault();
                    B.IsActive = Convert.ToBoolean(Convert.ToInt32(StatusID));
                    B.ModifiedOn = DateTime.Now;
                    B.ModifiedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    db.SaveChanges();
                    ViewBag.BranchID = 0;
                }
                else
                {
                    ViewBag.BranchID = BranchID;
                }
            }
            var SchoolBranches = db.TblSchoolBranches.Where(x => x.SchoolID == Schoolid).ToList();
            return View(SchoolBranches);
        }

        [HttpPost]
        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult ManageBranch(int SchoolID, string BranchName, string SchoolMonthlyFee, string BranchPercentage, string BranchMonthlyFee, int PackageID, int BranchID = 0, int RID = 0)
        {
            try
            {
                if (BranchID == 0 && RID == 0)
                {
                    TblSchoolBranch B = new TblSchoolBranch();
                    B.SchoolID = SchoolID;
                    B.BranchTitle = BranchName;
                    B.IsActive = true;
                    B.CreatedOn = DateTime.Now;
                    B.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    db.TblSchoolBranches.Add(B);
                    db.SaveChanges();

                    TblSchoolBranchesFee BF = new TblSchoolBranchesFee();
                    BF.BranchID = B.BranchID;
                    BF.SchoolID = SchoolID;
                    BF.PackageID = PackageID;
                    BF.Percentage = BranchPercentage;
                    BF.IsActive = true;
                    BF.MonthlyFee = Convert.ToInt32(SchoolMonthlyFee) * Convert.ToInt32(BranchPercentage) / 100;
                    BF.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    BF.AddedOn = DateTime.Now;
                    db.TblSchoolBranchesFees.Add(BF);
                    db.SaveChanges();
                    ViewBag.SuccessMessage = "Branch info Successfully Added.";
                }
                else
                {
                    TblSchoolBranch B = db.TblSchoolBranches.Where(x => x.BranchID == BranchID).FirstOrDefault();
                    B.SchoolID = SchoolID;
                    B.BranchTitle = BranchName;
                    B.ModifiedOn = DateTime.Now;
                    B.ModifiedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    db.SaveChanges();

                    var PreBranchPercentage = db.TblSchoolBranchesFees.Where(x => x.ID == RID).Select(x => x.Percentage).FirstOrDefault();
                    if (PreBranchPercentage == BranchPercentage)
                    {
                        TblSchoolBranchesFee BF = db.TblSchoolBranchesFees.Where(x => x.ID == RID).FirstOrDefault();
                        BF.Percentage = BranchPercentage;
                        BF.MonthlyFee = Convert.ToInt32(SchoolMonthlyFee) * Convert.ToInt32(BranchPercentage) / 100;
                        BF.ModifyBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        BF.ModifyOn = DateTime.Now;
                        db.SaveChanges();
                    }
                    else
                    {
                        var AllBranches = db.TblSchoolBranchesFees.Where(x => x.BranchID == BranchID).ToList();
                        foreach (var item in AllBranches)
                        {
                            TblSchoolBranchesFee BFU = db.TblSchoolBranchesFees.Where(x => x.ID == item.ID).FirstOrDefault();
                            BFU.IsActive = false;
                            BFU.ModifyBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                            BFU.ModifyOn = DateTime.Now;
                            db.SaveChanges();
                        }
                        TblSchoolBranchesFee BF = new TblSchoolBranchesFee();
                        BF.BranchID = B.BranchID;
                        BF.SchoolID = SchoolID;
                        BF.PackageID = PackageID;
                        BF.Percentage = BranchPercentage;
                        BF.IsActive = true;
                        BF.MonthlyFee = Convert.ToInt32(SchoolMonthlyFee) * Convert.ToInt32(BranchPercentage) / 100;
                        BF.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        BF.AddedOn = DateTime.Now;
                        db.TblSchoolBranchesFees.Add(BF);
                        db.SaveChanges();
                    }
                    ViewBag.SuccessMessage = "Branch info Successfully Update.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            ViewBag.SchoolID = SchoolID;
            var SchoolBranches = db.TblSchoolBranches.Where(x => x.SchoolID == SchoolID).ToList();
            return View(SchoolBranches);
        }

        [Authorize]
        [RestrictRoute]
        public PartialViewResult GetBranchesList(int SchoolID)
        {
            try
            {
                var BranchesList = db.TblSchoolBranches.Where(x => x.SchoolID == SchoolID).ToList();
                return PartialView("GetBranchesList", BranchesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult UpdateSchoolPackage(int Schoolid)
        {
            var Schools = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
            return View(Schools);
        }

        [HttpPost]
        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult UpdateSchoolPackage(int Schoolid, int PackageID)
        {
            try
            {
                var PrePackageID = db.TblSchoolFees.Where(x => x.PackageID == PackageID && x.SchoolID == Schoolid && x.IsActive == true).Select(x => x.PackageID).FirstOrDefault();
                if (PrePackageID == PackageID)
                {
                    ViewBag.ErrorMessage = "This school already has same package please choose another.";
                }
                else
                {
                    var AllSchoolData = db.TblSchoolFees.Where(x => x.SchoolID == Schoolid).ToList();
                    foreach (var item in AllSchoolData)
                    {
                        TblSchoolFee BFU = db.TblSchoolFees.Where(x => x.ID == item.ID).FirstOrDefault();
                        BFU.IsActive = false;
                        BFU.UpdatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        BFU.UpdateOn = DateTime.Now;
                        db.SaveChanges();
                    }
                    var Monthlyfee = db.TblSchoolPackages.Where(x => x.PackageID == PackageID).Select(x => x.Price).FirstOrDefault();
                    TblSchoolFee sf = new TblSchoolFee();
                    sf.SchoolID = Schoolid;
                    sf.PackageID = PackageID;
                    sf.MonthFee = Monthlyfee;
                    sf.IsActive = true;
                    sf.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    sf.AddedOn = DateTime.Now;
                    db.TblSchoolFees.Add(sf);
                    db.SaveChanges();

                    var AllBranches = db.TblSchoolBranchesFees.Where(x => x.SchoolID == Schoolid && x.IsActive == true).ToList();
                    foreach (var item in AllBranches)
                    {
                        TblSchoolBranchesFee BFU = db.TblSchoolBranchesFees.Where(x => x.ID == item.ID).FirstOrDefault();
                        BFU.IsActive = false;
                        BFU.ModifyBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        BFU.ModifyOn = DateTime.Now;
                        db.SaveChanges();

                        TblSchoolBranchesFee BF = new TblSchoolBranchesFee();
                        BF.BranchID = item.BranchID;
                        BF.SchoolID = Schoolid;
                        BF.PackageID = PackageID;
                        BF.Percentage = item.Percentage;
                        BF.IsActive = true;
                        BF.MonthlyFee = Convert.ToInt32(Monthlyfee) * Convert.ToInt32(item.Percentage) / 100;
                        BF.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        BF.AddedOn = DateTime.Now;
                        db.TblSchoolBranchesFees.Add(BF);
                        db.SaveChanges();
                    }

                    ViewBag.SuccessMessage = "School Package Successfully Update.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            var Schools = db.TblSchools.Where(x => x.SchoolId == Schoolid).FirstOrDefault();
            return View(Schools);
        }
        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult BranchPackagesLog(int Schoolid, int BranchID)
        {
            var Schools = db.TblSchoolBranchesFees.Where(x => x.SchoolID == Schoolid && x.BranchID == BranchID).ToList();
            return View(Schools);
        }

        

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult SchoolMonthlyCollectionRemaining(int SchoolID = 0 , int FeeMonth = 0 , int FeeYear = 0)
        {
            ViewBag.SchoolId = SchoolID;
            ViewBag.FeeMonth = FeeMonth;
            ViewBag.FeeYear = FeeYear;
            return View();
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult _SchoolMonthlyCollectionRemaining(int SchoolID, int FeeMonth = 0, int FeeYear = 0)
        {
            ViewBag.SchoolId = SchoolID;
            ViewBag.FeeMonth = FeeMonth;
            ViewBag.FeeYear = FeeYear;
            return PartialView("_SchoolMonthlyCollectionRemaining");
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public JsonResult UpdateBranch(int BranchID, int StatusID)
        {
            string Message = null;
            try
            {
                TblSchoolBranch B = db.TblSchoolBranches.Where(x => x.BranchID == BranchID).FirstOrDefault();
                B.IsActive = Convert.ToBoolean(Convert.ToInt32(StatusID));
                B.ModifiedOn = DateTime.Now;
                B.ModifiedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                db.SaveChanges();
                Message = "Successfully Update";
            }
            catch (Exception ex)
            {
                Message = ex.Message.ToString();
            }
            return Json(Message);
        }

        public string GetOrderNumber()
        {
            string MonthStr = "";
            int Month = DateTime.Now.Month;
            string Year = DateTime.Now.Year.ToString();
            if (Month < 10)
            {
                MonthStr = "0" + Month.ToString();
            }
            else
            {
                MonthStr = Month.ToString();
            }
            var YearStr = DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - 2);
            string Code = "";
            var aa = db.TblSchoolMonthlyPayments.OrderByDescending(x => x.ID).Select(x => x.OrderNo).FirstOrDefault();
            if (aa == null)
            {
                Code = "SSP-000000-" + MonthStr + YearStr;
            }
            else
            {
                Code = aa;
            }
            string[] pars = Code.Trim().Split('-');
            string rol = pars[1];
            if (pars[2].ToString().Substring(0, 2) != MonthStr)
            {
                rol = "000000";
            }

            var lastAddedId = rol;
            int val = 1;
            var I = ((Convert.ToInt32(lastAddedId)) + (Convert.ToInt32(val)));
            var result = I.ToString().PadLeft(6, '0');
            result = "UIC-" + result + "-" + MonthStr + YearStr;
            return result.ToString();
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public JsonResult PayfeeBranch(int BranchID , int Month , int year , string PaymentMode)
        {
            string Message = null;
            try
            {
                var Branch = db.TblSchoolBranches.Where(x => x.BranchID == BranchID).FirstOrDefault();
                var BranchFee = db.TblSchoolBranchesFees.Where(x => x.BranchID == BranchID && x.IsActive == true).FirstOrDefault();
                var CheckExit = db.TblSchoolMonthlyPayments.Where(x => x.SchoolID == Branch.SchoolID && x.BranchID == Branch.BranchID && x.PaymentMonth == Month && x.PaymentYear == year).FirstOrDefault();
                if (CheckExit == null)
                {
                    TblSchoolMonthlyPayment B = new TblSchoolMonthlyPayment();

                    B.SchoolID = Branch.SchoolID;
                    B.PackageID = BranchFee.PackageID;
                    B.BranchID = BranchID;
                    B.BranchPercentage = Convert.ToInt32(BranchFee.Percentage);
                    B.BranchAmount = BranchFee.MonthlyFee;
                    B.PaymentReceived = BranchFee.MonthlyFee;
                    B.PaymentMonth = Month;
                    B.PaymentYear = year;
                    B.PaymentMode = PaymentMode;
                    B.AddedOn = DateTime.Now;
                    B.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    B.OrderNo = GetOrderNumber();
                    db.TblSchoolMonthlyPayments.Add(B);
                    db.SaveChanges();
                    Message = "Fee Pay Successflly";
                }
                   else
                {
                    Message = "Fee Already Paid.";
                } 
            }
            catch (Exception ex)
            {
                Message = ex.Message.ToString();
            }
            return Json(Message);
        }
        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public JsonResult PayfeeSchool(int SchoolID, int Month, int year, string PaymentMode)
        {
            string Message = null;
            try
            {
                var Branches = db.TblSchoolBranches.Where(x => x.SchoolID == SchoolID).ToList();
                foreach (var item in Branches)
                {
                    var CheckExit = db.TblSchoolMonthlyPayments.Where(x => x.SchoolID == SchoolID && x.BranchID == item.BranchID && x.PaymentMonth == Month && x.PaymentYear == year).FirstOrDefault();
                    if(CheckExit == null)
                    {
                        var BranchFee = db.TblSchoolBranchesFees.Where(x => x.BranchID == item.BranchID && x.IsActive == true).FirstOrDefault();
                        TblSchoolMonthlyPayment B = new TblSchoolMonthlyPayment();

                        B.SchoolID = item.SchoolID;
                        B.PackageID = BranchFee.PackageID;
                        B.BranchID = item.BranchID;
                        B.BranchPercentage = Convert.ToInt32(BranchFee.Percentage);
                        B.BranchAmount = BranchFee.MonthlyFee;
                        B.PaymentReceived = BranchFee.MonthlyFee;
                        B.PaymentMonth = Month;
                        B.PaymentYear = year;
                        B.PaymentMode = PaymentMode;
                        B.AddedOn = DateTime.Now;
                        B.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        B.OrderNo = GetOrderNumber();
                        db.TblSchoolMonthlyPayments.Add(B);
                        db.SaveChanges();
                        Message = "Fee Pay Successfully";
                    }
                    else
                    {
                        Message = "Fee Already Paid.";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message.ToString();
            }
            return Json(Message);
        }


        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult SchoolMonthlyCollection(int FeeMonth = 0, int FeeYear = 0)
        {
            ViewBag.FeeMonth = FeeMonth;
            ViewBag.FeeYear = FeeYear;
            return View();
        }

        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public PartialViewResult _SchoolMonthlyCollection(int FeeMonth = 0, int FeeYear = 0)
        {
            ViewBag.FeeMonth = FeeMonth;
            ViewBag.FeeYear = FeeYear;
            return PartialView("_SchoolMonthlyCollection");
        }


        [Authorize]
        [RestrictRoute]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult SchoolBranches(int Schoolid ,int FeeMonth, int FeeYear)
        {
            ViewBag.FeeMonth = FeeMonth;
            ViewBag.FeeYear = FeeYear;
            var SchoolBranches = db.TblSchoolBranches.Where(x => x.SchoolID == Schoolid).ToList();
            return View(SchoolBranches);
        }
    }
}