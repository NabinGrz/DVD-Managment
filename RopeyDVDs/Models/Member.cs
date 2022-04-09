using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Member
    {
        [Key]
        public int MembershipNumber { get; set; }
        public int MembershipCategoryNumber { get; set; }
        [ForeignKey("MembershipCategoryNumber")]
        public MembershipCategory MembershipCategory { get; set; }
        public string MembershipLastName { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string MembershipFirstName { get; set; }
        [Required(ErrorMessage = "Address is required")]
        public string MembershipAddress { get; set; }
        public string MemberDOB { get; set; }
    }
}
