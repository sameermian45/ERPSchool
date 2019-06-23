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
    public class StudentController : Controller
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
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult AddNewStudent()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> AddNewStudent(int SchoolID, int BranchID, string StudentName, string FatherName, string Gender, DateTime DOB, DateTime DOA, string CNIC, string EmailAddress,
            int Std_Country, int Std_Postal_Province, int Std_Postal_City, int Std_Permanent_Province, int Std_Permanent_City, string Std_R_HomeAddress, string Std_P_HomeAddress, string Std_ContactNo,
            string Std_HomeContactNo, string GuardianName, string GuardianCNIC, int GuardianRelationship, string GuardianEmail, int GuardianCountry, int GuardianProvince, int GuardianCity,
            string GuardianHomeAddress, string GuardianContactPrimary, string GuardianContactSecoundary, int GuardianOccupation, int GuardianMonthlyIncome,
            string[] SchoolNameN, string[] ClassNameN, string[] StudyGradesN, string[] RemarksN, string[] DocumentName, HttpPostedFileBase[] Document, string DocumentDetails, string Password, string StudentUserName,
            string GuardianOfficeContact, int StudentUserType, string StudentBatchNo, string StudentBatchDetails, int Studyprogram, HttpPostedFileBase StdLogo, int IsActive = 0)
        {
            try
            {
                string TemRollNo = db.TblUserTypes.Where(x => x.UserTypeID == StudentUserType).Select(x => x.ShortCode).FirstOrDefault();
                var GetLastRollNo = db.Tbl_Std_Details.Where(x => x.Std_RollNo.Contains(TemRollNo) && x.SchoolID == SchoolID).OrderByDescending(x => x.ID).FirstOrDefault();
                string RollNo = "";
                if (GetLastRollNo == null)
                {
                    RollNo = TemRollNo + "_00001";
                }
                else
                {
                    string[] TemGetLastRollNo = GetLastRollNo.Std_RollNo.Split('_');
                    int Value = Convert.ToInt32(TemGetLastRollNo[1].ToString()) + 1;
                    RollNo = Value.ToString().PadLeft(5, '0');
                }
                string DefaultUserName = RollNo + "@TheSchoolSystem.com";
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
                    obj.UserTypeId = StudentUserType;
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

                    AspNetUserRole r2 = new AspNetUserRole();
                    r2.RoleId = "2c22cfc7-0871-4f05-b4ef-b410c7ededd5";
                    r2.UserId = obj.Id;
                    db.AspNetUserRoles.Add(r2);
                    db.SaveChanges();
                }
                if (UserID != 0)
                {
                    Tbl_Std_Details std = new Tbl_Std_Details();

                    std.SchoolID = SchoolID;
                    std.UserID = UserID;
                    std.BranchID = BranchID;
                    std.StudentName = StudentName;
                    std.FatherName = FatherName;
                    std.DOB = DOB;
                    std.Gender = Gender;
                    std.RegistrationDate = DOA;
                    std.CNIC = CNIC;
                    std.Contact = Std_ContactNo;
                    std.Home_Phone = Std_HomeContactNo;
                    std.Email = EmailAddress;
                    std.CountryID = Std_Country;
                    std.P_ProvincesID = Std_Postal_Province;
                    std.P_CityID = Std_Postal_City;
                    std.P_Address = Std_P_HomeAddress;
                    std.R_ProvincesID = Std_Permanent_Province;
                    std.R_CityID = Std_Permanent_City;
                    std.R_Address = Std_R_HomeAddress;
                    std.G_Name = GuardianName;
                    std.G_ContactPrimary = GuardianContactPrimary;
                    std.G_Releationship = GuardianRelationship;
                    std.G_CountryID = GuardianCountry;
                    std.G_ProvincesID = GuardianProvince;
                    std.G_CityID = GuardianCity;
                    std.G_Address = GuardianHomeAddress;
                    std.G_Occupation = GuardianOccupation;
                    std.G_MonthlyIncome = GuardianMonthlyIncome;
                    std.G_ContactSecoundary = GuardianContactSecoundary;
                    std.G_WorkContact = GuardianOfficeContact;
                    std.G_Email = GuardianEmail;
                    std.G_CNIC = GuardianCNIC;
                    std.Std_BatchNo = StudentBatchNo;
                    std.Std_BatchDetails = StudentBatchDetails;
                    std.Std_RollNo = RollNo;
                    std.Std_Study_ProgramID = Studyprogram;
                    std.Std_DocDetails = DocumentDetails;

                    if (StdLogo != null)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            StdLogo.InputStream.CopyTo(ms);
                            std.Std_Picture = ms.GetBuffer();
                        }
                        string FilePath = DefaultUserName + System.IO.Path.GetExtension(StdLogo.FileName);
                        var path = Path.Combine(Server.MapPath("~/ProfileImagesUploads"), FilePath);
                        StdLogo.SaveAs(path);
                        std.ProfilePicturePath = "/ProfileImagesUploads/" + FilePath;
                    }
                    std.IsActive = Convert.ToBoolean(IsActive);
                    std.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                    std.AddedOn = DateTime.Now;
                    db.Tbl_Std_Details.Add(std);
                    db.SaveChanges();


                    if (Document != null)
                    {
                        int a = 0;
                        foreach (var item in Document)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if(item == null)
                                {
                                    break;
                                }
                                Tbl_Std_Last_School_Document_Details edu = new Tbl_Std_Last_School_Document_Details();
                                edu.SchoolID = SchoolID;
                                edu.StudentID = std.StudentID;
                                edu.Document_Name = DocumentName[a];
                                item.InputStream.CopyTo(ms);
                                edu.Document_Image = ms.GetBuffer();
                                edu.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                                edu.AddedOn = DateTime.Now;
                                db.Tbl_Std_Last_School_Document_Details.Add(edu);
                                db.SaveChanges();
                            }
                        }
                        int b = 0;
                        if (SchoolNameN != null)
                        {
                            foreach (var item in SchoolNameN)
                            {
                                Tbl_Std_Last_School_Details st = new Tbl_Std_Last_School_Details();
                                st.SchoolID = SchoolID;
                                st.StudentID = std.StudentID;
                                st.Last_SchoolName = SchoolNameN[b];
                                st.Last_SchoolDetails = RemarksN[b];
                                st.Last_SchoolClassName = ClassNameN[b];
                                st.Study_Grades = StudyGradesN[b];
                                st.AddedBy = ERP_SchoolSystem.Classes.UserInfo.GetUserID();
                                st.AddedOn = DateTime.Now;
                                db.Tbl_Std_Last_School_Details.Add(st);
                                db.SaveChanges();
                            }
                        }
                    }

                    ViewBag.SuccessMessage = "New Student Successfully Added";
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

        [Authorize]
        public PartialViewResult Get_Std_Postal_ProvincesList(int CountryID)
        {
            try
            {
                var ProvincesList = db.TblProvinces.Where(x => x.CountryID == CountryID).ToList();
                return PartialView("Get_Std_Postal_ProvincesList", ProvincesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }
        [Authorize]
        public PartialViewResult Get_Std_Permanent_ProvincesList(int CountryID)
        {
            try
            {
                var ProvincesList = db.TblProvinces.Where(x => x.CountryID == CountryID).ToList();
                return PartialView("Get_Std_Permanent_ProvincesList", ProvincesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        public PartialViewResult Get_Std_Postal_CitiesList(int ProvinceID)
        {
            try
            {
                var CitiesList = db.TblCities.Where(x => x.ProvincesID == ProvinceID).ToList();
                return PartialView("Get_Std_Postal_CitiesList", CitiesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        public PartialViewResult Get_Std_Permanent_CitiesList(int ProvinceID)
        {
            try
            {
                var CitiesList = db.TblCities.Where(x => x.ProvincesID == ProvinceID).ToList();
                return PartialView("Get_Std_Permanent_CitiesList", CitiesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        public PartialViewResult Get_G_ProvincesList(int CountryID)
        {
            try
            {
                var ProvincesList = db.TblProvinces.Where(x => x.CountryID == CountryID).ToList();
                return PartialView("Get_G_ProvincesList", ProvincesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }

        [Authorize]
        public PartialViewResult Get_G_CitiesList(int ProvinceID)
        {
            try
            {
                var CitiesList = db.TblCities.Where(x => x.ProvincesID == ProvinceID).ToList();
                return PartialView("Get_G_CitiesList", CitiesList);
            }
            catch (Exception ex)
            {
                return PartialView(null);
            }
        }
    }
}