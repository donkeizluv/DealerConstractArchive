using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationLab.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View();
        }

        [HttpPost]          
        public async Task<IActionResult> DoLogin([FromForm]string userName = "", [FromForm]string pwd = "")
        {
            const string Issuer = "https://hdsaison.com.vn";
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, "Dant", ClaimValueTypes.String, Issuer));
            var userIdentity = new ClaimsIdentity("UserLogin");
            userIdentity.AddClaims(claims);
            var userPrincipal = new ClaimsPrincipal(userIdentity);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                userPrincipal,
                new AuthenticationProperties
                {
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(60),
                    IsPersistent = false,
                    AllowRefresh = false
                });

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LogOut()
        {
            if (User.Identities.Any(u => u.IsAuthenticated))
            {
                await HttpContext.SignOutAsync();
            }
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }
    }
}