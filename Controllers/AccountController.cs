using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class AccountController : Controller
    {
        private readonly LearningContext _context;
        public AccountController(LearningContext context)
        {
            _context = context;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [HttpPost]
        public IActionResult Login(User u)
        {
            var data = _context.Users.FirstOrDefault(m => m.Email == u.Email && m.PasswordHash == u.PasswordHash);
            if (data != null)
            {
                bool isValid = data.Email == u.Email && data.PasswordHash == u.PasswordHash;
                if (isValid)
                {
                    var id = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, u.Email) }, CookieAuthenticationDefaults.AuthenticationScheme);
                    var prin = new ClaimsPrincipal(id);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, prin);
                    HttpContext.Session.SetString("Name", u.Email);

                    // Store user ID in TempData
                    HttpContext.Session.SetString("UserId", u.Id.ToString());
                    TempData["userId"] = data.Id;
                    TempData.Keep();

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["loginmsg"] = "Invalid Password";
                }
            }
            else
            {
                TempData["loginmsg"] = "User Not Found";
            }
            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return RedirectToAction("Login", "Account");
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(User u)
        {
            var data = _context.Users.Add(u);
            _context.SaveChanges();
            return RedirectToAction("Login");
        }

    }

}
