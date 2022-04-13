#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RopeyDVDs.Data;
using RopeyDVDs.Models;

namespace RopeyDVDs.Controllers
{
    public class LoansController : Controller
    {
        private readonly RopeyDVDsContext _context;

        public LoansController(RopeyDVDsContext context)
        {
            _context = context;
        }

        // GET: Loans
        public async Task<IActionResult> Index()
        {
            var ropeyDVDsContext = _context.Loans.Include(l => l.Copy).Include(l => l.LoanType).Include(l => l.Member);
            return View(await ropeyDVDsContext.ToListAsync());
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Copy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // GET: Loans/Create
        public IActionResult Create()
        {
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopys, "CopyNumber", "CopyNumber");
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanTypes, "LoanTypeNumber", "Loan_Type");
            ViewData["MemberNumber"] = new SelectList(_context.Members, "MembershipNumber", "MembershipAddress");
            return View();
        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LoanNumber,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopys, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanTypes, "LoanTypeNumber", "Loan_Type", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Members, "MembershipNumber", "MembershipAddress", loan.MemberNumber);
            return View(loan);
        }

        // GET: Loans/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopys, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanTypes, "LoanTypeNumber", "Loan_Type", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Members, "MembershipNumber", "MembershipAddress", loan.MemberNumber);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanNumber,LoanTypeNumber,CopyNumber,MemberNumber,DateOut,DateDue,DateReturned")] Loan loan)
        {
            if (id != loan.LoanNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanNumber))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CopyNumber"] = new SelectList(_context.DVDCopys, "CopyNumber", "CopyNumber", loan.CopyNumber);
            ViewData["LoanTypeNumber"] = new SelectList(_context.LoanTypes, "LoanTypeNumber", "Loan_Type", loan.LoanTypeNumber);
            ViewData["MemberNumber"] = new SelectList(_context.Members, "MembershipNumber", "MembershipAddress", loan.MemberNumber);
            return View(loan);
        }

        // GET: Loans/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(l => l.Copy)
                .Include(l => l.LoanType)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.LoanNumber == id);
            if (loan == null)
            {
                return NotFound();
            }

            return View(loan);
        }

        // POST: Loans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            _context.Loans.Remove(loan);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.LoanNumber == id);
        }
    }
}
