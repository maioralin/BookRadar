using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BooksApi.Controllers
{
    public class PrivacyController : Controller
    {
        // GET: Privacy
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult LoginSuccess()
        {
            return View();
        }
    }
}