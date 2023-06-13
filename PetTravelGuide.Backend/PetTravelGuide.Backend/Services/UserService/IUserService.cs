using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models.Response;

namespace PetTravelGuide.Backend.Services.UserService;

public interface IUserService
{
    Task<User?> GetUserByEmail(string email);

    Task<User?> GetUserById(long id);

    Task<bool> UserExists(string email);

    Task<IResponse> ChangePasswordByEmail(string email, string oldPassword, string newPassword);

    Task<IResponse> ChangePasswordById(long id, string oldPassword, string newPassword);

    Task<IResponse> ResetPasswordById(long id, string newPassword);

    Task<IResponse> ResetPasswordByEmail(string email, string newPassword);

    Task<IResponse> ResetPassword(User user, string newPassword);

    Task<IResponse> Login(string email, string password);

    Task<IResponse> InsertUser(User user, string password, bool isReviewer);
}