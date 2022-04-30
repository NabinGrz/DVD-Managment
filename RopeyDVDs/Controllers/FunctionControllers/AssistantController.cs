﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RopeyDVDs.Data;
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
    [Authorize(Roles = "Manager,Assistant")]
    public class AssistantController : Controller
    {

        private readonly RopeyDVDsContext _dbcontext;


        public AssistantController(RopeyDVDsContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        //FUNCTION 4
        //https://localhost:44344/Assistant/GetList
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
                               select new { dvdTitle = dt, castMember = c, actorDetails = a, studio = s, producer = p };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.listProducer = listProducer;

            return View(listProducer);
        }

        //last loan data not coming
        //FUNCTION 5
        //https://localhost:7135/Assistant/GetLoanDetails?copynumber=4
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
            var lastloan = loan.LastOrDefault();
            var member = _dbcontext.Members.ToList();
            var dvdCopy = _dbcontext.DVDCopys.ToList();
            var loanDetails = from l in loan
                              join m in member on l.MemberNumber equals m.MembershipNumber into table1
                              from m in table1.ToList().Where(m => m.MembershipNumber == l.MemberNumber).ToList()

                              join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber into table2
                              from dc in table2.ToList().Where(dc => dc.CopyNumber == l.CopyNumber && l.CopyNumber == copynumber).ToList()

                              join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber into table3
                              from dt in table3.ToList().Where(dt => dt.DVDNumber == dc.DVDNumber).ToList()
                              select new { dvdTitle = dt, loan = l, member = m, dvdCopy = dc };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.loanDetails = loanDetails;
            Console.WriteLine(ViewBag.loanDetails);
            ViewBag.copyNumber = _dbcontext.DVDCopys.ToArray();
            return View(loanDetails);
        }

        //FOR FUNCTION 13
        //https://localhost:44344/Assistant/GetDVDofNoLoan
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

        //FUNCTION 8 ayena
        // Assistant/GetLoans
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
                      from mc in table2.Distinct().ToList().Where(mc => mc.MembershipCategoryNumber == m.MembershipCategoryNumber && l.DateReturned != c)
                      group new { l, m, mc } by new { m.MembershipFirstName, m.MembershipCategoryNumber, mc.MembershipCategoryTotalLoans }
                      into grp
                      select new
                      {
                          grp.Key.MembershipFirstName,
                          grp.Key.MembershipCategoryNumber,
                          grp.Key.MembershipCategoryTotalLoans,
                      };
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;

            ViewBag.totalloans = dvd;
            return View(dvd);
        }

        // GET: Actors/Delete/5

        //FOR FUNCTION 11 //Total count not coming accurately
        //https://localhost:44344/Assistant/GetDVDCopyListNotLoaned
        public IActionResult GetDVDCopyListNotLoaned()
        {
            /**
            select dt.DVDTitleName,dc.CopyNumber,m.MembershipFirstName,l.DateOut,count(l.DateOut) as "Total Loans" from Members m
            inner join loans l on l.MemberNumber= m.MembershipNumber
            inner join DVDCopys dc on dc.CopyNumber = l.CopyNumber
            inner join DVDTitles dt on dt.DVDNumber = dc.DVDNumber
			where l.DateReturned <> '0'
			group by dt.DVDTitleName,dc.CopyNumber,m.MembershipFirstName,l.DateOut
            
            */
            String c = "0";
            var member = _dbcontext.Members.ToList();
            var loan = _dbcontext.Loans.ToList();
            var dvdTitle = _dbcontext.DVDTitles.ToList();
            var dvdCopy = _dbcontext.DVDCopys.ToList();

            var copyloan = from l in loan
                           join m in member on l.MemberNumber equals m.MembershipNumber into table1
                           from m in table1.Distinct().ToList().Where(m => m.MembershipNumber == l.MemberNumber).Distinct().ToList()
                           join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber into table2
                           from dc in table2.Distinct().ToList().Where(dc => dc.CopyNumber == l.CopyNumber).Distinct().ToList()
                           join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber into table3
                           from dt in table3.Distinct().ToList().Where(dt => dt.DVDNumber == dc.DVDNumber && l.DateReturned != c).Distinct().ToList()
                           group new { l, m, dc, dt } by new { dt.DVDTitleName, dc.CopyNumber, m.MembershipFirstName, l.DateOut}
                           into grp
                           select new
                           {
                               TotalLoans = grp.Key.DateOut.Count(),
                               grp.Key.DVDTitleName,
                               grp.Key.CopyNumber,
                               grp.Key.MembershipFirstName,
                               grp.Key.DateOut,
                               
                           };
            ViewBag.totalloans = copyloan;
            return View(copyloan);
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
        //FUNCTION 10 PART 1
        //https://localhost:44344/Assistant/GetListOfDVDCopy
        public IActionResult GetListOfDVDCopy(bool copyDeleted = false)
        {
            ViewBag.copyDeleted = copyDeleted;

            /**
          select dc.CopyNumber,dc.DVDNumber,l.DateReturned,dc.DatePurchased from DVDCopys dc
INNER JOIN Loans l
on dc.CopyNumber = l.CopyNumber
where (dc.DatePurchased < (GETDATE()-365) and l.DateReturned <> '0')
          */
            DateTime currentDate = DateTime.Now.Date;
            DateTime lastDate = currentDate.Subtract(new TimeSpan(365, 0, 0, 0, 0));
            String d = "0";
            var loan = _dbcontext.Loans.ToList();
            var dvdCopy = _dbcontext.DVDCopys.ToList();

            var dvd = from dc in dvdCopy
                      join l in loan on dc.CopyNumber equals l.CopyNumber into table1
                      from l in table1.Distinct().Where(l => l.CopyNumber == dc.CopyNumber && l.DateReturned != d && dc.DatePurchased < lastDate)

                      select new { loan = l, dvdCopy = dc };
            ViewBag.dvdList = dvd;
            return View(dvd);
        }

        //FUNCTION 10 PART2
        [HttpGet]
        public async Task<IActionResult> DeleteCopy(int copynumber)
        {
            var copy = _dbcontext.DVDCopys.Where(l => l.CopyNumber == copynumber).First();
            _dbcontext.DVDCopys.Remove(copy);
            _dbcontext.SaveChanges();

            return RedirectToAction("GetListOfDVDCopy", new { copyDeleted = true });
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

    }
}
