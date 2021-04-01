using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using SchoolManagement.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser()
                {
                    UserName = model.UserName,
                    Email = model.UserName,
                };

                IdentityResult createResult = await _userManager.CreateAsync(user, model.Password);

                if (createResult.Succeeded)
                {
                   _logger.LogInformation($"User with Id = {user.Id} and username {user.UserName} has been created.");
                   await _signInManager.SignInAsync(user,isPersistent:false);

                   _logger.LogInformation($"User with Id = {user.Id} and username {user.UserName} has been signed in.");
                   await _userManager.UpdateAsync(user);
                   return RedirectToAction("Students", "Home");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        _logger.LogInformation($"Failed to create user with username = {user.UserName}.");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnURL = returnUrl
            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model , string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByNameAsync(model.UserName);

                if(user == null)
                {
                    _logger.LogInformation($"Failed to sign in user with Id = {user.Id} and username = {user.UserName}.");
                    return View("NotFound");
                }

                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user,model.Password, isPersistent: false,lockoutOnFailure:false);

                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"User with Id = {user.Id} logged in.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, errorMessage: "Invalid login credentials.");
                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Index", "Home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> isEmailInUse(string UserName)
        {
            IdentityUser user = await _userManager.FindByEmailAsync(UserName);
            if(user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The provided {UserName} is already in use.");
            }
        }

    }
}
