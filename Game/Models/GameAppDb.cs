using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Game.Models
{
    public class GameAppDb : DbContext
    {
        public GameAppDb()
            : base("name=DefaultConnection")
        {

        }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<RestaurantReview> Reviews { get; set; }
    }
}