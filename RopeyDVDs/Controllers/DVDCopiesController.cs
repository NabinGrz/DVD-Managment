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
    public class DVDCopiesController : Controller
    {
        private readonly RopeyDVDsContext _context;

        public DVDCopiesController(RopeyDVDsContext context)
        {
            _context = context;
        }

        // GET: DVDCopies
        public async Task<IActionResult> Index()
        {
            var ropeyDVDsContext = _context.DVDCopys.Include(d => d.DVDTitle);
            return View(await ropeyDVDsContext.ToListAsync());
        }

        // GET: DVDCopies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopys
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.CopyNumber == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // GET: DVDCopies/Create
        public IActionResult Create()
        {
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitles, "DVDNumber", "DVDNumber");
            return View();
        }

        // POST: DVDCopies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDCopy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitles, "DVDNumber", "DVDNumber", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // GET: DVDCopies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopys.FindAsync(id);
            if (dVDCopy == null)
            {
                return NotFound();
            }
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitles, "DVDNumber", "DVDNumber", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // POST: DVDCopies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CopyNumber,DVDNumber,DatePurchased")] DVDCopy dVDCopy)
        {
            if (id != dVDCopy.CopyNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDCopy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDCopyExists(dVDCopy.CopyNumber))
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
            ViewData["DVDNumber"] = new SelectList(_context.DVDTitles, "DVDNumber", "DVDNumber", dVDCopy.DVDNumber);
            return View(dVDCopy);
        }

        // GET: DVDCopies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDCopy = await _context.DVDCopys
                .Include(d => d.DVDTitle)
                .FirstOrDefaultAsync(m => m.CopyNumber == id);
            if (dVDCopy == null)
            {
                return NotFound();
            }

            return View(dVDCopy);
        }

        // POST: DVDCopies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDCopy = await _context.DVDCopys.FindAsync(id);
            _context.DVDCopys.Remove(dVDCopy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDCopyExists(int id)
        {
            return _context.DVDCopys.Any(e => e.CopyNumber == id);
        }
    }
}
