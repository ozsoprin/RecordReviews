using System;
using System.Collections.Generic;
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
using RecordReviews.Authorization;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class ArtistsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthorizationService _authorizationService;
        private readonly UserManager<IdentityUser> _userManager;

        public ArtistsController(ApplicationDbContext context,
            IAuthorizationService authorizationService,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _authorizationService = authorizationService;
            _userManager = userManager;
        }

        // GET: Artists
        [AllowAnonymous]
        public async Task<IActionResult> Index(string searchString)
        {
            var aList = _context.Artists.ToList();
            var countryList = new HashSet<string>();
            foreach (var artist in aList)
            {
                countryList.Add(artist.BirthPlace);
            }

            ViewBag.ArtistsCountry = countryList;
            var artists = _context.Artists.Select(a => a);
            var isAuthorized = User.IsInRole(Constants.ManagersRole) ||
                               User.IsInRole(Constants.AdministratorsRole);
            var currentUserId = _userManager.GetUserId(User);
            if (!isAuthorized)
            {
                artists = artists.Where(r => r.Status == ArtistStatus.Approved
                                             || r.OwnerID == currentUserId).Select(a => a);
            }
            if (!String.IsNullOrEmpty(searchString))
            {
                artists = artists.Where(a => a.ArtistName.Contains(searchString));
            }

            ViewBag.ArtistStatistic = _context.Artists.Select(a => new { Name = a.ArtistName, Rate = a.AvgRate }).ToList();
            return View(await artists.OrderByDescending(a => a.AvgRate).ToListAsync());
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
            var isAuthorized = User.IsInRole(Constants.ManagersRole) ||
                               User.IsInRole(Constants.AdministratorsRole);

            var currentUserId = _userManager.GetUserId(User);

            if (!isAuthorized
                && currentUserId != artist.OwnerID
                && artist.Status != ArtistStatus.Approved)
            {
                return Forbid();
            }
            artist.PageViews++;

           
            if (HttpContext.Session.GetString("LastArtists") != null)
            {
                var artistList = JsonConvert.DeserializeObject<Artist[]>(HttpContext.Session.GetString("LastArtists")).ToList();
                if (artistList.Count == 10)
                {
                    artistList.RemoveAt(artistList.Count - 1);
                }
                artistList.Insert(0, artist);
                var lastArtists = new Artist[artistList.Count];
                for (int i = 0; i < artistList.Count; i++)
                {
                    var tmpArtist = artistList.ElementAt(i);
                    lastArtists[i] = new Artist
                    {
                        ArtistName = tmpArtist.ArtistName,
                        Genre = tmpArtist.Genre,
                        ArtistID = tmpArtist.ArtistID,
                        AvgRate = tmpArtist.AvgRate,
                        BirthPlace = tmpArtist.BirthPlace,
                        PageViews = tmpArtist.PageViews
                    };
                }
                HttpContext.Session.SetString("LastArtists", JsonConvert.SerializeObject(lastArtists));
            }
            else
            {
                var lastArtists =  new []
                {
                    new Artist{
                    ArtistName = artist.ArtistName,
                    Genre = artist.Genre,
                    ArtistID = artist.ArtistID,
                    AvgRate = artist.AvgRate,
                    BirthPlace = artist.BirthPlace,
                    PageViews = artist.PageViews
                    }
                };
                HttpContext.Session.SetString("LastArtists", JsonConvert.SerializeObject(lastArtists));
            }

            ViewBag.ArtistsYouMightLike = GetRecommended().ToList();
            ViewBag.ReviewesForArtist = artist.GetReviews();

            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(int? id, ArtistStatus status)
        {
            if (id == null)
            {
                return NotFound();
            }

            var artist = await _context.Artists
                .Include(r => r.Albums).FirstOrDefaultAsync(m => m.ArtistID == id);
            if (artist == null)
            {
                return NotFound();
            }

            var contactOperation = (status == ArtistStatus.Approved)
                ? Operations.Approve
                : Operations.Reject;

            var isAuthorized = await _authorizationService.AuthorizeAsync(User, artist,
                contactOperation);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            artist.Status = status;
            _context.Artists.Update(artist);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Artists", new { id = artist.ArtistID });
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
                artist.OwnerID = _userManager.GetUserId(User);
                var isAuthorized = await _authorizationService.AuthorizeAsync(
                    User, artist,
                    Operations.Create);
                if (!isAuthorized.Succeeded)
                {
                    return Forbid();
                }

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
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, artist,
                Operations.Update);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }


            return View(artist);
        }

        // POST: Artists/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArtistID,ArtistName,BirthPlace,Genre,AvgRate,PageViews")] Artist editedArtist)
        {
            if (id != editedArtist.ArtistID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var artist = await _context
                    .Artists.AsNoTracking()
                    .FirstOrDefaultAsync(m => m.ArtistID == id);

                if (artist == null)
                {
                    return NotFound();
                }
                try
                {
                    artist.ArtistName = editedArtist.ArtistName;
                    artist.BirthPlace = editedArtist.BirthPlace;
                    artist.Genre = editedArtist.Genre;
                    var isAuthorized = await _authorizationService.AuthorizeAsync(
                        User, artist,
                        Operations.Update);
                    if (!isAuthorized.Succeeded)
                    {
                        return Forbid();
                    }

                    _context.Attach(artist).State = EntityState.Modified;

                    if (artist.Status == ArtistStatus.Approved)
                    {
                        // If the contact is updated after approval, 
                        // and the user cannot approve,
                        // set the status back to submitted so the update can be
                        // checked and approved.
                        var canApprove = await _authorizationService.AuthorizeAsync(User,
                            artist,
                            Operations.Approve);

                        if (!canApprove.Succeeded)
                        {
                            artist.Status = ArtistStatus.Submitted;
                        }
                    }
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
            return View(editedArtist);
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

            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, artist,
                Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
            return View(artist);
        }

        // POST: Artists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var artist = await _context.Artists.Where(a=>a.ArtistID ==id).Include(a=>a.Albums).ThenInclude(a=>a.Reviews).FirstOrDefaultAsync();
            if (artist == null)
            {
                return NotFound();
            }
            var isAuthorized = await _authorizationService.AuthorizeAsync(
                User, artist,
                Operations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return Forbid();
            }
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

        public IEnumerable<Artist> GetRecommended()
        {
            if (HttpContext.Session.GetString("LastArtists") != null)
            {
                var artistList = JsonConvert.DeserializeObject<Artist[]>(HttpContext.Session.GetString("LastArtists"));
                var visits = artistList.AsQueryable();
                var Genre = visits.GroupBy(artist => artist.Genre)
                    .OrderByDescending(group => group.Count())
                    .First().Key;

                var lastartist = artistList[0];

                var recommendedArtist = _context.Artists.Where(a => a.Genre == Genre && a.ArtistID != lastartist.ArtistID).OrderBy(album => Guid.NewGuid())
                    .AsEnumerable();

                return recommendedArtist;
            }

            return null;
        }
    }
}
