using ERP_SchoolSystem.Models;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Controllers
{
    public class EmployeeController : Controller
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
        public ActionResult NewEmployee()
        {
            return View();
        }


        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> NewEmployee(int SchoolID, int BranchID, string FirstName, string LastName, string Gender, string FatherName
            , string HusbandName, DateTime DOB, DateTime DOJ, string MaritalStatus, string CNIC, string EmailAddress, int Country,
            int Province, int City, string HomeAddress, int EmployeeStatus, string ContactNo1, string ContactNo2, int Department,
            int Designation, string JobDescription, string DutyLocation, string BankAccountNo, string NTNNo, string IncomeTax,
            string[] InstituteNameN, string[] DegreeTitleN, string[] DegreeCompletionYearN, string[] RemarksN, string[] CompanyNameN,
            string[] JobTitleN, string[] JobDescriptionN, double[] EmpSalaryN, DateTime[] StartDateN,
            DateTime[] EndDateN, string[] RemarksExpN, string Password, int EmployeeUserType, HttpPostedFileBase EmpLogo = null,
            int TaxFiler = 0, int IsActive = 0)
        {
            try
            {
                string UserShortCode = db.TblUserTypes.Where(x => x.UserTypeID == EmployeeUserType).Select(x => x.ShortCode).FirstOrDefault();
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
                
                string DefaultUserName = EmpCode + "@TheSchoolSystem.com";
                
                int UserID = 0;
                var user = new ApplicationUser { UserName = DefaultUserName, Email = DefaultUserName };
                var result = await UserManager.CreateAsync(user, Password);
                if (result.Succeeded)
                {
                    AspNetUser obj = db.AspNetUsers.Where(x => x.Email == DefaultUserName).FirstOrDefault();
                    obj.SchoolID = SchoolID;
                    obj.BranchID = BranchID;
                    obj.UserName = DefaultUserName;
                    obj.Password = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(Password);
                    obj.UserTypeId = EmployeeUserType;
                    obj.IsActive = true;
                    obj.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    obj.AddedOn = DateTime.Now;
                    db.SaveChanges();
                    UserID = obj.UserID;

                    AspNetUserRole r1 = new AspNetUserRole();
                    r1.RoleId = "64daf2a4-c239-4300-b63c-5265868ac8bb";
                    r1.UserId = obj.Id;
                    db.AspNetUserRoles.Add(r1);
                    db.SaveChanges();
                }
                if(UserID != 0)
                {
                    TblEmployee Emp = new TblEmployee();
                    Emp.EmpCode = EmpCode;
                    Emp.UserID = UserID;
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
                    if (IncomeTax != null)
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
                    }
                    Emp.IsActive = Convert.ToBoolean(IsActive);
                    Emp.Createdby = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    Emp.CreatedOn = DateTime.Now;
                    db.TblEmployees.Add(Emp);
                    db.SaveChanges();

                    int a = 0;
                    foreach (var item in InstituteNameN)
                    {
                        TblEmpEduDetail EmpEdu = new TblEmpEduDetail();
                        EmpEdu.EmpID = Emp.EmpID;
                        EmpEdu.DegreeTitle = DegreeTitleN[a];
                        EmpEdu.InstituteName = InstituteNameN[a];
                        EmpEdu.DegreeCompletionYear = DegreeCompletionYearN[a];
                        EmpEdu.Remarks = RemarksN[a];
                        db.TblEmpEduDetails.Add(EmpEdu);
                        db.SaveChanges();
                        a++;
                    }
                    int b = 0;
                    foreach (var item in CompanyNameN)
                    {
                        TblEmpExpDetail EmpExp = new TblEmpExpDetail();
                        EmpExp.EmpID  = Emp.EmpID;
                        EmpExp.CompanyName = CompanyNameN[b];
                        EmpExp.JobTitle = JobTitleN[b];
                        EmpExp.JobDescription  = JobDescriptionN[b];
                        EmpExp.StartDate = StartDateN[b];
                        EmpExp.EndDate = EndDateN[b];
                        EmpExp.EmpSalary = EmpSalaryN[b];
                        EmpExp.Remarks = RemarksN[b];
                        db.TblEmpExpDetails.Add(EmpExp);
                        db.SaveChanges();
                        b++;
                    }
                    ViewBag.SuccessMessage = "New Employee Successfully Added";
                }
                else
                {
                    ViewBag.ErrorMessage = "Error on Creating User.. Try Again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            return View();
        }

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