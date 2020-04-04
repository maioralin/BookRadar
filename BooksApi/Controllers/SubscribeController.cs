using BooksMiddletier.OtherClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static BooksApi.DbHelper;

namespace BooksApi.Controllers
{
    public class SubscribeController : Controller
    {
        // GET: Subscribe
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string email, string platform, string city)
        {
            try
            {
                DbHelper db = new DbHelper();
                List<DbParameter> parameters = new List<DbParameter>();
                parameters.Add(new DbParameter("Email", System.Data.ParameterDirection.Input, email));
                parameters.Add(new DbParameter("Platform", System.Data.ParameterDirection.Input, platform));
                parameters.Add(new DbParameter("City", System.Data.ParameterDirection.Input, city));
                var result = db.ExecuteNonQuery("spBooks_Subscribe", parameters);
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[Save] There was an error subscribing user. Request: email: {email}, platform: {platform}, city: {city}");
            }
            return View("Success");
        }
    }
}