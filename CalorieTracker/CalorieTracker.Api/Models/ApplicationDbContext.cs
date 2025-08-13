using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CalorieTracker.Api.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Food> Foods => Set<Food>();
        public DbSet<Dish> Dishes => Set<Dish>();
        public DbSet<DishItem> DishItems => Set<DishItem>();
        public DbSet<DailyLog> DailyLogs => Set<DailyLog>();
        public DbSet<DailyLogEntry> DailyLogEntries => Set<DailyLogEntry>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<DishItem>()
                .HasOne(di => di.Dish)
                .WithMany(d => d.Items)
                .HasForeignKey(di => di.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DishItem>()
                .HasOne(di => di.Food)
                .WithMany()
                .HasForeignKey(di => di.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<DailyLog>()
                .HasIndex(l => new { l.UserId, l.Date })
                .IsUnique();

            builder.Entity<DailyLogEntry>()
                .HasOne(e => e.DailyLog)
                .WithMany(l => l.Entries)
                .HasForeignKey(e => e.DailyLogId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}