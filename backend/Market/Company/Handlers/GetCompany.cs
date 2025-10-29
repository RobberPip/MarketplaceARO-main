using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using DB.Entities;

namespace Market.Company;
public class CompanyOut {
    public required Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public required CompanyStatus Status { get; set; }
    /// <summary>
    /// Наличие в реестре ARO
    /// </summary>
    public required bool IsRegistry { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public static CompanyOut From(DB.Entities.Company source) => new() {
        Id = source.Id,
        Name = source.Name,
        Description = source.Description,
        Status = source.Status,
        IsRegistry = source.IsRegistry,
        CreatedAt = source.CreatedAt,
        UpdatedAt = source.UpdatedAt
    };
}
public static partial class Company {
    public static async Task<Results<
            Ok<List<CompanyOut>>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
        >>
        GetCompanies(AppDbContext db, CompanyStatus status) {
        var companies = await db.Companies.Where(c => c.Status == status).ToListAsync();
        var companyOutList = companies.Select(c => CompanyOut.From(c)).ToList();
        return TypedResults.Ok(companyOutList);
    }
}