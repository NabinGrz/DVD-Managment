#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Models;
using RopeyDVDs.Services;
using RopeyDVDs.ViewModels;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace RopeyDVDs.Controllers
{
    public class AccountController : Controller
    {
        //injecting usermanager and signinmanager
        private readonly UserManager<IdentityUser> _userManager; //for crud
        private readonly SignInManager<IdentityUser> _signInManager; //for signing functions(signinasync,signoutasync,issgindedin)
        private readonly RopeyDVDsContext _context;

        //initializing managers
        public AccountController(UserManager<IdentityUser> userManager,
                                      SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        //FOR REGISTER page vierw
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        //FOR REGISTER
        //
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registermodel)
        {
            //check if incoming model object is valid
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = registermodel.Email,
                    Email = registermodel.Email,
                };
                //creating new user 
                var result = await _userManager.CreateAsync(user, registermodel.Password);

                if (result.Succeeded)
                {//specify session or Persistent cookie
                 //session coolkie is immediatlry loss after closong broswer
                 // Persistent coolkie is not loss even after closong broswer

                    //after successful registration, logging the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }
                //for error message
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                //showing error message when model is invalid
                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(registermodel);
        }


        //FOR LOGIN PAGE VIEW
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        //FOR LOGIN
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel loginmodel)
        {
            if (ModelState.IsValid)
            {
                //sign in users
                var result = await _signInManager.PasswordSignInAsync(loginmodel.Email, loginmodel.Password, !loginmodel.RememberMe, false);//if lock account on failure
                //returnns sign in result
                if (result.Succeeded)
                {
                    //redirecting to allfunction page view
                    return RedirectToAction("AllFunctions", "Assistant");

                }
                ModelState.AddModelError(string.Empty, "Invalid Username/Password");

            }
            return View(loginmodel);
        }

        public async Task<IActionResult> Logout()
        {
            //for log out user
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
        //Account/ResetPassword
        [HttpGet]
        [Authorize(Roles ="Assistant,Manager")]
        public  IActionResult ResetPassword()
        {
            //for reset password view
            return View();
        }


        //Account/ResetPassword
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                //getting user details for reseeting password
                var user = await _userManager.GetUserAsync(User); //gets current logged in user records
                if(user == null)
                {
                    return RedirectToAction("Login");
                }

                //changing user password
                var result = await _userManager.ChangePasswordAsync(user,model.CurrentPassword,model.NewPassword);
                if(!result.Succeeded)
                {
                    foreach (var error in result.Errors)

                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }

                await _signInManager.RefreshSignInAsync(user);
                return View("ResetPasswordConfirmation");
            }
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        //for access denied view
        public IActionResult AccessDenied()
        {

            return View();
        }


    }
}
