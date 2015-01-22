namespace Game.Migrations
{
    using Game.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Web.Security;
    using WebMatrix.WebData;

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

            SeedMembership();
        }

        private void SeedMembership()
        {
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", 
                "UserProfile", "UserId", "UserName", autoCreateTables: true);

            var roles = (SimpleRoleProvider)Roles.Provider;
            var membership = (SimpleMembershipProvider)Membership.Provider;

            if(!roles.RoleExists("Admin"))
            {
                roles.CreateRole("Admin");
            }
            if(membership.GetUser("Insanity", false) == null)
            {
                membership.CreateUserAndAccount("Insanity", "test1234");
            }
            if(!roles.GetRolesForUser("Insanity").Contains("Admin"))
            {
                roles.AddUsersToRoles(new[] { "Insanity" }, new[] { "Admin" });
            }
        }
    }
}
