using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using RecordReviews.Data;

namespace RecordReviews.Models
{
    public class Album
    {
        public Album()
        {
            PageViews = 0;
            AvgRate = 0.0;
        }

        [Key] public int AlbumId { get; set; }

        [DisplayName("Album Title")]
        [Required(ErrorMessage = "Album's Title is Required")]
        public string AlbumTitle { get; set; }

        [DisplayName("Artist's Name")]
        [Required(ErrorMessage = "Artist's Name is Required")]
        public string ArtistName { get; set; }

        public int ArtistId { get; set; }

        [DisplayName("Release Date")]
        [Required(ErrorMessage = "Release Date is Required")]
        [DataType(DataType.Date, ErrorMessage = "Please Enter a Valid Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime ReleaseDate { get; set; }
        
        [Required(ErrorMessage = "Genre is Required")]
        public string Genre { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public Artist Artist { get; set; }
        
        [DisplayName("Average Rate")]
        public double? AvgRate { get; set; }
        
        [DisplayName("Page Views")]
        public int? PageViews { get; set; }

        public void UpdateAlbumRate()
        {
            var sum = 0.0;
            var count = 0;
            foreach (var review in Reviews)
            {
                sum += (double)review.Rate;
                count++;
            }

            AvgRate = count != 0 ? Math.Round(sum / count, 2) : 0;


        }

        public void DeleteAlbum(ApplicationDbContext _context)
        {
            foreach (var review in Reviews)
            {
                _context.Reviews.Remove(review);
            }
            _context.Albums.Remove(this);
        }
    }
}
