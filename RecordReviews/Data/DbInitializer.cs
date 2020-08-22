using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RecordReviews.Models;

namespace RecordReviews.Data
{
    public static class DbInitializer
    {
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
                new IdentityUser {UserName = "PennyWise",Email = "PennyW@IT.com"},
                new IdentityUser {UserName = "Chucky",Email = "Chucky@ChildsPlay.com"},
                new IdentityUser {UserName = "Michael Myers",Email = "Mike@Halloween.com"},
                new IdentityUser {UserName = "Freddy Krueger",Email = "Freddy@NightmareOnElmStreet.com"},
                new IdentityUser {UserName = "Jason Voorhees",Email = "Jason@Fridaythe13th.com"},
                new IdentityUser {UserName = "The Monster",Email = "Monster@Frankenstein.com"},
            };
            foreach (var user in users)
            {
                context.Users.Add(user);
            }

            context.SaveChanges();

            var reviews = new[]
            {
                new Review {Comment = "Makes me clam when I want to kill someone", CreationTime = DateTime.Parse("2020-05-10"), Rate = 8, User = context.Users.SingleOrDefault(u=>u.UserName == "Michael Myers"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Chances Are")},
                new Review {Comment = "Helps to show my emotions", CreationTime = DateTime.Parse("2020-05-10"), Rate = 7, User = context.Users.SingleOrDefault(u=>u.UserName == "Michael Myers"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "25")},
                new Review {Comment = "Old stuff are always better", CreationTime = DateTime.Parse("2020-05-10"), Rate = 10, User = context.Users.SingleOrDefault(u=>u.UserName == "Michael Myers"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Unplugged")},
                new Review {Comment = "Makes me dance like a kid", CreationTime = DateTime.Parse("2020-07-01"), Rate = 9, User = context.Users.SingleOrDefault(u=>u.UserName == "PennyWise"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Graduation")},
                new Review {Comment = "It's just ok i guess", CreationTime = DateTime.Parse("2020-07-01"), Rate = 5, User = context.Users.SingleOrDefault(u=>u.UserName == "PennyWise"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "This Is Acting")},
                new Review {Comment = "Great hear on friday night", CreationTime = DateTime.Parse("2020-03-13"), Rate = 10, User = context.Users.SingleOrDefault(u=>u.UserName == "Jason Voorhees"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "American Gangster")},
                new Review {Comment = "Pure garbage, I liked the he better before hearing this one", CreationTime = DateTime.Parse("2020-03-13"), Rate = 2, User = context.Users.SingleOrDefault(u=>u.UserName == "Jason Voorhees"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "25")},
                new Review {Comment = "Love Rap albums, this is one of my favorites", CreationTime = DateTime.Parse("2020-03-13"), Rate = 9, User = context.Users.SingleOrDefault(u=>u.UserName == "Jason Voorhees"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Graduation")},
                new Review {Comment = "My dad told me to listen...It's nice but I still prefer Drake", CreationTime = DateTime.Parse("2020-04-18"), Rate = 6, User = context.Users.SingleOrDefault(u=>u.UserName == "Chucky"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Unplugged")},
                new Review {Comment = "Her voice makes me cry sometimes", CreationTime = DateTime.Parse("2020-04-18"), Rate = 7, User = context.Users.SingleOrDefault(u=>u.UserName == "Chucky"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "This Is Acting")},
                new Review {Comment = "Love Jay, one of the best in the game", CreationTime = DateTime.Parse("2020-04-18"), Rate = 9, User = context.Users.SingleOrDefault(u=>u.UserName == "Chucky"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "American Gangster")},
                new Review {Comment = "The best song in this one is 'Pray'", CreationTime = DateTime.Parse("2020-06-04"), Rate = 9, User = context.Users.SingleOrDefault(u=>u.UserName == "Freddy Krueger"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "American Gangster")},
                new Review {Comment = "Just ok, didn't feel it this time", CreationTime = DateTime.Parse("2020-06-04"), Rate = 4, User = context.Users.SingleOrDefault(u=>u.UserName == "Freddy Krueger"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Chances Are")},
                new Review {Comment = "I am cleaning my nails to this album", CreationTime = DateTime.Parse("2020-06-04"), Rate = 7, User = context.Users.SingleOrDefault(u=>u.UserName == "Freddy Krueger"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Unplugged")},
                new Review {Comment = "Me no like", CreationTime = DateTime.Parse("2020-02-25"), Rate = 3, User = context.Users.SingleOrDefault(u=>u.UserName == "The Monster"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "Graduation")},
                new Review {Comment = "Good Sia very much", CreationTime = DateTime.Parse("2020-02-25"), Rate = 10, User = context.Users.SingleOrDefault(u=>u.UserName == "The Monster"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "This Is Acting")},
                new Review {Comment = "Adele is OK", CreationTime = DateTime.Parse("2020-02-25"), Rate = 7, User = context.Users.SingleOrDefault(u=>u.UserName == "The Monster"), Album = context.Albums.SingleOrDefault(a => a.AlbumTitle == "25")},
            };
            foreach (var review in reviews)
            {
                context.Reviews.Add(review);
                review.UpdateRateAfterAdding();
            }
            

            context.SaveChanges();

        }
    }
}
