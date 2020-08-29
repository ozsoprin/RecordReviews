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
            ViewBag.Top10Artists = _context.Artists.OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            ViewBag.Top10Albums = _context.Albums.OrderByDescending(a=>a.AvgRate).Take(10).ToList();
            ViewBag.Top10Users = _context.Users.Take(10).ToList();
            return View();
        }
    }
}
