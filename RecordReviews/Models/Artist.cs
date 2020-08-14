using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecordReviews.Models
{
    public class Artist
    {
        public Artist()
        {
            AvgRate = 0.0;
            PageViews = 0;
        }

        public Artist(string name, string birthPlace, string genre)
        {
            Name = name;
            BirthPlace = birthPlace;
            Genre = genre;
        }

        [Key] public int Id { get; set; }

        [DisplayName("Artist's Name")]
        [Required(ErrorMessage = "Artist's Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Artist's Birth Place is Required")]
        [DisplayName("Birth Place")]
        public string BirthPlace { get; set; }

        [DisplayName("Average Rate")] public double? AvgRate { get; set; }

        [DisplayName("Page Views")] public int? PageViews { get; set; }

        [Required(ErrorMessage = "Genre is Required")]
        [DisplayName("Genre")]
        public string Genre { get; set; }
    }
}