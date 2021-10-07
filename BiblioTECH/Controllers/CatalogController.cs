using BiblioTECH.Models;
using BiblioTECH.Models.Catalog;
using BiblioTECH.Models.CheckoutModel;
using BiblioTECH.Models.LibraryAssetModel;
using BiblioTECH.WebServices.Interfaces;
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
    [Authorize(Roles = "Admin,User")]
    public class CatalogController : Controller
    {
        private readonly ILibraryAssetService _assetsService;
        private readonly ICheckoutService _checkoutsService;
        private readonly IWebService _webServices;
        private readonly ILibraryBranchService _branchService;
        private readonly IBookService _bookService;
        private readonly IVideoService _videoService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CatalogController(UserManager<ApplicationUser> userManager, ILibraryAssetService assetsService, ICheckoutService checkoutsService, IWebService webService, ILibraryBranchService branchService, IBookService bookService, IVideoService videoService)
        {
            _assetsService = assetsService;
            _checkoutsService = checkoutsService;
            _branchService = branchService;
            _bookService = bookService;
            _videoService = videoService;

            _userManager = userManager;
            _webServices = webService;

        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            ViewData["BranchesNames"] = _branchService.GetAll()
        .Select(n => new SelectListItem
        {
            Value = n.Id.ToString(),
            Text = n.Name.ToString()
        }).ToList();
            var model = new AddLibraryAssetModel
            {
                Branches = _branchService.GetAll().Select(a => a.Name)

            };
            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult PlaceAdd(AddLibraryAssetModel model)
        {
            if (ModelState.IsValid)
            {
                //Cream obiectul

                if (model.Category == "1")
                {
                    var newAsset = new Book
                    {
                        Title = model.Title,
                        Year = int.Parse(model.Year),
                        Cost = float.Parse(model.Cost),
                        ImageUrl = _webServices.CopyAssetPhoto(model.Image),
                        ISBN = model.ISBN,
                        Author = model.Author,
                        DeweyIndex = model.DeweyIndex,
                        Location = _branchService.SetLibraryBranch(int.Parse(model.Location))
                    };
                    _assetsService.Add(newAsset);
                }
                else if (model.Category == "2")
                {
                    var newAsset = new Video
                    {
                        Title = model.Title,
                        Year = int.Parse(model.Year),
                        Cost = float.Parse(model.Cost),
                        ImageUrl = _webServices.CopyAssetPhoto(model.Image),
                        Director = model.Director,
                        Location = _branchService.SetLibraryBranch(int.Parse(model.Location))

                    };
                    _assetsService.Add(newAsset);
                }

                //Facem redirect spre Index dupa ce creem asset-ul
                return RedirectToAction("Index");
            }

            return View(model);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            //Trebuie customizat
            return View();
        }


        [Authorize(Roles = "Admin")]
        //[HttpPost] trebe implementate Delete-ul pentru a putea fi post
        public IActionResult PlaceDelete(int id)
        {
            if (id != 0)
            {
                _assetsService.Delete(id);
                return RedirectToAction("Index");
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from PlaceDelete action of CatalogController"
            });

        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var assetModels = _assetsService.GetAll();

            var listingResult = assetModels
                .Select(a => new AssetIndexListingModel
                {
                    Id = a.Id,
                    ImageUrl = a.ImageUrl,
                    AuthorOrDirector = _assetsService.GetAuthorOrDirector(a.Id),
                    Dewey = _assetsService.GetDeweyIndex(a.Id),
                    Title = _assetsService.GetTitle(a.Id),
                    Type = _assetsService.GetType(a.Id),
                    LocationName = _assetsService.GetCurrentLocation(a.Id).Name
                }).ToList();

            var model = new AssetIndexModel
            {
                Assets = listingResult
            };

            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> DetailAsync(int id)
        {

            var userId = _userManager.GetUserId(HttpContext.User);
            var user = _userManager.Users
                .FirstOrDefault(us => us.Id == userId);



            var asset = _assetsService.Get(id);

            var currentHolds = _checkoutsService.GetCurrentHolds(id).Select(a => new AssetHoldModel
            {
                HoldPlaced = _checkoutsService.GetCurrentHoldPlaced(a.Id),
                PatronName = _checkoutsService.GetCurrentHoldPatron(a.Id)
            });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assetsService.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImageUrl = asset.ImageUrl,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(id),
                CurrentLocation = _assetsService.GetCurrentLocation(id)?.Name,
                Dewey = _assetsService.GetDeweyIndex(id),
                CheckoutHistory = _checkoutsService.GetCheckoutHistory(id),
                CurrentAssociatedLibraryCard = _assetsService.GetLibraryCardByAssetId(id),
                Isbn = _assetsService.GetIsbn(id),
                LatestCheckout = _checkoutsService.GetLatestCheckout(id),
                CurrentHolds = currentHolds,
                PatronName = _checkoutsService.GetCurrentPatron(id),
            };

            if (user != null)
            {
                var userInRoleAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (userInRoleAdmin == false)

                {

                    model.CheckedOutByMe = _checkoutsService.IsCheckedOutByMe(id, user.Email);
                    model.HoldedByMe = _checkoutsService.IsHoldedByMe(id, user.Email);
                    model.UserFees = _checkoutsService.userFees(user.Email);
                }
            }

            return View(model);
        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            ViewData["BranchesNames"] = _branchService.GetAll()
        .Select(n => new SelectListItem
        {
            Value = n.Id.ToString(),
            Text = n.Name.ToString()
        }).ToList();
            var asset = _assetsService.Get(id);
            var model = new EditLibraryAssetModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assetsService.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                AuthorOrDirector = _assetsService.GetAuthorOrDirector(id),
                CurrentLocation = _assetsService.GetCurrentLocation(id),
                Dewey = _assetsService.GetDeweyIndex(id),
                Isbn = _assetsService.GetIsbn(id)
            };

            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Edit(EditLibraryAssetModel model)
        {

            if (model.Type == "Book")
            {
                var editedBook = _bookService.Get(model.AssetId);
                editedBook.Title = model.Title;
                editedBook.Year = model.Year;
                editedBook.Cost = model.Cost;
                editedBook.ImageUrl = model.Image != null ? _webServices.CopyAssetPhoto(model.Image) : editedBook.ImageUrl;
                editedBook.ISBN = model.Isbn;
                editedBook.Author = model.AuthorOrDirector;
                editedBook.DeweyIndex = model.Dewey;
                editedBook.Location = _branchService.SetLibraryBranch(int.Parse(model.CurrentLocation.Name));
                _bookService.Edit(editedBook);
            }
            else if (model.Type == "Video")
            {
                var editedVideo = _videoService.Get(model.AssetId);
                editedVideo.Title = model.Title;
                editedVideo.Year = model.Year;
                editedVideo.Cost = model.Cost;
                editedVideo.Director = model.AuthorOrDirector;
                editedVideo.ImageUrl = model.Image != null ? _webServices.CopyAssetPhoto(model.Image) : editedVideo.ImageUrl;
                _videoService.Edit(editedVideo);
            }

            //Facem redirect spre Index dupa ce creem asset-ul
            return RedirectToAction("Index");

        }




        [HttpGet]
        public IActionResult Checkout(int id)
        {
            var asset = _assetsService.Get(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                IsCheckedOut = _checkoutsService.IsCheckedOut(id)
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceCheckout(int assetId)
        {
            if (assetId != 0)
            {
                //Extragem user-ul curent
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _userManager.Users
                    .First(us => us.Id == userId);

                _checkoutsService.CheckoutItem(assetId, user.Email);
                return RedirectToAction("Detail", new { id = assetId });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from PlaceCheckout action of CatalogController"
            });

        }



        [HttpGet]
        public IActionResult Hold(int id)
        {
            var asset = _assetsService.Get(id);

            var model = new CheckoutModel
            {
                AssetId = id,
                ImageUrl = asset.ImageUrl,
                Title = asset.Title,
                LibraryCardId = "",
                HoldCount = _checkoutsService.GetCurrentHolds(id).Count()
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult PlaceHold(int assetId)
        {
            if (assetId != 0)
            {
                //Extragem user-ul curent
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _userManager.Users
                    .First(us => us.Id == userId);
                _checkoutsService.PlaceHold(assetId, user.Email);
                return RedirectToAction("Detail", new { id = assetId });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from PlaceHold action of CatalogController"
            });

        }


        [HttpPost]
        public IActionResult PlaceCancelHold(int assetId)
        {
            if (assetId != 0)
            {
                //Extragem user-ul curent
                var userId = _userManager.GetUserId(HttpContext.User);
                var user = _userManager.Users
                    .First(us => us.Id == userId);
                _checkoutsService.CancelHold(assetId, user.Email);
                return RedirectToAction("Detail", new { id = assetId });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from PlaceCancelHold action of CatalogController"
            });

        }


        [HttpGet]
        public IActionResult CheckIn(int id)
        {
            if (id != 0)
            {
                _checkoutsService.CheckInItem(id);
                return RedirectToAction("Detail", new { id });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from CheckIn action of CatalogController"
            });

        }



        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult MarkLost(int id)
        {
            if (id != 0)
            {
                _checkoutsService.MarkLost(id);
                return RedirectToAction("Detail", new { id });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from MarkLost action of CatalogController"
            });

        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult MarkFound(int id)
        {
            if (id != 0)
            {
                _checkoutsService.MarkFound(id);
                return RedirectToAction("Detail", new { id });
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from MarkFound action of CatalogController"
            });

        }




    }
}
