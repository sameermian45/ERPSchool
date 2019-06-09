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
        public ActionResult AddNewSchool()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public async System.Threading.Tasks.Task<ActionResult> AddNewSchool(string SchoolName, string SRPersonName, int Country, int City, string RegistrationNo, string PtclNo, string CellNo2, int IsActive, DateTime RegistrationDate, string SROffiersContactNo, int Province, string Address, string NTNNo, string CellNo1, string Email,string Website, HttpPostedFileBase SchoolLogo, string DefaultUserName, string DefaultPassword, string SchoolShortCode ,int PackageID)
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
                s.PackageId = PackageID;
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

                TblSchoolBranch B = new TblSchoolBranch();
                B.BranchTitle = "Head Office";
                B.SchoolID = s.SchoolId;
                B.IsActive = true;
                B.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                B.CreatedOn = DateTime.Now;
                db.TblSchoolBranches.Add(B);
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
    }
}