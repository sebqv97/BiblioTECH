using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TechData;
using TechData.Interfaces;
using TechData.Models;

namespace TechServices
{
    public class LibraryBranchService : ILibraryBranchService
    {
        private readonly TechContext _context;
        private readonly ILibraryAssetService _libraryAsset;
        private readonly IPatronService _patronService;

        public LibraryBranchService(TechContext context, ILibraryAssetService libraryAsset, IPatronService patronService)
        {
            _context = context;
            _libraryAsset = libraryAsset;
            _patronService = patronService;
        }

        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return _context.LibraryBranches
                .Include(b => b.Patrons)
                .Include(b => b.LibraryAssets)
                .FirstOrDefault(p => p.Id == branchId);
        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            return _context.LibraryBranches.Include(a => a.Patrons).Include(a => a.LibraryAssets);
        }


        public int GetAssetCount(int branchId)
        {
            return Get(branchId).LibraryAssets.Count();
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            return _context.LibraryBranches.Include(a => a.LibraryAssets)
                .First(b => b.Id == branchId).LibraryAssets;
        }

        public float GetAssetsValue(int branchId)
        {
            var assetsValue = GetAssets(branchId).Select(a => a.Cost);
            return assetsValue.Sum();
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            var hours = _context.BranchHours.Where(a => a.Branch.Id == branchId);

            var displayHours =
                DataHelpers.HumanizeBusinessHours(hours);

            return displayHours;
        }

        public int GetPatronCount(int branchId)
        {
            return Get(branchId).Patrons.Count();
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            return _context.LibraryBranches.Include(a => a.Patrons).First(b => b.Id == branchId).Patrons;
        }

        //TODO: Implement 
        public bool IsBranchOpen(int branchId)
        {
            return true;
        }

        public void Delete(int id)
        {
            //Localizam branch-ul
            var branchToDelete = _context.LibraryBranches
                .FirstOrDefault(br => br.Id == id);

            //Stergem orarul
            var branchTimeTable = _context.BranchHours
                .Where(br => br.Branch == branchToDelete);
            if (branchTimeTable.Any())
            {
                foreach (BranchHours day in branchTimeTable)
                    _context.Remove(day);
            }



            //Localizam toate asset-urile care sunt in branch-ul respectiv

            var assets = _libraryAsset.GetAll()
                .Where(ass => ass.Location == branchToDelete);



            //Stergem asset-urile

            if (assets.Any())
            {

                foreach (LibraryAsset asset in assets)
                {
                    //if (asset.Location == branchToDelete)
                    // {
                    _context.Remove(asset);
                    //}
                }
            }


            //Localizam toti patronii care sunt in branch-ul respectiv
            var patrons = _patronService.GetAll()
                .Where(pa => pa.HomeLibraryBranch == branchToDelete);


            //Stergem toti patronii
            if (patrons.Any())
            {
                foreach (Patron patron in patrons)
                {
                    _context.Remove(patron);
                }
            }


            //Stergem branch-ul
            _context.Remove(branchToDelete);

            //Salvam
            _context.SaveChanges();
        }

        public LibraryBranch SetLibraryBranch(int branchId)
        {
            return _context.LibraryBranches
                .FirstOrDefault(br => br.Id == branchId);
        }

        public void Edit(LibraryBranch editedBranch)
        {
            //Salvam branch-ul fara edit
            var uneditedBranch = Get(editedBranch.Id);

            //Schimbam de la toti patronii numele branch-ului
            var patrons = _patronService.GetAll()
                .Where(ho => ho.HomeLibraryBranch == uneditedBranch);
            if (patrons.Any())
            {
                foreach (Patron patron in patrons)
                {
                    patron.HomeLibraryBranch = editedBranch;
                }
            }
            //Schimbam de la toate asset-urile numele branch-ului
            var assets = _libraryAsset.GetAll()
                .Where(loc => loc.Location == uneditedBranch);
            if (assets.Any())
            {
                foreach (LibraryAsset asset in assets)
                {
                    asset.Location = editedBranch;
                }
            }

            //Semnalam ca branch-ul va fi schimbat
            _context.Entry(editedBranch).State = EntityState.Modified;
            _context.SaveChanges();

        }


    }
}
