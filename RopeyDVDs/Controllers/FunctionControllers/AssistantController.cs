using Microsoft.AspNetCore.Authorization;
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
            var loanDetails = (from l in loan
                               join m in member on l.MemberNumber equals m.MembershipNumber into table1
                               from m in table1.ToList().Where(m => m.MembershipNumber == l.MemberNumber).ToList()

                               join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber into table2
                               from dc in table2.ToList().Where(dc => dc.CopyNumber == l.CopyNumber && l.CopyNumber == copynumber).ToList()

                               join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber into table3
                               from dt in table3.ToList().Where(dt => dt.DVDNumber == dc.DVDNumber).ToList()
                               group new { dt, l, m, dc } by new { l.CopyNumber, dt.DVDTitleName, m.MembershipFirstName, m.MembershipLastName, m.MemberDOB, m.MembershipAddress, l.DateDue, l.DateOut, l.DateReturned }
                              into grp
                               select new
                               {
                                   grp.Key.CopyNumber,
                                   grp.Key.DVDTitleName,
                                   grp.Key.MembershipFirstName,
                                   grp.Key.MembershipLastName,
                                   grp.Key.MemberDOB,
                                   grp.Key.MembershipAddress,
                                   grp.Key.DateDue,
                                   grp.Key.DateOut,
                                   grp.Key.DateReturned,
                               });
            //var r = _context.Actors.FirstOrDefault();
            //ViewBag.last = r;
            ViewBag.loanDetails = loanDetails;
            ViewBag.copyNumber = _dbcontext.DVDCopys.ToArray();
            return View(loanDetails);
        }

        //FUNCTION 6
        public IActionResult ListAllMembers()
        {
            ViewBag.allmembers = _dbcontext.Members.ToList();
            return RedirectToAction("Index", "Members");
        }

        public IActionResult AddDVDCopy(int membershipNumber)
        {
            ViewBag.MemberAge = _dbcontext.Members.Where(x => x.MembershipNumber == membershipNumber).First();
            String dob = ViewBag.MemberAge.MemberDOB;//GETTING MEMBER DOB
            String todaysDate = DateTime.Now.ToShortDateString();

            //MEMBER NUMBER
            ViewBag.memberNumber = membershipNumber;

            //CONVERTING IN DATE TIME
            DateTime cDOB = DateTime.Parse(dob);
            DateTime ctodaysDate = DateTime.Parse(todaysDate);

            Console.WriteLine(todaysDate);
            Console.WriteLine(cDOB);//{11/25/1992 12:00:00 AM}
            Console.WriteLine(ctodaysDate);//{5/1/2022 12:00:00 AM}

            //CALCULATING YEARS FOR AGE
            TimeSpan dayDiff = ctodaysDate.Subtract(cDOB);
            Console.Write(dayDiff.Days.ToString());
            var age = dayDiff.Days / 365;
            Console.Write(age);

            ViewBag.memAge = age;
            //GETTING MEMBER SHIP DETAILS
            /**
                 select distinct m.MembershipNumber,m.MembershipCategoryNumber,mc.MembershipCategoryTotalLoans as "Allowed Loan",count(l.CopyNumber) as 'Member Total Loan' 
    from Members m
    inner join loans l on m.MembershipNumber = l.MemberNumber
    inner join MembershipCategorys mc on m.MembershipCategoryNumber = mc.MembershipCategoryNumber
    where m.MembershipNumber = 8
    group by m.MembershipNumber,m.MembershipCategoryNumber,mc.MembershipCategoryTotalLoans
  
             */
            var loans = _dbcontext.Loans.ToList();
            var members = _dbcontext.Members.ToList();
            var memberCategory = _dbcontext.MembershipCategorys.ToList();

            var details = from l in loans
                          join m in members on l.MemberNumber equals m.MembershipNumber into table1
                          from m in table1.Where(m => m.MembershipNumber == l.MemberNumber && m.MembershipNumber == membershipNumber).ToList()

                          join mc in memberCategory on m.MembershipCategoryNumber equals mc.MembershipCategoryNumber into table2
                          from mc in table2.Where(mc => mc.MembershipCategoryNumber == m.MembershipCategoryNumber).ToList()
                          group new { l, m, mc } by new { m.MembershipNumber, m.MembershipCategoryNumber, mc.MembershipCategoryTotalLoans }
                          into grp
                          select new
                          {
                              grp.Key.MembershipNumber,
                              grp.Key.MembershipCategoryNumber,
                              grp.Key.MembershipCategoryTotalLoans,
                              TotalLoans = grp.Count()
                          };
            ViewBag.Details = details;
            //{ MembershipNumber = 8, MembershipCategoryNumber = 5, MembershipCategoryTotalLoans = "20", TotalLoans = 8 }
            Console.WriteLine("============================================");
            Console.WriteLine(details);
            //FOR LOANTYPE
            ViewBag.LoanTypeNumber = _dbcontext.LoanTypes.ToList();

            //FOR COPY NUMBER
            ViewBag.Copy = _dbcontext.DVDCopys.ToList();
            return View();
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
        //https://localhost:44344/Assistant/GetTotalLoans
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

            var dvd = (from m in member
                       join l in loan on m.MembershipNumber equals l.MemberNumber into table1
                       from l in table1.Distinct().ToList().Where(l => l.MemberNumber == m.MembershipNumber).Distinct().ToList()

                       join mc in membercat on m.MembershipCategoryNumber equals mc.MembershipCategoryNumber into table2
                       from mc in table2.Distinct().ToList().Where(mc => mc.MembershipCategoryNumber == m.MembershipCategoryNumber)
                       group new { l, m, mc } by new { m.MembershipFirstName, m.MembershipCategoryNumber, mc.MembershipCategoryTotalLoans }
                      into grp
                       select new
                       {
                           //
                           grp.Key.MembershipFirstName,
                           grp.Key.MembershipCategoryNumber,
                           grp.Key.MembershipCategoryTotalLoans,
                           TotalLoans = grp.Count(),
                       }).OrderBy(x => x.MembershipFirstName);
            ViewBag.totalloans = dvd;
            return View(dvd);
        }


        //FUNCTION 9
        public IActionResult AddDVDTitle()
        {
            ViewBag.producer = _dbcontext.Producers;
            ViewBag.actor = _dbcontext.Actors;
            return View();
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

            var copyloan = (from l in loan
                            join m in member on l.MemberNumber equals m.MembershipNumber into table1
                            from m in table1.Distinct().ToList().Where(m => m.MembershipNumber == l.MemberNumber).Distinct().ToList()
                            join dc in dvdCopy on l.CopyNumber equals dc.CopyNumber into table2
                            from dc in table2.Distinct().ToList().Where(dc => dc.CopyNumber == l.CopyNumber).Distinct().ToList()
                            join dt in dvdTitle on dc.DVDNumber equals dt.DVDNumber into table3
                            from dt in table3.Distinct().ToList().Where(dt => dt.DVDNumber == dc.DVDNumber && l.DateReturned != c).Distinct().ToList()
                            group new { l, m, dc, dt } by new { dt.DVDTitleName, dc.CopyNumber, m.MembershipFirstName, l.DateOut }
                           into grp
                            select new
                            {
                                TotalLoans = grp.Count(),
                                grp.Key.DVDTitleName,
                                grp.Key.CopyNumber,
                                grp.Key.MembershipFirstName,
                                grp.Key.DateOut,

                            }).OrderBy(X => X.TotalLoans);
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

        //FUNCTION 7 SHOWING ALL 
        public IActionResult ListAllLoans()
        {
            /*
            select l.CopyNumber,m.MembershipFirstName,l.DateDue,l.DateReturned from loans l
            inner join members m on l.MemberNumber = m.MembershipNumber
            where l.DateReturned = '0'
             */

            DateTime currentDate = DateTime.Now.Date;
            DateTime lastDate = currentDate.Subtract(new TimeSpan(365, 0, 0, 0, 0));
            String d = "0";
            var loan = _dbcontext.Loans.ToList();
            var member = _dbcontext.Members.ToList();
            var loanDetail = (from l in loan
                             join m in member on l.MemberNumber equals m.MembershipNumber into table1
                             from m in table1.ToList().Where(m => m.MembershipNumber == l.MemberNumber && l.DateReturned == d)
                              orderby l.CopyNumber ascending
                              select new { loan = l, member = m });
            ViewBag.loanDetails = loanDetail;
            return View(loanDetail);
        }


        public IActionResult EditDVDCopyDetails(int CopyNumber)
        {
            //GET LOAN DETAILS OF THE COPY NUMBER
            ViewBag.UserLoanDetails = _dbcontext.Loans.Where(l => l.CopyNumber == CopyNumber).First();
            var cop = ViewBag.UserLoanDetails;

            //GET DVD OF THE COPY NUMBER
            ViewBag.CopyDVDNumber = _dbcontext.DVDCopys.Where(c => c.CopyNumber == CopyNumber).First();
            int copydvdnum = ViewBag.CopyDVDNumber.DVDNumber;

            //GET PENALTY CHARGE OF DVD NUMBER
            ViewBag.DVDNumber = _dbcontext.DVDTitles.Where(d => d.DVDNumber == copydvdnum).First();
            ViewBag.PenaltyCharge = ViewBag.DVDNumber.PenaltyCharge;
            int pCharge = int.Parse(ViewBag.PenaltyCharge);

            //CALCULATING DATE OF RETURN
            DateTime dueDate = DateTime.Parse(ViewBag.UserLoanDetails.DateDue);
            DateTime returnDate = DateTime.Now.Date.Date;

            //GETTING ONLY DATE
            var onlydate = returnDate.ToShortDateString();

            //GETTING DAY DIFFERENCE
            TimeSpan difference = returnDate.Subtract(dueDate);
            int dueDay = difference.Days;

            ViewBag.ReturnDate = onlydate;
            if (dueDay < 0)
            {
                ViewBag.OverDue = "0";
                ViewBag.TotalCharge = "0";
            }
            else
            {
                ViewBag.OverDue = dueDay;
                int totalCharge = dueDay * pCharge;
                ViewBag.TotalCharge = totalCharge;
                Console.WriteLine(difference);
            }


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecordDVDCopy
        (Loan loan, int loanNumber, int loantypenumber, int copynumber, int membernumber, string dateOut, string dateReturned, string dateDue)
        {
            var copyDB = _dbcontext.DVDCopys.ToList();
            var loanDB = _dbcontext.LoanTypes.ToList();
            var memberDB = _dbcontext.Members.ToList();
            loan.LoanNumber = loanNumber;
            loan.LoanTypeNumber = loantypenumber;
            loan.CopyNumber = copynumber;
            loan.MemberNumber = membernumber;
            loan.DateOut = dateOut;
            loan.DateDue = dateDue;
            loan.DateReturned = dateReturned;
            //loan.LoanType = loanDB.Where(l=> l.LoanTypeNumber == loantypenumber).First();
            //loan.Copy = copyDB.Where(c => c.CopyNumber ==  copynumber).First();
            //loan.Member = memberDB.Where(m => m.MembershipNumber == membernumber).First();

            if (ModelState.IsValid)
            {
                _dbcontext.Loans.Update(loan);
                var result = await _dbcontext.SaveChangesAsync();
                Console.WriteLine(result);
                return RedirectToAction("ListAllLoans");

            }

            return View();

        }

        public IActionResult UpdateDVDCopyDetails(int copyNumber)
        {
            ViewBag.loan_details = _dbcontext.Loans.Where(x => x.CopyNumber == copyNumber).First();
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

        //FUNCTION 12
        /**
       select distinct m.MembershipFirstName,m.MembershipLastName,m.MembershipAddress,dt.DVDTitleName,l.DateOut,DATEDIFF(day, l.DateOut, GETDATE()) as "No of days" from members m
          inner join Loans l
          on l.MemberNumber = m.MembershipNumber
          inner join DVDCopys dc
          on dc.CopyNumber = l.CopyNumber
          inner join DVDTitles dt
          on dt.DVDNumber = dc.DVDNumber
          where(max(l.DateOut)<= (GETDATE()-31) )
ORDER BY L.DateOut;
        
         */

        //AllFunctions
        public IActionResult AllFunctions()
        {
            return View();
        }
    }
}
