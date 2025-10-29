using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using DB.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Market.Profile;

public class UpdateProfileIn {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Login { get; set; }
    public string? Description { get; set; }
    public Guid? CompanyId { get; set; }
    public Guid? IndustryId { get; set; }
    public Guid? RoleId { get; set; }
    public string? CompanyName { get; set; }
}

// public class CreateTodoInValidator : AbstractValidator<UpdateProfileIn> {
//     public CreateTodoInValidator() {
//         RuleFor(s => s.FirstName)
//             .NotNull().WithMessage("Имя обязательно для заполнения.")
//             .MinimumLength(3).WithMessage("Имя должно содержать минимум 3 символа.");

//         RuleFor(s => s.LastName)
//             .NotNull().WithMessage("Фамилия обязательна для заполнения.")
//             .MinimumLength(3).WithMessage("Фамилия должна содержать минимум 3 символа.");
//     }
// }

public class UpdateProfileOut {
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Login { get; set; }
    public string? Description { get; set; }

    public ProfileCompanyOut? Company { get; set; }
    public ProfileIndustryOut? Industry { get; set; }
    public ProfileRoleOut? Role { get; set; }

    public required Guid UserId { get; set; }
    public required Guid ProfileId { get; set; }
    public required string UserEmail { get; set; }

    public required DateTime UpdatedAt { get; set; }
    public required DateTime CreatedAt { get; set; }
}
public static partial class Profile {

    public static async Task<Results<
          Ok<UpdateProfileOut>,
          BadRequest<ValidationProblemDetails>,
          NotFound<ErrorResponse>,
          InternalServerError<ErrorResponse>
        >> UpdateProfile(
          AppDbContext db, Ctx ctx, UpdateProfileIn updateDto
      ) {
        var profile = await ctx.GetCurrentProfile();

        if (profile is null) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "UserProfile is not found."
            });
        }


        if (updateDto.IndustryId.HasValue) {
            var industryExists = await db.CompanyIndustries
                .AnyAsync(i => i.Id == updateDto.IndustryId.Value);
            if (!industryExists) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "CompanyIndustry does not exist."
                });
            }
        }

        if (updateDto.RoleId.HasValue) {
            var profileRoleExists = await db.ProfileRoles
                .AnyAsync(i => i.Id == updateDto.RoleId.Value);
            if (!profileRoleExists) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "Profile role does not exist."
                });
            }
        }

        profile.FirstName = updateDto.FirstName ?? profile.FirstName;
        profile.LastName = updateDto.LastName ?? profile.LastName;
        profile.Login = !string.IsNullOrWhiteSpace(updateDto.Login) ? updateDto.Login : profile.Login;
        profile.Description = updateDto.Description ?? profile.Description;
        profile.ProfileIndustryId = updateDto.IndustryId ?? profile.ProfileIndustryId;
        profile.ProfileRoleId = updateDto.RoleId ?? profile.ProfileRoleId;
        profile.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        profile = await ctx.GetCurrentProfile();
        if (profile == null) {
            return TypedResults.InternalServerError(new ErrorResponse { Detail = "Profile not found." });
        }

        var res = new UpdateProfileOut {
            UserId = profile.Id,
            ProfileId = profile.Id,
            UserEmail = profile?.Email ?? "",
            Login = profile?.Login ?? "",

            FirstName = profile?.FirstName ?? "",
            LastName = profile?.LastName ?? "",

            Company = null,
            Role = null,
            Industry = null,

            Description = profile?.Description ?? "",
            CreatedAt = profile?.CreatedAt ?? DateTime.MinValue,
            UpdatedAt = profile?.UpdatedAt ?? DateTime.MinValue,
        };

        if (updateDto.CompanyId != null) {
            profile.CurrentCompanyId = updateDto.CompanyId ?? profile.CurrentCompanyId;
            await db.SaveChangesAsync();
        } else if (!string.IsNullOrWhiteSpace(updateDto.CompanyName)) {
            var company = db.Companies.FirstOrDefault(c => c.Name == updateDto.CompanyName && c.Status == CompanyStatus.Approved);

            profile.CurrentCompanyId = company?.Id ?? Guid.NewGuid();

            db.Companies.Add(new DB.Entities.Company {
                Id = profile.CurrentCompanyId.Value,
                Name = updateDto.CompanyName,
                IsRegistry = false,
                Status = CompanyStatus.Init,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CreatedBy = profile.Id,
                UpdatedBy = profile.Id,
            });
            await db.SaveChangesAsync();
        }


        // TODO: handle nulls
        if (profile.CurrentCompanyId != null) {
            var company = (await ctx.GetCurrentProfile()).ProfileCompany;
            res.Company = new ProfileCompanyOut {
                Id = company.Id,
                Name = company.Name,
                Status = company.Status
            };
        }

        if (profile?.ProfileIndustry != null) {
            res.Industry = new ProfileIndustryOut {
                Id = profile.ProfileIndustry.Id,
                Name = profile.ProfileIndustry.Name,
            };
        }

        if (profile?.ProfileRole != null) {
            res.Role = new ProfileRoleOut {
                Id = profile.ProfileRole.Id,
                Name = profile.ProfileRole.Name,
            };
        }

        return TypedResults.Ok(res);
    }
}
