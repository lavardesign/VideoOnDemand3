﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using VOD.UI.Models;
using VOD.Common.Entities;
using VOD.Database.Services;
using VOD.UI.Services;
using VOD.Common.Extensions;

namespace VOD.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<VODUser> _signInManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<VODUser> signInMgr)
        {
            _logger = logger;
            _signInManager = signInMgr;
        }

        public IActionResult Index()
        {
            if (!_signInManager.IsSignedIn(User))
                return RedirectToPage("/Account/Login", new { Area = "Identity" });

            return RedirectToAction("Dashboard", "Membership");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
