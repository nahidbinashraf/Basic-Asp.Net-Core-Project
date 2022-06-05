using ASPBasicProjectWithAuth.Models;
using ASPBasicProjectWithAuth.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ASPBasicProjectWithAuth.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
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
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email
                };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmaationLink = Url.Action("ConfrimaMail", "Account", new { userID= user.Id, token = token }, Request.Scheme);

                    if (User.IsInRole("Admin") && signInManager.IsSignedIn(User))
                    {
                        return RedirectToAction("ListUsers", "Administrator");
                    }

                    ViewBag.ErrorTitle = "Check you confirmation mail";
                    ViewBag.ErrorMessage = confirmaationLink;
                    return View("Error");
                    //else
                    //{
                    //    await signInManager.SignInAsync(user, isPersistent: false);
                    //    return RedirectToAction("index", "home");
                    //}
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }


        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnURL)
        {
            model.ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            var user = await userManager.FindByEmailAsync(model.Email);
            
            if (ModelState.IsValid)
            {
                if (user != null && await userManager.CheckPasswordAsync(user, model.Password) && !user.EmailConfirmed)
                {
                    ModelState.AddModelError("", "Email not confirmed yet");
                    return View(model);
                }
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(returnURL) && Url.IsLocalUrl(returnURL))
                    {
                        return Redirect(returnURL);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var findUser = await userManager.FindByEmailAsync(email);
            if (findUser == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"This {email} Email is already in use");
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();

        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                        new { ReturnUrl = returnUrl });
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            LoginViewModel loginViewModel = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }

            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View("Login", loginViewModel);
            }


            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            ApplicationUser user = null;

            if (email != null)
            {
                // Find the user
                user = await userManager.FindByEmailAsync(email);

                // If email is not confirmed, display login view with validation error
                if (user != null && !user.EmailConfirmed)
                {
                    ModelState.AddModelError(string.Empty, "Email not confirmed yet");
                    return View("Login", loginViewModel);
                }
            }
            var siginInResult = await signInManager.
                ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            if (siginInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                if (email != null)
                {

                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };

                        await userManager.CreateAsync(user);
                        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                        var confirmaationLink = Url.Action("ConfrimaMail", "Account", new { userID = user.Id, token = token }, Request.Scheme);


                        ViewBag.ErrorTitle = "Registration successful";
                        ViewBag.ErrorMessage = confirmaationLink;
                        return View("Error");


                    }

                    // Add a login (i.e insert a row for the user in AspNetUserLogins table)
                    await userManager.AddLoginAsync(user, info);




                    await signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }

                // If we cannot find the user email we cannot continue
                ViewBag.ErrorTitle = $"Email claim not received from: {info.LoginProvider}";
                ViewBag.ErrorMessage = "Please contact support on Pragim@PragimTech.com";

                return View("Error");
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfrimaMail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("index", "home");
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"The User ID {userId} is invalid";
                return View("NotFound");
            }

            var result = await userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                ViewBag.ErrorTitle = "Email confirmed";
                ViewBag.ErrorMessage = "Login Now";
                return View("Error");
            }

            ViewBag.ErrorTitle = "Email cannot be confirmed";
            return View("Error");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {

                var user = await userManager.FindByEmailAsync(model.Email);
                if (user != null && await userManager.IsEmailConfirmedAsync(user))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                            new { email = model.Email, token = token }, Request.Scheme);

                    //ViewBag.ErrorTitle = "Paasword reset link";
                    //ViewBag.ErrorMessage = passwordResetLink;
                    //return View("Error");

                    return Redirect(passwordResetLink);
                }

                return View("Error");
            }
            return View(model);
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string email, string token)
        {
            if (email == null || token == null)
            {
                ModelState.AddModelError(string.Empty, "Reset token is invalid");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByEmailAsync(model.Email);
                var resetPass = await userManager.ResetPasswordAsync(user, model.Token, model.Password);
                if (resetPass.Succeeded)
                {
                    ViewBag.ErrorTitle = "Paasword reset success";
                    ViewBag.ErrorMessage = "Login Now";
                    return View("Error");
                }
                foreach (var error in resetPass.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
            return View(model);
        }
    }
}


