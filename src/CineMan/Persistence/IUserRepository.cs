using CineMan.Domain.Contracts.Users;
using CineMan.Errors;

namespace CineMan.Persistence;
public interface IUserRepository
{
    Task<Result<GetUserResponse>> GetUserProfileAsync(Guid id);
    Task<Result> UpdateProfileAsync(UpdateProfileRequest user, Guid userId);
    Task<Result> DeleteUserAsync(Guid id);
}