using Auth;
using DB;
using DB.Entities;
using Market;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Services;
using Market.ServiceExtensions;

namespace Market.Account;

public static partial class Account {
    public static async Task<IResult> CreateInvitation(
    [FromQuery] string email,
    [FromQuery] AccountRole role,
    AppDbContext db,
    Ctx ctx,
    InvitationService tokenService,
    HttpContext httpContext,
    IHtmlParser _htmlParser,
    EmailSender _emailSender) {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) {
            return Results.BadRequest("Invalid email.");
        }

        var account = await ctx.GetCurrentAccount();
        if (account == null) {
            return Results.Unauthorized();
        }

        var profile = await ctx.GetCurrentProfile();
        if (profile == null) {
            return Results.Unauthorized();
        }

        var userAccountProfile = account.Invitations
            .FirstOrDefault(ap => ap.ProfileId == profile.Id);

        if (userAccountProfile == null || userAccountProfile.Role != AccountRole.Owner) {
            return Results.Forbid();
        }

        var existingProfile = await db.Profiles.FirstOrDefaultAsync(p => p.Email == email);

        var existingInvite = await db.Invitations
            .FirstOrDefaultAsync(ap => ap.AccountId == account.Id && ap.Email.ToLower() == email.ToLower());

        if (existingInvite != null) {
            if (existingInvite.Status == InvitationStatus.Accepted) {
                return Results.Conflict("Пользователь уже состоит в аккаунте.");
            }

            db.Invitations.Remove(existingInvite);
            await db.SaveChangesAsync();
        }

        var accountProfile = new Invitations {
            Id = Guid.NewGuid(),
            AccountId = account.Id,
            Email = email,
            ProfileId = existingProfile?.Id,
            Role = role,
            Status = InvitationStatus.Invited,
            CreatedAt = DateTime.UtcNow
        };

        db.Invitations.Add(accountProfile);
        await db.SaveChangesAsync();

        var token = tokenService.GenerateToken(email);
        var host = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
        var url = $"{host}/api/v1/account/invitation/invite-accept?invitationId={accountProfile.Id}&token={Uri.EscapeDataString(token)}";

        var mail = _htmlParser.GetEmailPageAsString("InviteAccount.html", url, new Dictionary<string, string>
        {{ "accountName", account.FullName } });

        await _emailSender.SendAsync(email, "Приглашение в аккаунт", mail);

        return Results.Ok();
    }

    public static async Task<IResult> AcceptInvitation(
    [FromQuery] Guid invitationId,
    [FromQuery] string token,
    AppDbContext db,
    Ctx ctx,
    InvitationService invitationAcceptanceService) {
        var invitation = await db.Invitations.FirstOrDefaultAsync(i => i.Id == invitationId);
        if (invitation == null)
            return Results.NotFound("Invitation not found.");

        if (string.IsNullOrWhiteSpace(invitation.Email))
            return Results.BadRequest("Invitation email is missing.");
        var currentProfile = await ctx.GetCurrentProfile(false);
        var profileByEmail = await db.Profiles.FirstOrDefaultAsync(p => p.Email == invitation.Email);

        if (currentProfile == null) {
            if (profileByEmail == null) {
                var registerUrl = $"/app/signup?invitationId={invitationId}&token={token}";
                return Results.Redirect(registerUrl);
            } else {
                var loginUrl = $"/app/login?invitationId={invitationId}&token={token}";
                return Results.Redirect(loginUrl);
            }
        }

        if (!string.Equals(currentProfile.Email, invitation.Email, StringComparison.OrdinalIgnoreCase))
            return Results.Forbid();

        if (invitation.Status == InvitationStatus.Accepted)
            return Results.Redirect("/softwares");

        var success = await invitationAcceptanceService.TryAcceptInvitationAsync(invitationId, token, currentProfile);
        if (!success)
            return Results.BadRequest("Failed to accept invitation.");

        return Results.Redirect("/softwares");
    }
    public static async Task<IResult> UpdateRoleInvitation(
    [FromBody] List<AccountRoleUpdateItem> request,
    AppDbContext db,
    Ctx ctx) {
        var profile = await ctx.GetCurrentProfile();
        if (profile == null)
            return Results.Unauthorized();

        var account = await ctx.GetCurrentAccount();
        if (account == null)
            return Results.Unauthorized();

        var userAccountProfile = account.Invitations
            .FirstOrDefault(ap => ap.ProfileId == profile.Id);

        if (userAccountProfile == null || userAccountProfile.Role != AccountRole.Owner)
            return Results.Forbid();

        var ids = request.Select(r => r.AccountProfileId).ToList();

        var invitationsToUpdate = await db.Invitations
            .Where(ap => ids.Contains(ap.Id) && ap.AccountId == account.Id)
            .ToListAsync();

        if (!invitationsToUpdate.Any())
            return Results.NotFound("Ни один из указанных пользователей не найден в аккаунте.");

        foreach (var ap in invitationsToUpdate) {
            var update = request.FirstOrDefault(r => r.AccountProfileId == ap.Id);
            if (update != null) {
                ap.Role = update.Role;
            }
        }

        await db.SaveChangesAsync();
        return Results.Ok();
    }
    public static async Task<IResult> RemoveInvitation(
    [FromQuery] Guid invitationId,
    AppDbContext db,
    Ctx ctx) {
        var profile = await ctx.GetCurrentProfile();
        if (profile == null)
            return Results.Unauthorized();

        var account = await ctx.GetCurrentAccount();
        if (account == null)
            return Results.Unauthorized();

        var userAccountProfile = account.Invitations
            .FirstOrDefault(ap => ap.ProfileId == profile.Id);

        if (userAccountProfile == null || userAccountProfile.Role != AccountRole.Owner)
            return Results.Forbid();

        if (userAccountProfile.Id == invitationId)
            return Results.BadRequest("Нельзя удалить самого себя.");

        var target = await db.Invitations
            .Include(ap => ap.Profile)
            .FirstOrDefaultAsync(ap => ap.Id == invitationId && ap.AccountId == account.Id);

        if (target == null)
            return Results.NotFound("Профиль не найден в этом аккаунте.");

        if (target.Profile != null) {
            if (profile != null && profile.ActiveAccountId == account.Id) {
                var otherAccountProfile = await db.Invitations
                    .Where(ap => ap.ProfileId == target.Profile.Id && ap.AccountId != account.Id)
                    .FirstOrDefaultAsync();

                profile.ActiveAccountId = otherAccountProfile?.AccountId;
            }
        }

        db.Invitations.Remove(target);
        await db.SaveChangesAsync();

        return Results.Ok();
    }



    public class AccountRoleUpdateItem {
        public Guid AccountProfileId { get; set; }
        public AccountRole Role { get; set; }
    }

}