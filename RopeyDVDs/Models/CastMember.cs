using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RopeyDVDs.Models
{
    public class CastMember
    {
        [Key]
        public int CastMemberNo { get; set; }
        [ForeignKey("DVDNumber")]
        public int DVDNumber { get; set; }
        [ForeignKey("ActorNumber")]
        public int ActorNumber { get; set; }

    }
}
