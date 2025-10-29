using DB.Entities;
using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ARO.Integration.Gate.Internal;

namespace Market.Account;


public class AddAccountIn {
    // Common required fields
    public required AccountType Type { get; set; }
    public bool? IsDeveloper { get; set; }
    public required string? INN { get; set; }
    public required string? FullName { get; set; }
    public string? ShortName { get; set; }
    // public required bool CheckedRules { get; set; }
    // Optional required fields
    public string? Address { get; set; }
    public string? KPP { get; set; }
    public string? OGRN { get; set; }
    public required UpdateBrandIn Brand { get; set; }
}
public class UpdateAccountIn {
    // Common required fields
    public required AccountType Type { get; set; }
    public required string? INN { get; set; }
    public required string? FullName { get; set; }
    public string? ShortName { get; set; }
    // public required bool CheckedRules { get; set; }
    // Optional required fields
    public string? Address { get; set; }
    public string? KPP { get; set; }
    public string? OGRN { get; set; }
}
public class UpdateBrandIn {
    public required string PublicName { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public List<Guid>? PublicListIds { get; set; }
    public List<Guid>? MainActivityIds { get; set; }
}
public class PublicListOut {
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public static PublicListOut From(PublicList source) => new() {
        Id = source.Id,
        Name = source.Name,
    };
}
public static partial class Account {

    public static async Task<Results<
        Ok<GetAccountOut>,
        BadRequest<ValidationProblemDetails>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
        >> AddAccount(
          AppDbContext db, Ctx ctx, AddAccountIn accountDto
      ) {
        var profile = await ctx.GetCurrentProfile();

        if (profile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "UserProfile is not found."
            });
        }
        // if (!accountDto.CheckedRules) {
        //     return TypedResults.BadRequest(new ValidationProblemDetails {
        //         Title = "Validation Error",
        //         Errors = {
        //         { nameof(accountDto.CheckedRules), new[] { "Rules must be accepted." } }
        //     }
        //     });
        // }
        var existingAccounts = await db.Invitations
       .Include(ap => ap.Account)
       .Where(ap => ap.ProfileId == profile.Id &&
        (ap.Account.Type == AccountType.SelfEmployed || ap.Account.Type == AccountType.IndividualEntrepreneur))
       .Select(ap => ap.Account.Type)
       .ToListAsync();

        if ((accountDto.Type == AccountType.SelfEmployed || accountDto.Type == AccountType.IndividualEntrepreneur)
            && existingAccounts.Any()) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Validation Error",
                Errors = {
                { nameof(accountDto.Type), new[] { "You already have an account with this type (SelfEmployed or IndividualEntrepreneur)." } }
            }
            });
        }
        if ((accountDto.Type == AccountType.IndividualEntrepreneur || accountDto.Type == AccountType.LegalEntity) &&
        string.IsNullOrWhiteSpace(accountDto.Address)) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Validation Error",
                Errors = {
            { nameof(accountDto.Address), new[] { "Address is required for Individual Entrepreneur and Legal Entity." } }
        }
            });
        }
        if (accountDto.Type == AccountType.LegalEntity &&
            (string.IsNullOrWhiteSpace(accountDto.OGRN) || string.IsNullOrWhiteSpace(accountDto.KPP))) {

            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Validation Error",
                Errors = {
            { nameof(accountDto.KPP), new[] { "KPP and OGRN is required for Legal entity." } }
        }
            });
        }
        if (!string.IsNullOrWhiteSpace(accountDto.Brand.PublicName)) {
            var brandExists = await db.Brands
                .AnyAsync(b => b.PublicName == accountDto.Brand.PublicName);

            if (brandExists) {
                return TypedResults.BadRequest(new ValidationProblemDetails {
                    Title = "Validation Error",
                    Errors =
                    {
                        { nameof(accountDto.Brand.PublicName), new[] { "Brand with this name already exists." } }
                    }
                });
            }
        }
        List<PublicList> publicLists = new List<PublicList>();
        if (accountDto.Brand.PublicListIds != null && accountDto.Brand.PublicListIds.Any()) {
            publicLists = await db.PublicList
                .Where(pl => accountDto.Brand.PublicListIds.Contains(pl.Id))
                .ToListAsync();
        }
        List<MainActivity> mainActivity = new List<MainActivity>();
        if (accountDto.Brand.MainActivityIds != null && accountDto.Brand.MainActivityIds.Any()) {
            mainActivity = await db.MainActivity
                .Where(pl => accountDto.Brand.MainActivityIds.Contains(pl.Id))
                .ToListAsync();
        }
        var brand = new Brand {
            Id = Guid.NewGuid(),
            Status = BrandStatus.Pending,
            PublicName = accountDto.Brand.PublicName,
            Description = accountDto.Brand.Description,
            MainActivities = mainActivity,
            Website = accountDto.Brand.Website,
            Email = accountDto.Brand.Email,
            PhoneNumber = accountDto.Brand.PhoneNumber,
            PublicLists = publicLists,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        var entity = new DB.Entities.Account {
            Brand = brand,
            Status = AccountStatus.Pending,
            IsDeveloper = false,
            Type = accountDto.Type,
            INN = accountDto.INN,
            ShortName = accountDto.ShortName,
            FullName = accountDto.FullName,
            Address = accountDto.Address,
            KPP = accountDto.KPP,
            OGRN = accountDto.OGRN,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

        };
        entity.Invitations.Add(new Invitations {
            AccountId = entity.Id,
            Email = profile.Email,
            ProfileId = profile.Id,
            Role = AccountRole.Owner,
            Status = InvitationStatus.Accepted,
            CreatedAt = DateTime.UtcNow,
            AcceptedAt = DateTime.UtcNow

        });

        await db.Accounts.AddAsync(entity);
        await db.SaveChangesAsync();
        // TODO logger
        //logger.LogInformation($"Account with Id {entity.Id} was created for user {profile.Id}");

        var userProfile = await ctx.GetCurrentProfile();
        if (userProfile != null) {
            userProfile.ActiveAccountId = entity.Id;
            await db.SaveChangesAsync();
        }
        var res = GetAccountOut.From(entity);
        return TypedResults.Ok(res);
    }
    public static async Task<Results<
        Ok<GetBrandOut>,
        BadRequest<ValidationProblemDetails>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
    >> UpdateBrand(
        AppDbContext db, Ctx ctx, UpdateBrandIn brandDto, IConfiguration configuration
    ) {
        var account = await ctx.GetCurrentAccount();
        if (account is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Account is not found."
            });
        }
        var profile = await ctx.GetCurrentProfile();
        if (profile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Profile is not found."
            });
        }

        var userAccountProfile = account.Invitations
            .FirstOrDefault(ap => ap.ProfileId == profile.Id);

        if (userAccountProfile == null || userAccountProfile.Role != AccountRole.Owner) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Role error",
                Errors =
                    {
                        { nameof(brandDto.PublicName), new[] { "Role error is not Owner" } }
                    }
            });
        }
        var brand = account.Brand;

        if (brand == null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Brand is not found for the current account."
            });
        }
        if (brand.Status == BrandStatus.Pending) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Error status",
                Errors =
                    {
                        { nameof(brandDto.PublicName), new[] { "Status is pending" } }
                    }
            });
        }
        if (!string.IsNullOrWhiteSpace(brandDto.PublicName)) {
            var brandExists = await db.Brands
                .AnyAsync(b => b.PublicName == brandDto.PublicName && b.Id != brand.Id);

            if (brandExists) {
                return TypedResults.BadRequest(new ValidationProblemDetails {
                    Title = "Validation Error",
                    Errors =
                        {
                        { nameof(brandDto.PublicName), new[] { "Brand with this name already exists." } }
                    }
                });
            }
        }
        var aroToken = configuration["AROIntegrationToken"];
        var devUID = Gateway.GetDeveloperUID(aroToken, profile.Id);
        if (!string.IsNullOrEmpty(brandDto.PublicName) && devUID != null) {
            var successName = Gateway.ChangeDeveloperName(aroToken, profile.Id, devUID, brandDto.PublicName);
            if (!successName) {
                return TypedResults.BadRequest(new ValidationProblemDetails {
                    Status = 400,
                    Title = "BadRequest",
                    Errors =
                   {
                        { nameof(brandDto.PublicName), new[] { "Unable to change PublicName in external system (ARO)." } }
                    }
                });
            }
        }
        if (!string.IsNullOrEmpty(brandDto.Description) && devUID != null) {
            var successDesc = Gateway.ChangeDeveloperDesc(aroToken, profile.Id, devUID, brandDto.Description);
            if (!successDesc) {
                return TypedResults.BadRequest(new ValidationProblemDetails {
                    Status = 400,
                    Title = "BadRequest",
                    Errors =
                    {
                        { nameof(brandDto.Description), new[] { "Unable to change Description in external system (ARO)." } }
                    }
                });
            }
        }
        brand.Status = BrandStatus.Pending;
        brand.PublicName = brandDto.PublicName ?? brand.PublicName;
        brand.Description = brandDto.Description ?? brand.Description;
        brand.Website = brandDto.Website ?? brand.Website;
        brand.Email = brandDto.Email ?? brand.Email;
        brand.PhoneNumber = brandDto.PhoneNumber ?? brand.PhoneNumber;
        brand.UpdatedAt = DateTime.UtcNow;

        if (brandDto.PublicListIds is not null) {
            var existingLists = await db.PublicList
                .Where(pl => brandDto.PublicListIds.Contains(pl.Id))
                .ToListAsync();
            brand.PublicLists = existingLists;
        }
        if (brandDto.MainActivityIds is not null) {
            var existingLists = await db.MainActivity
                .Where(pl => brandDto.MainActivityIds.Contains(pl.Id))
                .ToListAsync();
            brand.MainActivities = existingLists;
        }
        await db.SaveChangesAsync();

        return TypedResults.Ok(GetBrandOut.From(brand));
    }


    public static async Task<Results<
        Ok<GetAccountOut>,
        BadRequest<ValidationProblemDetails>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
    >> UpdateActiveAccount(AppDbContext db, Ctx ctx, Guid? accountId) {
        var userProfile = await ctx.GetCurrentProfile();
        if (userProfile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "User is not found."
            });
        }

        if (accountId is null || accountId == Guid.Empty) {
            return TypedResults.BadRequest(new ValidationProblemDetails(new Dictionary<string, string[]> {
            { "accountId", new[] { "Account ID must not be null or empty." } }
        }));
        }
        var account = await db.Accounts.FirstOrDefaultAsync(a => a.Id == accountId);
        if (account is null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = $"Account with ID '{accountId}' not found."
            });
        }
        var publicLists = await db.PublicList
           .Select(pl => new GetPublicListOut {
               Id = pl.Id,
               Name = pl.Name
           })
           .ToListAsync();
        var options = new AccountOptionsOut {
            PublicLists = publicLists
        };
        userProfile.ActiveAccountId = accountId;
        userProfile.UpdatedAt = DateTime.UtcNow;
        var activeAccount = await ctx.GetCurrentAccount();
        try {
            await db.SaveChangesAsync();
            if (activeAccount is null) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = $"Active account with ID '{accountId}' not found."
                });
            }
            return TypedResults.Ok(GetAccountOut.From(activeAccount, options, userProfile.Id));

        } catch {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "Failed to update active account.",
            });
        }
    }
    public static async Task<Results<
    Ok<GetAccountOut>,
    BadRequest<ValidationProblemDetails>,
    NotFound<ErrorResponse>,
    InternalServerError<ErrorResponse>>> UpdateIsDeveloperStatus(
    Guid accountId,
    AppDbContext db,
    Ctx ctx,
    bool isDeveloper,
    IConfiguration configuration) {
        var profile = await ctx.GetCurrentProfile();
        if (profile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "UserProfile is not found."
            });
        }

        var account = await db.Accounts
            .Include(a => a.Invitations)
            .Include(a => a.Brand)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account is null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Account not found."
            });
        }

        var isOwner = account.Invitations.Any(ap => ap.ProfileId == profile.Id && ap.Role == AccountRole.Owner);
        if (!isOwner) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Access Denied",
                Detail = "You are not the owner of this account."
            });
        }
        if (isDeveloper && account.IsDeveloper) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Validation Error",
                Errors = {
                { nameof(isDeveloper), new[] { "You cannot become a developer more than once." } }
            }
            });
        }
        // if (isDeveloper && !account.IsDeveloper) {
        //     var devName = account?.Brand?.PublicName;
        //     var devDesc = account?.Brand?.Description;
        //     var aroToken = configuration["AROIntegrationToken"];
        //     var aroResultUid = Gateway.SetDeveloperRole(aroToken, profile.Id, devName, devDesc);

        //     if (aroResultUid is null) {
        //         return TypedResults.BadRequest(new ValidationProblemDetails {
        //             Title = "ARO Integration Error",
        //             Errors = {
        //             { "ARO", new[] { "Unable to create developer role in external system (ARO)." } }
        //         }
        //         });
        //     }
        // }
        account.IsDeveloper = isDeveloper;
        account.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        var result = GetAccountOut.From(account);
        return TypedResults.Ok(result);
    }
    public static async Task<Results<
        Ok<GetAccountOut>,
        NotFound<ErrorResponse>,
        BadRequest<ValidationProblemDetails>,
        InternalServerError<ErrorResponse>>> UpdateAccount(
        Guid accountId,
        AppDbContext db,
        Ctx ctx,
        UpdateAccountIn accountDto) {
        var profile = await ctx.GetCurrentProfile();
        if (profile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "UserProfile is not found."
            });
        }

        var account = await db.Accounts
            .Include(a => a.Brand)
            .Include(a => a.Invitations)
            .FirstOrDefaultAsync(a => a.Id == accountId);

        if (account is null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Account not found."
            });
        }

        var isOwner = account.Invitations
            .Any(ap => ap.ProfileId == profile.Id && ap.Role == AccountRole.Owner);
        if (!isOwner) {
            return TypedResults.BadRequest(new ValidationProblemDetails {
                Title = "Access Denied",
                Detail = "You are not the owner of this account."
            });
        }
        account.Type = accountDto.Type;
        account.INN = accountDto.INN ?? account.INN;
        account.ShortName = accountDto.ShortName ?? account.ShortName;
        account.FullName = accountDto.FullName ?? account.FullName;
        account.Address = accountDto.Address ?? account.Address;
        account.KPP = accountDto.KPP ?? account.KPP;
        account.OGRN = accountDto.OGRN ?? account.OGRN;
        account.UpdatedAt = DateTime.UtcNow;
        account.Status = AccountStatus.Pending;

        await db.SaveChangesAsync();

        var result = GetAccountOut.From(account);
        return TypedResults.Ok(result);
    }
}
