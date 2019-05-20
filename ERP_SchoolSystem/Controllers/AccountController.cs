using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using ERP_SchoolSystem.Models;
using System.Web.Security;
using Microsoft.AspNet.Identity.EntityFramework;

namespace ERP_SchoolSystem.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities();

        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
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

        [AllowAnonymous]
        public ActionResult AccessDenied()
        {
            return View();
        }
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public async Task<ActionResult> Login(string returnUrl)
        {
            string UserName = null;
            string Password = null;
            if (Request.Cookies["TheSchollSystemUsername"] != null)
            {
                UserName = Request.Cookies["TheSchollSystemUsername"].Value;
            }
            if (Request.Cookies["TheSchollSystemPassword"] != null)
            {
                Password = Request.Cookies["TheSchollSystemPassword"].Value;
            }
            if (UserName != null && Password != null)
            {
                using (ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities())
                {
                    var IsUserValid = db.AspNetUsers.Where(x => x.UserName == UserName && x.Password == Password).FirstOrDefault();
                    if (IsUserValid != null)
                    {
                        var SchoolDetails = db.TblSchools.Where(x => x.SchoolId == IsUserValid.SchoolID).FirstOrDefault();
                        var checkpayment = db.TblSchoolMonthlyPayments.Where(x => x.SchoolID == SchoolDetails.SchoolId && x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year).FirstOrDefault();

                        if(IsUserValid.UserTypeId == 1)
                        {
                            int RememberMe = 1;
                            string DP = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Decrypt(Password);
                            var result = await SignInManager.PasswordSignInAsync(UserName, DP, Convert.ToBoolean(RememberMe), shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    return RedirectToLocal(returnUrl);
                                case SignInStatus.LockedOut:
                                    ViewBag.Message = "Your user name has been locked due to invalid attemps. Please contact to system admin.";
                                    return View();
                                case SignInStatus.Failure:
                                default:
                                    ViewBag.Message = "The user name or password provided is incorrect.";
                                    return View();
                            }
                        }
                        else if (checkpayment == null)
                        {
                            if (IsUserValid.UserTypeId == 2)
                            {
                                return RedirectToAction("PaymentAlert", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                                return View();
                            }
                        }
                        else if (IsUserValid.IsActive == true && SchoolDetails.IsActive == true)
                        {
                            int RememberMe = 1;
                            var result = await SignInManager.PasswordSignInAsync(UserName, ERP_SchoolSystem.Classes.Encrypt_Decrypt.Decrypt(Password), Convert.ToBoolean(RememberMe), shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    return RedirectToLocal(returnUrl);
                                case SignInStatus.LockedOut:
                                    ViewBag.Message = "Your user name has been locked due to invalid attemps. Please contact to system admin.";
                                    return View();
                                case SignInStatus.Failure:
                                default:
                                    ViewBag.Message = "The user name or password provided is incorrect.";
                                    return View();
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                        return View();
                    }
                }
            }
            else
            {
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }

        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(string UserName, string Password, string returnUrl, int RememberMe = 0)
        {
            try
            {
                using (ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities())
                {
                    string P = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(Password);
                    var IsUserValid = db.AspNetUsers.Where(x => x.UserName == UserName && x.Password == P).FirstOrDefault();
                    if (IsUserValid != null)
                    {
                        var SchoolDetails = db.TblSchools.Where(x => x.SchoolId == IsUserValid.SchoolID).FirstOrDefault();
                        var checkpayment = db.TblSchoolMonthlyPayments.Where(x => x.SchoolID == SchoolDetails.SchoolId && x.Month == DateTime.Now.Month && x.Year == DateTime.Now.Year).FirstOrDefault();
                        if (IsUserValid.UserTypeId == 1)
                        {
                            var result = await SignInManager.PasswordSignInAsync(UserName, Password, Convert.ToBoolean(RememberMe), shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    string EmpName = "System Admin";
                                    string ProfilePicturePath = "Not Added";
                                    int EmpAdded = 0;
                                    var Emp = db.TblEmployees.Where(x => x.UserID == IsUserValid.UserID).FirstOrDefault();
                                    if (Emp != null)
                                    {
                                        EmpName = Emp.FirstName + " " + Emp.LastName;
                                        ProfilePicturePath = Emp.ProfilePicturePath;
                                        EmpAdded = 1;
                                    }
                                    CreateCookies(UserName, Password, RememberMe, IsUserValid.UserID, EmpName, IsUserValid.SchoolID, IsUserValid.UserTypeId, IsUserValid.BranchID, ProfilePicturePath, EmpAdded);
                                    return RedirectToLocal(returnUrl);
                                case SignInStatus.LockedOut:
                                    ViewBag.Message = "Your user name has been locked due to invalid attemps. Please contact to system admin.";
                                    return View();
                                case SignInStatus.Failure:
                                default:
                                    ViewBag.Message = "The user name or password provided is incorrect.";
                                    return View();
                            }
                        }
                        else if (checkpayment == null)
                        {
                            if (IsUserValid.UserTypeId == 2)
                            {
                                return RedirectToAction("PaymentAlert", "Home");
                            }
                            else
                            {
                                ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                                return View();
                            }
                        }
                        else if (IsUserValid.IsActive == true && SchoolDetails.IsActive == true)
                        {
                            var result = await SignInManager.PasswordSignInAsync(UserName, Password, Convert.ToBoolean(RememberMe), shouldLockout: false);
                            switch (result)
                            {
                                case SignInStatus.Success:
                                    string EmpName = "System Admin";
                                    string ProfilePicturePath = "Not Added";
                                    int EmpAdded = 0;
                                    var Emp = db.TblEmployees.Where(x => x.UserID == IsUserValid.UserID).FirstOrDefault();
                                    if (Emp != null)
                                    {
                                        EmpName = Emp.FirstName + " " + Emp.LastName;
                                        ProfilePicturePath = Emp.ProfilePicturePath;
                                        EmpAdded = 1;
                                    }
                                    CreateCookies(UserName, Password, RememberMe, IsUserValid.UserID, EmpName, IsUserValid.SchoolID, IsUserValid.UserTypeId, IsUserValid.BranchID, ProfilePicturePath, EmpAdded);
                                    return RedirectToLocal(returnUrl);
                                case SignInStatus.LockedOut:
                                    ViewBag.Message = "Your user name has been locked due to invalid attemps. Please contact to system admin.";
                                    return View();
                                case SignInStatus.Failure:
                                default:
                                    ViewBag.Message = "The user name or password provided is incorrect.";
                                    return View();
                            }
                        }
                        else
                        {
                            ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.Message = "Your user name has been deactive. Please contact to system admin.";
                        return View();
                    }
                }
            }
            catch (Exception Ex)
            {
                ViewBag.Message = Ex.Message.ToString() ;
                return View();
            }
        }





        public void CreateCookies(string UserName, string Password, int RememberMe , int UserID ,string EmpName, int SchoolID , int? UserTypeID , int? BranchID , string  ProfilePicturePath , int EMPADDED)
        {

            System.Web.HttpCookie ckUserID = new System.Web.HttpCookie("TheSchollSystemUserID");
            ckUserID.Value = UserID.ToString();
            ckUserID.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckUserID);

            System.Web.HttpCookie ckEmpName = new System.Web.HttpCookie("TheSchollSystemEmpName");
            ckEmpName.Value = EmpName;
            ckEmpName.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckEmpName);

            System.Web.HttpCookie ckSchoolID = new System.Web.HttpCookie("TheSchollSystemSchoolID");
            ckSchoolID.Value = SchoolID.ToString();
            ckSchoolID.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckSchoolID);

            System.Web.HttpCookie ckUserTypeID = new System.Web.HttpCookie("TheSchollSystemUserTypeID");
            ckUserTypeID.Value = UserTypeID.ToString();
            ckUserTypeID.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckUserTypeID);

            System.Web.HttpCookie ckBranchID = new System.Web.HttpCookie("TheSchollSystemBranchID");
            ckBranchID.Value = BranchID.ToString();
            ckBranchID.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckBranchID);

            System.Web.HttpCookie ckProfilePicture = new System.Web.HttpCookie("TheSchollSystemProfilePicturePath");
            ckProfilePicture.Value = ProfilePicturePath.ToString();
            ckProfilePicture.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckProfilePicture);

            System.Web.HttpCookie ckEMPADDED = new System.Web.HttpCookie("TheSchollSystemEMPADDED");
            ckEMPADDED.Value = EMPADDED.ToString();
            ckEMPADDED.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckEMPADDED);

            System.Web.HttpCookie ckUsername = new System.Web.HttpCookie("TheSchollSystemUsername");
            ckUsername.Value = UserName;
            ckUsername.Expires = DateTime.Now.AddDays(1);
            Response.Cookies.Add(ckUsername);


            if (RememberMe == 1)
            {
                System.Web.HttpCookie ckPassword = new System.Web.HttpCookie("TheSchollSystemPassword");
                ckPassword.Value = ERP_SchoolSystem.Classes.Encrypt_Decrypt.Encrypt(Password);
                ckPassword.Expires = DateTime.Now.AddDays(1);
                Response.Cookies.Add(ckPassword);
            }
            else
            {
                if (Request.Cookies["TheSchollSystemPassword"] != null)
                {
                    System.Web.HttpCookie ckPassword = new System.Web.HttpCookie("TheSchollSystemPassword");
                    ckPassword.Expires = DateTime.Now.AddDays(-1);
                    Response.Cookies.Add(ckPassword);
                }
            }
        }


        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
            }
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }


        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            if (Request.Cookies["TheSchollSystemUserID"] != null)
            {
                System.Web.HttpCookie ckUserID = new System.Web.HttpCookie("TheSchollSystemUserID");
                ckUserID.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckUserID);
            }

            if (Request.Cookies["TheSchollSystemEmpName"] != null)
            {
                System.Web.HttpCookie ckEmpName = new System.Web.HttpCookie("TheSchollSystemEmpName");
                ckEmpName.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckEmpName);
            }

            if (Request.Cookies["TheSchollSystemSchoolID"] != null)
            {
                System.Web.HttpCookie ckSchoolID = new System.Web.HttpCookie("TheSchollSystemSchoolID");
                ckSchoolID.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckSchoolID);
            }

            if (Request.Cookies["TheSchollSystemUserTypeID"] != null)
            {
                System.Web.HttpCookie ckUserTypeID = new System.Web.HttpCookie("TheSchollSystemUserTypeID");
                ckUserTypeID.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckUserTypeID);
            }

            if (Request.Cookies["TheSchollSystemBranchID"] != null)
            {
                System.Web.HttpCookie ckBranchID = new System.Web.HttpCookie("TheSchollSystemBranchID");
                ckBranchID.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckBranchID);
            }
            if (Request.Cookies["TheSchollSystemUsername"] != null)
            {
                System.Web.HttpCookie ckUsername = new System.Web.HttpCookie("TheSchollSystemUsername");
                ckUsername.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckUsername);
            }
            if (Request.Cookies["TheSchollSystemPassword"] != null)
            {
                System.Web.HttpCookie ckPassword = new System.Web.HttpCookie("TheSchollSystemPassword");
                ckPassword.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckPassword);
            }

            if (Request.Cookies["TheSchollSystemProfilePicturePath"] != null)
            {
                System.Web.HttpCookie ckProfilePicture = new System.Web.HttpCookie("TheSchollSystemProfilePicturePath");
                ckProfilePicture.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckProfilePicture);
            }

            if (Request.Cookies["TheSchollSystemEMPADDED"] != null)
            {
                System.Web.HttpCookie ckEMPADDED = new System.Web.HttpCookie("TheSchollSystemEMPADDED");
                ckEMPADDED.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(ckEMPADDED);
            }
            


            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}