using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class DVDCategory
    {
        [Key]
        public int CategoryNumber { get; set; }
        [StringLength(100, MinimumLength = 10)]
        public string CategoryDescription { get; set; }
        [Required(ErrorMessage = "Restriction is required")]
        public string AgeRestricted { get; set; }
    }
}
