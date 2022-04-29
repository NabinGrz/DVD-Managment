using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        [Required]
        public String Id { get; set; }
        [Required(ErrorMessage = "Role Name is required")]
        public String RoleName { get; set; }
        public List<String> Users { get; set; }
    }
}
