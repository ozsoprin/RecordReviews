using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Models;

namespace RecordReviews.Data
{
    public class ArtistContext : DbContext
    {
        public ArtistContext(DbContextOptions<ArtistContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
    }
}
