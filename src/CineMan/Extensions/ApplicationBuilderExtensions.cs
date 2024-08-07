using Bogus;
using CineMan.Persistence.Data;
using CineMan.Domain.Models.AvailableShowTimes;
using CineMan.Domain.Models.Movies;
using CineMan.Domain.Models.UserBookings;
using CineMan.Domain.Models.Users;

namespace CineMan.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void SeedData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var faker = new Faker();

        SeedMovies(dbContext, faker);
        SeedAvailableShowTimes(dbContext, faker);
        SeedUsers(dbContext, faker);
        SeedUserBookings(dbContext, faker);
        UpdateUsersWithBookings(dbContext);

        dbContext.SaveChanges();
    }

    private static void UpdateUsersWithBookings(AppDbContext dbContext)
    {
        var users = dbContext.Users.ToList();
        var userBookings = dbContext.UserBookings.ToList();

        foreach (var user in users)
        {
            user.Bookings = userBookings.Where(ub => ub.UserId == user.Id).Select(ub => ub.Id).ToList();
        }
    }

    private static void SeedUserBookings(AppDbContext dbContext, Faker faker)
    {
        if (dbContext.UserBookings.ToList().Count != 0) return;

        List<UserBooking> userBookings = new();

        for (var i = 0; i < 10; i++)
        {
            userBookings.Add(new UserBooking
            {
                Id = Guid.NewGuid(),
                UserId = dbContext.Users.ToList()[i].Id,
                TheatreName = faker.Company.CompanyName(),
                MovieId = dbContext.Movies.ToList()[i].Id,
                TimeSlot = faker.Random.Enum<TimeSlot>(),
                Status = faker.Random.Enum<BookingStatus>(),
                BookedSeats = faker.Random.Number(1, 5),
                TotalAmount = faker.Random.Decimal(50, 100),
                OrderDate = faker.Date.Past()
            });

        }

        dbContext.UserBookings.AddRange(userBookings);
        dbContext.SaveChanges();
    }

    private static void SeedUsers(AppDbContext dbContext, Faker faker)
    {
        if (dbContext.Users.ToList().Count != 0) return;

        List<User> users = new();

        for (var i = 0; i < 10; i++)
        {
            users.Add(new User
            {
                Id = Guid.NewGuid(),
                Firstname = faker.Name.FirstName(),
                Lastname = faker.Name.LastName(),
                DateOfBirth = faker.Date.BetweenDateOnly(new(1950, 1, 1), new(2015, 12, 31)),
                Address = new Address
                {
                    Street = faker.Address.StreetAddress(),
                    City = faker.Address.City(),
                    State = faker.Address.State(),
                    Country = faker.Address.Country(),
                    ZipCode = faker.Address.ZipCode()
                },
                Email = faker.Internet.Email(),
                IsEmailConfirmed = true,
                Password = faker.Internet.Password(),
                Bookings = new List<Guid>()
            });

        }

        dbContext.Users.AddRange(users);
        dbContext.SaveChanges();
    }

    private static void SeedAvailableShowTimes(AppDbContext dbContext, Faker faker)
    {
        if (dbContext.AvailableShowTimes.ToList().Count != 0) return;

        List<AvailableShowTimes> availableShowTimes = new();

        for (var i = 0; i < 10; i++)
        {
            availableShowTimes.Add(new AvailableShowTimes
            {
                Id = Guid.NewGuid(),
                MovieId = dbContext.Movies.ToList()[i].Id,
                ShowDate = faker.Date.FutureDateOnly(),
                SeatPerTimeSlot = new Dictionary<TimeSlot, int>
                {
                    { TimeSlot.MORNING, faker.Random.Number(50, 100) },
                    { TimeSlot.AFTERNOON, faker.Random.Number(50, 100) },
                    { TimeSlot.EVENING, faker.Random.Number(50, 100) }
                },
                TheatreName = faker.Company.CompanyName(),
                PricePerSeat = faker.Random.Number(50, 100)
            });

        }

        dbContext.AddRange(availableShowTimes);
        dbContext.SaveChanges();
    }

    private static void SeedMovies(AppDbContext dbContext, Faker faker)
    {
        if (dbContext.Movies.ToList().Count != 0) return;

        List<Movie> movies = new();

        for (var i = 0; i < 10; i++)
        {
            movies.Add(new Movie
            {
                Id = Guid.NewGuid(),
                Name = faker.Name.FirstName(),
                Description = faker.Lorem.Sentences(3, " "),
                Rating = $"{Math.Round(faker.Random.Decimal(1, 5), 1)}",
                PosterUrl = faker.Image.PicsumUrl(),
                RunningTime = faker.Random.Number(90, 180),
                ReleaseDate = faker.Date.FutureDateOnly(),
                Genre = faker.Random.Enum<Genre>(),
                Format = faker.Random.Enum<Format>(),
                Language = faker.Random.Enum<Language>(),
                IsFeatured = faker.Random.Bool()
            });
        }

        dbContext.Movies.AddRange(movies);
        dbContext.SaveChanges();
    }
}