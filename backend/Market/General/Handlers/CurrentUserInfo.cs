using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Db.Entities;
using System.Security.Claims;
using DB.Entities;

namespace Market;

public static partial class General {
    public static async Task<IResult> GetCurrentUserInfo(
        ClaimsPrincipal userPrincipal,
        UserManager<User> userManager,
        AppDbContext dbContext) {
        var user = await userManager.GetUserAsync(userPrincipal);
        if (user is null)
            return Results.Unauthorized();

        var roles = await userManager.GetRolesAsync(user);
        var profile = await dbContext.Profiles.FirstOrDefaultAsync(p => p.Id == user.Id);

        if (profile is null)
            return Results.NotFound(new ErrorResponse(404, "Profile not found"));

        var accounts = await dbContext.Accounts
            .Include(a => a.Brand)
            .ThenInclude(b => b.Avatar)
            .Include(a => a.Invitations)
            .Where(a => a.Invitations.Any(ap => ap.ProfileId == profile.Id && ap.Status == InvitationStatus.Accepted))
            .ToListAsync();

        var avatar = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == profile.AvatarId);
        var avatarUrl = avatar != null ? $"/api/v1/files/{avatar.Id}" : null;

        var dto = new CurrentUserInfoDto {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Login = profile.Login,
            Avatar = avatarUrl,
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            EmailConfirmed = user.EmailConfirmed,
            TwoFactorEnabled = user.TwoFactorEnabled,
            Lockout = user.LockoutEnd != null,
            AccessFailedCount = user.AccessFailedCount,
            Roles = roles.ToList(),
            Accounts = accounts.Select(a => AccountsDto.From(a, user.Id)).ToList(),
            ActiveAccountId = profile.ActiveAccountId,
            IsDevStatus = profile.ActiveAccountId != null
                ? accounts.FirstOrDefault(q => q.Id == profile.ActiveAccountId) != null
                    ? accounts.FirstOrDefault(q => q.Id == profile.ActiveAccountId)!.IsDeveloper
                    : null
                : null,
        };
        return Results.Ok(dto);
    }
    public class CurrentUserInfoDto {
        public Guid Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Login { get; set; }
        public string? Avatar { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool Lockout { get; set; }
        public int AccessFailedCount { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public required List<AccountsDto> Accounts { get; set; }
        public required Guid? ActiveAccountId { get; set; }
        public bool? IsDevStatus { get; set; }
    }
    public class AccountsDto {
        public Guid? Id { get; set; }
        public required string? INN { get; set; }
        public required string? FullName { get; set; }
        public string? ShortName { get; set; }
        public required AccountRole? UserRole { get; set; }
        public required AccountType Type { get; set; }
        public required BrandDto Brand { get; set; }
        public static AccountsDto From(DB.Entities.Account source, Guid userId) => new() {
            Id = source.Id,
            INN = source.INN,
            Type = source.Type,
            ShortName = source.ShortName,
            Brand = source.Brand != null ? BrandDto.From(source.Brand) : null!,
            FullName = source.FullName,
            UserRole = source.Invitations
        .FirstOrDefault(ap => ap.AccountId == source.Id && ap.ProfileId == userId)
        ?.Role ?? null
        };

    }
    public class BrandDto {
        public Guid? Id { get; set; }
        public required AvatarOut Avatar { get; set; }
        public required string? PublicName { get; set; }
        public static BrandDto From(DB.Entities.Brand source) => new() {
            Id = source.Id,
            Avatar = source.Avatar != null
            ? new AvatarOut() {
                Url = $"/api/v1/files/{source.Avatar.Id}",
                UpdatedAt = source.Avatar.UpdatedAt,
            }
            : new AvatarOut() {
                Url = null,
                UpdatedAt = null
            },
            PublicName = source.PublicName
        };
    }
}
