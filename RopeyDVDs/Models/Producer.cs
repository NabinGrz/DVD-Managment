using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class Producer
    {
        [Key]
        public int ProducerNumber { get; set; }
        [Required(ErrorMessage = "Name is required")]
        public string ProducerName { get; set; }
    }
}
