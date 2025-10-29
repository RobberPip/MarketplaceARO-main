using Microsoft.AspNetCore.Builder;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;

namespace Market.Profile;

public static partial class Profile {

  public static void AddProfileEndpoints(this WebApplication app) {

    var router = app.MapGroup("/")
      .RequireAuthorization()
      .AddFluentValidationAutoValidation()
      .WithOpenApi()
      .WithTags(["Profile"]);


    router.MapGet("/api/v1/profile", GetProfile).RequireAuthorization();
    router.MapGet("/api/v1/profile/options", GetProfileOptions).AllowAnonymous();
    // TODO: create separate endpoint for registration only
    router.MapPatch("/api/v1/profile", UpdateProfile);

    router.MapPost("/api/v1/profile/avatar", General.CreateAvatar<DB.Entities.Profile>).RequireAuthorization().DisableAntiforgery();
    router.MapDelete("/api/v1/profile/avatar", General.DeleteAvatar<DB.Entities.Profile>).RequireAuthorization();
  }
}


