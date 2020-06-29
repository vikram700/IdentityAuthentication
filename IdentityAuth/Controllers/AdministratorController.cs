using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAuth.Entities;
using IdentityAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace IdentityAuth.Controllers
{
    public class AdministratorController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppIdentityUser> userManager;

        public AdministratorController(RoleManager<IdentityRole> roleManager,
            UserManager<AppIdentityUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoleAsync(CreateRoleModel roleModel)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole()
                {
                    Name = roleModel.RoleName
                };

                var response = await roleManager.CreateAsync(identityRole);

                if (response.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administrator");
                }

                foreach (var error in response.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(roleModel);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = roleManager.Roles;
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRoleAsync(string id)
        {
            var role = await roleManager.FindByIdAsync(id);

            if (role is null)
            {
                return View("NotFound");
            }

            var model = new EditRoleModel()
            {
                Id = id,
                RoleName = role.Name,
            };

            foreach (var user in userManager.Users)
            {
                bool isInRole = await userManager.IsInRoleAsync(user, role.Name);

                if (isInRole)
                {
                    model.Users.Add(user.UserName);
                }
            }

            return View(model);
        } 
    }
}
