// General.AddGeneralEndpoints.cs
using Shared;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Market;

public static partial class General {
  public static void AddGeneralEndpoints(this WebApplication app) {
    var router = app.MapGroup("/")
        .RequireAuthorization()
        .AddFluentValidationAutoValidation()
        .WithOpenApi()
        .WithTags(["General"]);

    router.MapGet("/api/v1/inn-search", GetInfoCompanies).RequireAuthorization();

    router.MapGet("/auth/current-user-info", General.GetCurrentUserInfo)
        .WithName("GetCurrentUserInfo")
        .Produces<CurrentUserInfoDto>(StatusCodes.Status200OK)
        .Produces<ErrorResponse>(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization();
  }
}
