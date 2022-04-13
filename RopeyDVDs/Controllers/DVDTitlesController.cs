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
    public class DVDTitlesController : Controller
    {
        private readonly RopeyDVDsContext _context;

        public DVDTitlesController(RopeyDVDsContext context)
        {
            _context = context;
        }

        // GET: DVDTitles
        public async Task<IActionResult> Index()
        {
            var ropeyDVDsContext = _context.DVDTitles.Include(d => d.Category).Include(d => d.Produce).Include(d => d.Studio);
            return View(await ropeyDVDsContext.ToListAsync());
        }

        // GET: DVDTitles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitles
                .Include(d => d.Category)
                .Include(d => d.Produce)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.DVDNumber == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // GET: DVDTitles/Create
        public IActionResult Create()
        {
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategorys, "CategoryNumber", "AgeRestricted");
            ViewData["ProduceNumber"] = new SelectList(_context.Producers, "ProducerNumber", "ProducerName");
            ViewData["StudioNumber"] = new SelectList(_context.Studios, "StudioNumber", "StudioName");
            return View();
        }

        // POST: DVDTitles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DVDNumber,ProduceNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dVDTitle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategorys, "CategoryNumber", "AgeRestricted", dVDTitle.CategoryNumber);
            ViewData["ProduceNumber"] = new SelectList(_context.Producers, "ProducerNumber", "ProducerName", dVDTitle.ProduceNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studios, "StudioNumber", "StudioName", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // GET: DVDTitles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitles.FindAsync(id);
            if (dVDTitle == null)
            {
                return NotFound();
            }
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategorys, "CategoryNumber", "AgeRestricted", dVDTitle.CategoryNumber);
            ViewData["ProduceNumber"] = new SelectList(_context.Producers, "ProducerNumber", "ProducerName", dVDTitle.ProduceNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studios, "StudioNumber", "StudioName", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // POST: DVDTitles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DVDNumber,ProduceNumber,CategoryNumber,StudioNumber,DateReleased,StandardCharge,PenaltyCharge")] DVDTitle dVDTitle)
        {
            if (id != dVDTitle.DVDNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dVDTitle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DVDTitleExists(dVDTitle.DVDNumber))
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
            ViewData["CategoryNumber"] = new SelectList(_context.DVDCategorys, "CategoryNumber", "AgeRestricted", dVDTitle.CategoryNumber);
            ViewData["ProduceNumber"] = new SelectList(_context.Producers, "ProducerNumber", "ProducerName", dVDTitle.ProduceNumber);
            ViewData["StudioNumber"] = new SelectList(_context.Studios, "StudioNumber", "StudioName", dVDTitle.StudioNumber);
            return View(dVDTitle);
        }

        // GET: DVDTitles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dVDTitle = await _context.DVDTitles
                .Include(d => d.Category)
                .Include(d => d.Produce)
                .Include(d => d.Studio)
                .FirstOrDefaultAsync(m => m.DVDNumber == id);
            if (dVDTitle == null)
            {
                return NotFound();
            }

            return View(dVDTitle);
        }

        // POST: DVDTitles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dVDTitle = await _context.DVDTitles.FindAsync(id);
            _context.DVDTitles.Remove(dVDTitle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DVDTitleExists(int id)
        {
            return _context.DVDTitles.Any(e => e.DVDNumber == id);
        }
    }
}
