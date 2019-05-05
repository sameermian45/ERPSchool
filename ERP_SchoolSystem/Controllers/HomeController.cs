using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ERP_SchoolSystem.Controllers
{

    public class HomeController : Controller
    {
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PageNotFound403()
        {
            DataSet ds = ERP_SchoolSystem.Classes.DAL.DAL.VerifyTaxFiler("","","");


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