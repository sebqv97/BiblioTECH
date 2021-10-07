﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TechData.Models
{
    public class Patron
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string Telephone { get; set; }
        [Required]
        public string Gender { get; set; }

        [Required]
        public LibraryCard LibraryCard { get; set; }
        [Required]
        public LibraryBranch HomeLibraryBranch { get; set; }
    }
}
