using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index(string searchString)
        {
            var applicationDbContext = _context.Reviews.Include(r => r.Album).ThenInclude(a => a.Artist);
            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = (IIncludableQueryable<Review, Artist>) applicationDbContext.Where(a => a.Album.AlbumTitle.Contains(searchString));
            }
            var mostReviewedAlbum = applicationDbContext.GroupBy(r => r.AlbumId)
                .Select(r => new { AlbumID = r.Key, Count = r.Count()})
                .OrderByDescending(x => x.Count).Take(1).SingleOrDefault();
            var mostReviewedArtist = applicationDbContext
                .Join(_context.Albums, review => review.AlbumId, album => album.AlbumId, (review, album) => album)
                .Join(_context.Artists, album => album.ArtistId, artist => artist.ArtistID, (album, artist) => artist)
                .GroupBy(a => a.ArtistID)
                .Select(r => new { ArtistID = r.Key, Count = r.Count() })
                .OrderByDescending(x => x.Count).Take(1).SingleOrDefault();
            var mostReviewedGenre = applicationDbContext
                .Join(_context.Albums, review => review.AlbumId, album => album.AlbumId, (review, album) => album)
                .Join(_context.Artists, album => album.ArtistId, artist => artist.ArtistID, (album, artist) => artist)
                .GroupBy(a => a.Genre)
                .Select(r => new { Genre = r.Key, Count = r.Count() })
                .OrderByDescending(x => x.Count).Take(1).SingleOrDefault();
            ViewBag.MostReviewedArtist = _context.Artists.FirstOrDefault(a=>a.ArtistID == mostReviewedArtist.ArtistID);
            ViewBag.MostReviewedGenre = mostReviewedGenre.Genre;
            ViewBag.MostReviewedGenreAlbums = _context.Albums.Where(a => a.Genre == mostReviewedGenre.Genre).OrderByDescending(a=>a.AvgRate).Take(2).ToList();
            ViewBag.MostReviewedAlbum = _context.Albums.FirstOrDefault(a => a.AlbumId == mostReviewedAlbum.AlbumID);
            return View(await applicationDbContext.OrderByDescending(r=>r.CreationTime).ToListAsync());
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Album).Include(r=>r.User)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create(int? albumID)
        {
            ViewData["AlbumId"] = new SelectList(_context.Albums, "AlbumId", "AlbumTitle");

            if (albumID != null)
            {
                var album = _context.Albums.SingleOrDefault(a => a.AlbumId == albumID);
                return View(new Review { AlbumId = album.AlbumId});
            }
            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ReviewId,AlbumId,Comment,Rate")] Review review)
        {
            if (ModelState.IsValid)
            {
                review.CreationTime = DateTime.Now;
                review.Album = await _context.Albums.Include(a=>a.Reviews).Include(a => a.Artist).ThenInclude(a=>a.Albums).ThenInclude(a=>a.Reviews).FirstOrDefaultAsync(a => a.AlbumId == review.AlbumId);
                _context.Add(review);
                review.UpdateRate();
                await _context.SaveChangesAsync();
                return RedirectToAction("Details","Albums", new { id = review.Album.AlbumId });
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "AlbumId", "AlbumTitle", review.AlbumId);
            return View(review);
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["AlbumId"] = new SelectList(_context.Albums, "AlbumId", "AlbumTitle", review.AlbumId);
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ReviewId,AlbumId,Comment,Rate,CreationTime")] Review review)
        {
            if (id != review.ReviewId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.ReviewId))
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
            ViewData["AlbumId"] = new SelectList(_context.Albums, "AlbumId", "AlbumTitle", review.AlbumId);
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Album).ThenInclude(a => a.Artist).ThenInclude(a => a.Albums).ThenInclude(a => a.Reviews)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.Album).ThenInclude(a => a.Artist).ThenInclude(a=>a.Albums).ThenInclude(a=>a.Reviews)
                .FirstOrDefaultAsync(m => m.ReviewId == id);
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
            review.UpdateRate();
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.ReviewId == id);
        }
    }
}
