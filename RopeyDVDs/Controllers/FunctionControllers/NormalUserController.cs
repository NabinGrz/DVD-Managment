using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;

namespace RopeyDVDs.Controllers
{
    public class NormalUserController : Controller
    {
        private readonly RopeyDVDsContext _context;

        public NormalUserController(RopeyDVDsContext context)
        {
            _context = context;
        }
        // https://localhost:7135/NormalUser/GetActorDetails?name=Smith
        //FUNCTION 1
        [Authorize]
        public IActionResult GetActorDetails(String surname)
        {

            var dvdTitle = _context.DVDTitles.ToList();
            var castMember = _context.CastMembers.ToList();
            var actorDetails = _context.Actors.ToList();
            var details =
                from d in dvdTitle
                join c in castMember
                on d.DVDNumber equals c.DVDNumber into table1
                from c in table1.Distinct().ToList().Where(c => c.DVDNumber == d.DVDNumber)
                join a in actorDetails on c.ActorNumber equals a.ActorNumber into table2
                from a in table2.Distinct().ToList().Where(a => a.ActorNumber == c.ActorNumber && a.ActorSurname == surname)
                select new { dvdTitle = d, castMember = c, actorDetails = a };
            var r = _context.Actors.FirstOrDefault();
            ViewBag.last = r;
            ViewBag.name = details;
            ViewBag.actorName = _context.Actors.ToArray();
            return View(details);
        }
   

        //FUNCTION 2
        [Authorize]
       public IActionResult GetActorCopy(int actorID)
        {
            /**
             select dt.DVDTitleName,(dc.Stock - Count(*)) as "Dif(Stock Left)" from  loans l
            inner join DVDCopys dc
            on dc.CopyNumber = l.CopyNumber
            inner join DVDTitles dt
            on dt.DVDNumber = dc.DVDNumber
            group by l.CopyNumber, dc.Stock,dt.DVDTitleName
             * */
            ViewBag.actorName = _context.Actors.ToArray();
            String n = "0";
            var dvdTitle = _context.DVDTitles.ToList();
            var dvdCopy = _context.DVDCopys.ToList();
            var castMember = _context.CastMembers.ToList();
            var actorDetails = _context.Actors.ToList();
            var loan = _context.Loans.ToList(); var details = (from l in loan
                          join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber
                          join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber
                          join c in castMember on dt.DVDNumber equals c.DVDNumber
                          group new { l, dc, dt,c } by new { l.CopyNumber, dc.Stock, dt.DVDTitleName,c.ActorNumber}
                          into grp
                          select new
                          {
                              DVDTitleName = grp.Key.DVDTitleName,
                              Stock = grp.Key.Stock,
                              CastActorNumber = grp.Key.ActorNumber,
                              LoanCount = grp.Count(),
                          }).Where(x => x.CastActorNumber == actorID);
            ViewBag.name = details;
            //Console.WriteLine(ViewBag.name);
            return View(details);
        }
   
    }
}
