using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RopeyDVDs.Data;

namespace RopeyDVDs.Controllers
{
    public class AssistantController : Controller
    {

        private readonly RopeyDVDsContext _dbcontext;


        public AssistantController(RopeyDVDsContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        // GET: AssistantController
        public ActionResult Index()
        {
            return View();
        }
        /**

         * 
         */
        // GET: AssistantController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }
        //FUNCTION 4
        public IActionResult GetList()
        {
            /**
        select dt.DVDTitleName,dt.DateReleased,p.ProducerName,s.StudioName,c.CastMemberNo,a.ActorFirstName,a.ActorSurname
            from DVDTitles dt
inner join CastMembers c on c.DVDNumber = dt.DVDNumber
inner join Producers p on p.ProducerNumber = dt.ProducerNumber
inner join Studios s on s.StudioNumber = dt.StudioNumber
inner join Actors a on a.ActorNumber = c.ActorNumber
order by  dt.DateReleased asc,a.ActorSurname asc
            */
            var dvdTitle = _dbcontext.DVDTitles.ToList();
            var producer = _dbcontext.Producers.ToList();
            var castMember = _dbcontext.CastMembers.ToList();
            var studio = _dbcontext.Studios.ToList();
            var actor = _dbcontext.Actors.ToList();
            var listProducer = from dt in dvdTitle
                          join c in castMember on dt.DVDNumber equals c.DVDNumber into table1
                          from c in table1.ToList().Where(c => c.DVDNumber == dt.DVDNumber).ToList()

                          join p in producer on dt.ProducerNumber equals p.ProducerNumber into table2
                          from p in table2.ToList().Where(p => p.ProducerNumber == dt.ProducerNumber).ToList()

                          join s in studio on dt.StudioNumber equals s.StudioNumber into table3
                          from s in table3.ToList().Where(s => s.StudioNumber == dt.StudioNumber).ToList()

                          join a in actor on c.ActorNumber equals a.ActorNumber into table4
                          from a in table4.ToList().Where(a => a.ActorNumber == c.ActorNumber).ToList()
                          orderby dt.DateReleased ascending, a.ActorSurname ascending
                          select new { dvdTitle = dt, castMember = c, actorDetails = a,studio =s, producer  = p };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.listProducer = listProducer;

            return View(listProducer);
        }

        //FUNCTION 5
        public IActionResult GetLoanDetails(int copynumber)
        {
            /**
                   select dc.CopyNumber,m.MembershipFirstName,dt.DVDTitleName,l.DateOut,l.DateDue,l.DateReturned from Members m
            inner join loans l on l.MemberNumber= m.MembershipNumber
            inner join DVDCopys dc on dc.CopyNumber = l.CopyNumber
            inner join DVDTitles dt on dt.DVDNumber = dc.DVDNumber
            where l.CopyNumber = 2
            */
            var dvdTitle = _dbcontext.DVDTitles.ToList();
            var loan = _dbcontext.Loans.ToList();
            var member = _dbcontext.Members.ToList();
            var dvdCopy = _dbcontext.DVDCopys.ToList();
            var loanDetails = from l in loan
                               join m in member on l.MemberNumber equals m.MembershipNumber into table1
                               from m in table1.ToList().Where(m => m.MembershipNumber == l.MemberNumber).ToList()

                               join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber into table2
                               from dc in table2.ToList().Where(dc => dc.CopyNumber == l.CopyNumber && l.CopyNumber ==  copynumber).ToList()

                               join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber into table3
                               from dt in table3.ToList().Where(dt => dt.DVDNumber == dc.DVDNumber).ToList()
                               select new { dvdTitle = dt, loan  = l, member = m,dvdCopy = dc };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.loanDetails = loanDetails;
            return View(loanDetails);
        }





        //FOR FUNCTION 13
        public IActionResult GetDVDofNoLoan()
        {
            /**
        select distinct dt.DVDTitleName  from DVDTitles dt
inner join DVDCopys dc on dt.DVDNumber = dc.DVDNumber
inner join loans l on dc.CopyNumber = l.CopyNumber
where (l.DateReturned = '0' and L.DateOut >= (GETDATE()-31))
            */
            DateTime currentDate = DateTime.Now.Date;
            DateTime lastDate = currentDate.Subtract(new TimeSpan(31, 0, 0, 0, 0));
            String d = "0";
            var dvdTitle = _dbcontext.DVDTitles.ToList();
            var loan = _dbcontext.Loans.ToList();
            var dvdCopy = _dbcontext.DVDCopys.ToList();

            var dvd = from dt in dvdTitle
                              join dc in dvdCopy on dt.DVDNumber equals dc.DVDNumber into table1
                              from dc in table1.Distinct().ToList().Where(dc => dc.DVDNumber == dt.DVDNumber).Distinct().ToList()

                              join l in loan on dc.CopyNumber equals l.CopyNumber into table2
                              from l in table2.Distinct().ToList().Where(l => l.CopyNumber == dc.CopyNumber && l.DateReturned == d && DateTime.Parse(l.DateOut) >= lastDate).Distinct().ToList()

                              select new { dvdTitle = dt, loan = l, dvdCopy = dc };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.dvd = dvd;
            return View(dvd);
        }


        public IActionResult GetTotalLoans()
        {
            /**
    select distinct m.MembershipFirstName,m.MembershipCategoryNumber,mc.MembershipCategoryTotalLoans,count(l.CopyNumber) as 'Total Loan' 
    from Members m
    inner join loans l on m.MembershipNumber = l.MemberNumber
    inner join MembershipCategorys mc on m.MembershipCategoryNumber = mc.MembershipCategoryNumber
    where l.DateReturned <> '0'
    group by m.MembershipFirstName,m.MembershipCategoryNumber,mc.MembershipCategoryTotalLoans
    ORDER BY M.MembershipFirstName ASC
            */
            String c = "0";
            var member = _dbcontext.Members.ToList();
            var loan = _dbcontext.Loans.ToList();
            var membercat = _dbcontext.MembershipCategorys.ToList();

            var dvd = from m in member
                      join l in loan on m.MembershipNumber equals l.MemberNumber into table1
                      from l in table1.Distinct().ToList().Where(l => l.MemberNumber == m.MembershipNumber).Distinct().ToList()

                      join mc in membercat on m.MembershipNumber equals mc.MembershipCategoryNumber into table2
                      from mc in table2.Distinct().ToList().Where(mc => mc.MembershipCategoryNumber == m.MembershipCategoryNumber && l.DateReturned != c )
                      
                      select new { member = m, loan = l, membercat = mc };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.dvd = dvd;
            return View(dvd);
        }
        //FOR FUNCTIONN 12 ..WRONXA
        /**
         * 
         * 
        select m.MembershipFirstName,m.MembershipLastName,M.MembershipAddress,L.DateOut,dt.DVDTitleName,DATEDIFF(DAY,L.DateOut,'2022-04-14') aS 'No of days' from Loans l
inner join Members m
on m.MembershipNumber = l.MemberNumber
INNER JOIN DVDCopys dc
on l.CopyNumber = dc.CopyNumber
inner join DVDTitles dt
on dt.DVDNumber = dc.DVDNumber
where (L.DateOut >= (GETDATE()-31) and l.DateReturned = '0')

        //for function 11
           select count(l.DateOut),DC.CopyNumber,m.MembershipFirstName,dt.DVDTitleName,l.DateOut,l.DateReturned from Members m
            inner join loans l on l.MemberNumber= m.MembershipNumber
            inner join DVDCopys dc on dc.CopyNumber = l.CopyNumber
            inner join DVDTitles dt on dt.DVDNumber = dc.DVDNumber
           GROUP BY dc.CopyNumber,m.MembershipFirstName,dt.DVDTitleName,l.DateOut,l.DateReturned
		   having  (l.DateReturned <> '0' )


         */
        // GET: AssistantController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AssistantController/Create
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

        // GET: AssistantController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: AssistantController/Edit/5
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

        // GET: AssistantController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: AssistantController/Delete/5
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
