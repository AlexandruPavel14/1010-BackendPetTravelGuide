using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace YourNamespace.Controllers
{
    [AllowAnonymous] 
    [ApiController]
    [Route("api/[controller]")]
    public class PasswordResetController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public PasswordResetController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("request")]
        public IActionResult RequestPasswordReset([FromBody] PasswordResetRequestModel model)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            // Generate a random code
            var code = GenerateRandomCode(6);
            Console.WriteLine($"Password reset code for user {user.Email}: {code}");

            // TODO: Send the code to the user via email or SMS

            // Save the code and its expiration date in the database
            user.PasswordResetCode = code;
            user.PasswordResetCodeExpirationDate = DateTime.UtcNow.AddMinutes(10);
            _dbContext.SaveChanges();

            return Ok("Password reset code generated successfully.");
        }

        [HttpPost("reset")]
        public IActionResult ResetPassword([FromBody] PasswordResetModel model)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Email == model.Email);

            if (user == null)
            {
                return BadRequest("Invalid email address.");
            }

            if (string.IsNullOrEmpty(user.PasswordResetCode) || user.PasswordResetCodeExpirationDate == null)
            {
                return BadRequest("Password reset code not generated.");
            }

            if (user.PasswordResetCodeExpirationDate.Value < DateTime.UtcNow)
            {
                return BadRequest("Password reset code expired.");
            }

            if (user.PasswordResetCode != model.Code)
            {
                return BadRequest("Invalid password reset code.");
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Password is required.");
            }

            // Hash the new password
            var hashedPassword = HashPassword(model.Password);

            // Update the user's password and reset the password reset code
            user.Parola = hashedPassword;
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpirationDate = null;
            _dbContext.SaveChanges();

            return Ok("Password reset successfully.");
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the hashed bytes to a hexadecimal string
                var hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hashedString;
            }
        }
    }

    public class PasswordResetRequestModel
    {
        public string? Email { get; set; }
    }

    public class PasswordResetModel
    {
        public string? Email { get; set; }
        public string? Code { get; set; }
        public string? Password { get; set; }
    }
}
