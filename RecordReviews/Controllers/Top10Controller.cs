using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
            ViewBag.Top10Artists = _context.Artists.OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            ViewBag.Top10Albums = _context.Albums.OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            var topUsers= _context.Reviews.GroupBy(r => r.User.Id)
                .Select(r => new { UserId = r.Key, Count = r.Count() })
                .OrderByDescending(x => x.Count).Take(10).ToList();
            var Top10UserList = new List<IdentityUser>();
            foreach (var userPair in topUsers)
            {
                var user = _context.Users.SingleOrDefault(u => u.Id == userPair.UserId);
                Top10UserList.Add(user);
            }

            ViewBag.Top10Users = Top10UserList;
            return View();
        }
    }
}
