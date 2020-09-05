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
using Microsoft.AspNetCore.Authorization;

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
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(
            [Bind("type,PrimaryKey,SecondaryKey,MinRate,MaxRate,Genre,Country,MaxDateTime,MinDateTime")]
            Search search)
        {
            if (ModelState.IsValid)
            {
                switch (search.type)
                {
                    case "Album":

                        if (search.PrimaryKey != null)
                        {
                            var album = _context.Albums.Where(a => a.AlbumTitle == search.PrimaryKey).FirstOrDefault();
                            if (album != null)
                            {
                                return RedirectToAction("Details", "Albums", new {id = album.AlbumId});
                            }

                        }

                        // if (search.SecondaryKey != null)
                        // {
                        //     var artist = _context.Artists.Where(a => a.Name == search.SecondaryKey).FirstOrDefault();
                        //     // if (artist == null)
                        //     // {
                        //     //     ModelState.AddModelError("SecondaryKey",
                        //     //         "You cannot search for an artist that does not exist");
                        //     //     return View(search);
                        //     // }
                        //
                        // }

                        search.MinRate = search.MinRate == null ? 0 : search.MinRate;
                        search.MaxRate = search.MaxRate == null ? 10 : search.MaxRate;

                        if (search.MinRate > search.MaxRate)
                        {
                            ModelState.AddModelError("MinRate", "Invalid Rate Range");
                            return View(search);
                        }

                        search.PrimaryKey = search.PrimaryKey == null ? "" : search.PrimaryKey;
                        var genre = search.Genre != null ? search.Genre : "";
                        var artistName = search.SecondaryKey == null ? "" : search.SecondaryKey;

                        search.MinDateTime = search.MinDateTime == null ? DateTime.MinValue : search.MinDateTime;
                        search.MaxDateTime = search.MaxDateTime == null ? DateTime.Now : search.MaxDateTime;

                        if (search.MaxDateTime < search.MinDateTime)
                        {
                            ModelState.AddModelError("MinDateTime", "Invalid Date Range");
                            return View(search);
                        }

                        var query = _context.Albums.Join(_context.Artists,
                            album => album.ArtistId,
                            artist => artist.ArtistID,
                            (album, artist) => new
                            {
                                Id = album.AlbumId,
                                Artist = album.Artist,
                                Title = album.AlbumTitle,
                                ArtistId = artist.ArtistID,
                                ReleaseDate = album.ReleaseDate,
                                AvgRate = album.AvgRate,
                                Genre = album.Genre,
                                PageViewsAlbum = album.PageViews,
                                PageViewsArtist = artist.PageViews,
                                BirthPlace = artist.BirthPlace,
                                AvgRateArtist = artist.AvgRate
                            });


                        var results = (from element in query
                            where element.Title.Contains(search.PrimaryKey)
                                  && element.Artist.ArtistName
                                      .Contains(artistName) //#Critical Difference! (contain sides was opposite) 
                                  && search.MinRate <= element.AvgRate
                                  && element.AvgRate <= search.MaxRate
                                  && element.Genre.Contains(genre)
                                  && search.MinDateTime <= element.ReleaseDate
                                  && element.ReleaseDate <= search.MaxDateTime
                            select new
                            {
                                Id = element.Id,
                                Artist = element.Artist,
                                Title = element.Title,
                                ReleaseDate = element.ReleaseDate,
                                Genre = element.Genre,
                                PageViews = element.PageViewsAlbum,
                                AvgRate = element.AvgRate,
                                ArtistId = element.ArtistId
                            }).Distinct().ToList();


                        var resultsAsAlbums = new List<Album>();
                        results.ForEach(anon =>
                        {
                            var a = new Album
                            {
                                AlbumId = anon.Id,
                                Artist = anon.Artist,
                                AlbumTitle = anon.Title,
                                ReleaseDate = anon.ReleaseDate,
                                Genre = anon.Genre,
                                PageViews = anon.PageViews,
                                AvgRate = anon.AvgRate,
                                ArtistId = anon.ArtistId
                            };
                            resultsAsAlbums.Add(a);
                        });

                        TempData["results"] = resultsAsAlbums;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    case "Artist":
                        if (search.PrimaryKey != null)
                        {
                            var artist = _context.Artists.FirstOrDefault(a => a.ArtistName == search.PrimaryKey);

                            if (artist != null)
                            {
                                return RedirectToAction("Details", "Artists", new {id = artist.ArtistID});
                            }

                        }

                        search.MinRate = search.MinRate == null ? 0 : search.MinRate;
                        search.MaxRate = search.MaxRate == null ? 10 : search.MaxRate;

                        if (search.MinRate > search.MaxRate)
                        {
                            ModelState.AddModelError("MinRate", "Invalid Range");
                            return View(search);
                        }

                        search.MinDateTime = search.MinDateTime == null ? DateTime.MinValue : search.MinDateTime;
                        search.MaxDateTime = search.MaxDateTime == null ? DateTime.Now : search.MaxDateTime;

                        if (search.MaxDateTime < search.MinDateTime)
                        {
                            ModelState.AddModelError("MinDateTime", "Invalid Date Range");
                            return View(search);
                        }

                        var BirthPlace = search.Country != null ? search.Country : ""; //#name changed
                        var name = search.PrimaryKey == null ? "" : search.PrimaryKey;

                        var artistQuery = _context.Albums.Join(_context.Artists,
                            album => album.ArtistId,
                            artist => artist.ArtistID,
                            (album, artist) => new
                            {
                                //#different names
                                AlbumId = album.AlbumId, //
                                Name = album.ArtistName, //
                                AlbumTitle = album.AlbumTitle, //
                                ArtistId = artist.ArtistID,
                                ReleaseDate = album.ReleaseDate,
                                AlbumAvgRate = album.AvgRate, //
                                Genre = album.Genre,
                                PageViewsAlbum = album.PageViews,
                                PageViewsArtist = artist.PageViews,
                                BirthPlace = artist.BirthPlace,
                                AvgRateArtist = artist.AvgRate
                            });


                        var artistResults = (from element in artistQuery
                            where
                                element.Name.Contains(name)
                                && search.MinRate <= element.AvgRateArtist
                                && element.AvgRateArtist <= search.MaxRate
                                && element.BirthPlace.Contains(BirthPlace)
                                && search.MinDateTime <= element.ReleaseDate
                                && element.ReleaseDate <= search.MaxDateTime

                            select new
                            {
                                //#different items were selected => the ones that matches the Artitst's model
                                Id = element.ArtistId,
                                Name = element.Name,
                                BirthPlace = element.BirthPlace,
                                PageViews = element.PageViewsArtist,
                                AvgRate = element.AvgRateArtist
                            }).Distinct().ToList();


                        var resultsAsArtist = new List<Artist>();
                        artistResults.ForEach(anon =>
                        {
                            var a = new Artist
                            {
                                ArtistID = anon.Id,
                                ArtistName = anon.Name,
                                BirthPlace = anon.BirthPlace,
                                PageViews = anon.PageViews,
                                AvgRate = anon.AvgRate

                            };
                            resultsAsArtist.Add(a);
                        });

                        TempData["results2"] = resultsAsArtist;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    case "Review":

                        if (search.PrimaryKey != null)
                        {
                            var album = _context.Albums.Where(a => a.AlbumTitle == search.PrimaryKey).FirstOrDefault();
                            if (album != null)
                            {
                                var reviews = _context.Reviews.Where(r => r.AlbumId == album.AlbumId).ToList();
                                TempData["results3"] = reviews;
                                return RedirectToAction("SearchResult", search.type);
                            }

                            //return RedirectToAction("GetReviewsByAlbum", "Review", new { id = album.Id });
                        }


                        if (search.SecondaryKey != null)
                        {
                            var writtenBy___ = _context.Reviews.Where(r => r.User.UserName == search.SecondaryKey)
                                .FirstOrDefault();
                            if (writtenBy___ == null)
                            {
                                ModelState.AddModelError("SecondaryKey",
                                    "You cannot search for an user name that does not exist");
                                return View(search);
                            }

                        }

                        search.MinRate = search.MinRate == null ? 0 : search.MinRate;
                        search.MaxRate = search.MaxRate == null ? 10 : search.MaxRate;

                        if (search.MinRate > search.MaxRate)
                        {
                            ModelState.AddModelError("MinRate", "Invalid Range");
                            return View(search);
                        }

                        search.MinDateTime = search.MinDateTime == null ? DateTime.MinValue : search.MinDateTime;
                        search.MaxDateTime = search.MaxDateTime == null ? DateTime.Now : search.MaxDateTime;

                        if (search.MaxDateTime < search.MinDateTime)
                        {
                            ModelState.AddModelError("MinDateTime", "Invalid Date Range");
                            return View(search);
                        }

                        //search.PrimaryKey = search.PrimaryKey == null ? "" : search.PrimaryKey;
                        var writtenBy = search.SecondaryKey != null ? search.SecondaryKey : ""; //#name changed
                        var albumTitle = search.PrimaryKey == null ? "" : search.PrimaryKey;

                        var reviewQuery = _context.Reviews.GroupBy(review => review.User.UserName);


                        var reviewResults = new List<Review>();
                        foreach (var group in reviewQuery.Where(element => element.Key.Contains(writtenBy)))
                        {
                            var relavent = group.Where(element =>
                                element.Album.AlbumTitle.Contains(albumTitle)
                                && search.MinRate <= element.Rate
                                && element.Rate <= search.MaxRate
                                && search.MinDateTime <= element.CreationTime
                                && element.CreationTime <= search.MaxDateTime);


                            foreach (var entity in relavent)
                            {
                                reviewResults.Add(new Review
                                {
                                    CreationTime = entity.CreationTime,
                                    Rate = entity.Rate,
                                    ReviewId = entity.ReviewId,
                                    Album = _context.Albums.FirstOrDefault(a => a.AlbumId == entity.AlbumId),
                                    AlbumId = entity.AlbumId,
                                    User = _context.Users.SingleOrDefault(u => u == entity.User),
                                    Comment = entity.Comment
                                });
                            }

                        }

                        TempData["results3"] = reviewResults;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    default:
                        break;
                }

                _context.SaveChanges();
                return RedirectToAction("SearchResult", search.type);
            }

            return View(search);
        }
    }
}


      