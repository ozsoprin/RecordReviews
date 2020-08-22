using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [DataType(DataType.DateTime, ErrorMessage = "Please Enter a Valid Date")]
        public DateTime ReleaseDate { get; set; }
        
        [Required(ErrorMessage = "Genre is Required")]
        public string Genre { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public Artist Artist { get; set; }
        
        [DisplayName("Average Rate")]
        public double? AvgRate { get; set; }
        
        [DisplayName("Page Views")]
        public int? PageViews { get; set; }
    }
}
