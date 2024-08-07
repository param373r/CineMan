using CineMan.Domain.Models.Users;

namespace CineMan.Domain.Contracts.Users;

public record UpdateProfileRequest(
    string? FirstName,
    string? LastName,
    DateOnly? DateOfBirth,
    Address? Address
);