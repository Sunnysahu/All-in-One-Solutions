using GlobalAnnotationUtils.Common;
using GlobalAnnotationUtils.Errors;
using GlobalAnnotationUtils.Models;
using GlobalAnnotationUtils.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GlobalAnnotationUtils.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<Result<User>> GetByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);

            if (user == null)
                return Result<User>.Failure(UserErrors.NotFound);

            return Result<User>.Success(UserSuccess.UserFound, user);
        }
    }
}
