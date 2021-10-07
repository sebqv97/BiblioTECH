using System.Collections.Generic;
using TechData.Models;

namespace TechData.Interfaces
{
    public interface ILibraryBranchService
    {
        IEnumerable<LibraryBranch> GetAll();
        IEnumerable<LibraryAsset> GetAssets(int branchId);

        LibraryBranch Get(int branchId);
        LibraryBranch SetLibraryBranch(int branchId);

        void Add(LibraryBranch newBranch);
        void Delete(int id);
        void Edit(LibraryBranch editedBranch);

        IEnumerable<string> GetBranchHours(int branchId);
        bool IsBranchOpen(int branchId);
        int GetAssetCount(int branchId);
        int GetPatronCount(int branchId);
        float GetAssetsValue(int id);
    }
}
