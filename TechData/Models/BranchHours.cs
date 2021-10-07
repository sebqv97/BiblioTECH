using System.ComponentModel.DataAnnotations;

namespace TechData.Models
{
    public class BranchHours
    {
        public int Id { get; set; }

        [Required]
        public LibraryBranch Branch { get; set; }

        [Required]
        public int DayOfWeek { get; set; }

        [Required]
        public int OpenTime { get; set; }

        [Required]
        public int CloseTime { get; set; }
    }
}
