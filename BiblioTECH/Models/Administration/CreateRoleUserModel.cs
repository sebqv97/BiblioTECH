using System.ComponentModel.DataAnnotations;

namespace BiblioTECH.Models.Administration
{
    public class CreateRoleUserModel
    {

        [Required]
        [Display(Name = "Nume rol")]
        public string RoleName { get; set; }
    }
}
