using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Models;
using SchoolManagement.ViewModels.Administration;
using SchoolManagement.ViewModels.Administration.Roles;
using SchoolManagement.ViewModels.Administration.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SchoolManagement.Controllers
{
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly string rolesView = "/Views/Administration/Roles/Roles.cshtml";
        private readonly string editRoleView = "/Views/Administration/Roles/EditRole.cshtml";
        private readonly string createRoleView = "/Views/Administration/Roles/CreateRole.cshtml";
        private readonly string usersView = "/Views/Administration/Users/Users.cshtml";

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public IActionResult Roles()
        {
            IEnumerable<IdentityRole> roles = _roleManager.Roles;

            return View(rolesView, roles);
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View(createRoleView);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = new IdentityRole()
                {
                    Name = model.RoleName
                };

                IdentityResult createResult = await _roleManager.CreateAsync(role);

                if (createResult.Succeeded)
                {
                    return RedirectToAction("Roles", "Administration");
                }
                else
                {
                    foreach (var error in createResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(createRoleView, model);
        }

        [Authorize(Policy ="EditRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            IEnumerable<IdentityUser> usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);

            EditRoleViewModel model = new EditRoleViewModel()
            {
                RoleName = role.Name,
                RoleID = roleId,
                UserNames = usersInRole.Select(u => u.UserName).ToList()

            };
            return View(editRoleView, model);
        }

        [Authorize(Policy = "EditRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> EditRole(string roleId, EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole role = await _roleManager.FindByIdAsync(roleId);
                role.Name = model.RoleName;

                IdentityResult updateResult = await _roleManager.UpdateAsync(role);

                if (updateResult.Succeeded)
                {
                    return RedirectToAction("Roles", "Administration");
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(editRoleView, model);
                }
            }
            return View(editRoleView, model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            IdentityResult deleteResult = await _roleManager.DeleteAsync(role);

            if (deleteResult.Succeeded)
            {
                return RedirectToAction("Roles", "Administration");
            }
            else
            {
                foreach (var error in deleteResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(rolesView);
            }
        }

        [Authorize(Policy = "EditRolePolicy")]
        [HttpGet]
        public async Task<IActionResult> AddUsersInRole(string roleId)
        {
            ViewBag.RoleId = roleId;
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            IEnumerable<IdentityUser> identityUsers = _userManager.Users;
            List<AddUsersInRoleViewModel> model = new List<AddUsersInRoleViewModel>();

                foreach (var identityUser in identityUsers)
                {
                    AddUsersInRoleViewModel addUsersInRoleViewModel = new AddUsersInRoleViewModel();
                    addUsersInRoleViewModel.UserName = identityUser.UserName;
                    addUsersInRoleViewModel.UserID = identityUser.Id;

                    if (await _userManager.IsInRoleAsync(identityUser, role.Name))
                    {
                        addUsersInRoleViewModel.IsSelected = true;
                    }
                    else if (!(await _userManager.IsInRoleAsync(identityUser, role.Name)))
                    {
                        addUsersInRoleViewModel.IsSelected = false;
                    }
                    else
                    {
                        continue;
                    }
                    model.Add(addUsersInRoleViewModel);
                }
            return View("/Views/Administration/Roles/AddUsersInRole.cshtml", model);
        }

        [Authorize(Policy = "EditRolePolicy")]
        [HttpPost]
        public async Task<IActionResult> AddUsersInRole(string roleId, List<AddUsersInRoleViewModel> model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult addAndRemoveToRoleResult = null;

                IdentityRole role = await _roleManager.FindByIdAsync(roleId);


                for (int i = 0; i < model.Count; i++)
                {
                    IdentityUser user = await _userManager.FindByIdAsync(model[i].UserID);

                    if(model[i].IsSelected && !(await _userManager.IsInRoleAsync(user,role.Name)))
                    {
                        addAndRemoveToRoleResult = await _userManager.AddToRoleAsync(user, role.Name);
                    }
                    else if(!(model[i].IsSelected) && await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        addAndRemoveToRoleResult = await _userManager.RemoveFromRoleAsync(user, role.Name);
                    }
                    else
                    {
                        continue;
                    }
                    if (addAndRemoveToRoleResult.Succeeded)
                    {
                        continue;
                    }
                    else
                    {
                        foreach (var error in (await _roleManager.UpdateAsync(role)).Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                return RedirectToAction("EditRole", new { roleId });
            }
            return View("/Views/Administration/Roles/AddUsersInRole.cshtml", model);
        }

        [HttpGet]
        public IActionResult Users()
        {
            IEnumerable<IdentityUser> identityUsers = _userManager.Users;
            return View(usersView,identityUsers);
        }
    
        
        [HttpGet]
        public async Task<IActionResult> EditUser(string userId)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
            IList<string> rolesInUser =  await _userManager.GetRolesAsync(identityUser);
            IList<Claim> claimsInUser = await _userManager.GetClaimsAsync(identityUser);

            EditUserViewModel model = new EditUserViewModel()
            {
                UserID = userId,
                UserName = identityUser.UserName,
                Email = identityUser.Email,
                Roles = rolesInUser,
                Claims = claimsInUser.Select(c => $"{c.Type} : {c.Value}").ToList()
            };
            return View("/Views/Administration/Users/EditUser.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(string userId, EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
                identityUser.Email = model.Email;
                identityUser.UserName = model.UserName;

                IdentityResult identityResult = await _userManager.UpdateAsync(identityUser);
                if (identityResult.Succeeded)
                {
                    return RedirectToAction("EditUser", "Administration", new { userId });
                }
                else
                {
                    foreach (var error in identityResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View("/Views/Administration/Users/EditUser.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> EditRolesInUser(string userId)
        {
            ViewBag.userId = userId;
            IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
            List<EditRolesInUserViewModel> model = new List<EditRolesInUserViewModel>();

            foreach (var role in _roleManager.Roles)
            {
                EditRolesInUserViewModel editRolesInUserViewModel = new EditRolesInUserViewModel();
                editRolesInUserViewModel.RoleID = role.Id;
                editRolesInUserViewModel.RoleName = role.Name;

                if(await _userManager.IsInRoleAsync(identityUser, role.Name))
                {
                    editRolesInUserViewModel.IsSelected = true;
                }
                else
                {
                    editRolesInUserViewModel.IsSelected = false;
                }
                model.Add(editRolesInUserViewModel);
            }
            return View("/Views/Administration/Users/EditRolesInUser.cshtml",model);
        }

        [HttpPost]
        public async Task<IActionResult> EditRolesInUser(string userId, List<EditRolesInUserViewModel> model)
        {
            if (ModelState.IsValid)
            {
                //IdentityResult identityResult = null;
                IdentityUser identityUser = await _userManager.FindByIdAsync(userId);
                IList<string> userRoles =  await _userManager.GetRolesAsync(identityUser);
                IdentityResult removeFromRolesResult = await _userManager.RemoveFromRolesAsync(identityUser, userRoles);

                if (!removeFromRolesResult.Succeeded)
                {
                    foreach (IdentityError error in removeFromRolesResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("/Views/Administration/Users/EditRolesInUser.cshtml", model);
                }

                IdentityResult addToRolesResult = await _userManager.AddToRolesAsync(identityUser, model.Where(s => s.IsSelected).Select(r => r.RoleName));

                if (!addToRolesResult.Succeeded)
                {
                    foreach (IdentityError error in addToRolesResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("/Views/Administration/Users/EditRolesInUser.cshtml", model);
                }
                return RedirectToAction("EditUser", "Administration", new { userId });
            }
            return View("/Views/Administration/Users/EditRolesInUser.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            IdentityUser identityUser = await _userManager.FindByIdAsync(userId);

            IdentityResult deleteResult = await _userManager.DeleteAsync(identityUser);
            if (deleteResult.Succeeded)
            {
                return RedirectToAction("Users", "Administration");
            }
            else
            {
                foreach (var error in deleteResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("/Views/Administration/Users/Users.cshtml");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditClaimsInUser(string userId)
        {
            IdentityUser identityUser= await _userManager.FindByIdAsync(userId);
            
            IList<Claim> existingUserClaim = await _userManager.GetClaimsAsync(identityUser);
            EditClaimsInUserViewModel model = new EditClaimsInUserViewModel()
            {
                UserID = userId
            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim();
                userClaim.ClaimType = claim.Type;

                if (existingUserClaim.Any(c=> c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                else
                {
                    userClaim.IsSelected = false;
                }
                model.UserClaims.Add(userClaim);
            }
            return View("/Views/Administration/Users/EditClaimsInUser.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> EditClaimsInUser(EditClaimsInUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser identityUser = await _userManager.FindByIdAsync(model.UserID);

                IList<Claim> userClaims = await _userManager.GetClaimsAsync(identityUser);

                IdentityResult removeClaimsResult = await _userManager.RemoveClaimsAsync(identityUser, userClaims);

                if (!removeClaimsResult.Succeeded)
                {
                    foreach (IdentityError error in removeClaimsResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("/Views/Administration/Users/EditClaimsInUser.cshtml", model);
                }

                IdentityResult updateClaimsResult = await _userManager.AddClaimsAsync(identityUser,
                    model.UserClaims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

                if (!updateClaimsResult.Succeeded)
                {
                    foreach (var error in updateClaimsResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                        return View("/Views/Administration/Users/EditClaimsInUser.cshtml", model);
                    }
                }
                return RedirectToAction("EditUser", "Administration", new { userId = model.UserID });
            }
            return View("/Views/Administration/Users/EditClaimsInUser.cshtml", model);
        }
    }
}
