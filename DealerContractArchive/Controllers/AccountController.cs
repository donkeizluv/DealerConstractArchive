using DealerContractArchive.EntityModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationLab.Controllers
{
    public class AccountController : Controller
    {
        private const string LoginStatusKey = "LoginStatus";
        //maybe private methods are more suitable since controllers dont seem to get call anywhere in code :/
        public string Issuer
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Issuer");
            }
        }
        public bool NoPwdCheck
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<bool>("NoPwdCheck");
            }
        }

        public string Domain
        {
            get
            {
                return _config.GetSection("Authentication").GetValue<string>("Domain");
            }
        }

        private DealerContractContext _context;
        private IConfiguration _config;
        public AccountController(DealerContractContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

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
            ViewBag.NoFooter = true;
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

        //not proper
        //[DllImport("advapi32.dll")]
        //public static extern bool LogonUser(string userName, string domainName, string password, int LogonType, int LogonProvider, ref IntPtr phToken);
        //private bool ValidateCredentials(string userName, string password)
        //{
        //    IntPtr tokenHandler = IntPtr.Zero;
        //    return LogonUser(userName, Domain, password, 3, 0, ref tokenHandler);
        //}

        private bool ValidateCredentials(string userName, string pwd)
        {
            if (NoPwdCheck) return true;
            using (var pc = new System.DirectoryServices.AccountManagement.PrincipalContext(System.DirectoryServices.AccountManagement.ContextType.Domain, Domain))
            {
                // validate the credentials
                return pc.ValidateCredentials(userName, pwd);
            }
        }
        private LoginLevel GetLoginLevel(string userName, string pwd)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(pwd))
                return LoginLevel.Error;
            if (!ValidateCredentials(userName, pwd)) return LoginLevel.Error;
            using (_context)
            {
                var user = _context.Users.FirstOrDefault(u => string.Compare(u.Username, userName, true) == 0);
                if (user == null) return LoginLevel.Error;
                var accountType = user.Type;
                if (!Enum.IsDefined(typeof(LoginLevel), accountType))
                    return LoginLevel.Error;
                return (LoginLevel)Enum.Parse(typeof(LoginLevel), accountType);
            }
        }

    }
}