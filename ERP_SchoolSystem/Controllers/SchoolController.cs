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
        public async System.Threading.Tasks.Task<ActionResult> AddNewSchool(string SchoolName, string SRPersonName, int Country, int City, string RegistrationNo, string PtclNo, string CellNo2, int IsActive, DateTime RegistrationDate, string SROffiersContactNo, int Province, string Address, string NTNNo, string CellNo1, string Email,string Website, HttpPostedFileBase SchoolLogo, string UserName, string Password)
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
                s.AdminUserName = UserName;
                if (SchoolLogo != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        SchoolLogo.InputStream.CopyTo(ms);
                        s.SchoolLogo = ms.GetBuffer();
                    }
                }
                s.CreatedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                s.CreatedDateTime = DateTime.Now;
                db.TblSchools.Add(s);
                db.SaveChanges();
                var user = new ApplicationUser { UserName = UserName, Email = UserName };
                var result = await UserManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    AspNetUser obj = db.AspNetUsers.Where(x => x.Email == UserName).FirstOrDefault();
                    obj.SchoolID = s.SchoolId;
                    obj.UserName = UserName;
                    obj.Password = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(Password);
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