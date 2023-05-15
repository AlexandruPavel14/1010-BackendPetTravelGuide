using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend_pet_travel_guide.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public AdminController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("users")]
        public ActionResult<IEnumerable<User>> GetUsers()
        {
            var users = _dbContext.Users.ToList();
            return Ok(users);
        }

        [HttpGet("users/count")]
        public ActionResult<int> GetUserCount()
        {
            var count = _dbContext.Users.Count();
            return Ok(count);
        }
        [HttpDelete("users/{userId}")]
        
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteUser(int userId)
        {
            var user = _dbContext.Users.Find(userId);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            _dbContext.SaveChanges();

            return NoContent();
        }
    }
}
