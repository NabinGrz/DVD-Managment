using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}
