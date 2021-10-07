using BiblioTECH.Models.Patron;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TechData.Interfaces;
using TechData.Models;

namespace BiblioTECH.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PatronController : Controller
    {
        private readonly IPatronService _patronService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PatronController(IPatronService patronService, UserManager<ApplicationUser> userManager)
        {
            _patronService = patronService;
            _userManager = userManager;
        }


        [HttpGet]
        public IActionResult Index()
        {
            var allPatrons = _patronService.GetAll();

            var patronModels = allPatrons
                .Select(p => new PatronDetailModel
                {
                    Id = p.Id,
                    LastName = p.LastName ?? "No First Name Provided",
                    FirstName = p.FirstName ?? "No Last Name Provided",
                    LibraryCardId = p.LibraryCard?.Id,
                    OverdueFees = p.LibraryCard?.Fees,
                    HomeLibrary = p.HomeLibraryBranch?.Name,


                }).ToList();

            var model = new PatronIndexModel
            {
                Patrons = patronModels
            };

            return View(model);
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult Detail(int id)
        {
            var patron = _patronService.Get(id);

            var model = new PatronDetailModel
            {
                Id = patron.Id,
                LastName = patron.LastName ?? "No Last Name Provided",
                FirstName = patron.FirstName ?? "No First Name Provided",
                Address = patron.Address ?? "No Address Provided",
                HomeLibrary = patron.HomeLibraryBranch?.Name ?? "No Home Library",
                MemberSince = patron.LibraryCard?.Created,
                OverdueFees = patron.LibraryCard?.Fees,
                LibraryCardId = patron.LibraryCard?.Id,
                Telephone = string.IsNullOrEmpty(patron.Telephone) ? "No Telephone Number Provided" : patron.Telephone,
                AssetsCheckedOut = _patronService.GetCheckouts(id).ToList(),
                CheckoutHistory = _patronService.GetCheckoutHistory(id),
                Holds = _patronService.GetHolds(id),
                Email = patron.Email ?? "No Email Provided"
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Add()
        {
            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.Users
                .First(us => us.Id == userId);
            var newPatron = new Patron
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Address = user.Address,
                DateOfBirth = user.DateOfBirth,
                Telephone = user.Telephone,
                Gender = user.Gender,
                Email = user.Email
            };

            int branchId = int.Parse(user.HomeBranch);

            _patronService.Add(newPatron, branchId);

            return RedirectToAction("Index", "Catalog");
        }
    }
}
