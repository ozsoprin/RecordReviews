using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RecordReviews.Models
{
    public class Review
    {
        [Key] public int ReviewId { get; set; }

        public IdentityUser User { get; set; }
        
        public int AlbumId { get; set; }

        public Album Album { get; set; }

        [Required(ErrorMessage = "Must Fill a Comment")]
        [DataType(DataType.MultilineText)]
        public string Comment { get; set; }

        [Required(ErrorMessage = "Please Rate")]
        [Range(0, 10, ErrorMessage = "Please Rate This Album On a Scale Of 1-10")]
        public int Rate { get; set; }

        [DisplayName("Created on")] public DateTime CreationTime { get; set; }

        public void UpdateRateAfterAdding()
        {
            var sum = 0.0;
            var cnt = 0;
            foreach (var review in Album.Reviews)
            {
                sum += review.Rate;
                cnt++;
            }

            Album.AvgRate = Math.Round(sum / cnt, 2);
            sum = 0.0;
            cnt = 0;
            foreach (var album in Album.Artist.Albums)
            {
                if (album.AvgRate != null) sum += (double)album.AvgRate;
                cnt++;
            }

            Album.Artist.AvgRate = Math.Round(sum / cnt, 2);
        }
    }
}
