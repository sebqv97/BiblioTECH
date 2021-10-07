using System.Collections.Generic;
using TechData.Models;

namespace TechData.Interfaces
{
    public interface ILibraryAssetService
    {
        IEnumerable<LibraryAsset> GetAll();


        LibraryAsset Get(int id);
        LibraryBranch GetCurrentLocation(int id);
        LibraryCard GetLibraryCardByAssetId(int id);


        void Add(LibraryAsset newAsset);
        void Delete(int id);
        string GetAuthorOrDirector(int id);
        string GetDeweyIndex(int id);
        string GetType(int id);
        string GetTitle(int id);
        string GetIsbn(int id);

    }
}
