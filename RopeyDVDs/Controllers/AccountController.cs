#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Models;
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

        public AccountController(UserManager<IdentityUser> userManager,
                                      SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        //FOR REGISTER page vierw
        public IActionResult Register()
        {
            return View();
        }
        //FOR REGISTER
        //
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel registermodel)
        {
            //check if incoming model object is valid
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = registermodel.Username,
                };

                var result = await _userManager.CreateAsync(user, registermodel.Password);

                if (result.Succeeded)
                {//specify session or Persistent cookie
                 //session coolkie is immediatlry loss after closong broswer
                 // Persistent coolkie is not loss even after closong broswer
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Privacy", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");

            }
            return View(registermodel);
        }


        //FOR LOGIN PAGE VIEW
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        //FOR LOGIN
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginmodel)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginmodel.Username, loginmodel.Password, loginmodel.RememberMe, false);//if lock account on failure
                //returnns sign in result
                if (result.Succeeded)
                {
                    return RedirectToAction("Privacy", "Home");

                }
                //switch (result)
                //{
                //    case SignInStatus.Success:
                //        return RedirectToAction(nameof(Login), "Home");
                //}
                ModelState.AddModelError(string.Empty, "Invalid Username/Password");

            }
            return View(loginmodel);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}
