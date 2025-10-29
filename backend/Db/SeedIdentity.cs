using DB.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace DB;

public class SeedIdentity(IConfiguration configuration, IWebHostEnvironment environment) {
    private readonly IConfiguration configuration = configuration;
    private readonly IWebHostEnvironment _environment = environment;

    public void Seed(ModelBuilder modelBuilder) {
        var roles = new List<IdentityRole<Guid>>()
        {
            new IdentityRole<Guid>()
            {
                Id = new Guid("9fe5fe8c-0c77-4ae3-91d6-ea61d6d857fc"),
                Name = "Member",
                NormalizedName = "MEMBER",
                ConcurrencyStamp = new Guid("004e09cf-d37c-4929-ab1f-6dd631dd057f").ToString(),
            },
            new IdentityRole<Guid>()
            {
                Id = new Guid("72b7084b-a821-4447-8d31-b359fa0e9a76"),
                Name = "Developer",
                NormalizedName = "DEVELOPER",
                ConcurrencyStamp = new Guid("76a71e2f-7c45-4c77-b181-12561dce095a").ToString()
            },
            new IdentityRole<Guid>()
            {
                Id = new Guid("3a265dde-7ada-4ffc-a15a-4d152ea5893b"),
                Name = "Admin",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = new Guid("8d7aaa0f-38d9-446d-9a27-b2ded50b51ee").ToString()
            }
        };
        if (!_environment.IsEnvironment("Testing"))
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);


        if (_environment.IsDevelopment() /*|| _environment.IsStaging()*/) {
            var user = ConfigureSeedUser(out string password);
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, password);

            var profile = new Profile() {
                Id = user.Id,
                Email = user.Email,
                Login = user.Email.Split('@')[0],
                CreatedAt = new DateTime(2024, 12, 1).ToUniversalTime(),
                UpdatedAt = new DateTime(2024, 12, 1).ToUniversalTime(),
            };
            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<Profile>().HasData(profile);

            var userRoles = new List<IdentityUserRole<Guid>>() {
                new IdentityUserRole<Guid>()
                {
                    UserId = user.Id,
                    RoleId = roles.First(q => q.Name == "Admin").Id
                }
            };
            modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(userRoles);
        }
    }

    private User ConfigureSeedUser(out string password) {
        var userId = configuration.GetSection("SeedIdentityUser:Id").Value ??
            throw new Exception("SeedIdentityUser:Id is not found in config");
        var userEmail = configuration.GetSection("SeedIdentityUser:Email").Value ??
            throw new Exception("SeedIdentityUser:Email is not found in config");
        var userPassword = configuration.GetSection("SeedIdentityUser:Password").Value ??
            throw new Exception("SeedIdentityUser:Password is not found in config");
        password = userPassword;
        return new User {
            Id = new Guid(userId),
            Email = userEmail,
            NormalizedEmail = userEmail.ToUpper(),
            UserName = userEmail,
            NormalizedUserName = userEmail.ToUpper(),
            EmailConfirmed = true,
            SecurityStamp = new Guid("69d150c1-6fe2-4c43-b890-86225589dd29").ToString(),
            ConcurrencyStamp = new Guid("4c8622db-6cd7-428b-b980-f0bc95637ad4").ToString(),
            CreatedAt = new DateTime(2024, 12, 1).ToUniversalTime(),
            UpdatedAt = new DateTime(2024, 12, 1).ToUniversalTime(),
        };
    }
}

