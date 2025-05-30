using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using MyPortfolio.Models;

namespace MyPortfolio.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;


        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
            
        }
        public IActionResult Logout()
        {
            // Clear session on logout
            HttpContext.Session.Remove("IsLoggedIn");

            return RedirectToAction("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(AppUser model)
        {
            //model.FullName = string.Empty;
            //model.ConfirmPassword = string.Empty;

            ModelState.Remove(nameof(model.FullName));
            ModelState.Remove(nameof(model.ConfirmPassword));

            if (ModelState.IsValid)
            {
                var user = await _context.AppUsers.SingleOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                
                if (user != null)
                {
                    // Login successful — set session/cookie/etc.
                    TempData["LoginStatus"] = "Login successful!";
                    TempData["LoginSuccess"] = true;
                    HttpContext.Session.SetString("IsLoggedIn", "true");
                    HttpContext.Session.SetString("Username", model.Email);
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["LoginStatus"] = "Invalid login credentials. Click OK to register.";
                    TempData["LoginSuccess"] = false;
                    return RedirectToAction("Register");
                }
                
            }
            ViewBag.Error = "Invalid credentials";
            return View("Register"); // Return the view with error messages if any
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(AppUser user)
        {
            if (ModelState.IsValid)
            {
                _context.AppUsers.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }

            return View(user);
        }
    }    
}
