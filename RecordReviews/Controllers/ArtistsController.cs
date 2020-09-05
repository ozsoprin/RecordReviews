﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ArtistsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Artists
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var applicationDbContext = _context.Artists.Select(a => a);
            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(a => a.ArtistName.Contains(searchString));
            }

            ViewBag.ArtistStatistic = _context.Artists.Select(a => new { Name = a.ArtistName, Rate = a.AvgRate }).ToList();
            return View(await applicationDbContext.OrderByDescending(a => a.AvgRate).ToListAsync());
        }

        // GET: Artists/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(a=>a.Albums).ThenInclude(a=>a.Reviews).FirstOrDefaultAsync(m => m.ArtistID == id);
            if (artist == null)
            {
                return NotFound();
            }

            artist.PageViews++;
            _context.SaveChanges();
            return View(artist);
        }

        // GET: Artists/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Artists/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArtistName,BirthPlace,Genre")] Artist artist)
        {
            if (ModelState.IsValid)
            {
                //Check if artist is already exist in db.
                var _artist = _context.Artists.Where(_ => _.ArtistName == artist.ArtistName).Select(_ => new { _.ArtistID })
                    .SingleOrDefault();
                if (_artist != null)
                {
                    return RedirectToAction("Details", new { id = _artist.ArtistID });
                }

                //Create new artist
                await _context.AddAsync(artist);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = artist.ArtistID });
            }

            return View(artist);
        }

        // GET: Artists/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists.FindAsync(id);
            if (artist == null)
            {
                return NotFound();
            }
            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistID,ArtistName,BirthPlace,Genre,AvgRate,PageViews")] Artist artist)
        {
            if (id != artist.ArtistID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(artist);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArtistExists(artist.ArtistID))
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
            return View(artist);
        }

        // GET: Artists/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .FirstOrDefaultAsync(m => m.ArtistID == id);
            if (artist == null)
            {
                return NotFound();
            }

            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.Where(a=>a.ArtistID ==id).Include(a=>a.Albums).ThenInclude(a=>a.Reviews).FirstOrDefaultAsync();
            foreach (var album in artist.Albums)
            {
                album.DeleteAlbum(_context);
            }
            _context.Artists.Remove(artist);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Artists");
        }

        private bool ArtistExists(int id)
        {
            return _context.Artists.Any(e => e.ArtistID == id);
        }
    }
}
