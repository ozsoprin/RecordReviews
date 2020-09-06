using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class Trends : Controller
    {
        private readonly ApplicationDbContext _context;

        public Trends(ApplicationDbContext context)
        {
            _context = context;
        }
        [AllowAnonymous]
        public ActionResult Index()
        {
            ViewBag.ReviewStatistic = _context.Reviews.GroupBy(r => r.CreationTime.Month)
                .Select(r => new { Month = r.Key, Count = r.Count() }).OrderBy(x => x.Month).ToList();

             var genreStatistic = _context.Reviews.Include(r => r.Album).ThenInclude(a => a.Artist).Select(r => r)
                .Join(_context.Albums, review => review.AlbumId, album => album.AlbumId, (review, album) => album)
                .Join(_context.Artists, album => album.ArtistId, artist => artist.ArtistID, (album, artist) => artist)
                .GroupBy(a => a.Genre)
                .Select(r => new { Genre = r.Key, Count = r.Count() })
                .OrderByDescending(x => x.Count);
            ViewBag.GenreStatistic = genreStatistic.ToList();
            ViewBag.TotalReviewNumber = _context.Reviews.Select(r => r).Count();
            ViewBag.MostReviewedGenreAlbums = _context.Albums.Where(a => a.Genre == genreStatistic.Take(1).SingleOrDefault().Genre).OrderByDescending(a => a.AvgRate).Take(2).ToList();

            return View();
        }
    }
}
