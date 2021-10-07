using System;
using System.ComponentModel.DataAnnotations;

namespace TechData.Models
{
    public class Checkout
    {
        public int Id { get; set; }

        [Required]
        public LibraryAsset LibraryAsset { get; set; }

        [Required]
        public LibraryCard LibraryCard { get; set; }

        [Required]
        public DateTime Since { get; set; }

        public DateTime Until { get; set; }
    }
}
