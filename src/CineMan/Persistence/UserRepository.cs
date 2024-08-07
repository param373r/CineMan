using CineMan.Domain.Contracts.Users;
using CineMan.Errors;
using CineMan.Errors.ErrorConstants;
using CineMan.Persistence.Data;

namespace CineMan.Persistence;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext dbContext, ILogger<UserRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Result> DeleteUserAsync(Guid id)
    {
        _logger.LogInformation($"Fetching user with ID {id}", id);
        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            _logger.LogError(UserErrors.UserNotFound.Message);
            throw new Exception(UserErrors.UserNotFound.Message);
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"User with ID {id} has been deleted.");

        return Result.Success();
    }

    public async Task<Result<GetUserResponse>> GetUserProfileAsync(Guid id)
    {
        _logger.LogInformation($"Fetching user profile with ID {id}", id);
        var user = await _dbContext.Users.FindAsync(id);

        if (user is null)
        {
            _logger.LogError(UserErrors.UserNotFound.Message);
            throw new Exception(UserErrors.UserNotFound.Message);
        }

        _logger.LogInformation($"User profile with ID {id} has been fetched.");

        return Result.Success(GetUserResponse.FromDomain(user));
    }

    public async Task<Result> UpdateProfileAsync(UpdateProfileRequest request, Guid id)
    {
        var _user = await _dbContext.Users.FindAsync(id);

        if (_user is null)
        {
            _logger.LogError(UserErrors.UserNotFound.Message);
            return Result.Failure(UserErrors.UserNotFound);
        }

        if (request.DateOfBirth is not null)
        {
            if (request.DateOfBirth.Value > DateOnly.FromDateTime(DateTime.Today))
            {
                _logger.LogError(UserErrors.InvalidDate.Message);
                return Result.Failure(UserErrors.InvalidDate);
            }
            else if (DateTime.Now.Subtract(request.DateOfBirth.Value.ToDateTime(TimeOnly.MaxValue)).TotalDays / 365 < 13)
            {
                _logger.LogError(UserErrors.AgeTooSmall.Message);
                return Result.Failure(UserErrors.AgeTooSmall);
            }

            _user.DateOfBirth = request.DateOfBirth.Value;
        }

        _user.Firstname = request.FirstName is null ? _user.Firstname : request.FirstName;
        _user.Lastname = request.LastName is null ? _user.Lastname : request.LastName;
        _user.Address = request.Address is null ? _user.Address : request.Address;

        _logger.LogInformation($"Updating user profile with ID {id}", id);
        _dbContext.Users.Update(_user);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"User profile with ID {id} has been updated.");

        return Result.Success();
    }
}