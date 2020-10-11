using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using RecordReviews.Data;
using RecordReviews.Models;
using RecordReviews.Authorization;

namespace RecordReviews.Controllers
{
    public class AlbumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;

        public AlbumsController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        // GET: Albums
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var albums = _context.Albums.Include(a => a.Artist).Select(r => r);
            var isAuthorized = User.IsInRole(Constants.ManagersRole) ||
                               User.IsInRole(Constants.AdministratorsRole);
            var currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized)
            {
                albums = albums.Where(r => r.Status == AlbumStatus.Approved || r.OwnerID == currentUserId).Select(a => a);
            }

            if (!String.IsNullOrEmpty(searchString))
            {
                albums = albums.Where(a => a.AlbumTitle.Contains(searchString));
            }

            ViewBag.AlbumStatistic = _context.Albums
                .Select(a => new {Title = a.AlbumTitle, Rate = a.AvgRate}).ToList();

            return View(await albums.OrderByDescending(a => a.AvgRate).ToListAsync());
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
            var isAuthorized = User.IsInRole(Constants.ManagersRole) ||
                               User.IsInRole(Constants.AdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != album.OwnerID
                && album.Status != AlbumStatus.Approved)
            {
                return Forbid();
            }

            ViewBag.AlbumReviews = _context.Albums.Include(a => a.Reviews).SingleOrDefault(a => a.AlbumId == id)
                ?.Reviews.ToList();
            ViewBag.AlbumYouMightLike = _context.Albums.OrderByDescending(a => a.AvgRate).ToList();
            album.PageViews++;

            if (HttpContext.Session.GetString("LastAlbums") != null)
            {
                var albumlist = JsonConvert.DeserializeObject<Album[]>(HttpContext.Session.GetString("LastAlbums")).ToList();
                if (albumlist.Count == 10)
                {
                    albumlist.RemoveAt(albumlist.Count - 1);
                }
                albumlist.Insert(0, album);
                var lastAlbums = new Album[albumlist.Count];
                for (int i = 0; i < albumlist.Count; i++)
                {
                    var tmpAlbum = albumlist.ElementAt(i);
                    lastAlbums[i] = new Album()
                    {
                        AlbumId = tmpAlbum.AlbumId,
                        AlbumTitle = tmpAlbum.AlbumTitle,
                        ArtistName = tmpAlbum.ArtistName,
                        Genre = tmpAlbum.Genre,
                        ArtistId = tmpAlbum.ArtistId,
                        AvgRate = tmpAlbum.AvgRate,
                        PageViews = tmpAlbum.PageViews
                    };
                }
                HttpContext.Session.SetString("LastAlbums", JsonConvert.SerializeObject(lastAlbums));
            }
            else
            {
                var lastAlbums = new[]
                {
                    new Album(){
                        AlbumId = album.AlbumId,
                        AlbumTitle = album.AlbumTitle,
                        ArtistName = album.ArtistName,
                        Genre = album.Genre,
                        ArtistId = album.ArtistId,
                        AvgRate = album.AvgRate,
                        PageViews = album.PageViews
                    }
                };
                HttpContext.Session.SetString("LastAlbums", JsonConvert.SerializeObject(lastAlbums));
            }

            ViewBag.AlbumYouMightLike = GetRecommended().ToList();

            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, AlbumStatus status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var album = await _context.Albums
                .Include(a => a.Artist).Include(a => a.Reviews)
                .FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }

            var contactOperation = (status == AlbumStatus.Approved)
                ? Operations.Approve
                : Operations.Reject;

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, album,
                contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            album.Status = status;
            _context.Albums.Update(album);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Albums", new { id = album.AlbumId });
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

                //Generating a new artist if not existed
                var newArtist = new Artist(album.ArtistName, "Unknown", album.Genre);
                newArtist.OwnerID = _userManager.GetUserId(User);
                await _context.Artists.AddAsync(newArtist);
                await _context.SaveChangesAsync();

                //Saving the new album
                album.OwnerID = _userManager.GetUserId(User);
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    User, album,
                    Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }
                album.Artist = newArtist;
                album.ArtistId = newArtist.ArtistID;
                await _context.Albums.AddAsync(album);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = album.AlbumId });
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

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, album,
                Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistID", "ArtistName", album.ArtistId);
            return View(album);
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AlbumId,AlbumTitle,ArtistName,ArtistId,ReleaseDate,Genre,AvgRate,PageViews")] Album editedAlbum)
        {
            if (id != editedAlbum.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var album = await _context
                    .Albums.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.AlbumId == id);

                if (album == null)
                {
                    return NotFound();
                }
                try
                {
                    album.AlbumTitle = editedAlbum.AlbumTitle;
                    album.ArtistName = editedAlbum.ArtistName;
                    album.ReleaseDate = editedAlbum.ReleaseDate;
                    album.Genre = editedAlbum.Genre;

                    var isAuthorized = await _authorizationService.AuthorizeAsync(
                        User, album,
                        Operations.Update);

                    if (!isAuthorized.Succeeded)
                    {
                        return Forbid();
                    }

                    _context.Attach(album).State = EntityState.Modified;

                    if (album.Status == AlbumStatus.Approved)
                    {
                        // If the contact is updated after approval, 
                        // and the user cannot approve,
                        // set the status back to submitted so the update can be
                        // checked and approved.
                        var canApprove = await _authorizationService.AuthorizeAsync(User,
                            album,
                            Operations.Approve);

                        if (!canApprove.Succeeded)
                        {
                            album.Status = AlbumStatus.Submitted;
                        }
                    }


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
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistID", "ArtistName", editedAlbum.ArtistId);
            return View();
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
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, album,
                Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var album = await _context.Albums.Where(a=> a.AlbumId == id).Include(a=>a.Artist).Include(a=>a.Reviews).FirstOrDefaultAsync();
            if (album == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, album,
                Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }

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

        public IEnumerable<Album> GetRecommended()
        {
            if (HttpContext.Session.GetString("LastAlbums") != null)
            {
                var albumList = JsonConvert.DeserializeObject<Album[]>(HttpContext.Session.GetString("LastAlbums"));
                var visits = albumList.AsQueryable();
                var Genre = visits.GroupBy(a => a.Genre)
                    .OrderByDescending(group => group.Count())
                    .First().Key;

                List<int> albumIds = new List<int>();
                foreach (var album in albumList)
                {
                    albumIds.Add(album.AlbumId);
                }

                var recommendedAlbums = _context.Albums.Where(a => a.Genre == Genre && !albumIds.Contains(a.AlbumId)).OrderBy(album => Guid.NewGuid())
                    .AsEnumerable();

                


                return recommendedAlbums;
            }

            return null;
        }
    }
}
