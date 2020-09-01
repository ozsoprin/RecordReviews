using System;
using System.Collections.Generic;
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
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AlbumsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Albums
        public async Task<IActionResult> Index(string searchString)
        {
            var applicationDbContext = _context.Albums.Include(a => a.Artist).Select(a=>a);
            if (!String.IsNullOrEmpty(searchString))
            {
                applicationDbContext = applicationDbContext.Where(a => a.AlbumTitle.Contains(searchString));
            }

            ViewBag.AlbumStatistic = _context.Albums
                .Select(a => new {Title = a.AlbumTitle, Rate = a.AvgRate}).ToList();

            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist).Include(a=>a.Reviews)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistID", "ArtistName");
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AlbumTitle,ArtistName,ReleaseDate,Genre")] Album album)
        {
            if (ModelState.IsValid)
            {
                //Check valid release date
                if (album.ReleaseDate > DateTime.Now)
                {
                    ModelState.AddModelError("ReleaseDate", "Album's release date is not valid");
                    return View(album);
                }

                //Check if the album already exists
                var _album = _context.Albums.Where(_ => _.AlbumTitle == album.AlbumTitle && _.Artist == album.Artist)
                    .Select(_ => new { _.AlbumId }).SingleOrDefault();
                if (_album != null)
                {
                    return RedirectToAction("Details", new { id = _album.AlbumId });
                }
                else
                {
                    //Check if the album's artist already exist in the DB
                    var _artist = _context.Artists.Where(_ => _.ArtistName == album.ArtistName).Select(_ => new { _.ArtistID })
                        .SingleOrDefault();
                    if (_artist != null)
                    {
                        //Saving the new album
                        album.ArtistId = _artist.ArtistID;
                        await _context.Albums.AddAsync(album);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = album.AlbumId });
                    }
                    else
                    {
                        //Generating a new artist if not existed
                        var newArtist = new Artist(album.ArtistName, "Unknown", album.Genre);
                        await _context.Artists.AddAsync(newArtist);
                        await _context.SaveChangesAsync();

                        //Saving the new album
                        album.Artist = newArtist;
                        album.ArtistId = newArtist.ArtistID;
                        await _context.Albums.AddAsync(album);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = album.AlbumId });
                    }
                }
            }
            return View(album);
        }

        // GET: Albums/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FindAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistID", "ArtistName", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumTitle,ArtistName,ArtistId,ReleaseDate,Genre,AvgRate,PageViews")] Album album)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(album);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AlbumExists(album.AlbumId))
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
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistID", "ArtistName", album.ArtistId);
            return View(album);
        }

        // GET: Albums/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.Where(a=> a.AlbumId == id).Include(a=>a.Artist).Include(a=>a.Reviews).FirstOrDefaultAsync();
            foreach (var review in album.Reviews)
            {
                _context.Reviews.Remove(review);
            }
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            album.Artist.UpdateArtistRate();
            await _context.SaveChangesAsync();
            return RedirectToAction("Index","Albums");
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.AlbumId == id);
        }
    }
}
