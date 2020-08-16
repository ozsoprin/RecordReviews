using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        // GET: Top10Controller
        public ActionResult Index()
        {
            //change to - decending
            ViewBag.topArtists = _context.Artists.OrderBy(_ => _.AvgRate).Take(10).ToList();
            ViewBag.topAlbums = _context.Albums.OrderBy(_ => _.AvgRate).Take(10).ToList();
            ViewBag.topUsers = (from a in _context.Users
                    join b in _context.Reviews on a.Id equals b.UserId
                    group a by new { UserID = a.Id, UserEmail = a.Email, ReviewID = b.Id }
                    into c
                    select new { UserID = c.Key.UserID, UserEmail = c.Key.UserEmail, Assigned = c.Count() })
                .AsEnumerable()
                .OrderBy(_ => _.Assigned).Take(10).ToList();
            return View();
        }
    }
}
