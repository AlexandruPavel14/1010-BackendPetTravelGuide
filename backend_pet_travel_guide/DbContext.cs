using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System.Security.Cryptography;
using System.Text;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }  
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Pet> Pets { get; set; }
    public DbSet<Invitation> Invitations { get; set; }
    public DbSet<UserInvitation> UserInvitations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Seed default admin role
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = 1, RoleName = "Admin" });

        // Seed default user and moderator roles
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = 2, RoleName = "User" });
        modelBuilder.Entity<Role>().HasData(new Role { RoleId = 3, RoleName = "Moderator" });

        modelBuilder.Entity<UserRole>()
            .HasKey(ur => new { ur.UserId, ur.RoleId });

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId);

        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId);

        // Hash default admin password
        var defaultPassword = "password";
        var hashedPassword = HashPassword(defaultPassword);

        // Seed default admin user
        modelBuilder.Entity<User>().HasData(new User
        {
            UserId = 1,
            Nume = "Admin",
            Prenume = "Admin",
            NrTelefon = "123456789",
            Email = "admin@example.com",
            Parola = hashedPassword, // Store the hashed password
            IsAdmin = true
        });

        modelBuilder.Entity<UserInvitation>()
            .HasKey(ui => new { ui.UserId, ui.InvitationId });

        modelBuilder.Entity<UserInvitation>()
            .HasOne(ui => ui.User)
            .WithMany(u => u.UserInvitations)
            .HasForeignKey(ui => ui.UserId);

        modelBuilder.Entity<UserInvitation>()
            .HasOne(ui => ui.Invitation)
            .WithMany(i => i.UserInvitations)
            .HasForeignKey(ui => ui.InvitationId);
            // Seed default user roles for admin user
            modelBuilder.Entity<UserRole>().HasData(new UserRole
            {
                UserId = 1,
                RoleId = 1 // Admin role
            });
        }


    public User RegisterUser(string name, string surname, string phoneNumber, string email, string password)
    {
        var user = new User
        {
            Nume = name,
            Prenume = surname,
            NrTelefon = phoneNumber,
            Email = email,
            Parola = HashPassword(password), // Hash the password before storing it
            IsUser = true
        };

        Users.Add(user);
        SaveChanges();

        // Add default user role to new user
        AddUserRole(user.UserId, 2); // Add user role

        return user;
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

    public bool AddUserRole(int userId, int roleId)
    {
        var user = Users.Find(userId);
        var role = Roles.Find(roleId);

        if (user == null || role == null)
        {
            return false;
        }

        // Check if user already has role
        if (UserRoles.Any(ur => ur.UserId == userId && ur.RoleId == roleId))
        {
            return false;
        }

        var userRole = new UserRole
        {
            User = user,
            Role = role
        };

        UserRoles.Add(userRole);
        SaveChanges();

        // Update user roles
        if (role.RoleName == "Admin")
        {
            user.IsAdmin = true;
        }
        else if (role.RoleName == "Moderator")
        {
            user.IsModerator = true;
        }

        SaveChanges();

        return true;
    }

    internal object RegisterUser(string? name, string? surname, string? phoneNumber, string? email, string? password, object value)
    {
        throw new NotImplementedException();
    }
    
}
