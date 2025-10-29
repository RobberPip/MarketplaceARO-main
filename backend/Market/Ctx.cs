using System.Security.Claims;
using Db.Entities;
using DB;
using DB.Entities;
using Market.ServiceExtensions;
using Microsoft.EntityFrameworkCore;

namespace Market;


public class Ctx(HttpContext httpContext, AppDbContext db, InvitationService invitationTokenService) {
    public ClaimsPrincipal User => httpContext.User;
    public async Task<T?> GetEntityAsync<T>() where T : class, Db.Entities.IAvatarOwner {
        if (typeof(T) == typeof(DB.Entities.Profile)) {
            var profile = await GetCurrentProfile();
            if (profile is T typedAccount)
                return typedAccount;
            return null;
        }

        if (typeof(T) == typeof(DB.Entities.Brand)) {
            var account = await GetCurrentAccount();
            if (account?.Brand is T brand)
                return brand;
            return null;
        }

        throw new InvalidOperationException($"Unsupported type {typeof(T).Name}");
    }

    public Guid UserId =>
        new(User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                 ?? throw new UnauthorizedAccessException("User ID claim is missing."));
    public Guid? TryGetUserId() {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);
        return Guid.TryParse(claim?.Value, out var id) ? id : null;
    }

    public async Task<User?> CurrentUser() {
        return await db.Users.FindAsync(UserId);
    }

    public async Task<DB.Entities.Profile?> GetCurrentProfile(bool throwIfMissing = true) {
        var userId = TryGetUserId();
        if (userId == null) {
            if (throwIfMissing)
                throw new UnauthorizedAccessException("User ID claim is missing.");
            return null;
        }

        return await db.Profiles
            .Where(up => up.Id == userId.Value)
            .Include(up => up.ProfileCompany)
            .Include(up => up.ProfileRole)
            .Include(up => up.ProfileIndustry)
            .FirstOrDefaultAsync();
    }
    public async Task<DB.Entities.Account?> GetCurrentAccount() {
        var userProfile = await GetCurrentProfile();
        if (userProfile == null) throw new UnauthorizedAccessException("User not found.");
        if (userProfile.ActiveAccountId == null)
            return null;
        await invitationTokenService.CleanupExpiredInvitationsAsync(userProfile.ActiveAccountId.Value);
        return await db.Accounts.Where(up => up.Id == userProfile.ActiveAccountId.Value)
        .Include(a => a.Brand!).ThenInclude(b => b.PublicLists)
        .Include(a => a.Brand!).ThenInclude(b => b.Avatar)
        .Include(a => a.Brand!).ThenInclude(b => b.MainActivities)
        .Include(a => a.Invitations).ThenInclude(b => b.Profile).ThenInclude(b => b.Avatar)
        .FirstOrDefaultAsync();
    }
}