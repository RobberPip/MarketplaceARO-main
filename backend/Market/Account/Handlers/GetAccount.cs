using DB.Entities;
using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Market.Profile;
using static Market.General;

namespace Market.Account;

public class GetPublicListOut {
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
public class GetMainActivityOut {
    public Guid Id { get; set; }
    public string? Name { get; set; }
}
public class AccountOptionsOut {
    [Required]
    public List<GetPublicListOut> PublicLists { get; set; } = [];
    [Required]
    public List<GetMainActivityOut> MainActivities { get; set; } = [];
}
public class GetBrandOut {
    public Guid Id { get; set; }
    public required BrandStatus Status { get; set; }
    public required AvatarOut Avatar { get; set; }
    /// <summary>
    /// Публичное имя
    /// </summary>
    public string? PublicName { get; set; }
    public string? Description { get; set; }
    /// <summary>
    /// Основная деятельность
    /// </summary>
    public List<GetMainActivityOut>? MainActivities { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<GetPublicListOut>? PublicLists { get; set; }

    public static GetBrandOut From(Brand source) => new() {
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
        Status = source.Status,
        PublicName = source.PublicName,
        Description = source.Description,
        MainActivities = source.MainActivities?.Select(ma => new GetMainActivityOut {
            Id = ma.Id,
            Name = ma.Name
        }).ToList(),
        Website = source.Website,
        Email = source.Email,
        PhoneNumber = source.PhoneNumber,
        PublicLists = source.PublicLists?.Select(pl => new GetPublicListOut {
            Id = pl.Id,
            Name = pl.Name
        }).ToList(),
        CreatedAt = source.CreatedAt,
        UpdatedAt = source.UpdatedAt
    };
}
public class GetAccountOut {
    public Guid? Id { get; set; }
    public AccountStatus Status { get; set; }
    public required bool IsDeveloper { get; set; }
    // Common required fields
    public required AccountType Type { get; set; }
    public required string? INN { get; set; }
    public string? ShortName { get; set; }
    public required string? FullName { get; set; }
    // Optional required fields
    public required AccountOptionsOut Options { get; set; }
    public string? Address { get; set; }
    public string? KPP { get; set; }
    public string? OGRN { get; set; }
    public List<InvitationsOut> Invitations { get; set; } = new();
    public AccountRole? UserRole { get; set; }
    public GetBrandOut? Brand { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public static GetAccountOut From(DB.Entities.Account source, AccountOptionsOut? options = null, Guid? userId = null) => new() {
        Id = source.Id,
        Status = source.Status,
        IsDeveloper = source.IsDeveloper,
        ShortName = source.ShortName,
        Type = source.Type,
        INN = source.INN,
        FullName = source.FullName,
        Address = source.Address,
        Brand = source.Brand is not null ? GetBrandOut.From(source.Brand) : null,
        KPP = source.KPP,
        OGRN = source.OGRN,
        CreatedAt = source.CreatedAt,
        UpdatedAt = source.UpdatedAt,
        // правильная логика:
        UserRole = userId is not null
         ? source.Invitations.FirstOrDefault(p => p.ProfileId == userId)?.Role
         : null,
        Options = options ?? new AccountOptionsOut(),
        Invitations = source.Invitations?.Select(InvitationsOut.From).ToList() ?? new()
    };
}
public class GetAccountSummaryOut {
    public Guid? Id { get; set; }
    public string? ShortName { get; set; }
}
public class InvitationsOut {
    public Guid Id { get; set; }
    public string? Email { get; set; }
    public AccountRole Role { get; set; }
    public InvitationStatus Status { get; set; }
    public UserProfileOut? Profile { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? AcceptedAt { get; set; }

    public static InvitationsOut From(Invitations source) => new() {
        Id = source.Id,
        Email = source.Email,
        Role = source.Role,
        Status = source.Status,
        CreatedAt = source.CreatedAt,
        AcceptedAt = source.AcceptedAt,
        Profile = source.Profile is not null ? UserProfileOut.From(source.Profile) : null
    };
}

public static partial class Account {

    public static async Task<Results<
        Ok<GetAccountOut>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
      >>
      GetAccount(AppDbContext db, Ctx ctx) {
        var account = await ctx.GetCurrentAccount();
        if (account is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Account is not found."
            });
        }
        // TODO TEMP INTO AdminPanel ---
        if (account.Status != AccountStatus.Approved && account.CreatedAt <= DateTime.UtcNow.AddHours(-1)) {
            account.Status = AccountStatus.Approved;
            if (account.Brand != null) {
                account.Brand.Status = BrandStatus.Approved;
            }
            db.Accounts.Update(account);
            await db.SaveChangesAsync();
        }

        var invitation = account.Invitations?.FirstOrDefault(p => p.ProfileId == ctx.UserId);
        var userRole = invitation?.Role;

        if (userRole == AccountRole.Owner) {
            if (account.Type == AccountType.SelfEmployed || account.Type == AccountType.IndividualEntrepreneur) {
                var profile = await db.Profiles.FirstOrDefaultAsync(p => p.Id == ctx.UserId);
                if (profile != null) {
                    var parts = (account.FullName ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length >= 2) {
                        profile.LastName = parts[0];
                        profile.FirstName = parts[1];

                        db.Profiles.Update(profile);
                        await db.SaveChangesAsync();
                    }
                }
            }
        }
        // ---


        var publicLists = await db.PublicList
            .Select(pl => new GetPublicListOut {
                Id = pl.Id,
                Name = pl.Name
            })
            .ToListAsync();
        var mainActivities = await db.MainActivity
            .Select(pl => new GetMainActivityOut {
                Id = pl.Id,
                Name = pl.Name
            })
            .ToListAsync();
        var options = new AccountOptionsOut {
            PublicLists = publicLists,
            MainActivities = mainActivities
        };
        var result = GetAccountOut.From(account, options);

        result.UserRole = userRole;
        return TypedResults.Ok(result);
    }
    public static async Task<AccountOptionsOut> GetAccountOptions(AppDbContext db) {
        var publicLists = await db.PublicList
            .Select(pl => new GetPublicListOut {
                Id = pl.Id,
                Name = pl.Name
            })
            .ToListAsync();
        var mainActivities = await db.MainActivity
            .Select(pl => new GetMainActivityOut {
                Id = pl.Id,
                Name = pl.Name
            })
            .ToListAsync();
        return new AccountOptionsOut {
            PublicLists = publicLists,
            MainActivities = mainActivities
        };
    }
    public static async Task<Results<
        Ok<List<GetAccountSummaryOut>>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
      >>
      GetAccountsByName(AppDbContext db, Ctx ctx, string name) {
        var account = await ctx.GetCurrentAccount();
        if (account is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Account is not found."
            });
        }
        var lowerName = name.ToLower();

        var result = await db.Accounts
        .Where(a => a.Id != account.Id && a.ShortName != null && a.ShortName.ToLower().Contains(lowerName) && a.Status == AccountStatus.Approved)
        .Take(10)
        .Select(a => new GetAccountSummaryOut {
            Id = a.Id,
            ShortName = a.ShortName
        }).ToListAsync();

        return TypedResults.Ok(result);
    }
}