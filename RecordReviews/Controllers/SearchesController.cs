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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Index(
            [Bind("type,PrimaryKey,SecondaryKey,MinRate,MaxRate,Genre,Country,MaxDateTime,MinDateTime")]
            Search search)
        {
            if (ModelState.IsValid)
            {
                ViewBag.Type = search.type;
                string albumName; 
                string genre; 
                string artistName;
                string birthPlace;
                var minRate = search.MinRate == null ? 0.0 : (double)search.MinRate;
                var maxRate = search.MaxRate == null ? 10.0 : (double)search.MaxRate;
                var minDate = (DateTime)(search.MinDateTime == null ? DateTime.MinValue : search.MinDateTime);
                var maxDate = (DateTime)(search.MaxDateTime == null ? DateTime.Now : search.MaxDateTime);

                if (minRate > 10 || minRate < 0)
                {
                    ModelState.AddModelError("MinRate", "Invalid Rate");
                    return View(search);
                }

                if (maxRate > 10 || maxRate < 0)
                {
                    ModelState.AddModelError("MaxRate", "Invalid Rate");
                    return View(search);
                }

                if (minRate > maxRate)
                {
                    ModelState.AddModelError("MinRate", "Invalid Rate Range");
                    return View(search);
                }
                
                if (search.MaxDateTime < search.MinDateTime)
                {
                    ModelState.AddModelError("MinDateTime", "Invalid Date Range");
                    return View(search);
                }

                switch (search.type)
                {
                    case "Album":
                        
                        albumName = search.PrimaryKey == null ? "" : search.PrimaryKey.ToLower();
                        genre = search.Genre == null ? "" : search.Genre.ToLower();
                        artistName = search.SecondaryKey == null ? "" : search.SecondaryKey.ToLower();
                        
                        var albumQuery = _context.Albums
                            .Where(element => 
                                element.AlbumTitle.ToLower().Contains(albumName) &
                                element.Artist.ArtistName.ToLower().Contains(artistName) &
                                maxRate >= element.AvgRate & minRate <= element.AvgRate &
                                element.Genre.ToLower().Contains(genre) &
                                maxDate >= element.ReleaseDate &
                                minDate <= element.ReleaseDate)
                            .Select(a => a).ToList();

                        ViewBag.Results = albumQuery;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    case "Artist":
                        
                        artistName = search.PrimaryKey == null ? "" : search.PrimaryKey.ToLower();
                        genre = search.Genre == null ? "" : search.Genre.ToLower();
                        birthPlace = search.Country == null ? "" : search.Country.ToLower();

                        var artistQuery = _context.Artists
                            .Include(a => a.Albums)
                            .Where(element => 
                                element.ArtistName.ToLower().Contains(artistName) &
                                maxRate >= element.AvgRate & 
                                minRate <= element.AvgRate &
                                element.Genre.ToLower().Contains(genre) &
                                element.BirthPlace.ToLower().Contains(birthPlace))
                            .Select(a => a).ToList();

                        List<Artist> artists = new List<Artist>();
                        foreach (var artist in artistQuery)
                        {
                            if (artist.FindLastAlbum() <= maxDate || artist.FindEarliestAlbum() >= minDate)
                            {
                                artists.Add(artist);
                            }
                        }

                        ViewBag.Results = artists;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    case "Review":
                        
                        albumName = search.PrimaryKey == null ? "" : search.PrimaryKey.ToLower();
                        artistName = search.SecondaryKey == null ? "" : search.SecondaryKey.ToLower();
                        
                        var reviewQuery = _context.Reviews.Include(r =>r.Album)
                            .ThenInclude(a=>a.Artist)
                            .Where(element => 
                                element.Album.Artist.ArtistName.ToLower().Contains(artistName) &
                                element.Album.AlbumTitle.ToLower().Contains(albumName) &
                                maxDate >= element.CreationTime &
                                minDate <= element.CreationTime &
                                maxRate >= element.Rate &
                                minRate <= element.Rate)
                            .Select(a => a).ToList();
                        
                        ViewBag.Results = reviewQuery;
                        break;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    default:
                        break;
                }

                _context.SaveChanges();
            }

            return View(search);
        }
    }
}


      