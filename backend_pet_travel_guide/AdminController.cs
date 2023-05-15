using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace YourNamespace.Controllers
{
    [AllowAnonymous] 
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UsersController(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult RegisterUser([FromBody] RegistrationModel model)
        {
            // Check if user with same email already exists
            if (_dbContext.Users.Any(u => u.Email == model.Email))
            {
                return BadRequest("A user with that email already exists.");
            }

            // Register new user
            var user = _dbContext.RegisterUser(model.Name, model.Surname, model.PhoneNumber, model.Email, model.Password);

            return Ok(user);
        }



[HttpGet("locations")]
public IActionResult GetAllLocations()
{
    var locations = _dbContext.Locations.Select(l => new {
        Name = l.name,
        Latitude = l.latitude,
        Longitude = l.longitude
    }).ToList();

    if (locations == null || locations.Count == 0) {
        return NotFound();
    }

    return Ok(new { locations });
}





        [AllowAnonymous] 
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (string.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Password is required.");
            }

            var user = _dbContext.Users.SingleOrDefault(u => u.Email == model.Email);

            // Check if user exists and password matches
            if (user == null)
            {
                return BadRequest("Invalid email or password.");
            }
            else if (!VerifyPassword(model.Password, user.Parola))
            {
                return BadRequest("Invalid email or password.");
            }

            // Create JWT token
            var token = CreateJwtToken(user);

            // Print welcome message in terminal
            Console.WriteLine($"Hello {user.Nume}!");

            return Ok(new { Token = token, Message = "User connected successfully." });
        }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetUserData()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null)
        {
            return BadRequest("User ID claim not found.");
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Return the user's name and email
        return Ok(new { Nume = user.Nume, Prenume = user.Prenume, Email = user.Email, NrTelefon = user.NrTelefon });
    }

    [HttpGet("me/petscreated")]
    [Authorize]
    public IActionResult GetAllPets()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null)
        {
            return BadRequest("User ID claim not found.");
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = _dbContext.Users.Include(u => u.Pets).FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user.Pets);
    }


    [HttpPost("me/pets")]
    [Authorize]
    public IActionResult AddPetToUser([FromBody] AddPetModel model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null)
        {
            return BadRequest("User ID claim not found.");
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = _dbContext.Users.Include(u => u.Pets).FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var pet = new Pet
        {
            Name = model.Name,
            Species = model.Species,
            Age = model.Age,
            Color = model.Color,
            AddedAt = DateTime.UtcNow
        };

        user.Pets.Add(pet);
        _dbContext.SaveChanges();

        return Ok(pet);
    }

    [HttpDelete("me/pets/{petId}")]
    [Authorize]
    public IActionResult DeletePet(int petId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
        {
            return BadRequest("User ID claim not found.");
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            return BadRequest("Invalid user ID.");
        }

        var user = _dbContext.Users.Include(u => u.Pets).FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var pet = user.Pets.FirstOrDefault(p => p.PetId == petId);
        if (pet == null)
        {
            return NotFound("Pet not found.");
        }

        _dbContext.Pets.Remove(pet);
        _dbContext.SaveChanges();

        return NoContent();
    }


    [HttpPut("me/pets/{petId}")]
    [Authorize]
    public IActionResult UpdatePet([FromBody] UpdatePetModel model, int petId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null)
        {
            return BadRequest("User ID claim not found.");
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = _dbContext.Users.Include(u => u.Pets).FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var pet = user.Pets.FirstOrDefault(p => p.PetId == petId);
        if (pet == null)
        {
            return NotFound("Pet not found.");
        }

        // Update the pet properties
        pet.Name = model.Name ?? pet.Name;
        pet.Species = model.Species ?? pet.Species;
        pet.Age = model.Age ?? pet.Age;
        pet.Color = model.Color ?? pet.Color;

        _dbContext.SaveChanges();

        return Ok(pet);
    }

[HttpPost("invitation")]
[Authorize(Roles = "Admin")]
public IActionResult GenerateInvitationLink()
{
    // Check if user is authenticated
    if (!User.Identity.IsAuthenticated)
    {
        return BadRequest("You must be logged in to generate an invitation link.");
    }

    // Generate a unique invitation code
    var invitationCode = GenerateInvitationCode();

    // Get the ID of the currently logged-in user
    var currentUser = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

    // Save the invitation code to the database
    var invitation = new Invitation
    {
        InvitationCode = invitationCode,
        CreatedById = currentUser // Set the ID of the current user as the creator
    };
    _dbContext.Invitations.Add(invitation);
    _dbContext.SaveChanges();
    
    // Return the invitation link
    var invitationLink = $"{Request.Scheme}://{Request.Host}/api/users/register?code={invitationCode}";
    return Ok(new { InvitationLink = invitationLink });
}




private string GenerateInvitationCode()
{
    var bytes = new byte[16];
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(bytes);
    }
    return Convert.ToBase64String(bytes);
}


    [HttpPut("me")]
    [Authorize]
    public IActionResult UpdateUserData([FromBody] UpdateUserDataModel model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // Get the user ID from the JWT token
        if (userIdClaim == null)
        {
            return BadRequest("User ID claim not found.");
        }

        var userId = int.Parse(userIdClaim.Value);

        var user = _dbContext.Users.FirstOrDefault(u => u.UserId == userId);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        // Update user data
        user.Nume = model.Name;
        user.Prenume = model.Surname;
        user.NrTelefon = model.Phone;
        user.Email = model.Email;
        _dbContext.SaveChanges();

        // Return success message
        return Ok("User data updated successfully.");
    }
        private string CreateJwtToken(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var jwtKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new ArgumentNullException("Jwt:Key configuration value is null or empty.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()));

            if (!string.IsNullOrEmpty(user.Email))
            {
                claims.Add(new Claim(ClaimTypes.Email, user.Email));
            }

            if (user.IsAdmin)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }
            else if (user.IsModerator)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Moderator"));
            }
            else
            {
                claims.Add(new Claim(ClaimTypes.Role, "User"));
            }

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        private bool VerifyPassword(string password, string hashedPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert the hashed bytes to a hexadecimal string
                var hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

                return hashedString == hashedPassword;
            }
        }
    }

    public class RegistrationModel
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? InvitationCode { get; set; } 
    }

    public class UserRoleModel
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }

    public class LoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}
  
  public class UpdateUserDataModel
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class AddPetModel
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int Age { get; set; }
    public string? Color { get; set; }
}

public class UpdatePetModel
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public int? Age { get; set; }
    public string? Color { get; set; }
}

