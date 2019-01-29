using HCL_MiniProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HCL_MiniProject.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// The params that we read from the URL, are the result of try to upload and save a new request, and if the
        /// resoult is success then we try to get the id generated for the order.
        /// This data is sended to the view in the viewbag, and the view validate the content and decide how to show to the user.
        /// When isnt data in the url the view doesnt show any message.
        /// </summary>
        /// <returns>View Home/Index</returns>
        public ActionResult Index()
        {
            ViewBag.Process = Request.Params["result"];
            ViewBag.Id = Request.Params["idOrder"];
            return View();
        }

        //Application Description
        public ActionResult About()
        {
            ViewBag.Message = "Application description page.";

            return View();
        }

        //Contact Information
        public ActionResult Contact()
        {
            ViewBag.Message = "Contact page.";

            return View();
        }
    }
}