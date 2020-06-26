using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityAuth.Entities;
using IdentityAuth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityAuth.Controllers
{   
    public class AuthController : Controller
    {
        private readonly UserManager<AppIdentityUser> _userManager;
        private readonly SignInManager<AppIdentityUser> _signInManager;

        public AuthController(UserManager<AppIdentityUser> userManager,
            SignInManager<AppIdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterUser registerUser)
        {
            if(ModelState.IsValid)
            {
                var user = new AppIdentityUser()
                {
                    FirstName = registerUser.FirstName,
                    LastName = registerUser.LastName,
                    UserName = registerUser.UserName,
                    Email = registerUser.Email
                };

                var userExistResponse = await _userManager.FindByEmailAsync(user.Email);

                if (userExistResponse != null)
                {
                    ModelState.AddModelError("", $"User already exists with given email {user.Email}");
                }
                else
                {
                    var response = await _userManager.CreateAsync(user, registerUser.Password);

                    if (response.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction("Index", "Home");
                    }

                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    } 
                }
            }

            return View(registerUser);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginUser loginUser, [FromQuery] string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                AppIdentityUser user = await _userManager.FindByEmailAsync(loginUser.Email);

                if (user is null)
                {
                    return View(loginUser);
                }

                var response = await _signInManager.PasswordSignInAsync(user.UserName, loginUser.Password, loginUser.RememberMe, false);

                if (response.Succeeded)
                {
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }
                }
            }

            return View(loginUser);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("index", "home");
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<JsonResult> IsEmailAlreadyInUse(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email is already in use, try with other email or go to login page");
            }
        }
    }
}
