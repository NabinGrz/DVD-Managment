using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class DVDTitle
    {
        [Key]
        public int DVDNumber { get; set; }
        [ForeignKey("ProducerNumber")]
        public int ProducerNumber { get; set; }
        [ForeignKey("CategoryNumber")]
        public int CategoryNumber { get; set; }
        [ForeignKey("StudioNumber")]
        public int StudioNumber { get; set; }
        public string DVDTitleName { get; set; }
        public DateTime? DateReleased { get; set; }
        public string StandardCharge { get; set; }
        public string PenaltyCharge { get; set; }
    }
}
