using DealerContractArchive.EntityModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationLab.Controllers
{
    public class AccountController : Controller
    {
        private const string Issuer = "https://hdsaison.com.vn";
        private const string LoginStatusKey = "LoginStatus";
        private readonly string Domain = "sgvf.sgcf"; //doesnt really matter :/

        public enum LoginLevel
        {
            Error,
            User,
            ReadOnly,
            Admin
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identities.Any(u => u.IsAuthenticated))
            {
                return RedirectToAction("Index", "Home");
            }
            if (TempData.ContainsKey(LoginStatusKey))
                ViewBag.LoginStatus = TempData[LoginStatusKey];
            //return login form view
            return View();
        }
        
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> DoLogin([FromForm]string userName = "", [FromForm]string pwd = "")
        {

            var loginLevel = GetLoginLevel(userName, pwd);
            if (loginLevel == LoginLevel.Error) return LoginFail();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName, ClaimValueTypes.String, Issuer),
                new Claim(ClaimTypes.Role, loginLevel.ToString(), ClaimValueTypes.String, Issuer)
            };
            var userIdentity = new ClaimsIdentity("UserCred");
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

        private IActionResult LoginFail()
        {
            TempData["LoginStatus"] = "Login failed."; //pass data to redirect
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Forbidden()
        {
            return View();
        }


        [DllImport("advapi32.dll")]
        public static extern bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken);
        private bool ValidateCredentials(string userName, string password)
        {
            IntPtr tokenHandler = IntPtr.Zero;
            return LogonUser(userName, Domain, password, 3, 0, ref tokenHandler);
        }
        private LoginLevel GetLoginLevel(string userName, string pwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                return LoginLevel.Error;
            if (!ValidateCredentials(userName, pwd)) return LoginLevel.Error;
            using (var context = new DealerContractContext())
            {
                var user = context.Users.FirstOrDefault(u => string.Compare(u.Username, userName, true) == 0);
                if (user == null) return LoginLevel.Error;
                var accountType = user.Type;
                if (!Enum.IsDefined(typeof(LoginLevel), accountType))
                    return LoginLevel.Error;
                return (LoginLevel)Enum.Parse(typeof(LoginLevel), accountType);
            }   
        }

    }
}