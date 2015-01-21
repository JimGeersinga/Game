namespace Game.Migrations
{
    using Game.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Game.Models.GameAppDb>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Game.Models.GameAppDb context)
        {
            context.Restaurants.AddOrUpdate(r => r.Name,
                new Restaurant { Name = "sabatino's", City = "Baltimore", Country = "USA" },
                new Restaurant { Name = "Great Lake", City = "Chicago", Country = "USA" },
                new Restaurant
                {
                    Name = "Smaka",
                    City = "Gothenburg",
                    Country = "Sweden",
                    Reviews =
                        new List<RestaurantReview>
                        {
                            new RestaurantReview { Rating = 9, Body = "Great food!", ReviewerName = "Jim"}
                        }
                });
            for (int i=0; i<1000; i++)
            {
                context.Restaurants.AddOrUpdate(r => r.Name,
                     new Restaurant { Name = i.ToString(), City = "Somewhere", Country = "USA" });
            }
            context.UserProfiles.AddOrUpdate(r => r.UserName,
                new UserProfile { UserId = 1, UserName = "Jim", UserEmail = "jimchippy@hotmail.com", UserPoints = 0 });
           
        }
    }
}
