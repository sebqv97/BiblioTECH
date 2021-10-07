using Microsoft.AspNetCore.Http;

namespace BiblioTECH.WebServices.Interfaces
{
    public interface IWebService
    {
        string CopyAssetPhoto(IFormFile file);
        string CopyBranchPhoto(IFormFile file);

    }
}
