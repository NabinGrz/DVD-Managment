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
        // GET: NormalUserController
        public ActionResult Index()
        {
            return View();
        }
        // https://localhost:7135/NormalUser/GetActorDetails?name=Smith
        // GET: NormalUserController/Details/5
        //FUNCTION 1
        public IActionResult GetActorDetails(String name)
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
                from a in table2.Distinct().ToList().Where(a => a.ActorNumber == c.ActorNumber && a.ActorSurname == name)
                select new { dvdTitle = d, castMember = c, actorDetails = a };
            var r = _context.Actors.FirstOrDefault();
            ViewBag.last = r;
            ViewBag.name = details;

            return View(details);
        }
        /**
         SELECT DISTINCT count(l.DateReturned) as cc,L.LoanTypeNumber,d.DatePurchased,DT.DVDNumber,C.ActorNumber,A.ActorFirstName,A.ActorSurname,l.DateReturned
  FROM Loans L
  INNER JOIN DVDCopys D ON L.CopyNumber = D.CopyNumber
  
  INNER JOIN DVDTitles DT ON DT.DVDNumber = D.DVDNumber
  INNER JOIN CastMembers C ON D.DVDNumber = C.DVDNumber
  INNER JOIN Actors A ON C.ActorNumber = A.ActorNumber
  group by (l.DateReturned)
  having (L.DateReturned <> '0');
         */

        //FUNCTION 2
        //public IActionResult GetActorsLoanCopy(String name)
        //{

        //    var dvdTitle = _context.DVDTitles.ToList();
        //    var dvdCopy = _context.DVDCopys.ToList();
        //    var castMember = _context.CastMembers.ToList();
        //    var actorDetails = _context.Actors.ToList();
        //    var loan = _context.Loans.FirstOrDefault();
        //    var details = from d in dvdTitle join dc in dvdCopy //dvdtitle dvdcopy
        //                  on d.DVDNumber equals dc.DVDNumber into table1
        //                  from dc in table1.Distinct().ToList().Where(dc => dc.DVDNumber == d.DVDNumber) //dvdcopy castmember

        //                  join c in castMember on dc.DVDNumber equals c.DVDNumber into table2
        //                  from c in table2.Distinct().ToList().Where(c => c.DVDNumber == dc.DVDNumber) //castmember dvdcopy

        //                  join l in loan on dc.CopyNumber equals l.CopyNumber into table4
        //                  from dcc in table4.Distinct().ToList().Where(dcc => dcc.CopyNumber == l.CopyNumber) //actor castmember

        //                  join a in actorDetails on c.ActorNumber equals a.ActorNumber into table3
        //                  from a in table3.Distinct().ToList().Where(a => a.ActorNumber == c.ActorNumber && a.ActorSurname == name)
        //                  select new { dvdTitle = d, castMember = c, actorDetails = a ,loan = l,dvdCopy = dc};

        //    //var r = _context.Actors.FirstOrDefault();
        //    //ViewBag.last = r;
        //    ViewBag.name = details;

        //    return View(details);
        //}
        public IActionResult GetDateOut(int name)
        {
            /**
select DT.DVDTitleName,DC.CopyNumber,L.DateOut,DATEDIFF(DAY,L.DateOut,'2022-04-14') aS dIFF from DVDTitles dt
inner join DVDCopys dc 
on dt.DVDNumber = dc.DVDNumber
inner join Loans l
on dc.CopyNumber = l.CopyNumber
where (L.DateOut >= (GETDATE()-31));

             DateTime currentDate = DateTime.Now.Date;
            */

            DateTime currentDate = DateTime.Now.Date;
            DateTime lastDate = currentDate.Subtract(new TimeSpan(31, 0, 0, 0, 0));

        
            var dvdTitle = _context.DVDTitles.ToList();
            var dvdCopy = _context.DVDCopys.ToList();
            var castMember = _context.CastMembers.ToList();
            var member = _context.Members.ToList();
            var loan = _context.Loans.ToList();

            

        var details = from d in dvdTitle
                          join dc in dvdCopy
                          on d.DVDNumber equals dc.DVDNumber into table1
                          from dc in table1.Distinct().ToList().Where(dc => dc.DVDNumber == d.DVDNumber)
                          join l in loan on dc.CopyNumber equals l.CopyNumber into table2
                          from l in table2.Distinct().ToList().Where(l => l.CopyNumber == dc.CopyNumber)
                          join c in castMember
                          on dc.DVDNumber equals c.DVDNumber into table3
                          from c in table3.Distinct().ToList().Where(c => c.DVDNumber == dc.DVDNumber)
                          join m in member
                          on l.MemberNumber equals m.MembershipNumber into table4
                          from m in table4.Distinct().ToList().Where(m => m.MembershipNumber == l.MemberNumber && m.MembershipNumber == name && DateTime.Parse(l.DateOut) >= lastDate)
                          select new { dvdTitle = d, castMember = c,dvdCopy =dc,loan  = l,member  = m };

            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.date = details;

            return View(details);
        }
        // GET: NormalUserController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: NormalUserController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NormalUserController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: NormalUserController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: NormalUserController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: NormalUserController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
