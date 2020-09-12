using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecordReviews.Models;
using RecordReviews.Authorization;

namespace RecordReviews.Data
{
    public static class DbInitializer
    {
        public static class ReviewGenerator
        {
            public static void GenerateReview(ApplicationDbContext context, Album album, IdentityUser user,int type,string adminID)
            {
                var rand = new Random();
                var dateGen = new RandomDateTime(album.ReleaseDate);
                var date = dateGen.Next();
                int rate = 0;
                switch (type)
                {
                    case 0:
                        rate = (rand.Next() % 5) + 1;
                        break;
                    case 1:
                        rate = (rand.Next() % 5) + 4;
                        break;
                    case 2:
                        rate = (rand.Next() % 4) + 7;
                        break;
                }
                string comment = "Best Album Ever!";
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
                    case 5:
                        comment = "It's OK, nothing more, nothing less";
                        break;
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
                var review = new Review { Comment = comment, Rate = rate, Album = album, User = user, CreationTime = date, AlbumId   = album.AlbumId};
                review.OwnerID = adminID;
                review.Status = ReviewStatus.Approved;
                context.Reviews.Add(review);
                album.UpdateAlbumRate();
                album.PageViews++;
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

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, "abc123ABC!", "admin@RecordReviews.com");
                await EnsureRole(serviceProvider, adminID, Constants.AdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, "abc123ABC!", "manager@RecordReviews.com");
                await EnsureRole(serviceProvider, managerID, Constants.ManagersRole);

                InitializeDB(context,adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
            string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
            string uid, string role)
        {
            IdentityResult IR = null;
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        public static void InitializeDB(ApplicationDbContext context, string adminID)
        {
            context.Database.EnsureCreated();
            if (context.Artists.Any())
            {
                return; //DB is full already
            }

            var artists = new []
            {
                new Artist {ArtistName = "Kanye West",BirthPlace = "United States", Genre = "Rap"},
                new Artist {ArtistName = "Eminem",BirthPlace = "United States", Genre = "Rap"},
                new Artist {ArtistName = "Adele",BirthPlace = "United Kingdom", Genre = "Pop"},
                new Artist {ArtistName = "Sia",BirthPlace = "Australia", Genre = "Pop"},
                new Artist {ArtistName = "Tones and I",BirthPlace = "Australia", Genre = "Pop"},
                //new Artist {ArtistName = "Bob Marley",BirthPlace = "Jamaica", Genre = "Reggae"},
                new Artist {ArtistName = "Jimi Hendrix",BirthPlace = "United States", Genre = "Rock"},
                new Artist {ArtistName = "Bon Jovi",BirthPlace = "United States", Genre = "Rock"},
                new Artist {ArtistName = "Beyoncé",BirthPlace = "United States", Genre = "Pop"},
                new Artist {ArtistName = "Taylor Swift",BirthPlace = "United States", Genre = "Pop"},
                new Artist {ArtistName = "Ed Sheeran",BirthPlace = "United Kingdom", Genre = "Pop"},
                new Artist {ArtistName = "Hozier",BirthPlace = "Ireland", Genre = "Rock"},
                new Artist {ArtistName = "Enrique Iglesias",BirthPlace = "Spain", Genre = "Pop"},
                new Artist {ArtistName = "Avicii",BirthPlace = "Sweeden", Genre = "Electronic"},
                new Artist {ArtistName = "David Guetta",BirthPlace = "France", Genre = "Electronic"},
                new Artist {ArtistName = "Calvin Harris",BirthPlace = "Scotland", Genre = "Electronic"},
                new Artist {ArtistName = "Drake",BirthPlace = "Canada", Genre = "Rap"},
                new Artist {ArtistName = "Ariana Grande",BirthPlace = "United States", Genre = "Pop"},
                new Artist {ArtistName = "Kendrick Lamar",BirthPlace = "United States", Genre = "Rap"},
                new Artist {ArtistName = "Bruno Mars",BirthPlace = "United States", Genre = "Pop"},
                new Artist {ArtistName = "Shakira",BirthPlace = "Colombia", Genre = "Pop"},
            };
            foreach (var artist in artists)
            {
                context.Artists.Add(artist);
                artist.OwnerID = adminID;
                artist.Status = ArtistStatus.Approved;
            }

            context.SaveChanges();
            var albums = new []
            {
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "Graduation",ReleaseDate = DateTime.Parse("2007-09-11")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "The College Dropout",ReleaseDate = DateTime.Parse("2004-02-10")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "Late Registration",ReleaseDate = DateTime.Parse("2005-08-30")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "My Beautiful Dark Twisted Fantasy",ReleaseDate = DateTime.Parse("2013-06-18")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "The Life of Pablo",ReleaseDate = DateTime.Parse("2016-02-14")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "Ye",ReleaseDate = DateTime.Parse("2018-06-01")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "Jesus Is King",ReleaseDate = DateTime.Parse("2019-10-25")},
                new Album {ArtistName = "Kanye West",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kanye West"), Genre = "Rap",AlbumTitle = "808s and Heartbreak",ReleaseDate = DateTime.Parse("2008-11-24")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Infinite",ReleaseDate = DateTime.Parse("1996-11-12")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "The Slim Shady LP",ReleaseDate = DateTime.Parse("1999-02-23")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "The Marshall Mathers LP",ReleaseDate = DateTime.Parse("2000-05-23")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "The Eminem Show",ReleaseDate = DateTime.Parse("2002-05-26")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Encore",ReleaseDate = DateTime.Parse("2004-11-12")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Relapse",ReleaseDate = DateTime.Parse("2009-05-15")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Recovery",ReleaseDate = DateTime.Parse("2010-06-18")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "The Marshall Mathers LP 2",ReleaseDate = DateTime.Parse("2013-11-05")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Revival",ReleaseDate = DateTime.Parse("2017-12-15")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Kamikaze",ReleaseDate = DateTime.Parse("2018-08-31")},
                new Album {ArtistName = "Eminem",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Eminem"), Genre = "Rap",AlbumTitle = "Music to Be Murdered By",ReleaseDate = DateTime.Parse("2020-01-17")},
                new Album {ArtistName = "Adele",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Adele"), Genre = "Pop",AlbumTitle = "19",ReleaseDate = DateTime.Parse("2008-01-28")},
                new Album {ArtistName = "Adele",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Adele"), Genre = "Pop",AlbumTitle = "21",ReleaseDate = DateTime.Parse("2011-01-24")},
                new Album {ArtistName = "Adele",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Adele"), Genre = "Pop",AlbumTitle = "25",ReleaseDate = DateTime.Parse("2015-11-20")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "OnlySee",ReleaseDate = DateTime.Parse("1997-12-23")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "Healing Is Difficult",ReleaseDate = DateTime.Parse("2001-07-09")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "Colour the Small One",ReleaseDate = DateTime.Parse("2004-01-12")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "Some People Have Real Problems",ReleaseDate = DateTime.Parse("2008-01-08")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "We Are Born",ReleaseDate = DateTime.Parse("2010-06-18")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "1000 Forms of Fear",ReleaseDate = DateTime.Parse("2014-07-04")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "This Is Acting",ReleaseDate = DateTime.Parse("2016-01-29")},
                new Album {ArtistName = "Sia",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Sia"), Genre = "Pop",AlbumTitle = "Everyday Is Christmas",ReleaseDate = DateTime.Parse("2017-11-17")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Chances Are",ReleaseDate = DateTime.Parse("1981-10-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "The Wailing Wailers",ReleaseDate = DateTime.Parse("1965-01-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Soul Rebels",ReleaseDate = DateTime.Parse("1970-12-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Soul Revolution",ReleaseDate = DateTime.Parse("1971-01-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "The Best of The Wailers",ReleaseDate = DateTime.Parse("1971-08-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Catch a Fire",ReleaseDate = DateTime.Parse("1973-04-13")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Burnin'",ReleaseDate = DateTime.Parse("1973-10-19")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Chances Are",ReleaseDate = DateTime.Parse("1981-10-01")},
                //new Album {ArtistName = "Bob Marley",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bob Marley"), Genre = "Reggae",AlbumTitle = "Exodus",ReleaseDate = DateTime.Parse("1977-06-03")},
                new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Are You Experienced",ReleaseDate = DateTime.Parse("1967-08-23")},
                new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Axis: Bold as Love",ReleaseDate = DateTime.Parse("1968-01-15")},
                new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Electric Ladyland",ReleaseDate = DateTime.Parse("1968-10-16")},
                new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Band of Gypsys",ReleaseDate = DateTime.Parse("1970-03-25")},
                //new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Woodstock: Music from the Original Soundtrack and More",ReleaseDate = DateTime.Parse("1970-05-27")},
                //new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Historic Performances Recorded at the Monterey International Pop Festival",ReleaseDate = DateTime.Parse("1970-08-26")},
                new Album {ArtistName = "Jimi Hendrix",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Jimi Hendrix"), Genre = "Rock",AlbumTitle = "Smash Hits",ReleaseDate = DateTime.Parse("1969-07-30")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "Dangerously in Love",ReleaseDate = DateTime.Parse("2003-06-23")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "B'Day",ReleaseDate = DateTime.Parse("2006-09-01")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "I Am... Sasha Fierce",ReleaseDate = DateTime.Parse("2008-11-14")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "4",ReleaseDate = DateTime.Parse("2011-06-24")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "Beyoncé",ReleaseDate = DateTime.Parse("2013-12-13")},
                new Album {ArtistName = "Beyoncé",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Beyoncé"), Genre = "Pop",AlbumTitle = "Lemonade",ReleaseDate = DateTime.Parse("2016-04-23")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Taylor Swift",ReleaseDate = DateTime.Parse("2006-10-24")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Fearless",ReleaseDate = DateTime.Parse("2008-11-11")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Speak Now",ReleaseDate = DateTime.Parse("2010-10-25")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Red",ReleaseDate = DateTime.Parse("2012-10-22")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "1989",ReleaseDate = DateTime.Parse("2014-10-27")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Reputation",ReleaseDate = DateTime.Parse("2017-11-10")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Lover",ReleaseDate = DateTime.Parse("2019-08-23")},
                new Album {ArtistName = "Taylor Swift",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Taylor Swift"), Genre = "Pop",AlbumTitle = "Folklore",ReleaseDate = DateTime.Parse("2020-07-24")},
                //new Album {ArtistName = "Ed Sheeran",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ed Sheeran"), Genre = "Pop",AlbumTitle = "+",ReleaseDate = DateTime.Parse("2011-09-09")},
                new Album {ArtistName = "Ed Sheeran",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ed Sheeran"), Genre = "Pop",AlbumTitle = "×",ReleaseDate = DateTime.Parse("2014-06-23")},
                new Album {ArtistName = "Ed Sheeran",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ed Sheeran"), Genre = "Pop",AlbumTitle = "÷",ReleaseDate = DateTime.Parse("2017-03-03")},
                new Album {ArtistName = "Ed Sheeran",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ed Sheeran"), Genre = "Pop",AlbumTitle = "No.6 Collaborations Project",ReleaseDate = DateTime.Parse("2019-07-12")},
                new Album {ArtistName = "Tones and I",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Tones and I"), Genre = "Pop",AlbumTitle = "The Kids Are Coming",ReleaseDate = DateTime.Parse("2019-08-30")},
                new Album {ArtistName = "Hozier",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Hozier"), Genre = "Rock",AlbumTitle = "Hozier",ReleaseDate = DateTime.Parse("2014-09-19")},
                new Album {ArtistName = "Hozier",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Hozier"), Genre = "Rock",AlbumTitle = "Wasteland, Baby!",ReleaseDate = DateTime.Parse("2019-03-01")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Bon Jovi",ReleaseDate = DateTime.Parse("1984-01-21")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Slippery When Wet",ReleaseDate = DateTime.Parse("1986-08-18")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "New Jersey",ReleaseDate = DateTime.Parse("1988-09-19")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Keep the Faith",ReleaseDate = DateTime.Parse("1992-11-03")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "These Days",ReleaseDate = DateTime.Parse("1995-06-27")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Crush",ReleaseDate = DateTime.Parse("2000-06-13")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Bounce",ReleaseDate = DateTime.Parse("2002-10-08")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Have a Nice Day",ReleaseDate = DateTime.Parse("2005-09-20")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Lost Highway",ReleaseDate = DateTime.Parse("2007-06-19")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "The Circle",ReleaseDate = DateTime.Parse("2009-11-10")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "What About Now",ReleaseDate = DateTime.Parse("2013-03-08")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "Burning Bridges",ReleaseDate = DateTime.Parse("2015-08-21")},
                new Album {ArtistName = "Bon Jovi",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bon Jovi"), Genre = "Rock",AlbumTitle = "This House Is Not for Sale",ReleaseDate = DateTime.Parse("2016-11-04")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Enrique Iglesias",ReleaseDate = DateTime.Parse("1995-11-21")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Vivir",ReleaseDate = DateTime.Parse("1997-01-21")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Cosas Del Amor",ReleaseDate = DateTime.Parse("1998-09-22")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Enrique",ReleaseDate = DateTime.Parse("1999-11-23")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Escape",ReleaseDate = DateTime.Parse("2001-10-30")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Quizás",ReleaseDate = DateTime.Parse("2002-09-17")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "7",ReleaseDate = DateTime.Parse("2003-11-25")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Insomniac",ReleaseDate = DateTime.Parse("2007-06-12")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Euphoria",ReleaseDate = DateTime.Parse("2010-07-06")},
                new Album {ArtistName = "Enrique Iglesias",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Enrique Iglesias"), Genre = "Pop",AlbumTitle = "Sex and Love",ReleaseDate = DateTime.Parse("2014-03-14")},
                new Album {ArtistName = "Avicii",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Avicii"), Genre = "Electronic",AlbumTitle = "True",ReleaseDate = DateTime.Parse("2013-09-13")},
                new Album {ArtistName = "Avicii",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Avicii"), Genre = "Electronic",AlbumTitle = "Stories",ReleaseDate = DateTime.Parse("2015-10-02")},
                new Album {ArtistName = "Avicii",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Avicii"), Genre = "Electronic",AlbumTitle = "Tim",ReleaseDate = DateTime.Parse("2019-06-06")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "Just a Little More Love",ReleaseDate = DateTime.Parse("2002-06-10")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "Guetta Blaster",ReleaseDate = DateTime.Parse("2004-06-07")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "Pop Life",ReleaseDate = DateTime.Parse("2007-06-18")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "One Love",ReleaseDate = DateTime.Parse("2009-08-21")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "Nothing but the Beat",ReleaseDate = DateTime.Parse("2011-08-26")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "Listen",ReleaseDate = DateTime.Parse("2014-11-21")},
                new Album {ArtistName = "David Guetta",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "David Guetta"), Genre = "Electronic",AlbumTitle = "7",ReleaseDate = DateTime.Parse("2018-09-14")},
                new Album {ArtistName = "Calvin Harris",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Calvin Harris"), Genre = "Electronic",AlbumTitle = "I Created Disco",ReleaseDate = DateTime.Parse("2007-06-15")},
                new Album {ArtistName = "Calvin Harris",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Calvin Harris"), Genre = "Electronic",AlbumTitle = "Ready for the Weekend",ReleaseDate = DateTime.Parse("2009-08-14")},
                new Album {ArtistName = "Calvin Harris",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Calvin Harris"), Genre = "Electronic",AlbumTitle = "18 Months",ReleaseDate = DateTime.Parse("2012-10-26")},
                new Album {ArtistName = "Calvin Harris",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Calvin Harris"), Genre = "Electronic",AlbumTitle = "Motion",ReleaseDate = DateTime.Parse("2014-10-31")},
                new Album {ArtistName = "Calvin Harris",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Calvin Harris"), Genre = "Electronic",AlbumTitle = "Funk Wav Bounces Vol. 1",ReleaseDate = DateTime.Parse("2017-06-30")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Thank Me Later",ReleaseDate = DateTime.Parse("2010-06-15")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Take Care",ReleaseDate = DateTime.Parse("2011-11-15")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Nothing Was the Same",ReleaseDate = DateTime.Parse("2013-09-24")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Views",ReleaseDate = DateTime.Parse("2016-04-29")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Scorpion",ReleaseDate = DateTime.Parse("2018-06-29")},
                new Album {ArtistName = "Drake",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Drake"), Genre = "Rap",AlbumTitle = "Thank Me Later",ReleaseDate = DateTime.Parse("2018-09-14")},
                new Album {ArtistName = "Ariana Grande",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ariana Grande"), Genre = "Pop",AlbumTitle = "Yours Truly",ReleaseDate = DateTime.Parse("2013-08-30")},
                new Album {ArtistName = "Ariana Grande",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ariana Grande"), Genre = "Pop",AlbumTitle = "My Everything",ReleaseDate = DateTime.Parse("2014-08-25")},
                new Album {ArtistName = "Ariana Grande",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ariana Grande"), Genre = "Pop",AlbumTitle = "Dangerous Woman",ReleaseDate = DateTime.Parse("2016-05-20")},
                new Album {ArtistName = "Ariana Grande",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ariana Grande"), Genre = "Pop",AlbumTitle = "Sweetener",ReleaseDate = DateTime.Parse("2018-08-17")},
                new Album {ArtistName = "Ariana Grande",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Ariana Grande"), Genre = "Pop",AlbumTitle = "Thank U, Next",ReleaseDate = DateTime.Parse("2019-02-08")},
                new Album {ArtistName = "Bruno Mars",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bruno Mars"), Genre = "Pop",AlbumTitle = "Doo-Wops and Hooligans",ReleaseDate = DateTime.Parse("2010-10-04")},
                new Album {ArtistName = "Bruno Mars",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bruno Mars"), Genre = "Pop",AlbumTitle = "Unorthodox Jukebox",ReleaseDate = DateTime.Parse("2012-12-07")},
                new Album {ArtistName = "Bruno Mars",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Bruno Mars"), Genre = "Pop",AlbumTitle = "24K Magic",ReleaseDate = DateTime.Parse("2016-11-18")},
                new Album {ArtistName = "Kendrick Lamar",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kendrick Lamar"), Genre = "Rap",AlbumTitle = "Section.80",ReleaseDate = DateTime.Parse("2011-07-02")},
                new Album {ArtistName = "Kendrick Lamar",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kendrick Lamar"), Genre = "Rap",AlbumTitle = "Good Kid, M.A.A.D City",ReleaseDate = DateTime.Parse("2012-10-22")},
                new Album {ArtistName = "Kendrick Lamar",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kendrick Lamar"), Genre = "Rap",AlbumTitle = "To Pimp a Butterfly",ReleaseDate = DateTime.Parse("2015-03-16")},
                new Album {ArtistName = "Kendrick Lamar",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Kendrick Lamar"), Genre = "Rap",AlbumTitle = "Damn",ReleaseDate = DateTime.Parse("2017-04-14")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Magia",ReleaseDate = DateTime.Parse("1991-06-24")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Peligro",ReleaseDate = DateTime.Parse("1993-03-25")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Pies Descalzos",ReleaseDate = DateTime.Parse("1995-10-06")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Dónde Están los Ladrones?",ReleaseDate = DateTime.Parse("1998-09-29")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Laundry Service",ReleaseDate = DateTime.Parse("2001-11-13")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Fijación Oral, Vol. 1",ReleaseDate = DateTime.Parse("2005-06-03")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Oral Fixation, Vol. 2",ReleaseDate = DateTime.Parse("2005-11-28")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "She Wolf",ReleaseDate = DateTime.Parse("2009-10-09")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Sale el Sol",ReleaseDate = DateTime.Parse("2010-10-19")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "Shakira",ReleaseDate = DateTime.Parse("2014-03-21")},
                new Album {ArtistName = "Shakira",Artist = context.Artists.SingleOrDefault(a => a.ArtistName == "Shakira"), Genre = "Pop",AlbumTitle = "El Dorado",ReleaseDate = DateTime.Parse("2017-05-26")},
            };
            foreach (var album in albums)
            {
                context.Albums.Add(album);
                album.OwnerID = adminID;
                album.Status = AlbumStatus.Approved;
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
            var artistList = context.Artists.ToList();
            var UserList = context.Users.ToList();
            var rand = new Random();
            foreach (var album in albumList)
            {
                var type = rand.Next() % 3;
                foreach (var user in UserList)
                {
                    if(rand.Next() % 4 == 0) continue;
                    ReviewGenerator.GenerateReview(context, album, user, type, adminID);
                }
            }
            context.SaveChanges();
            foreach (var artist in artistList)
            {
                artist.UpdateArtistRate();
                var multipler = rand.Next() % 15 + 1;
                artist.PageViews = artist.Albums.Count() * multipler + (rand.Next() % 500 + 1);
            }
            context.SaveChanges();
        }


    }
}
