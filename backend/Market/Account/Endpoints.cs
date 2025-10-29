using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Market.Account;

public static partial class Account {

  public static void AddAccountEndpoints(this WebApplication app) {

    var router = app.MapGroup("/")
      .RequireAuthorization()
      .AddFluentValidationAutoValidation()
      .WithOpenApi()
      .WithTags(["Account"]);

    router.MapGet("/api/v1/account", GetAccount).RequireAuthorization();
    router.MapGet("/api/v1/accounts/{name}", GetAccountsByName).RequireAuthorization();
    router.MapPost("/api/v1/account", AddAccount).RequireAuthorization();
    router.MapPut("/api/v1/account/{accountId:guid}", UpdateAccount).RequireAuthorization();
    router.MapPatch("/api/v1/account/{accountId:guid}/developer", UpdateIsDeveloperStatus).RequireAuthorization();
    router.MapPatch("/api/v1/brand", UpdateBrand).RequireAuthorization();
    router.MapPut("/api/v1/active-account", UpdateActiveAccount).RequireAuthorization();
    router.MapGet("/api/v1/account/options", GetAccountOptions).RequireAuthorization();


    router.MapPost("/api/v1/brand/avatar", General.CreateAvatar<DB.Entities.Brand>).RequireAuthorization().DisableAntiforgery();
    router.MapDelete("/api/v1/brand/avatar", General.DeleteAvatar<DB.Entities.Brand>).RequireAuthorization();

    router.MapPost("/api/v1/account/invitation/invite", CreateInvitation).RequireAuthorization();
    router.MapGet("/api/v1/account/invitation/invite-accept", AcceptInvitation).AllowAnonymous();
    router.MapPut("/api/v1/account/invitation/update-roles", UpdateRoleInvitation);
    router.MapDelete("/api/v1/account/invitation/remove", RemoveInvitation);
  }
}


