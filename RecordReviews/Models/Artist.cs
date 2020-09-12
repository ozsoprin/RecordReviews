using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecordReviews.Models
{
    public enum ArtistStatus
    {
        Submitted,
        Approved,
        Rejected
    }

    public class Artist
    {
        public Artist()
        {
            Status = ArtistStatus.Submitted;
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

        public void UpdateArtistRate()
        {
            var sum = 0.0;
            var count = 0;
            foreach (var album in Albums)
            {
                if(album.Reviews == null) continue;
                album.UpdateAlbumRate();
                if (album.AvgRate != null)
                {
                    sum += (double) album.AvgRate;
                    count++;
                }
            }

            AvgRate = count !=0 ? Math.Round(sum / count, 2) : 0;
        }

        public DateTime FindEarliestAlbum()
        {
            var earliestAlbumDateTime = DateTime.MaxValue;
            
            foreach (var album in Albums)
            {
                if (album.ReleaseDate < earliestAlbumDateTime)
                {
                    earliestAlbumDateTime = album.ReleaseDate;
                }
            }

            return earliestAlbumDateTime;
        }

        public DateTime FindLastAlbum()
        {
            var lastAlbumDateTime = DateTime.MinValue;

            foreach (var album in Albums)
            {
                if (album.ReleaseDate > lastAlbumDateTime)
                {
                    lastAlbumDateTime = album.ReleaseDate;
                }
            }

            return lastAlbumDateTime;
        }


        public ArtistStatus Status { get; set; }

        public string OwnerID { get; set; }
    }
}
