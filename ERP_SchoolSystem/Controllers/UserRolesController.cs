using ERP_SchoolSystem.Filters;
using ERP_SchoolSystem.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Controllers
{
    public class UserRolesController : Controller
    {
        ERP_SchoolSystemEntities db = new ERP_SchoolSystemEntities();

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        public ActionResult SystemRoles(string RoleID)
        {
            if (RoleID != null)
            {
                ViewBag.RoleID = RoleID;
            }
            var roles = db.AspNetRoles.Where(x => x.Name != "WebUser" && x.Name != "SuperAdmin").ToList();
            return View(roles);
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin")]
        [HttpPost]
        public ActionResult SystemRoles(string RoleID, string RoleName)
        {
            try
            {
                if (RoleID == null)
                {
                    var RoleNameExit = db.AspNetRoles.Where(x => x.Name == RoleName).FirstOrDefault();
                    if (RoleNameExit != null)
                    {
                        ViewBag.ErrorMessage = "Role Already Exit";
                    }
                    else
                    {
                        ApplicationDbContext context = new ApplicationDbContext();
                        var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
                        var role = new Microsoft.AspNet.Identity.EntityFramework.IdentityRole();
                        role.Name = RoleName;
                        roleManager.Create(role);
                        ViewBag.SuccessMessage = "New Role Successfully Added";
                    }
                }
                else
                {
                    AspNetRole obj = db.AspNetRoles.Where(x => x.Id.ToString() == RoleID).FirstOrDefault();
                    obj.Name = RoleName;
                    db.SaveChanges();
                    ViewBag.SuccessMessage = "RoleName Successfully Update";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            var roles = db.AspNetRoles.Where(x => x.Name != "WebUser" && x.Name != "SuperAdmin").ToList();
            return View(roles);
        }

        [Authorize]
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin")]
        public ActionResult AssignUserRoles()
        {
            int SchoolID = ERP_SchoolSystem.Classes.UserInfo.GetSchoolID();
            if (SchoolID == 0)
            {
                var SystemUser = db.AspNetUsers.ToList();
                return View(SystemUser);
            }
            else
            {
                var SystemUser = db.AspNetUsers.Where(x => x.SchoolID == SchoolID).ToList();
                return View(SystemUser);
            }
        }


        public PartialViewResult UserRoles_Partial(string UserID)
        {
            ViewBag.UserID = UserID;
            return PartialView("UserRoles_Partial");
        }

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
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
        [CustomAuthorizeAttribute(Roles = "SuperAdmin,Admin")]
        [HttpPost]
        public ActionResult AssignUserRoles(string UserId, string[] RoleId)
        {
            try
            {
                List<AspNetUserRole> roles = (from r in db.AspNetUserRoles where (r.UserId == UserId) select r).ToList();
                foreach (var item in roles)
                {
                    var RoleName = db.AspNetRoles.Where(x => x.Id == item.RoleId).FirstOrDefault();
                    if(RoleName.Name == "SuperAdmin")
                    {

                    }
                    else  if (RoleName.Name == "WebUser")
                    {
                        
                    }
                    else
                    {
                        var result = UserManager.RemoveFromRoles(UserId, RoleName.Name);
                    }
                }
                if (RoleId != null)
                {
                    foreach (var u in RoleId)
                    {
                        var RoleName = db.AspNetRoles.Where(x => x.Id == u).FirstOrDefault();
                        if (RoleName.Name == "SuperAdmin")
                        {

                        }
                        else if (RoleName.Name == "WebUser")
                        {

                        }
                        else
                        {
                            var result = UserManager.AddToRole(UserId, RoleName.Name);
                        }
                    }
                }

                ViewBag.SuccessMessage = "Role's Successfully Assign";
            }
            catch(Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message.ToString();
            }
            int SchoolID = ERP_SchoolSystem.Classes.UserInfo.GetSchoolID();
            if (SchoolID == 0)
            {
                var SystemUser = db.AspNetUsers.ToList();
                return View(SystemUser);
            }
            else
            {
                var SystemUser = db.AspNetUsers.Where(x => x.SchoolID == SchoolID).ToList();
                return View(SystemUser);
            }
        }
    }
}