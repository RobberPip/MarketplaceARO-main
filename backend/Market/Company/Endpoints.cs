using DB.Entities;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;

namespace Market.Company;

public static partial class Company {

    public static void AddCompaniesEndpoints(this WebApplication app) {

        var router = app.MapGroup("/")
            .WithOpenApi()
            .WithTags(["Market"]);


        router.MapGet("/api/v1/companies", GetCompanies).WithOpenApi(op => {
            op.Parameters[0].Name = "status";
            op.Parameters[0].Description = "Статус компаний";
            return op;
        }); ;
    }
}