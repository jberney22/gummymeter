using GummyMeter.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace GummyMeter.Data
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Favorite> Favorites => Set<Favorite>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Rating> Ratings { get; set; }
    }

}
