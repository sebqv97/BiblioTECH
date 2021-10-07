using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BiblioTECH.Models.Branch
{
    public class AddBranchModel
    {
        [Required]
        [StringLength(40, ErrorMessage = "Numele poate contine doar 40 de caractere")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Numele poate contine doar 100 de caractere")]
        public string Address { get; set; }

        [Required]
        [StringLength(15, ErrorMessage = "Numele poate contine doar 15 de caractere")]
        public string Telephone { get; set; }

        [Required]
        [StringLength(130, ErrorMessage = "Numele poate contine doar 130 de caractere")]
        public string Description { get; set; }

        [DisplayName("Imagine")]
        public IFormFile Image { get; set; }
    }
}
