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
        public IActionResult GetActorsLoanCopy(String name)
        {
            /**
       select distinct a.ActorFirstName,dt.DVDTitleName,dc.Stock from Actors a
       inner join CastMembers c
       on a.ActorNumber = c.ActorNumber
       inner join DVDTitles dt
       on dt.DVDNumber = c.DVDNumber
       inner join DVDCopys  dc
       on dc.DVDNumber = dt.DVDNumber
       inner join loans l
       on l.CopyNumber = dc.CopyNumber
       where(a.ActorFirstName = 'Will' and dc.Stock >= 1 and l.DateReturned <> '0')
    */
            var dvdTitle = _context.DVDTitles.ToList();
            var dvdCopy = _context.DVDCopys.ToList();
            var castMember = _context.CastMembers.ToList();
            var actorDetails = _context.Actors.ToList();
            var loan = _context.Loans.ToList();
            // (from lm in loan where lm.CopyNumber == copynumber select lm).Max(m => m.DateOut);
            var aN= (from a in actorDetails where a.ActorSurname == name select a.ActorNumber).ToList();
            int actorNum = int.Parse(aN[0].ToString());
            Console.WriteLine(actorNum);
           //
           var cD = (from c in castMember where(c.ActorNumber == actorNum) select c.DVDNumber).ToList();
            int dvdNum = int.Parse(cD.ToString());
            Console.WriteLine(dvdNum);
            var details = from a in actorDetails
                          join c in castMember on a.ActorNumber equals c.ActorNumber into table1
                          from c in table1.Distinct().ToList().Where(c => c.ActorNumber == a.ActorNumber && a.ActorSurname == name && c.DVDNumber == dvdNum)
                          from dc in dvdCopy
                          join dt2 in dvdTitle on dc.DVDNumber equals dt2.DVDNumber into table3
                          from dt2 in table3.Distinct().ToList().Where(dt2 => dt2.DVDNumber == dc.DVDNumber && dc.Stock >=1)
                          from l in loan
                          join dc3 in dvdCopy on l.CopyNumber equals dc3.CopyNumber into table4
                          from dc3 in table4.Distinct().ToList().Where(dc3 => dc3.CopyNumber == l.CopyNumber && l.DateReturned != "0")
                          select new {castMember = c, actorDetails = a, loan = l, dvdCopy = dc };
            ViewBag.name = details;
            Console.WriteLine(ViewBag.name);
            return View(details);

        }



        //Function 3
        //https://localhost:7135/NormalUser/GetDateOut?memNumber=3
        public IActionResult GetDateOut(int memberNumber)
        {
            /**
select distinct DT.DVDTitleName,DC.CopyNumber,L.DateOut,DATEDIFF(DAY,L.DateOut,'2022-04-14') aS dIFF from DVDTitles dt
inner join DVDCopys dc 
on dt.DVDNumber = dc.DVDNumber
inner join Loans l
on dc.CopyNumber = l.CopyNumber
inner join CastMembers c
on c.DVDNumber = dc.DVDNumber
inner join Members m
on l.MemberNumber = m.MembershipNumber
where (L.DateOut >= (GETDATE()-31) and m.MembershipNumber = 3);

             DateTime currentDate = DateTime.Now.Date;
            */

            DateTime currentDate = DateTime.Now.Date;
            DateTime lastDate = currentDate.Subtract(new TimeSpan(31, 0, 0, 0, 0));
            Console.WriteLine("============================================================");
            Console.WriteLine("CURRENT" + currentDate);
            Console.WriteLine("LAST DATE" + lastDate);

            var dvdTitle = _context.DVDTitles.ToList();
            var dvdCopy = _context.DVDCopys.ToList();
            var castMember = _context.CastMembers.ToList();
            var member = _context.Members.ToList();
            var loan = _context.Loans.ToList();


            var details = from d in dvdTitle
                          join dc in dvdCopy
                          on d.DVDNumber equals dc.DVDNumber into table1
                          from dc in table1.ToList().Distinct().Where(dc => dc.DVDNumber == d.DVDNumber)
                          join l in loan on dc.CopyNumber equals l.CopyNumber into table2
                          from l in table2.ToList().Distinct().Where(l => l.CopyNumber == dc.CopyNumber)
                          join c in castMember
                          on dc.DVDNumber equals c.DVDNumber into table3
                          from c in table3.ToList().Distinct().Where(c => c.DVDNumber == dc.DVDNumber)
                          join m in member
                          on l.MemberNumber equals m.MembershipNumber into table4
                          from m in table4.ToList().Distinct().Where(m => m.MembershipNumber == l.MemberNumber && m.MembershipNumber == memberNumber && DateTime.Parse(l.DateOut) >= lastDate)
                          select new { dvdTitle = d, castMember = c, dvdCopy = dc, loan = l, member = m };

            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.date = details;
            ViewBag.memberNumbers = _context.Members.ToArray();
            return View(details);
        }
    }
}
