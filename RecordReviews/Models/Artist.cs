using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RecordReviews.Models
{
    public class Artist
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Artist's Name")]
        [Required(ErrorMessage = "Artist's Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Artist's Birth Place is Required")]
        [DisplayName("Birth Place")]
        public string BirthPlace { get; set; }

        [DisplayName("Average Rate")]
        public double AvgRate { get; set; }
        
        [DisplayName("Page Views")]
        public int PageViews { get; set; }

        [Required(ErrorMessage = "Genre is Required")]
        [DisplayName("Genre")]
        public string Genre { get; set; }

    }
}
