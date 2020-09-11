using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RecordReviews.Data;
using RecordReviews.Models;

namespace RecordReviews.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Top10Artists = _context.Artists.OrderByDescending(a => a.AvgRate).Take(10).ToList();
            ViewBag.Top10Albums = _context.Albums.OrderByDescending(a => a.AvgRate).Take(10).ToList();
            var topUsers = _context.Reviews.GroupBy(r => r.User.Id)
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
