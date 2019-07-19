using ERP_SchoolSystem.Filters;
using ERP_SchoolSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Controllers
{
    public class HomeController : Controller
    {
        ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [EmpDataFilter]
        public ActionResult Index()
        {
            return View();
        }
        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [EmpDataFilter]
        public ActionResult DashBoard()
        {
            return View();
        }

        public ActionResult PaymentAlert()
        {
            return View();
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [EmpDataFilter]
        public ActionResult AccountSetting()
        {
            int UserID = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
            var EmpData = db.TblEmployees.Where(x => x.UserID == UserID).FirstOrDefault();
            return View(EmpData);
        }

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

        [HttpPost]
        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [EmpDataFilter]
        public async System.Threading.Tasks.Task<ActionResult> AccountSetting(string UserName, string NewPassword, HttpPostedFileBase EmpLogo)
        {
            try
            {
                if(EmpLogo == null)
                {
                    var user = await UserManager.FindByNameAsync(UserName);
                    if (user == null)
                    {
                        ViewBag.ErrorMessage = "The user does not exist";
                    }
                    var result = UserManager.RemovePassword(user.Id);
                    result = UserManager.AddPassword(user.Id, NewPassword);
                    if (result.Succeeded)
                    {
                        NewPassword = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(NewPassword);
                        AspNetUser obj = db.AspNetUsers.Where(x => x.Id == user.Id).FirstOrDefault();
                        obj.Password = NewPassword;
                        obj.UpdateBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                        obj.UpdateOn = DateTime.Now;
                        db.SaveChanges();

                        if (Request.Cookies["TheSchollSystemPassword"] != null)
                        {
                            System.Web.HttpCookie ckPassword = new System.Web.HttpCookie("TheSchollSystemPassword");
                            ckPassword.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(ckPassword);
                        }

                        ViewBag.SuccessMessage = "Password has been changed successfully";
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Error !!!... , Try Again.";
                    }
                }
                else
                {
                    int UserID = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    TblEmployee obj = db.TblEmployees.Where(x => x.UserID == UserID).FirstOrDefault();
                    using (MemoryStream ms = new MemoryStream())
                    {
                        EmpLogo.InputStream.CopyTo(ms);
                        obj.EmpPicture = ms.GetBuffer();
                    }
                    string FilePath = ERP_SchoolSystem.Classes.UserInfo.GetUserName() + System.IO.Path.GetExtension(EmpLogo.FileName);
                    var path = Path.Combine(Server.MapPath("~/ProfileImagesUploads"), FilePath);
                    EmpLogo.SaveAs(path);
                    obj.ProfilePicturePath = "/ProfileImagesUploads/" + FilePath;
                    db.SaveChanges();

                    //DeleteCookies 
                    if (Request.Cookies["TheSchollSystemProfilePicturePath"] != null)
                    {
                        System.Web.HttpCookie ckProfilePic = new System.Web.HttpCookie("TheSchollSystemProfilePicturePath");
                        ckProfilePic.Expires = DateTime.Now.AddDays(-1);
                        Response.Cookies.Add(ckProfilePic);
                    }
                    //AddCookies
                    System.Web.HttpCookie ckProfilePicture = new System.Web.HttpCookie("TheSchollSystemProfilePicturePath");
                    ckProfilePicture.Value = obj.ProfilePicturePath.ToString();
                    ckProfilePicture.Expires = DateTime.Now.AddDays(1);
                    Response.Cookies.Add(ckProfilePicture);
                    ViewBag.SuccessMessage = "Profile Picture has been changed successfully";
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
                return View();
            }
        }

        public JsonResult CheckPassword(string UserName, string OldPassword)
        {
            OldPassword = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(OldPassword);
            var UserPassword = db.AspNetUsers.Where(x => x.UserName == UserName && x.Password == OldPassword).FirstOrDefault();
            if (UserPassword == null)
            {
                return Json(false);
            }
            else
            {
                return Json(true);
            }
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [EmpDataFilter]
        public ActionResult UserProfile()
        {
            int UserID = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
            var EmpData = db.TblEmployees.Where(x => x.UserID == UserID).FirstOrDefault();
            return View(EmpData);
        }
        

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        public ActionResult UpdateEmpData()
        {
            return View();
        }


        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin,WebUser")]
        [HttpPost]
        public ActionResult UpdateEmpData(int SchoolID, int BranchID, string FirstName, string LastName, string Gender, string FatherName
            , string HusbandName, DateTime DOB, DateTime DOJ, string MaritalStatus, string CNIC, string EmailAddress, int Country,
            int Province, int City, string HomeAddress, int EmployeeStatus, string ContactNo1, string ContactNo2, int Department,
            int Designation, string JobDescription, string DutyLocation, string BankAccountNo, string NTNNo, string IncomeTax,
            HttpPostedFileBase EmpLogo = null, int TaxFiler = 0)
        {
            try
            {
                int UserTypeID = ERP_SchoolSystem.Classes.UserInfo.GetUserTypeID();
                string UserShortCode = db.TblUserTypes.Where(x => x.UserTypeID == UserTypeID).Select(x => x.ShortCode).FirstOrDefault();
                string SchoolShortCode = db.TblSchools.Where(x => x.SchoolId == SchoolID).Select(x => x.SchoolShortCode).FirstOrDefault();
                string TemEmpCode = SchoolShortCode + "_" + UserShortCode;
                var GetLastEmpCode = db.TblEmployees.Where(x => x.EmpCode.Contains(TemEmpCode)).OrderByDescending(x => x.EmpID).FirstOrDefault();
                string EmpCode = "";
                if (GetLastEmpCode == null)
                {
                    EmpCode = TemEmpCode + "_00001";
                }
                else
                {
                    string[] TemGetLastEmpCode = GetLastEmpCode.EmpCode.Split('_');
                    int Value = Convert.ToInt32(TemGetLastEmpCode[2].ToString()) + 1;
                    EmpCode = Value.ToString().PadLeft(5, '0');
                }

                TblEmployee Emp = new TblEmployee();
                Emp.EmpCode = EmpCode;
                Emp.UserID = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                Emp.SchoolID = SchoolID;
                Emp.FirstName = FirstName;
                Emp.LastName = LastName;
                Emp.FatherName = FatherName;
                Emp.HusbandName = HusbandName;
                Emp.Gender = Gender;
                Emp.DOB = DOB;
                Emp.DOJ = DOJ;
                Emp.MaritalStatus = MaritalStatus;
                Emp.CNIC = CNIC;
                Emp.Email = EmailAddress;
                Emp.CountryID = Country;
                Emp.ProvincesID = Province;
                Emp.CityID = City;
                Emp.HomeAddress = HomeAddress;
                Emp.Contact1 = ContactNo1;
                Emp.Contact2 = ContactNo2;
                Emp.StatusId = EmployeeStatus;
                Emp.DepartmentID = Department;
                Emp.DesignationID = Designation;
                Emp.NTNNo = NTNNo;
                Emp.BranchID = BranchID;
                Emp.JobDescription = JobDescription;
                Emp.DutyLocation = DutyLocation;
                Emp.BankAccountNo = BankAccountNo;
                Emp.TaxFiler = Convert.ToBoolean(TaxFiler);
                if (IncomeTax != "")
                {
                    Emp.IncomeTax = Convert.ToDouble(IncomeTax);
                }
                if (EmpLogo != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        EmpLogo.InputStream.CopyTo(ms);
                        Emp.EmpPicture = ms.GetBuffer();
                    }
                    string FilePath = ERP_SchoolSystem.Classes.UserInfo.GetUserName() + System.IO.Path.GetExtension(EmpLogo.FileName);
                    var path = Path.Combine(Server.MapPath("~/ProfileImagesUploads"), FilePath);
                    EmpLogo.SaveAs(path);
                    Emp.ProfilePicturePath = "/ProfileImagesUploads/" + FilePath;
                }
                Emp.IsActive = true;
                Emp.Createdby = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                Emp.CreatedOn = DateTime.Now;
                db.TblEmployees.Add(Emp);
                db.SaveChanges();

                System.Web.HttpCookie ckEmpName = new System.Web.HttpCookie("TheSchollSystemEmpName");
                ckEmpName.Value = Emp.FirstName + "" + Emp.LastName;
                ckEmpName.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(ckEmpName);

                System.Web.HttpCookie ckProfilePicture = new System.Web.HttpCookie("TheSchollSystemProfilePicturePath");
                ckProfilePicture.Value = Emp.ProfilePicturePath.ToString();
                ckProfilePicture.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(ckProfilePicture);

                System.Web.HttpCookie ckEMPADDED = new System.Web.HttpCookie("TheSchollSystemEMPADDED");
                ckEMPADDED.Value = "1";
                ckEMPADDED.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(ckEMPADDED);

                ViewBag.SuccessMessage = "New Employee Successfully Added";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
                return View();
            }
        }

        public ActionResult PageNotFound403()
        {
            return View();
        }
        public ActionResult PageNotFound404()
        {
            return View();
        }
        public ActionResult PageNotFound405()
        {
            return View();
        }
        public ActionResult PageNotFound500()
        {
            return View();
        }
        public ActionResult PageNotFound503()
        {
            return View();
        }

        
    }
}