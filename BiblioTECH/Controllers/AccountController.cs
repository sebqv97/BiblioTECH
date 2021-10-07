using BiblioTECH.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Threading.Tasks;
using TechData.Interfaces;
using TechData.Models;

namespace BiblioTECH.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILibraryBranchService _branchServices;
        private readonly IPatronService patronService;

        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager, ILibraryBranchService libraryBranchService, IPatronService patronService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            _branchServices = libraryBranchService;
            this.patronService = patronService;
        }


        [HttpGet]
        public IActionResult Register()
        {
            ViewData["BranchesNames"] = _branchServices.GetAll()
                .Select(n => new SelectListItem
                {
                    Value = n.Id.ToString(),
                    Text = n.Name.ToString()
                }).ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                // Copy data from RegisterViewModel to IdentityUser
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Address = model.Address,
                    DateOfBirth = model.DateOfBirth,
                    Telephone = model.Telephone,
                    Gender = model.Gender,
                    HomeBranch = model.HomeBranch
                };

                // Store user data in AspNetUsers database table
                var result = await userManager.CreateAsync(user, model.Password);

                // If user is successfully created, sign-in the user using
                // SignInManager and redirect to index action of HomeController
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Add", "Patron");
                }

                // If there are any errors, add them to the ModelState object
                // which will be displayed by the validation summary tag helper
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }



        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(
                    model.Email, model.Password, model.RememberMe, false);

                if (result.Succeeded)
                {
                    //Daca login-ul se face din afara butonului de login, se va folosi ReturnUrl
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        //se va folosi redirect local
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("index", "home");
                    }

                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }

            return View(model);
        }


        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsEmailInUse(string email, string area)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (area == "register")
            {
                {
                    if (user == null)
                    {
                        return Json(true);
                    }
                    else
                    {
                        return Json($"Adresa {email} este deja folosită.");
                    }
                }
            }
            else if (area == "login")
            {
                if (user != null)
                {
                    return Json(true);
                }
                else
                {
                    return Json($"Adresa {email} nu există în baza de date.");
                }

            }
            return View();


        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        [Authorize]
        public IActionResult ViewAccount()
        {
            var userId = userManager.GetUserId(HttpContext.User);
            var user = userManager.Users
                .FirstOrDefault(us => us.Id == userId);
            var patronId = patronService.GetPatronIdFromCurrentUser(user.Email);
            return RedirectToAction("Detail", "Patron", new { id = patronId });
        }
    }
}