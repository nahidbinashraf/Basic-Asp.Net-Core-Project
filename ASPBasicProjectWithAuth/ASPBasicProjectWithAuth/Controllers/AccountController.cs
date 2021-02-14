using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPBasicProjectWithAuth.Models;
using ASPBasicProjectWithAuth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ASPBasicProjectWithAuth.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager )
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
      
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
       
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = registerViewModel.Email, Email = registerViewModel.Email, City=registerViewModel.City };
                var reult = await userManager.CreateAsync(user, registerViewModel.Password);
                if (reult.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                foreach (var err in reult.Errors)
                {
                    ModelState.AddModelError("", err.Description);
                }

            }
            return View(registerViewModel);
        }

        [HttpPost][HttpGet]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) return Json(true);
            else return Json($"This {email} is already in use");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();

        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginViewModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password, loginViewModel.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return Redirect(returnUrl);
                    else return RedirectToAction("index", "home");
                }
                ModelState.AddModelError("", "Invalid login !!!");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }
       
    }
}