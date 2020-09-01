using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public IActionResult Index()
        {
            var aList = _context.Artists.ToList();
            var countryList = new HashSet<string>();
            foreach (var artist in aList)
            {
                countryList.Add(artist.BirthPlace);
            }

            ViewBag.ReviewStatistic = _context.Reviews.GroupBy(r => r.CreationTime.Month)
                .Select(r => new {Month = r.Key, Count = r.Count()}).OrderBy(x => x.Month).ToList();
            return View(countryList.ToArray());
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
