using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecordReviews.Models;

namespace RecordReviews.Data
{
    public class AlbumContext:DbContext
    {
        public AlbumContext(DbContextOptions<AlbumContext> options) : base(options)
        {
        }

        public DbSet<Album> Albums { get; set; }
    }
}
