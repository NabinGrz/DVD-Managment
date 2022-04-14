using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDTitle
    {
        [Key]
        public int DVDNumber { get; set; }
        public int ProducerNumber { get; set; }
        [ForeignKey("ProducerNumber")]
        public Producer Producer { get; set; }

        public int CategoryNumber { get; set; }
        [ForeignKey("CategoryNumber")]
        public DVDCategory Category { get; set; }

        public int StudioNumber { get; set; }
        [ForeignKey("StudioNumber")]
        public Studio Studio { get; set; }

        public string DVDTitleName { get; set; }
        public DateTime? DateReleased { get; set; }
        public string StandardCharge { get; set; }
        public string PenaltyCharge { get; set; }
    }
}
