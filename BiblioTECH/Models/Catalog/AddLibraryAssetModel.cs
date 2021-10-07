using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BiblioTECH.Models.LibraryAssetModel
{
    public class AddLibraryAssetModel
    {
        [Required]
        [DisplayName("Cateogrie")]
        public string Category { get; set; }

        [Required]
        [DisplayName("Titlu")]
        public string Title { get; set; }
        [DisplayName("An")]


        [Required]
        public string Year { get; set; } // Just store as an int for BC

        [Required]
        [DisplayName("Cost")]
        public string Cost { get; set; }

        [DisplayName("Imagine")]
        public IFormFile Image { get; set; }

        [Required]
        [DisplayName("Locatie")]
        public string Location { get; set; }

        public IEnumerable<string> Branches { get; set; }


        //books

        [DisplayName("ISBN #")]
        public string ISBN { get; set; }

        [DisplayName("Autor")]
        public string Author { get; set; }

        [DisplayName("Index Dewey")]
        public string DeweyIndex { get; set; }



        //video
        [DisplayName("Director")]
        public string Director { get; set; }
    }


}
