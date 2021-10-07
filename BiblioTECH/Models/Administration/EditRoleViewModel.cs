using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BiblioTECH.Models.Administration
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }

        public string Id { get; set; }

        [Required(ErrorMessage = "Numele rolului este OBLICATORIU")]
        public string RoleName { get; set; }

        public List<string> Users { get; set; }
    }
}
