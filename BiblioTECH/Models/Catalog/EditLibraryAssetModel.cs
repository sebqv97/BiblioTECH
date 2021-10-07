using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using TechData.Models;

namespace BiblioTECH.Models.Catalog
{
    public class EditLibraryAssetModel
    {
        [DisplayName("Imagine")]
        public int AssetId { get; set; }

        [DisplayName("Titlu")]
        public string Title { get; set; }

        public string Type { get; set; }

        [DisplayName("An")]
        public int Year { get; set; }

        [DisplayName("Cost")]
        public float Cost { get; set; }
        public string Status { get; set; }

        [DisplayName("Imagine")]
        public IFormFile Image { get; set; }

        [DisplayName("Autor")]

        public string AuthorOrDirector { get; set; }

        public LibraryBranch CurrentLocation { get; set; }

        [DisplayName("Numar unic international")]
        public string Isbn { get; set; }

        [DisplayName("Index Dewey")]
        public string Dewey { get; set; }




    }
}
