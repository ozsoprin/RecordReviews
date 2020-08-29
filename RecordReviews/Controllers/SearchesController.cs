using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Data;
using RecordReviews.Models;

using System.Data;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;

namespace RecordReviews.Controllers
{
    public class SearchesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Search
        public ActionResult Index()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(
        //    [Bind("type,PrimaryKey,SecondaryKey,MinRate,MaxRate,GenreCountry,MaxDateTime,MinDateTime")]Search search)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        switch (search.type)
        //        {
        //            case "Album":

        //                if (search.PrimaryKey != null)
        //                {
        //                    var album = _context.Albums.Where(a => a.AlbumTitle == search.PrimaryKey).FirstOrDefault();
        //                    if (album != null)
        //                    {
        //                        return RedirectToAction("Details", "Album", new { id = album.AlbumId });
        //                    }

        //                }

        //                // if (search.secondaryName != null)
        //                // {
        //                //     var artist = db.Artists.Where(a => a.Name == search.secondaryName).FirstOrDefault();
        //                //     // if (artist == null)
        //                //     // {
        //                //     //     ModelState.AddModelError("secondaryName",
        //                //     //         "You cannot search for an artist that does not exist");
        //                //     //     return View(search);
        //                //     // }
        //                //
        //                // }

        //                search.MinRate = search.MinRate == null ? 0 : search.MinRate;
        //                search.MaxRate = search.MaxRate == null ? 10 : search.MaxRate;

        //                if (search.MinRate > search.MaxRate)
        //                {
        //                    ModelState.AddModelError("ScoreLowerBound", "Invalid Score Range");
        //                    return View(search);
        //                }

        //                search.PrimaryKey = search.PrimaryKey == null ? "" : search.PrimaryKey;
        //                var genre = search.GenreCountry != null ? search.GenreCountry : "";
        //                var artistName = search.SecondaryKey == null ? "" : search.SecondaryKey;

        //                search.MinDateTime = search.MinDateTime == null ? DateTime.MinValue : search.MinDateTime;
        //                search.MaxDateTime = search.MaxDateTime == null ? DateTime.Now : search.MaxDateTime;

        //                if (search.MaxDateTime < search.MinDateTime)
        //                {
        //                    ModelState.AddModelError("DateLowerBound", "Invalid Date Range");
        //                    return View(search);
        //                }

        //                var query = _context.Albums.Join(_context.Artists,
        //                    album => album.ArtistId,
        //                    artist => artist.ArtistID,
        //                    (album, artist) => new
        //                    {
        //                        Id = album.AlbumId,
        //                        Artist = album.Artist,
        //                        Title = album.AlbumTitle,
        //                        ArtistId = artist.ArtistID,
        //                        ReleaseDate = album.ReleaseDate,
        //                        AvgScore = album.AvgRate,
        //                        Genre = album.Genre,
        //                        PageViewsAlbum = album.PageViews,
        //                        PageViewsArtist = artist.PageViews,
        //                        OriginCountry = artist.BirthPlace,
        //                        AvgScoreArtist = artist.AvgRate
        //                    });


        //                var results = (from element in query
        //                               where element.Title.Contains(search.PrimaryKey)
        //                                     && element.Artist.Contains(artistName) //#Critical Difference! (contain sides was opposite) 
        //                                     && search.ScoreLowerBound <= element.AvgScore
        //                                     && element.AvgScore <= search.ScoreUpperBound
        //                                     && element.Genre.Contains(genre)
        //                                     && search.DateLowerBound <= element.ReleaseDate
        //                                     && element.ReleaseDate <= search.DateUpperBound
        //                               select new
        //                               {
        //                                   Id = element.Id,
        //                                   Artist = element.Artist,
        //                                   Title = element.Title,
        //                                   ReleaseDate = element.ReleaseDate,
        //                                   Genre = element.Genre,
        //                                   PageViews = element.PageViewsAlbum,
        //                                   AvgScore = element.AvgScore,
        //                                   ArtistId = element.ArtistId
        //                               }).Distinct().ToList();


        //                var resultsAsAlbums = new List<Album>();
        //                results.ForEach(anon =>
        //                {
        //                    var a = new Album
        //                    {
        //                        Id = anon.Id,
        //                        Artist = anon.Artist,
        //                        Title = anon.Title,
        //                        ReleaseDate = anon.ReleaseDate,
        //                        Genre = anon.Genre,
        //                        PageViews = anon.PageViews,
        //                        AvgScore = anon.AvgScore,
        //                        ArtistId = anon.ArtistId
        //                    };
        //                    resultsAsAlbums.Add(a);
        //                });

        //                TempData["results"] = resultsAsAlbums;
        //                break;

        //            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //            case "Artist":
        //                if (search.primaryName != null)
        //                {
        //                    var artist = db.Artists.FirstOrDefault(a => a.Name == search.primaryName);

        //                    if (artist != null)
        //                    {
        //                        return RedirectToAction("Details", "Artist", new { id = artist.Id });
        //                    }

        //                }

        //                search.ScoreLowerBound = search.ScoreLowerBound == null ? 0 : search.ScoreLowerBound;
        //                search.ScoreUpperBound = search.ScoreUpperBound == null ? 10 : search.ScoreUpperBound;

        //                if (search.ScoreLowerBound > search.ScoreUpperBound)
        //                {
        //                    ModelState.AddModelError("ScoreLowerBound", "Invalid Range");
        //                    return View(search);
        //                }

        //                search.DateLowerBound = search.DateLowerBound == null ? DateTime.MinValue : search.DateLowerBound;
        //                search.DateUpperBound = search.DateUpperBound == null ? DateTime.Now : search.DateUpperBound;

        //                if (search.DateUpperBound < search.DateLowerBound)
        //                {
        //                    ModelState.AddModelError("DateLowerBound", "Invalid Date Range");
        //                    return View(search);
        //                }

        //                var originCountry = search.GenreCountry != null ? search.GenreCountry : ""; //#name changed
        //                var name = search.primaryName == null ? "" : search.primaryName;

        //                var artistQuery = db.Albums.Join(db.Artists,
        //                    album => album.ArtistId,
        //                    artist => artist.Id,
        //                    (album, artist) => new
        //                    {
        //                        //#different names
        //                        AlbumId = album.Id, //
        //                        Name = album.Artist, //
        //                        AlbumTitle = album.Title, //
        //                        ArtistId = artist.Id,
        //                        ReleaseDate = album.ReleaseDate,
        //                        AlbumAvgScore = album.AvgScore, //
        //                        Genre = album.Genre,
        //                        PageViewsAlbum = album.PageViews,
        //                        PageViewsArtist = artist.PageViews,
        //                        OriginCountry = artist.OriginCountry,
        //                        AvgScoreArtist = artist.AvgScore
        //                    });


        //                var artistResults = (from element in artistQuery
        //                                     where
        //                                         element.Name.Contains(name)
        //                                         && search.ScoreLowerBound <= element.AvgScoreArtist
        //                                         && element.AvgScoreArtist <= search.ScoreUpperBound
        //                                         && element.OriginCountry.Contains(originCountry)
        //                                         && search.DateLowerBound <= element.ReleaseDate
        //                                         && element.ReleaseDate <= search.DateUpperBound

        //                                     select new
        //                                     {
        //                                         //#different items were selected => the ones that matches the Artitst's model
        //                                         Id = element.ArtistId,
        //                                         Name = element.Name,
        //                                         OriginCountry = element.OriginCountry,
        //                                         PageViews = element.PageViewsArtist,
        //                                         AvgScore = element.AvgScoreArtist
        //                                     }).Distinct().ToList();


        //                var resultsAsArtist = new List<Artist>();
        //                artistResults.ForEach(anon =>
        //                {
        //                    var a = new Artist
        //                    {
        //                        Id = anon.Id,
        //                        Name = anon.Name,
        //                        OriginCountry = anon.OriginCountry,
        //                        PageViews = anon.PageViews,
        //                        AvgScore = anon.AvgScore

        //                    };
        //                    resultsAsArtist.Add(a);
        //                });

        //                TempData["results2"] = resultsAsArtist;
        //                break;

        //            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //            case "Review":

        //                if (search.primaryName != null)
        //                {
        //                    var album = db.Albums.Where(a => a.Title == search.primaryName).FirstOrDefault();
        //                    if (album != null)
        //                    {
        //                        var reviews = db.Reviews.Where(r => r.AlbumId == album.Id).ToList();
        //                        TempData["results3"] = reviews;
        //                        return RedirectToAction("SearchResult", search.type);
        //                    }

        //                    //return RedirectToAction("GetReviewsByAlbum", "Review", new { id = album.Id });
        //                }


        //                if (search.secondaryName != null)
        //                {
        //                    var writtenBy___ = db.Reviews.Where(usr => usr.Username == search.secondaryName)
        //                        .FirstOrDefault();
        //                    if (writtenBy___ == null)
        //                    {
        //                        ModelState.AddModelError("secondaryName",
        //                            "You cannot search for an user name that does not exist");
        //                        return View(search);
        //                    }

        //                }

        //                search.ScoreLowerBound = search.ScoreLowerBound == null ? 0 : search.ScoreLowerBound;
        //                search.ScoreUpperBound = search.ScoreUpperBound == null ? 10 : search.ScoreUpperBound;

        //                if (search.ScoreLowerBound > search.ScoreUpperBound)
        //                {
        //                    ModelState.AddModelError("ScoreLowerBound", "Invalid Range");
        //                    return View(search);
        //                }

        //                search.DateLowerBound = search.DateLowerBound == null ? DateTime.MinValue : search.DateLowerBound;
        //                search.DateUpperBound = search.DateUpperBound == null ? DateTime.Now : search.DateUpperBound;

        //                if (search.DateUpperBound < search.DateLowerBound)
        //                {
        //                    ModelState.AddModelError("DateLowerBound", "Invalid Date Range");
        //                    return View(search);
        //                }

        //                //search.primaryName = search.primaryName == null ? "" : search.primaryName;
        //                var writtenBy = search.secondaryName != null ? search.secondaryName : ""; //#name changed
        //                var albumTitle = search.primaryName == null ? "" : search.primaryName;

        //                var reviewQuery = db.Reviews.GroupBy(review => review.Username);


        //                var reviewResults = new List<Review>();
        //                reviewQuery.Where(element => element.Key.Contains(writtenBy)).ForEach(group =>
        //                {
        //                    var relavent = group.Where(

        //                        element =>
        //                            element.AlbumTitle.Contains(albumTitle)
        //                            && search.ScoreLowerBound <= element.Score
        //                            && element.Score <= search.ScoreUpperBound
        //                            && search.DateLowerBound <= element.ReviewCreationTime
        //                            && element.ReviewCreationTime <= search.DateUpperBound);


        //                    relavent.ForEach(entity =>
        //                    {
        //                        reviewResults.Add(new Review
        //                        {
        //                            ReviewCreationTime = entity.ReviewCreationTime,
        //                            Score = entity.Score,
        //                            Id = entity.Id,
        //                            AlbumTitle = entity.AlbumTitle,
        //                            AlbumId = entity.AlbumId,
        //                            UserId = entity.UserId,
        //                            Username = entity.Username,
        //                            Text = entity.Text
        //                        });
        //                    });
        //                });

        //                TempData["results3"] = reviewResults;
        //                break;

        //            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //            default:
        //                break;
        //        }

        //        db.SaveChanges();
        //        return RedirectToAction("SearchResult", search.type);
        //    }
        //    return View(search);
        //}





        //// GET: Searches
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.Search.ToListAsync());
        //}

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
