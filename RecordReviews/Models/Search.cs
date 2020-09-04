using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RecordReviews.Models
{
    public class Search
    {
        [Key] public string type { get; set; }

        public string PrimaryKey { get; set; }
        public string SecondaryKey { get; set; } // for album (the artist); for review (username); 

        [Range(0, 10)] public double? MinRate { get; set; }

        [Range(0, 10)] public double? MaxRate { get; set; }

        public string Genre { get; set; }
        public string Country { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Please Enter a Valid Date")]
        public DateTime? MaxDateTime { get; set; }
        [DataType(DataType.Date, ErrorMessage = "Please Enter a Valid Date")]
        public DateTime? MinDateTime { get; set; }
    }
}
