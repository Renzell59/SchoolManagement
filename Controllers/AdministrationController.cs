using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.ViewModels.Administration;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly string editUsersView = "/Views/Administration/Users/EditUser.cshtml";
        private readonly string createUsersView = "/Views/Administration/Users/CreateUser.cshtml";

        public AdministrationController(RoleManager<IdentityRole> roleManager,
            UserManager<IdentityUser> userManager)
        {
            this._roleManager = roleManager;
            this._userManager = userManager;
        }
        public IActionResult Roles()
        {
            IEnumerable<IdentityRole> roles=  _roleManager.Roles;

            return View(rolesView,roles);
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
                    return RedirectToAction("Roles","Administration");
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

        [HttpGet]
        public async Task<IActionResult> EditRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);
            EditRoleViewModel model = new EditRoleViewModel()
            {
                RoleName = role.Name,
                RoleID=roleId
            };
            return View(editRoleView,model);
        }

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
                    return RedirectToAction("Roles","Administration");
                }
                else
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(editRoleView,model);
                }
            }
            return View(editRoleView, model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            IdentityRole role = await _roleManager.FindByIdAsync(roleId);

            IdentityResult deleteResult =  await _roleManager.DeleteAsync(role);

            if (deleteResult.Succeeded)
            {
                return RedirectToAction("Roles","Administration");
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

        [HttpGet]
        public IActionResult Users()
        {
            IEnumerable<IdentityUser> identityUsers = _userManager.Users;
            return View(usersView,identityUsers);
        }
    
        
    }
}
