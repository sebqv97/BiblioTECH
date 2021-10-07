using System.ComponentModel.DataAnnotations;

namespace TechData.Models
{
    public class Video : LibraryAsset
    {
        [Required]
        public string Director { get; set; }
    }
}
