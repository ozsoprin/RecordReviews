using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class Top10Controller : Controller
    {
        private readonly ApplicationDbContext _context;

        public Top10Controller(ApplicationDbContext context)
        {
            _context = context;
        }
        public ActionResult Index()
        {
            ViewData["Top10Artists"] = _context.Artists.Include(a=>a.ArtistID).Include(a => a.ArtistName).Include(a => a.PageViews).Include(a => a.AvgRate).OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            ViewData["Top10Albums"] = _context.Albums.Include(a=>a.AlbumTitle).Include(a => a.AlbumId).Include(a => a.ArtistName).Include(a => a.ArtistId).Include(a => a.PageViews).Include(a => a.AvgRate).OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            ViewData["Top10Users"] = _context.Users.Take(10).ToList();
            return View();
        }
    }
}
