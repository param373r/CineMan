using CineMan.Domain.Models.Users;

namespace CineMan.Domain.Contracts.Users;

public record GetUserResponse
(
    string? FirstName,
    string? LastName,
    string Email,
    DateOnly? DateOfBirth,
    Address? Address
)
{
    public static GetUserResponse FromDomain(User user)
    {
        return new GetUserResponse(
            user.Firstname,
            user.Lastname,
            user.Email,
            user.DateOfBirth,
            user.Address
        );
    }
}