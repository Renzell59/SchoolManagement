using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Web.Mvc;
using SchoolManagement.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using SchoolManagement.Utilities;

namespace SchoolManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly NotificationMetadata _notificationMetadata;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger,
            NotificationMetadata notificationMetadata)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._logger = logger;
            this._notificationMetadata = notificationMetadata;
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

                string emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                emailToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(emailToken));
                string confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = emailToken }, Request.Scheme);

                if (createResult.Succeeded)
                {
                    if (_signInManager.IsSignedIn(User))
                    {
                        return RedirectToAction("Users", "Administration");
                    }
                    _logger.LogInformation($"User with Id = {user.Id} and username {user.UserName} has been created.");
                    _logger.LogInformation($"Confirmation Link:{confirmationLink}");
                    return LocalRedirect("/Account/Login");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        _logger.LogInformation($"Failed to create user with username = {user.UserName}.");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel model = new LoginViewModel()
            {
                ReturnURL = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()

            };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid)
            {
                IdentityUser user = await _userManager.FindByEmailAsync(model.UserName);

                if (user == null)
                {
                    //_logger.LogInformation($"Failed to sign in user with Id = {user.Id} and username = {user.UserName}.");
                    return View("Register");
                }

                Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: model.RememberMe, lockoutOnFailure: false);


                if (signInResult.Succeeded)
                {
                    _logger.LogInformation($"User with Id = {user.Id} logged in.");
                    return LocalRedirect(returnUrl);
                }
                else
                {
                    if (user != null && !user.EmailConfirmed)
                    {
                        string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                        string confirmationEmailLink = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, token = token }, protocol:Request.Scheme);

                        ModelState.AddModelError(string.Empty, "Please confirm your email address");
                        _logger.LogInformation($"Confirmation Email Link:\r\n {confirmationEmailLink}");
                        return View(model);
                    }
                    ModelState.AddModelError(string.Empty, errorMessage: "Invalid login credentials.");
                    return View(model);
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
            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"The provided {UserName} is already in use.");
            }
        }

        /// <summary>
        /// Handles the logic to 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string returnUrl, string provider)
        {
                string redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
                AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
                return Challenge(properties, provider);
        }

        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            LoginViewModel loginViewModel = new LoginViewModel()
            {
                ReturnURL = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };

            ExternalLoginInfo externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

            if (externalLoginInfo == null)
            {
                ModelState.AddModelError(string.Empty, $"{externalLoginInfo.LoginProvider} cannot be found");
                return View("Login", loginViewModel);
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false, bypassTwoFactor:true);
            string email = externalLoginInfo.Principal.FindFirst(ClaimTypes.Email).Value;
            IdentityUser identityUser = null;

            identityUser = await _userManager.FindByEmailAsync(email);
            if(identityUser != null)
            {
                if (email != null && !(await _userManager.IsEmailConfirmedAsync(identityUser)))
                {
                    return await sendEmailConfirmationLink(loginViewModel, identityUser);
                }
            }

            if (signInResult.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                UserLoginInfo userLoginInfo = new UserLoginInfo(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, externalLoginInfo.ProviderDisplayName);

                if (email != null)
                {
                    if(identityUser == null)
                    {
                        identityUser = new IdentityUser()
                        {
                            UserName=email,
                            Email=email
                        };
                        await _userManager.CreateAsync(identityUser);
                        await _userManager.AddLoginAsync(identityUser, userLoginInfo);
                        if(!(await _userManager.IsEmailConfirmedAsync(identityUser)))
                        {
                            return await sendEmailConfirmationLink(loginViewModel, identityUser);
                        }
                    }
                    IdentityResult addLoginResult = await _userManager.AddLoginAsync(identityUser, userLoginInfo);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(identityUser,isPersistent:false);
                        return LocalRedirect(returnUrl);
                    }
                }

                ModelState.AddModelError(string.Empty, $"Sign in with {externalLoginInfo.LoginProvider} failed, please try again.");
                return View("Login", loginViewModel);
            }
        }

        /// <summary>
        /// Generates user email confirmation token and sends the link to the logger
        /// </summary>
        /// <param name="loginViewModel"></param>
        /// <param name="identityUser"></param>
        /// <returns></returns>
        private async Task<IActionResult> sendEmailConfirmationLink(LoginViewModel loginViewModel, IdentityUser identityUser)
        {
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string confirmationEmailLink = Url.Action("ConfirmEmail", "Account", new { userId = identityUser.Id, token = token }, Request.Scheme);

            _logger.LogWarning($"Confirmation Email Link: \r\n {confirmationEmailLink}");
            ModelState.AddModelError(string.Empty, "Please check your inbox to confirm your email.");
            return View("Login", loginViewModel);
        }

        public MimeMessage CreateMimeMessageFromEmailMessage(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(message.Sender);
            mimeMessage.To.Add(message.Receiver);
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = message.Content
            };
            return mimeMessage;
        }

        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
            IdentityResult confirmEmailResult = await _userManager.ConfirmEmailAsync(identityUser, Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token)));

            if (confirmEmailResult.Succeeded)
            {
                return View();
            }
            ModelState.AddModelError(string.Empty, "Email confirmation error.");
            return View();
        }

        [HttpGet]
        public IActionResult CreatePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePassword(CreatePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser loggedInUser = await _userManager.GetUserAsync(User);
                if(await _userManager.HasPasswordAsync(loggedInUser))
                {
                    return RedirectToAction("ResetPassword", "Account");
                }

                IdentityResult addPasswordResult = await _userManager.AddPasswordAsync(loggedInUser, model.Password);
                if (!addPasswordResult.Succeeded)
                {
                    foreach (var error in addPasswordResult.Errors)
                    {

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                return RedirectToAction("EditUser", "Administration", new { userId = loggedInUser.Id });
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser identityUser = await _userManager.FindByEmailAsync(model.Email);
                if (identityUser == null)
                {
                    ModelState.AddModelError(string.Empty, "Email does not exist.");
                    return RedirectToAction("Register", "Account");
                }
                else if(identityUser!= null && !(await _userManager.HasPasswordAsync(identityUser)))
                {
                    return RedirectToAction("CreatePassword", "Account");
                }

                string tempPassword = CreateTemporaryPassword(15);
                ViewBag.TempPassword = tempPassword;
                string token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);

                IdentityResult resetPasswordResult = await _userManager.ResetPasswordAsync(identityUser, token, tempPassword);
                if (resetPasswordResult.Succeeded)
                {
                    _logger.LogWarning($"Your temporary password is : {tempPassword}");
                    return View("ResetPasswordMessage");
                }
                foreach (var error in resetPasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            return View(model);
        }

        public string CreateTemporaryPassword(int length)
        {
            const string passwordCharacter = "!@#$%abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMOPQRSTUVWXYZ1234567890";

            StringBuilder s = new StringBuilder(length);
            Random r = new Random();

            while(0 < length--)
            {
                s.Append(passwordCharacter[r.Next(passwordCharacter.Length)]);
            }

            return s.ToString();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordMessage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser loggedInUser = await _userManager.GetUserAsync(User);
                IdentityResult changePasswordResult = await _userManager.ChangePasswordAsync(loggedInUser, model.CurrentPassword, model.NewPassword);

                if (changePasswordResult.Succeeded)
                {
                    return View("ChangePasswordMessage");
                }
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }
            return View(model);
        }
    }
}
