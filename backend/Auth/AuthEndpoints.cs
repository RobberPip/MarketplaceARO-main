using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using DB.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using ARO.Integration.Gate.Internal;
using Microsoft.Extensions.Configuration;

namespace Auth;

public static class AuthEndpoints {
    private static readonly EmailAddressAttribute _emailAddressAttribute = new();
    public static void AddAuthV2Endpoints(this IEndpointRouteBuilder endpoints) {

        var routeGroup = endpoints.MapGroup("/").WithTags(["Auth V2"]);

        var emailSender = endpoints.ServiceProvider.GetRequiredService<IEmailSender<User>>();

        routeGroup.MapPost("/api/v1/auth/forgot-password", async Task<Results<Ok, ValidationProblem>>
            (ForgotPasswordRequest resetRequest, IServiceProvider sp) => {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is not null && await userManager.IsEmailConfirmedAsync(user)) {
                    var code = await userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                    await emailSender.SendPasswordResetCodeAsync(user, resetRequest.Email, HtmlEncoder.Default.Encode(code));
                }

                return TypedResults.Ok();
            });

        routeGroup.MapPost("/api/v1/auth/reset-password", async Task<Results<Ok, ValidationProblem>>
            (ResetPasswordRequest resetRequest, IServiceProvider sp) => {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                var signInManager = sp.GetRequiredService<SignInManager<User>>();
                var configuration = sp.GetRequiredService<IConfiguration>();

                var user = await userManager.FindByEmailAsync(resetRequest.Email);

                if (user is null || !await userManager.IsEmailConfirmedAsync(user)) {
                    return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken()));
                }

                IdentityResult result;
                try {
                    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetRequest.ResetCode));
                    result = await userManager.ResetPasswordAsync(user, code, resetRequest.NewPassword);
                } catch (FormatException) {
                    result = IdentityResult.Failed(userManager.ErrorDescriber.InvalidToken());
                }

                if (!result.Succeeded) {
                    return CreateValidationProblem(result);
                }
                var tokenARO = configuration["AROIntegrationToken"];
                var aroPasswordChanged = Gateway.ChangePassword(tokenARO, user.Id, resetRequest.NewPassword); // предполагаемый метод

                if (!aroPasswordChanged) {
                    return CreateValidationProblem(IdentityResult.Failed(new IdentityError {
                        Code = "AROChangeFailed",
                        Description = "Unable to change password in external system (ARO)."
                    }));
                }
                await signInManager.SignInAsync(user, true);
                return TypedResults.Ok();
            });

        var accountGroup = routeGroup.MapGroup("/").RequireAuthorization();

        accountGroup.MapGet("/api/v1/auth/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
            (ClaimsPrincipal claimsPrincipal, IServiceProvider sp) => {
                var userManager = sp.GetRequiredService<UserManager<User>>();
                if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) {
                    return TypedResults.NotFound();
                }


                var email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email.");

                var res = new InfoResponse {
                    Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
                    IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
                };

                return TypedResults.Ok(new InfoResponse {
                    Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
                    IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
                });
            }
        );
        accountGroup.MapPost("/api/v1/auth/info", async Task<Results<Ok<InfoResponse>, ValidationProblem, NotFound>>
         (ClaimsPrincipal claimsPrincipal, InfoRequest infoRequest, HttpContext context, IServiceProvider sp) => {
             var userManager = sp.GetRequiredService<UserManager<User>>();
             var configuration = sp.GetRequiredService<IConfiguration>();
             if (await userManager.GetUserAsync(claimsPrincipal) is not { } user) {
                 return TypedResults.NotFound();
             }

             if (!string.IsNullOrEmpty(infoRequest.NewEmail) && !_emailAddressAttribute.IsValid(infoRequest.NewEmail)) {
                 return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.InvalidEmail(infoRequest.NewEmail)));
             }

             if (!string.IsNullOrEmpty(infoRequest.NewPassword)) {
                 if (string.IsNullOrEmpty(infoRequest.OldPassword)) {
                     return CreateValidationProblem("OldPasswordRequired",
                         "The old password is required to set a new password. If the old password is forgotten, use /resetPassword.");
                 }

                 var isOldPasswordValid = await userManager.CheckPasswordAsync(user, infoRequest.OldPassword);
                 if (!isOldPasswordValid) {
                     return CreateValidationProblem(IdentityResult.Failed(userManager.ErrorDescriber.PasswordMismatch()));
                 }

                 var validationTasks = userManager.PasswordValidators
                     .Select(v => v.ValidateAsync(userManager, user, infoRequest.NewPassword));
                 var validationResults = await Task.WhenAll(validationTasks);

                 var failedValidation = validationResults.FirstOrDefault(r => !r.Succeeded);
                 if (failedValidation != null) {
                     return CreateValidationProblem(failedValidation);
                 }

                 var tokenARO = configuration["AROIntegrationToken"];
                 bool passwordChanged = Gateway.ChangePassword(tokenARO, user.Id, infoRequest.NewPassword);

                 if (!passwordChanged) {
                     return CreateValidationProblem("AROChangeFailed", "Unable to change password in external system (ARO).");
                 }

                 var changePasswordResult = await userManager.ChangePasswordAsync(user, infoRequest.OldPassword, infoRequest.NewPassword);
                 if (!changePasswordResult.Succeeded) {
                     return CreateValidationProblem(changePasswordResult);
                 }
             }

             // TODO confirm email
             //  if (!string.IsNullOrEmpty(infoRequest.NewEmail)) {
             //      var email = await userManager.GetEmailAsync(user);

             //      if (email != infoRequest.NewEmail) {
             //          await SendConfirmationEmailAsync(user, userManager, context, infoRequest.NewEmail, isChange: true);
             //      }
             //  }

             return TypedResults.Ok(await CreateInfoResponseAsync(user, userManager));
         });
        // Debug claims
        accountGroup.MapGet("/debug/claims", (ClaimsPrincipal user) => {
            return Results.Json(user.Claims.Select(c => new { c.Type, c.Value }));
        }).RequireAuthorization();
    }

    private static async Task<InfoResponse> CreateInfoResponseAsync<User>(User user, UserManager<User> userManager)
         where User : class {
        return new() {
            Email = await userManager.GetEmailAsync(user) ?? throw new NotSupportedException("Users must have an email."),
            IsEmailConfirmed = await userManager.IsEmailConfirmedAsync(user),
        };
    }
    private static ValidationProblem CreateValidationProblem(string errorCode, string errorDescription) =>
    TypedResults.ValidationProblem(new Dictionary<string, string[]> {
            { errorCode, [errorDescription] }
    });

    private static ValidationProblem CreateValidationProblem(IdentityResult result) {
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors) {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions)) {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            } else {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}