using BiblioTECH.Models;
using BiblioTECH.Models.Branch;
using BiblioTECH.WebServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TechData.Interfaces;
using TechData.Models;

namespace BiblioTECH.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BranchController : Controller
    {
        private readonly ILibraryBranchService _branchService;
        private readonly IWebService _webService;

        public BranchController(ILibraryBranchService branchService, IWebService webService)
        {
            _branchService = branchService;
            _webService = webService;
        }



        [HttpGet]
        public IActionResult Add()
        {
            //Trebuie customizat
            return View();
        }

        [HttpPost]
        public IActionResult PlaceAdd(AddBranchModel model)
        {
            if (ModelState.IsValid)
            {
                var branch = new LibraryBranch
                {
                    Name = model.Name,
                    Address = model.Address,
                    Telephone = model.Telephone,
                    Description = model.Description,
                    ImageUrl = _webService.CopyBranchPhoto(model.Image),
                    OpenDate = DateTime.Now
                };
                _branchService.Add(branch);
                return RedirectToAction("Index");
            }

            return View(model);

        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var branch = _branchService.Get(id);
            var model = new EditBranchModel
            {
                BranchId = id,
                Name = branch.Name,
                Address = branch.Address,
                Telephone = branch.Telephone,
                Description = branch.Description
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult PlaceEdit(EditBranchModel model)
        {
            if (ModelState.IsValid)
            {
                var editedBranch = _branchService.Get(model.BranchId);
                editedBranch.Name = model.Name;
                editedBranch.Address = model.Address;
                editedBranch.Telephone = model.Telephone;
                editedBranch.Description = model.Description;
                editedBranch.ImageUrl = model.Image == null ? editedBranch.ImageUrl : _webService.CopyBranchPhoto(model.Image);

                _branchService.Edit(editedBranch);

                return RedirectToAction("index");
            }
            return View(model);

        }





        [HttpPost]
        public IActionResult PlaceDelete(int id)
        {
            if (id != 0)
            {
                _branchService.Delete(id);
                return RedirectToAction("Index");
            }
            return View(new ErrorViewModel
            {
                RequestId = "There is a problem with id from PlaceDelete action of BranchController"
            });
        }




        [HttpGet]
        [AllowAnonymous]
        public IActionResult Index()
        {
            var branchModels = _branchService.GetAll()
                .Select(br => new BranchDetailModel
                {
                    Id = br.Id,
                    BranchName = br.Name,
                    NumberOfAssets = _branchService.GetAssetCount(br.Id),
                    NumberOfPatrons = _branchService.GetPatronCount(br.Id),
                    IsOpen = _branchService.IsBranchOpen(br.Id),
                    Description = br.Description,
                    ImageUrl = br.ImageUrl
                }).ToList();

            var model = new BranchIndexModel
            {
                Branches = branchModels
            };

            return View(model);
        }



        [HttpGet]
        [AllowAnonymous]
        public IActionResult Detail(int id)
        {
            //IEnumerable<string>TimeTable = new List<string>() {}
            var branch = _branchService.Get(id);
            var model = new BranchDetailModel
            {
                Id = id,
                BranchName = branch.Name,
                Description = branch.Description,
                Address = branch.Address,
                Telephone = branch.Telephone,
                BranchOpenedDate = branch.OpenDate.ToString("yyyy-MM-dd"),
                NumberOfPatrons = _branchService.GetPatronCount(id),
                NumberOfAssets = _branchService.GetAssetCount(id),
                TotalAssetValue = _branchService.GetAssetsValue(id),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branchService.GetBranchHours(id)
            };

            return View(model);
        }
    }
}
