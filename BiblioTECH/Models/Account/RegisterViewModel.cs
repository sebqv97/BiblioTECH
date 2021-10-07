using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BiblioTECH.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        [DisplayName("Locație")]
        public string HomeBranch { get; set; }


        [Required]
        [DisplayName("Prenume")]
        [StringLength(20, ErrorMessage = "Prenumele poate contine doar 20 de caractere")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Nume")]
        [StringLength(20, ErrorMessage = "Numele poate contine doar 20 de caractere")]
        public string LastName { get; set; }

        [Required]
        [DisplayName("Adresa")]
        [StringLength(70, ErrorMessage = "Adresa poate contine maxim 70 de caractere")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Introduceti data dumnveavoastra de nastere")]
        [DisplayName("Data de naștere")]
        public DateTime DateOfBirth { get; set; }


        [DisplayName("Număr de telefon")]
        public string Telephone { get; set; }

        [Required]
        [DisplayName("Sex")]
        public string Gender { get; set; }

        [Required]
        [EmailAddress]
        //verificam in real-time daca mail-ul este luat
        [Remote(action: "IsEmailInUse", controller: "Account", "register")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password",
            ErrorMessage = "Parola nu coincide cu aceasta")]

        public string ConfirmPassword { get; set; }
    }
}

