using System.ComponentModel.DataAnnotations;
using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using DB.Entities;
using Market.Account;
using static Market.General;



namespace Market.Profile;

public class UserProfileOut {
    public required Guid UserId { get; set; }
    public required string UserEmail { get; set; }
    public string Login { get; set; }

    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Description { get; set; }
    public required AvatarOut Avatar { get; set; }
    public ProfileOptionsOut? Options { get; set; }

    public ProfileCompanyOut? Company { get; set; }
    public ProfileIndustryOut? Industry { get; set; }
    public ProfileRoleOut? Role { get; set; }

    public required DateTime? UpdatedAt { get; set; }
    public required DateTime CreatedAt { get; set; }
    public static UserProfileOut From(DB.Entities.Profile source) => new() {
        UserId = source.Id,
        UserEmail = source.Email ?? "",
        Login = source.Login ?? "",
        FirstName = source.FirstName,
        LastName = source.LastName,
        Avatar = source.Avatar is not null ? AvatarOut.From(source.Avatar) : new AvatarOut(),
        UpdatedAt = source.UpdatedAt,
        CreatedAt = source.CreatedAt
    };
}

public static partial class Profile {

    public static async Task<Results<
        Ok<UserProfileOut>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
      >>
      GetProfile(AppDbContext db, Ctx ctx) {
        var profile = await ctx.GetCurrentProfile();
        if (profile == null) {
            return TypedResults.InternalServerError(new ErrorResponse { Detail = "Profile not found." });
        }
        var roles = await db.ProfileRoles.Select(r => new ProfileRoleOut { Id = r.Id, Name = r.Name, Category = r.Category }).ToListAsync();
        var industries = await db.CompanyIndustries.Select(i => new ProfileIndustryOut { Id = i.Id, Name = i.Name }).OrderBy(r => r.Name).ToListAsync();
        var companies = await db.Companies.Where(c => c.Status == CompanyStatus.Approved || c.CreatedBy == profile.Id).Select(c => ProfileCompanyOut.From(c)).ToListAsync();

        var options = new ProfileOptionsOut {
            Roles = roles,
            Industries = industries,
            Companies = companies,
        };


        var res = new UserProfileOut {
            UserId = profile.Id,
            UserEmail = profile.Email ?? "",
            Login = profile.Login ?? "",
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Avatar = new AvatarOut(),
            Description = profile.Description,
            Company = null,
            Role = null,
            Industry = null,
            Options = options,
            CreatedAt = profile.CreatedAt,
            UpdatedAt = profile.UpdatedAt,

        };

        if (profile.ProfileCompany != null) {
            res.Company = new ProfileCompanyOut {
                Id = profile.ProfileCompany.Id,
                Name = profile.ProfileCompany.Name,
                Status = profile.ProfileCompany.Status
            };
        }

        if (profile.ProfileIndustry != null) {
            res.Industry = new ProfileIndustryOut {
                Id = profile.ProfileIndustry.Id,
                Name = profile.ProfileIndustry.Name,
            };
        }

        if (profile.ProfileRole != null) {
            res.Role = new ProfileRoleOut {
                Id = profile.ProfileRole.Id,
                Name = profile.ProfileRole.Name,
            };
        }

        // TODO
        var avatar = await db.Files.Where(a => a.Id == profile.AvatarId).FirstOrDefaultAsync();
        if (avatar != null) {
            res.Avatar.Url = $"/api/v1/files/{avatar.Id}";
            res.Avatar.UpdatedAt = avatar.UpdatedAt;
        }

        return TypedResults.Ok(res);
    }
}



public class ProfileRoleOut {
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Category { get; set; }
    public static ProfileRoleOut From(ProfileRole source) => new() {
        Id = source.Id,
        Name = source.Name,
        Category = source.Category
    };
}

public class ProfileIndustryOut {
    public Guid Id { get; set; }
    public string? Name { get; set; }

    public static ProfileIndustryOut From(ProfileIndustry source) => new() {
        Id = source.Id,
        Name = source.Name,
    };
}

public class ProfileCompanyOut {
    public required Guid Id { get; set; }
    public required string? Name { get; set; }
    public required CompanyStatus Status { get; set; }
    public static ProfileCompanyOut From(DB.Entities.Company source) => new() {
        Id = source.Id,
        Name = source.Name,
        Status = source.Status,
    };
}

public class ProfileOptionsOut {
    [Required]
    public List<ProfileRoleOut> Roles { get; set; } = [];
    [Required]
    public List<ProfileIndustryOut> Industries { get; set; } = [];
    [Required]
    public List<ProfileCompanyOut> Companies { get; set; } = [];
}
