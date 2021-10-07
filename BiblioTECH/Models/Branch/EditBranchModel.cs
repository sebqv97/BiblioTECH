using Microsoft.AspNetCore.Http;
using System.ComponentModel;

namespace BiblioTECH.Models.Branch
{
    public class EditBranchModel
    {
        public int BranchId { get; set; }

        [DisplayName("Nume")]
        public string Name { get; set; }

        [DisplayName("Adresa")]
        public string Address { get; set; }

        [DisplayName("Telefon")]
        public string Telephone { get; set; }

        [DisplayName("Descriere")]
        public string Description { get; set; }

        [DisplayName("Imagine")]

        public IFormFile Image { get; set; }
    }
}

