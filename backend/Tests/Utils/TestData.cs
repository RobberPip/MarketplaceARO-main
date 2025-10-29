using DB.Entities;
using Microsoft.AspNetCore.Identity;

namespace Tests.Utils {
    internal static class TestData {
        internal static IdentityTestDataContainer GetIdentityTestData() {
            var roles = new List<IdentityRole<Guid>>()
            {
                new IdentityRole<Guid>()
                {
                    Id = new Guid("ab2b53b5-ca6f-4e74-92c3-ba4304853d3d"),
                    Name = "Member",
                    NormalizedName = "MEMBER",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                },
                new IdentityRole<Guid>()
                {
                    Id = new Guid("418f477c-0837-49f0-a93d-ea15532f9185"),
                    Name = "Developer",
                    NormalizedName = "DEVELOPER",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole<Guid>()
                {
                    Id = new Guid("3ab5d877-de26-486b-ac07-94b82754be0f"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
            };

            var user = new User {
                Id = new Guid("5450721d-b3bd-4342-9aac-6fe315911a04"),
                Email = "test_user123@test.com",
                NormalizedEmail = "TEST_USER123@TEST.COM",
                UserName = "test_user123@test.com",
                NormalizedUserName = "TEST_USER123@TEST.COM",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };
            var passwordHasher = new PasswordHasher<User>();
            user.PasswordHash = passwordHasher.HashPassword(user, "n0rmAl###");

            var profile = new Profile() {
                Id = user.Id,
                Email = user.Email,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            var userRoles = new List<IdentityUserRole<Guid>>()
            {
                new IdentityUserRole<Guid>()
                {
                    UserId = user.Id,
                    RoleId = roles.First(q => q.Name == "Admin").Id
                }
            };

            return new IdentityTestDataContainer(user, profile, roles, userRoles);
        }
    }


    internal class IdentityTestDataContainer {
        internal User User { get; }
        internal Profile UserProfile { get; }
        internal IEnumerable<IdentityRole<Guid>> Roles { get; }
        internal IEnumerable<IdentityUserRole<Guid>> UserRoles { get; }
        internal IdentityTestDataContainer(User user, Profile profile,
            IEnumerable<IdentityRole<Guid>> roles, IEnumerable<IdentityUserRole<Guid>> userRoles) {
            User = user;
            UserProfile = profile;
            Roles = roles;
            UserRoles = userRoles;
        }
    }
}
