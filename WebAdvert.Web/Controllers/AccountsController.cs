using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _userPool;

        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool userPool
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userPool = userPool;
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Signup(SignupModel model)
        {
            if (ModelState.IsValid)
            {
                // Retrieves a new user with the pool configuration set up
                CognitoUser user = _userPool.GetUser(model.Email);
                if (user.Status != null)
                {
                    var msg = "User with this email already exists";
                    ModelState.AddModelError("UserExists", msg);
                    return View(model);
                }

                // add attribute name to user as thats we I specified in Cognito
                user.Attributes.Add(CognitoAttribute.Name.ToString(), model.Pasword);
                // create user
                var createdtUser = await _userManager.CreateAsync(user, model.Pasword);

                if (createdtUser.Succeeded)
                {
                    RedirectToAction("Confirm");
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }
    }
}