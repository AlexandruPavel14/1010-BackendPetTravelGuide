using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace YourNamespace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRolesController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public UserRolesController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("{userId}/add-moderator-role")]
        [Authorize(Roles = "Admin")]
        public IActionResult AddModeratorRole(int userId)
        {
            var isAuthenticated = User.Identity.IsAuthenticated;
            var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
            var isAdmin = roles.Contains("Admin");
            Console.WriteLine($"isAuthenticated: {isAuthenticated}, isAdmin: {isAdmin}");

            // Find the user with the specified ID
            var user = _dbContext.Users.SingleOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return BadRequest($"User with id {userId} does not exist.");
            }

            if (user.IsModerator)
            {
                return BadRequest($"User {user.Email} is already a moderator.");
            }

            // Update user roles
            var oldUserRole = _dbContext.UserRoles.SingleOrDefault(ur => ur.UserId == userId);
            var newUserRole = new UserRole { UserId = userId, RoleId = 3 }; // 3 is the moderator role ID

            if (oldUserRole == null)
            {
                _dbContext.UserRoles.Add(newUserRole);
            }
            else
            {
                _dbContext.UserRoles.Remove(oldUserRole);
                _dbContext.UserRoles.Add(newUserRole);
            }

            // Update user flags
            user.IsModerator = true;
            user.IsUser = false;
            _dbContext.SaveChanges();

            return Ok($"User {user.Email} is now a moderator.");

        }

        [HttpDelete("{userId}/remove-moderator-role")]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveModeratorRole(int userId)
        {
            // Find the user with the specified ID
            var user = _dbContext.Users.SingleOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return BadRequest($"User with id {userId} does not exist.");
            }

            if (!user.IsModerator)
            {
                return BadRequest($"User {user.Email} is not a moderator.");
            }

            // Update user roles
            var oldUserRole = _dbContext.UserRoles.SingleOrDefault(ur => ur.UserId == userId && ur.RoleId == 3); // 3 is the moderator role ID

            if (oldUserRole == null)
            {
                return BadRequest($"User {user.Email} does not have the moderator role.");
            }

            _dbContext.UserRoles.Remove(oldUserRole);

            // Update user flags
            user.IsModerator = false;
            user.IsUser = true;

            // Check if the user has any other roles
            var otherRoles = _dbContext.UserRoles.Where(ur => ur.UserId == userId && ur.RoleId != 3).ToList();

            if (otherRoles.Count == 0)
            {
                // If the user has no other roles, set them as a regular user in the user roles table
                var newUserRole = new UserRole { UserId = userId, RoleId = 2 }; // 2 is the user role ID
                _dbContext.UserRoles.Add(newUserRole);
            }

            _dbContext.SaveChanges();

            return Ok($"User {user.Email} is no longer a moderator.");
        }


    }
}