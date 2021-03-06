﻿using ERP_SchoolSystem.Models;
using System;
using System.Linq;
using System.Web;

namespace ERP_SchoolSystem.Classes
{
    public static class UserInfo
    {
        static UserInfo()
        {

        }
        public static int GetUserID()
        {
           int UserID = Convert.ToInt32(HttpContext.Current.Request.Cookies["TheSchollSystemUserID"].Value);
            return UserID;
        }
        public static string GetUserName()
        {
            string  Username = HttpContext.Current.Request.Cookies["TheSchollSystemUsername"].Value;
            return Username;
        } 
        public static int GetSchoolID()
        {
            int SchoolID = Convert.ToInt32(HttpContext.Current.Request.Cookies["TheSchollSystemSchoolID"].Value);
            return SchoolID;
        }

        public static int GetUserTypeID()
        {
            int UserTypeID = 0;
            try
            {
                 UserTypeID = Convert.ToInt32(HttpContext.Current.Request.Cookies["TheSchollSystemUserTypeID"].Value);
            }
            catch (Exception ex)
            {
                UserTypeID = 0;
            }
            return UserTypeID;
        }

        public static int BranchID()
        {
            int BranchID = Convert.ToInt32(HttpContext.Current.Request.Cookies["TheSchollSystemBranchID"].Value);
            return BranchID;
        }

        public static string EmpName()
        {
            string Empname = HttpContext.Current.Request.Cookies["TheSchollSystemEmpName"].Value;
            return Empname;
        }
        public static bool IsEmpAdded()
        {
            if(Convert.ToInt32(HttpContext.Current.Request.Cookies["TheSchollSystemEMPADDED"].Value) == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static string GetEmpImage()
        {
            string Path = "/Content/Images/noimageicon.png";
            if(HttpContext.Current.Request.Cookies["TheSchollSystemProfilePicturePath"].Value != "Not Added")
            {
                Path = HttpContext.Current.Request.Cookies["TheSchollSystemProfilePicturePath"].Value;
            }
            return Path;
        }

        public static string GetSchoolName()
        {
            string SchoolName = HttpContext.Current.Request.Cookies["TheSchollSystemSchoolName"].Value;
            return SchoolName;
        }
        public static string GetSchoolLogo()
        {
            string Path = "/Content/Images/noimageicon.png";
            if (HttpContext.Current.Request.Cookies["TheSchollSystemSchoollogoPath"].Value != "Not Added")
            {
                Path = HttpContext.Current.Request.Cookies["TheSchollSystemSchoollogoPath"].Value;
            }
            return Path;
        }

    }
}