using Microsoft.EntityFrameworkCore;
using CineMan.Domain.Models.Movies;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CineMan.Domain.Models.UserBookings;
using CineMan.Domain.Models.AvailableShowTimes;
using CineMan.Domain.Models.Users;
using System.Text.Json;
using CineMan.Serializers;

namespace CineMan.Persistence.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<AvailableShowTimes> AvailableShowTimes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserBooking> UserBookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .Property(m => m.Genre)
                .HasConversion(new EnumToStringConverter<Genre>());
            modelBuilder.Entity<Movie>()
                .Property(m => m.Format)
                .HasConversion(new EnumToStringConverter<Format>());
            modelBuilder.Entity<Movie>()
                .Property(m => m.Language)
                .HasConversion(new EnumToStringConverter<Language>());
            modelBuilder.Entity<Movie>()
                .Property(m => m.IsFeatured)
                .HasConversion(new BoolToStringConverter("false", "true"));

            modelBuilder.Entity<AvailableShowTimes>()
                .HasOne(ast => ast.Movie)
                .WithMany()
                .HasForeignKey(ast => ast.MovieId);
            modelBuilder.Entity<AvailableShowTimes>()
                .Property(ast => ast.SeatPerTimeSlot)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions { Converters = { new DictionaryEnumKeyConverter<TimeSlot, int>() } }),
                    v => JsonSerializer.Deserialize<Dictionary<TimeSlot, int>>(v, new JsonSerializerOptions { Converters = { new DictionaryEnumKeyConverter<TimeSlot, int>() } })!);

            modelBuilder.Entity<UserBooking>()
                .HasOne(ub => ub.Movie)
                .WithMany()
                .HasForeignKey(ub => ub.MovieId);
            modelBuilder.Entity<UserBooking>()
                .HasOne(ub => ub.User)
                .WithMany(u => u.UserBookings)
                .HasForeignKey(ub => ub.UserId);
            modelBuilder.Entity<UserBooking>()
                .Property(ub => ub.TimeSlot)
                .HasConversion(new EnumToStringConverter<TimeSlot>());
            modelBuilder.Entity<UserBooking>()
                .Property(ub => ub.Status)
                .HasConversion(new EnumToStringConverter<BookingStatus>());

            modelBuilder.Entity<User>()
                .OwnsOne(u => u.Address);
            modelBuilder.Entity<User>()
                .Property(u => u.IsEmailConfirmed)
                .HasConversion(new BoolToStringConverter("false", "true"));
        }
    }
}
