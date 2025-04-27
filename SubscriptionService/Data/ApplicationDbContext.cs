using Microsoft.EntityFrameworkCore;
using SubscriptionService.Models;
using System.Collections.Generic;

namespace SubscriptionService.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

        public DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();
        public DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SubscriptionPlan>().HasData(
                new SubscriptionPlan
                {
                    Id = 1,
                    Name = "Free",
                    //Description = "Access to view videos only.",
                    Price = 0.0m,
                    DurationInDays = 0 // No expiry
                },
                new SubscriptionPlan
                {
                    Id = 2,
                    Name = "Basic",
                    //Description = "Download up to 5 videos per month.",
                    Price = 9.99m,
                    DurationInDays = 30
                },
                new SubscriptionPlan
                {
                    Id = 3,
                    Name = "Premium",
                    //Description = "Unlimited downloads.",
                    Price = 19.99m,
                    DurationInDays = 30
                }
            );
        }

    }
}
