using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Models;

namespace RecordReviews.Data
{
    public class ReviewContext:DbContext
    {
        public ReviewContext(DbContextOptions<ReviewContext> options) : base(options)
        {
        }

        public DbSet<Review> Reviews { get; set; }
    }
}
