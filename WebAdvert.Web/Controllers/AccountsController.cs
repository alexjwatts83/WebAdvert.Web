using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Amazon.AspNetCore.Identity.Cognito;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAdvert.Web.Models;

namespace WebAdvert.Web.Controllers
{
    public class AccountsController : Controller
    {
        private readonly SignInManager<CognitoUser> _signInManager;
        private readonly UserManager<CognitoUser> _userManager;
        private readonly CognitoUserPool _userPool;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(
            SignInManager<CognitoUser> signInManager,
            UserManager<CognitoUser> userManager,
            CognitoUserPool userPool,
            ILogger<AccountsController> logger
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userPool = userPool;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Signup()
        {
            var model = new SignupModel()
            {
                Email = "test@test.com"
            };

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
                user.Attributes.Add(CognitoAttribute.Name.AttributeName, model.Password);

                // create user
                var createdtUser = await _userManager.CreateAsync(user, model.Password);

                if (createdtUser.Succeeded)
                {
                    return RedirectToAction("Confirm");
                } else
                {
                    foreach (var error in createdtUser.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Confirm()
        {
            var model = new ConfirmationModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Confirm(ConfirmationModel model)
        {
            if (!ModelState.IsValid)
            {
                // If we got this far, something failed, redisplay form
                return View();
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError("NotFound", "User with the email addres provided was not found");
                return View(model);
            }

            var confirmSignup = await (_userManager as CognitoUserManager<CognitoUser>).ConfirmSignUpAsync(user, model.Code, true);

            if (!confirmSignup.Succeeded)
            {
                foreach (var error in confirmSignup.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View(model);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}