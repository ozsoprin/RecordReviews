using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Index()
        {
            return View(await _context.Albums.ToListAsync());
        }

        // GET: Albums/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .FirstOrDefaultAsync(m => m.Id == id);
            if (album == null)
            {
                return NotFound();
            }

            album.PageViews++;
            await _context.SaveChangesAsync();

            return View(album);
        }

        // GET: Albums/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Albums/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Artist,ReleaseDate,Genre")] Album album)
        {
            if (ModelState.IsValid)
            {
                //Check valid release date
                if (album.ReleaseDate > DateTime.Now)
                {
                    ModelState.AddModelError("ReleaseDate","Album's release date is not valid");
                    return View(album);
                }

                //Check if the album already exists
                var _album = _context.Albums.Where(_ => _.Title == album.Title && _.Artist == album.Artist)
                    .Select(_ => new {_.Id}).SingleOrDefault();
                if (_album != null)
                {
                    return RedirectToAction("Details", new {id = _album.Id});
                }
                else
                {
                    //Check if the album's artist already exist in the DB
                    var _artist = _context.Artists.Where(_ => _.Name == album.Artist).Select(_ => new {_.Id})
                        .SingleOrDefault();
                    if (_artist != null)
                    {
                        //Saving the new album
                        album.ArtistId = _artist.Id;
                        await _context.Albums.AddAsync(album);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = album.Id });
                    }
                    else
                    {
                        //Generating a new artist if not existed
                        var newArtist = new Artist(album.Artist, "Unknown", album.Genre);
                        await _context.Artists.AddAsync(newArtist);
                        await _context.SaveChangesAsync();

                        //Saving the new album
                        album.ArtistId = newArtist.Id;
                        await _context.Albums.AddAsync(album);
                        await _context.SaveChangesAsync();
                        return RedirectToAction("Details", new { id = album.Id });
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
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Artist,ArtistId,ReleaseDate,Genre,AvgRate,PageViews")] Album album)
        {
            if (id != album.Id)
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
                    if (!AlbumExists(album.Id))
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
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var album = await _context.Albums.FindAsync(id);
            _context.Albums.Remove(album);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AlbumExists(int id)
        {
            return _context.Albums.Any(e => e.Id == id);
        }
    }
}
