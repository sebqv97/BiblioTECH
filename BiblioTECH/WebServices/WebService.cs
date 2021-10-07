using BiblioTECH.WebServices.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace BiblioTECH.WebServices
{
    public class WebService : IWebService
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public WebService(IWebHostEnvironment hostEnviroment)
        {
            _hostingEnvironment = hostEnviroment;
        }
        public string CopyAssetPhoto(IFormFile file)
        {
            string uniqueFileName = null;
            string imageUrl = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/Assets");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
                imageUrl = Path.Combine("/images/Assets/", uniqueFileName);
            }
            else
            {
                imageUrl = "/images/Assets/NoAssetCover.jpg";
            }
            return imageUrl;
        }

        public string CopyBranchPhoto(IFormFile file)
        {
            string uniqueFileName = null;
            string imageUrl = null;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/Branches");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                file.CopyTo(new FileStream(filePath, FileMode.Create));
                imageUrl = Path.Combine("/images/Branches/", uniqueFileName);
            }
            else
            {
                imageUrl = "/images/Branches/NoBranchCover.jpg";
            }
            return imageUrl;

        }
    }
}
