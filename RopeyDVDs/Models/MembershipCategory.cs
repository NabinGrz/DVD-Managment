using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class MembershipCategory
    {
        [Key]
        public int MembershipCategoryNumber { get; set; }
        [StringLength(100, MinimumLength = 10)]
        public string MembershipCategoryDescription { get; set; }
        public string MembershipCategoryTotalLoans { get; set; }
    }
}
