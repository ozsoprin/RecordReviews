using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class SearchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Searches
        public async Task<IActionResult> Index()
        {
            return View(await _context.Search.ToListAsync());
        }

        // GET: Searches/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Search
                .FirstOrDefaultAsync(m => m.type == id);
            if (search == null)
            {
                return NotFound();
            }

            return View(search);
        }

        // GET: Searches/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Searches/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("type,PrimaryKey,SecondaryKey,MinRate,MaxRate,GenreCountry,MaxDateTime,MinDateTime")] Search search)
        {
            if (ModelState.IsValid)
            {
                _context.Add(search);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(search);
        }

        // GET: Searches/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Search.FindAsync(id);
            if (search == null)
            {
                return NotFound();
            }
            return View(search);
        }

        // POST: Searches/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("type,PrimaryKey,SecondaryKey,MinRate,MaxRate,GenreCountry,MaxDateTime,MinDateTime")] Search search)
        {
            if (id != search.type)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(search);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SearchExists(search.type))
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
            return View(search);
        }

        // GET: Searches/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var search = await _context.Search
                .FirstOrDefaultAsync(m => m.type == id);
            if (search == null)
            {
                return NotFound();
            }

            return View(search);
        }

        // POST: Searches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var search = await _context.Search.FindAsync(id);
            _context.Search.Remove(search);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SearchExists(string id)
        {
            return _context.Search.Any(e => e.type == id);
        }
    }
}
