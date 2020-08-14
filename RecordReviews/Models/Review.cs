using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RecordReviews.Models
{
    public class Review
    {
        [Key] public int Id { get; set; }

        public int UserId { get; set; }

        [DisplayName("User Name")] public string Username { get; set; }

        public int AlbumId { get; set; }

        [DisplayName("Album Title")]
        [Required(ErrorMessage = "Please Enter an Album Title")]
        public string AlbumTitle { get; set; }

        [Required(ErrorMessage = "Must Fill a Comment")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Please Rate")]
        [Range(0, 10, ErrorMessage = "Please Rate This Album On a Scale Of 1-10")]
        public int Rate { get; set; }

        [DisplayName("Created on")] public DateTime CreationTime { get; set; }
    }
}