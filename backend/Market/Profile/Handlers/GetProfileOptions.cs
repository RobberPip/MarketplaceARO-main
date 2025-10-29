using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using DB.Entities;

namespace Market.Profile;

public static partial class Profile {

    public static async Task<Results<
            Ok<ProfileOptionsOut>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
          >>
          GetProfileOptions(AppDbContext db, Ctx ctx) {
        // TODO
        var roles = await db.ProfileRoles.Select(r => new ProfileRoleOut { Id = r.Id, Name = r.Name, Category = r.Category }).ToListAsync();
        var industries = await db.CompanyIndustries.Select(i => new ProfileIndustryOut { Id = i.Id, Name = i.Name }).OrderBy(r => r.Name).ToListAsync();
        var companies = await db.Companies.Where(c => c.Status == CompanyStatus.Approved).Select(c => ProfileCompanyOut.From(c)).ToListAsync();

        var options = new ProfileOptionsOut {
            Roles = roles,
            Industries = industries,
            Companies = companies,
        };

        return TypedResults.Ok(options);
    }
}

