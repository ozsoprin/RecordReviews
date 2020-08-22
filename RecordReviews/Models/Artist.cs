using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecordReviews.Models
{
    public class Artist
    {
        private Artist artist;
        private string v;

        public Artist()
        {
            PageViews = 0;
            AvgRate = 0.0;
        }

        public Artist(string artistName, string birthPlace, string genre)
        {
            this.ArtistName = artistName;
            this.BirthPlace = birthPlace;
            Genre = genre;
        }

        [Key]
        public int ArtistID { get; set; }

        [DisplayName("Artist's Name")]
        [Required(ErrorMessage = "Artist's Name is Required")]
        public string ArtistName { get; set; }

        [DisplayName("Birth Place")]
        [Required(ErrorMessage = "Artist's Birth Place is Required")]
        public string BirthPlace { get; set; }

        [DisplayName("Genre")]
        [Required(ErrorMessage = "Genre is Required")]
        public string Genre { get; set; }

        public ICollection<Album> Albums { get; set; }

        [DisplayName("Average Rate")]
        public double? AvgRate { get; set; }

        [DisplayName("Page Views")]
        public double? PageViews { get; set; }
    }
}
