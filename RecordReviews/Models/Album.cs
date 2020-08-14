﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace RecordReviews.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("Album Title")]
        [Required(ErrorMessage = "Album Title is Required")]
        public string Title { get; set; }

        [DisplayName("Artist's Name")]
        [Required(ErrorMessage = "Artist's Name is Required")]
        public string Artist { get; set; }

        public int ArtistId { get; set; }

        [DisplayName("Release Date")]
        [Required(ErrorMessage = "Release Date is Required")]
        [DataType(DataType.DateTime, ErrorMessage = "Please Enter a Valid Date")]
        public DateTime ReleaseDate { get; set; }


        [Required(ErrorMessage = "Genre is Required")]
        public string Genre { get; set; }

        [DisplayName("Average Rate")]
        public double AvgRate { get; set; }

        [DisplayName("Page Views")]
        public int PageViews { get; set; }
    }
}
