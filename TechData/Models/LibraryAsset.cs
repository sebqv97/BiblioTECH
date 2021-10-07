using System.ComponentModel.DataAnnotations;

namespace TechData.Models
{
    public abstract class LibraryAsset
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public int Year { get; set; } // Just store as an int for BC
        [Required]
        public Status Status { get; set; }

        [Required]
        public float Cost { get; set; }

        public string ImageUrl { get; set; }

        [Required]
        public LibraryBranch Location { get; set; }
    }
}
