using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RecordReviews.Models;

namespace RecordReviews.Data
{
    public static class DbInitializer
    {
        public static class ReviewGenerator
        {
            public static void GenerateBadReview(ApplicationDbContext context,Album album, IdentityUser user)
            {
                var rand = new Random();
                var dateGen = new RandomDateTime(album.ReleaseDate);
                var date = dateGen.Next();
                var rate = (rand.Next() % 5) + 1;
                string comment = "It's OK, nothing more, nothing less";
                switch (rate)
                {
                    case 1:
                        comment = "Pure garbage! I regret listening to it";
                        break;
                    case 2:
                        comment = "Bad album! Didn't feel it this time";
                        break;
                    case 3:
                        comment = "Won't listen to it again...";
                        break;
                    case 4:
                        comment = "Heard worst than that one";
                        break;
                }
                var review = new Review { Comment = comment, Rate = rate, Album = album, User = user, CreationTime = date};
                context.Reviews.Add(review);
                review.UpdateRate();
            }

            public static void GenerateGoodReview(ApplicationDbContext context, Album album, IdentityUser user)
            {
                var rand = new Random();
                var dateGen = new RandomDateTime(album.ReleaseDate);
                var date = dateGen.Next();
                var rate = (rand.Next() % 5) + 6;
                string comment = "Best Album Ever!";
                switch (rate)
                {
                    case 6:
                        comment = "Nice one, good listing";
                        break;
                    case 7:
                        comment = $"This album is totally {user.UserName} approved!";
                        break;
                    case 8:
                        comment = "Loved it! Can't wait to hear it once again";
                        break;
                    case 9:
                        comment = "Great album! Really recommended";
                        break;
                }
                var review = new Review { Comment = comment, Rate = rate, Album = album, User = user, CreationTime = date };
                context.Reviews.Add(review);
                review.UpdateRate();
            }
        }

        public class RandomDateTime
        {
            DateTime start;
            Random gen;
            int range;

            public RandomDateTime(DateTime dateTime)
            {
                start = DateTime.Parse("2020-01-01") < dateTime ? dateTime : DateTime.Parse("2020-01-01");
                gen = new Random();
                range = (DateTime.Today - start).Days;
            }

            public DateTime Next()
            {
                return start.AddDays(gen.Next(range)).AddHours(gen.Next(0, 24)).AddMinutes(gen.Next(0, 60)).AddSeconds(gen.Next(0, 60));
            }
        }
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Artists.Any())
            {
                return; //DB is full already
            }

            var artists = new []
            {
                new Artist {ArtistName = "Kanye West",BirthPlace = "United States", Genre = "Rap"},
                new Artist {ArtistName = "Jay Z",BirthPlace = "United States", Genre = "Rap"},
                new Artist {ArtistName = "Eric Clapton",BirthPlace = "United Kingdom", Genre = "Rock / Blues"},
                new Artist {ArtistName = "Adele",BirthPlace = "United Kingdom", Genre = "Soul / Pop"},
                new Artist {ArtistName = "Sia",BirthPlace = "Australia", Genre = "Pop"},
                new Artist {ArtistName = "Bob Marley",BirthPlace = "Jamaica", Genre = "Reggae"}
            };
            foreach (var artist in artists)
            {
                context.Artists.Add(artist);
            }

            context.SaveChanges();
            var albums = new []
            {
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "Graduation",ReleaseDate = DateTime.Parse("2007-09-11")},
                new Album {ArtistName = "Jay Z",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jay Z"), Genre = "Rap",AlbumTitle = "American Gangster",ReleaseDate = DateTime.Parse("2007-11-6")},
                new Album {ArtistName = "Eric Clapton",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eric Clapton"), Genre = "Rock / Blues",AlbumTitle = "Unplugged",ReleaseDate = DateTime.Parse("1992-01-16")},
                new Album {ArtistName = "Adele",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Adele"), Genre = "Soul / Pop",AlbumTitle = "25",ReleaseDate = DateTime.Parse("2015-11-20")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "This Is Acting",ReleaseDate = DateTime.Parse("2016-01-29")},
                new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Chances Are",ReleaseDate = DateTime.Parse("1981-10-01")},

            };
            foreach (var album in albums)
            {
                context.Albums.Add(album);
            }

            context.SaveChanges();

            var users = new[]
            {
                new IdentityUser {UserName = "Bugs Bunny",Email = "Bugs@LooneyTunes.com"},
                new IdentityUser {UserName = "Daffy Duck",Email = "Daffy@LooneyTunes.com"},
                new IdentityUser {UserName = "Tweety",Email = "Tweety@LooneyTunes.com"},
                new IdentityUser {UserName = "Porky Pig",Email = "Porkey@LooneyTunes.com"},
                new IdentityUser {UserName = "Foghorn Leghorn",Email = "Foghorn@LooneyTunes.com"},
                new IdentityUser {UserName = "Elmer Fudd",Email = "Elmer@LooneyTunes.com"},
                new IdentityUser {UserName = "Yosemite Sam",Email = "Sam@LooneyTunes.com"},
                new IdentityUser {UserName = "Sylvester",Email = "Sylvester@LooneyTunes.com"},
                new IdentityUser {UserName = "Tasmanian Devil",Email = "Tas@LooneyTunes.com"},
                new IdentityUser {UserName = "Marvin the Martian",Email = "Marvin@LooneyTunes.com"},
                new IdentityUser {UserName = "Speedy Gonzales",Email = "Speedy@LooneyTunes.com"},
                new IdentityUser {UserName = "Pepé Le Pew",Email = "Pepé@LooneyTunes.com"},
                new IdentityUser {UserName = "Gossamer",Email = "Gossamer@LooneyTunes.com"},
                new IdentityUser {UserName = "Lola Bunny",Email = "Lola@LooneyTunes.com"},
                new IdentityUser {UserName = "Wile E. Coyote",Email = "Wile@LooneyTunes.com"},
                new IdentityUser {UserName = "Road Runner",Email = "Runner@LooneyTunes.com"},
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }
            context.SaveChanges();

            var albumList = context.Albums.ToList();
            var UserList = context.Users.ToList();
            var rand = new Random();
            foreach (var album in albumList)
            {
                if (rand.Next() % 2 == 0)
                {
                    foreach (var user in UserList)
                    {
                        ReviewGenerator.GenerateBadReview(context, album, user);
                    }
                }
                else
                {
                    foreach (var user in UserList)
                    {
                        ReviewGenerator.GenerateGoodReview(context, album, user);
                    }
                }
            }
            context.SaveChanges();
        }
    }
}
