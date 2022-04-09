using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class Loan
    {
        [Key]
        public int LoanNumber { get; set; }
        public int LoanTypeNumber { get; set; }
        public int CopyNumber { get; set; }
        public int MemberNumber { get; set; }
        [ForeignKey("LoanTypeNumber")]
        public LoanType LoanType { get; set; }
        [ForeignKey("CopyNumber")]
        public DVDCopy Copy { get; set; }
        [ForeignKey("MemberNumber")]
        public Member Member { get; set; }
        public string DateOut { get; set; }
        public string DateDue { get; set; }
        public string DateReturned { get; set; }
    }
}
