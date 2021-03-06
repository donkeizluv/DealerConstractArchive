﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DealerContractArchive.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace DealerContractArchive.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        [Authorize]
        public IActionResult Index()
        {
            string role = string.Empty;
            var claim = HttpContext.User.FindFirst(ClaimTypes.Role);
            if (claim != null)
                role = claim.Value;
            HttpContext.Response.Cookies.Append("role", role);
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
