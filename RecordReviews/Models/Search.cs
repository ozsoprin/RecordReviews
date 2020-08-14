using System;
using System.ComponentModel.DataAnnotations;

namespace RecordReviews.Models
{
    public class Search
    {
        [Key] public string type { get; set; }

        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; } // for album (the artist); for review (username); 

        [Range(0, 10)] public double? MinRate { get; set; }

        [Range(0, 10)] public double? MaxRate { get; set; }

        public string GenreCountry { get; set; }

        public DateTime? MaxDateTime { get; set; }
        public DateTime? MinDateTime { get; set; }
    }
}