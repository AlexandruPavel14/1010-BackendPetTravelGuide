using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetTravelGuide.Backend.Database;
using PetTravelGuide.Backend.Database.Entities;
using PetTravelGuide.Backend.Models;
using PetTravelGuide.Backend.Models.AppSettings;
using PetTravelGuide.Backend.Models.Response;
using PetTravelGuide.Backend.Utils;
using PetTravelGuide.Backend.Validators;

namespace PetTravelGuide.Backend.Services.UserService;

public sealed class UserService : IUserService
{
    private readonly DatabaseContext _database;
    private readonly ILogger<UserService> _logger;
    private readonly JwtAuthenticationSettings _jwtOptions;
    private readonly UserServiceSettings _userServiceSettings;

    public UserService(
        DatabaseContext database,
        ILogger<UserService> logger,
        IOptions<JwtAuthenticationSettings> jwtOptions,
        IOptions<UserServiceSettings> userServiceSettings)
    {
        _database = database;
        _logger = logger;
        _jwtOptions = jwtOptions.Value;
        _userServiceSettings = userServiceSettings.Value;
    }

    /// <summary>
    /// Get user by email
    /// </summary>
    /// <param name="email">The email to search the user for</param>
    /// <returns>The <see cref="User">User</see> object with the specified email address</returns>
    public async Task<User?> GetUserByEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return null;
        }

        // sanitize email
        email = email.Trim().ToLower();
        return await _database.Users
            .Include(u => u.Roles)
            .Where(u => u.Email == email)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Get user by id
    /// </summary>
    /// <param name="id">The id to search user for</param>
    /// <returns>The <see cref="User">User</see> object with the specified id</returns>
    public async Task<User?> GetUserById(long id)
    {
        return await _database.Users
            .Include(u => u.Roles)
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns a boolean value if the user exists or not
    /// </summary>
    /// <param name="email">The email to search user for</param>
    /// <returns>Value representing whether or not the user exists</returns>
    public async Task<bool> UserExists(string email)
    {
        return await GetUserByEmail(email) != null;
    }

    public async Task<IResponse> ChangePasswordByEmail(string email, string oldPassword, string newPassword)
    {
        User? user = await GetUserByEmail(email);
        return user == null
            ? new ErrorResponse("Email-ul sau parola sunt incorecte")
            : await ChangePassword(user, oldPassword, newPassword);
    }

    public async Task<IResponse> ChangePasswordById(long id, string oldPassword, string newPassword)
    {
        User? user = await GetUserById(id);
        return user == null
            ? new ErrorResponse("Email-ul sau parola sunt incorecte")
            : await ChangePassword(user, oldPassword, newPassword);
    }

    private async Task<IResponse> ChangePassword(User user, string oldPassword, string newPassword)
    {
        bool passwordVerified;
        try
        {
            passwordVerified = await VerifyPassword(oldPassword, user.PasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "The password could not be verified because: {InternalError}", ex.Message);
            return new ErrorResponse("Parola veche este invalidă");
        }
        return !passwordVerified
            ? new ErrorResponse("Parola veche este invalidă")
            : await ChangePasswordInternal(user, newPassword);
    }

    public async Task<IResponse> ResetPasswordByEmail(string email, string newPassword)
    {
        User? user = await GetUserByEmail(email);
        if (user == null)
        {
            return new ErrorResponse("Nu s-a putut reseta parola");
        }

        return await ResetPassword(user, newPassword);
    }

    public async Task<IResponse> ResetPasswordById(long id, string newPassword)
    {
        User? user = await GetUserById(id);
        if (user == null)
        {
            return new ErrorResponse("Nu s-a putut reseta parola");
        }

        return await ResetPassword(user, newPassword);
    }

    public async Task<IResponse> ResetPassword(User user, string newPassword)
    {
        ValidationResult? validator = await new PasswordStrengthValidator(_userServiceSettings).ValidateAsync(new PasswordString(newPassword));
        return !validator.IsValid
            ? new ErrorResponse(string.Join("<br/>", validator.Errors.Select(e => e.ErrorMessage).ToArray()))
            : await ChangePasswordInternal(user, newPassword);
    }

    private async Task<IResponse> ChangePasswordInternal(User user, string newPassword)
    {
        try
        {
            user.PasswordHash = await PasswordStorage.CreateHash(newPassword);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not create password hash: {InternalError}", ex.Message);
            return new ErrorResponse("Nu s-a putut reseta parola");
        }
        _database.Attach(user).State = EntityState.Modified;
        await _database.SaveChangesAsync();
        return new SuccessDataResponse<long>(user.Id);
    }

    public async Task<IResponse> Login(string email, string password)
    {
        email = email.Trim().ToLower();
        User? user = await GetUserByEmail(email);
        if (user == null)
        {
            return new ErrorResponse("Email sau parolă invalidă");
        }

        if (user.Active.HasValue && !user.Active.Value)
        {
            _logger.LogWarning("The user with the email address {Email} and id {Id} tried to login", user.Email, user.Id);
            return new ErrorResponse("Contul tău de utilizator nu este activ în acest moment și nu te poți autentifica.");
        }
        bool passwordVerified;
        try
        {
            passwordVerified = await VerifyPassword(password, user.PasswordHash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "The password could not be verified because: {InternalError}", ex.Message);
            return new ErrorDataResponse<User?>(null, "Email-ul sau parola sunt incorecte");
        }
        if (!passwordVerified)
        {
            return new ErrorResponse($"Email-ul sau parola sunt incorecte.");
        }
        //Generates new jwt
        JwtSecurityToken jwtToken = CreateJwtToken(user);
        UserLoginResponseModel responseData = new()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(jwtToken),
            UserInfo = new BaseUserLoginModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            }
        };
        return new SuccessDataResponse<UserLoginResponseModel>(responseData);
    }

    private JwtSecurityToken CreateJwtToken(User user)
    {
        JwtTokenBuilder tokenBuilder = new JwtTokenBuilder()
            .AddSecurityKey(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtOptions.SigningKey)))
            .AddIssuer(_jwtOptions.Issuer)
            .AddAudience(_jwtOptions.Audience)
            .AddExpiryInMinutes(_jwtOptions.ExpiryInMinutes)
            .AddSubject(user.Email) // The user email is the subject of the token which must be unique
            .AddClaim("userId", user.Id.ToString())
            .AddRoles(user.Roles.Select(u => u.NormalizedName).Distinct().ToArray());
        return tokenBuilder.Build();
    }

    private static async Task<bool> VerifyPassword(string password, string? passwordHash)
    {
        if (string.IsNullOrEmpty(passwordHash) || string.IsNullOrEmpty(password))
        {
            return false;
        }

        return await PasswordStorage.VerifyPassword(password, passwordHash);
    }
    
    /// <summary>
    /// Insert a user to the database
    /// </summary>
    /// <param name="user">User edit information</param>
    /// <param name="password">Clear password</param>
    /// <param name="isReviewer">Flag for the reviewer role</param>
    public async Task<IResponse> InsertUser(User user, string password, bool isReviewer)
    {
        user.Email = user.Email.Trim().ToLower();
        if (await UserExists(user.Email))
        {
            return new ErrorResponse($"Utilizatorul cu email-ul {user.Email} exista deja");
        }

        ValidationResult? passwordValidator = await new PasswordStrengthValidator(_userServiceSettings)
            .ValidateAsync(new PasswordString(password));
        if (!passwordValidator.IsValid)
        {
            return new ErrorResponse(string.Join("<br/>", passwordValidator.Errors.Select(e => e.ErrorMessage).ToArray()));
        }

        ValidationResult? userValidator = await new UserValidator(_userServiceSettings).ValidateAsync(user);
        if (!userValidator.IsValid)
        {
            return new ErrorResponse(string.Join("<br/>", userValidator.Errors.Select(e => e.ErrorMessage).ToArray()));
        }

        // Generated values
        user.PasswordHash = await PasswordStorage.CreateHash(password);
        user.Roles = new List<Role>
        {
            await _database.Roles.FirstAsync(r => r.NormalizedName == (isReviewer ? "Reviewer" : "User"))
        };

        await _database.AddAsync(user);
        await _database.SaveChangesAsync();
        return new SuccessDataResponse<long>(user.Id, "Utilizator adaugat cu succes!");
    }
}