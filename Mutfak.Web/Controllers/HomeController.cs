﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mutfak.Web.Models;

namespace Mutfak.Web.Controllers
{
    public class HomeController : Controller
    {
        
        [HttpGet, AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet, AllowAnonymous]
        public ViewResult About()
        {
            return this.View();
        }

        [HttpGet, AllowAnonymous]
        public ViewResult Contact()
        {
            return this.View(new ContactModel());
        }
    }
}
