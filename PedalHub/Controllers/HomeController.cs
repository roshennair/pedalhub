﻿using Microsoft.AspNetCore.Mvc;
using PedalHub.Models;
using System.Diagnostics;

namespace PedalHub.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (HttpContext.User.IsInRole("Administrator"))
            {
                return RedirectToAction("AdminIndex");
            }

            return View();
        }

        public IActionResult AdminIndex()
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