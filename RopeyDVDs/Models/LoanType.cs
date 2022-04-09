using System.ComponentModel.DataAnnotations;

namespace RopeyDVDs.Models
{
    public class LoanType
    {
        [Key]
        public int LoanTypeNumber { get; set; }
        [Required(ErrorMessage = "Loan type is required")]
        public string Loan_Type { get; set; }
        public string LoanDuration { get; set; }
    }
}
