using Auth.Dtos;
using DB;
using DB.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared;
using Shared.Services;
using System.Data;
using System.Text;
using ARO.Integration.Gate.Internal;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

// TODO: Do not use Market entities! Remove all Profile usages
namespace Auth;

[ApiController]
[Route("[controller]")]
[Produces("application/json")]
[Consumes("application/json")]
[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status500InternalServerError)]
public class AuthController : ControllerBase {
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly AppDbContext _dbContext;
    private readonly ILogger<AuthController> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IMediator _mediator;

    private readonly EmailSender _emailSender;
    private readonly IHtmlParser _htmlParser;

    private readonly IConfiguration _configuration;

    public AuthController(SignInManager<User> signInManager, UserManager<User> userManager, AppDbContext dbContext,
        IHtmlParser htmlParser, EmailSender emailSender, IMediator mediator, ILogger<AuthController> logger,
        IWebHostEnvironment environment, IConfiguration configuration) {
        _signInManager = signInManager;
        _userManager = userManager;
        _dbContext = dbContext;
        _htmlParser = htmlParser;
        _emailSender = emailSender;
        _mediator = mediator;
        _logger = logger;
        _environment = environment;
        _configuration = configuration;
    }


    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Login([FromBody] LoginDto loginDto) {
        var user = await _userManager.FindByEmailAsync(loginDto.Email);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized"));

        var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, loginDto.RememberMe, false);
        switch (true) {
            case bool when result.Succeeded:
                _logger.LogInformation("The user with Id:{@UserId} is logged in", user.Id);
                await _mediator.Publish(new UserNotification(new UserSignedIn(user, loginDto.Data)));
                return Ok();
            case bool when result.IsLockedOut:
                return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString())); //User is locked. Try later...
            case bool when result.IsNotAllowed:
                return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString())); //Email is not confirmed
            case bool when result.RequiresTwoFactor:
                throw new NotImplementedException(); //TODO implement logic for 2FA 
            default:
                return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString())); //Email or password is not valid
        }
    }


    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Logout() {
        await _signInManager.SignOutAsync();
        return Ok();
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Register([FromBody] RegistrationDto registrationDto) {
        if (!registrationDto.CheckedAgreements || !registrationDto.CheckedRules) {
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = new Dictionary<string, string> {
                { "CheckedAgreementsAndRules", "Both agreements and rules must be accepted." }
            }
            });
        }
        var user = await _userManager.FindByEmailAsync(registrationDto.Email);
        if (user != null) {
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = new Dictionary<string, string> {
                    { "Email", "This user is already registered." }
                }
            });
        } else {
            var uid = Guid.NewGuid();

            var login = string.IsNullOrWhiteSpace(registrationDto.Email)
                    ? throw new ArgumentException("UserEmail is empty")
                    : registrationDto.Email.Split('@')[0];
            //     var token = _configuration["AROIntegrationToken"];
            //     try {
            //         bool loginCreated = Gateway.CreateLogin(
            //             token,
            //             uid,
            //             login,
            //             registrationDto.Password
            //         );
            //         if (!loginCreated) {
            //             return BadRequest(new ErrorResponse {
            //                 Status = 400,
            //                 Title = "BadRequest",
            //                 Errors = new Dictionary<string, string>
            //                 {
            //             { "LoginCreate", "Unable to create login. Please check provided credentials or try again later." }
            //         }
            //             });
            //         }
            //     } catch (Exception ex) {
            //         _logger.LogError($"Error create login {ex}");
            //         return BadRequest(new ErrorResponse {
            //             Status = 400,
            //             Title = "Login Creation Error",
            //             Detail = "An exception occurred while creating login.",
            //             Errors = new Dictionary<string, string>
            //             {
            //     { "Exception", ex.Message }
            // }
            //         });
            //     }

            user = new User {
                Id = uid,
                Email = registrationDto.Email,
                UserName = registrationDto.Email
            };

            using (var trans = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)) {
                try {
                    var result = await _userManager.CreateAsync(user, registrationDto.Password);
                    if (!result.Succeeded) {

                        var errors = result.Errors.ToDictionary(
                        err => err.Code,
                        err => err.Description
                    );
                        await trans.RollbackAsync();
                        return BadRequest(new ErrorResponse {
                            Status = 400,
                            Title = "BadRequest",
                            Errors = errors
                        });
                    }
                    await _userManager.AddToRoleAsync(user, "Member");

                    await _signInManager.SignInAsync(user, true);

                    await _mediator.Publish(new UserNotification(new UserCreated(user, registrationDto.Data)));

                    await trans.CommitAsync();
                } catch (Exception ex) {
                    await trans.RollbackAsync();
                    throw new Exception("Ошибка при обработке транзакции: " + ex.Message, ex);
                }
            }
            await SendConfirmEmail(new SendConfirmEmailDto { Email = user.Email });
        }
        return Ok();
    }
    [HttpPost("send-confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> SendConfirmEmail([FromBody] SendConfirmEmailDto sendConfirmEmailDto) {
        var user = await _userManager.FindByEmailAsync(sendConfirmEmailDto.Email);
        if (user is null) {
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist
        } else if (await _userManager.IsEmailConfirmedAsync(user)) {
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = new Dictionary<string, string> {
                { "User", "This user is already confirmed." }
                }
            });
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var tokenTransport = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

        var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
        var url = $"{host}/auth/confirm-email?userId={user.Id}&token={tokenTransport}";

        var email = _htmlParser.GetEmailPageAsString("ConfirmationEmail.html", url);
        await _emailSender.SendAsync(user.Email, "Подтверждение почты", email);
        _logger.LogInformation("Confirmation email sent to user with Id: {@UserId}", user.Id);
        return Ok();
    }

    [HttpGet("confirm-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string? userId, [FromQuery] string? token) {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //The confirmation link is invalid

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist
        if (user.EmailConfirmed) {
            return Unauthorized(new ErrorResponse(401, "Unauthorized", "Email уже подтверждён."));
        }
        using (var trans = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)) {
            try {
                var confirmToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
                var result = await _userManager.ConfirmEmailAsync(user, confirmToken);
                if (!result.Succeeded) {
                    var errors = result.Errors.Select(err => $"{err.Code}:{err.Description}");
                    return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString()));
                    //TODO redirect to wrong front-end page
                }

                await _signInManager.SignInAsync(user, true);

                await _mediator.Publish(new UserNotification(new UserEmailConfirmed(user)));

                await trans.CommitAsync();
            } catch {
                await trans.RollbackAsync();
                throw;
            }
        }
        _logger.LogInformation("User with Id:{@UserId} confirmed his email address", userId);
        return Redirect("/softwares");
    }

    [HttpPost("change-login")]
    [Authorize]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeLogin([FromBody] ChangeLoginDto changeLoginDto) {
        var alreadyHas = await _dbContext.Profiles.FirstOrDefaultAsync(u => u.Login == changeLoginDto.Login);
        if (alreadyHas is not null)
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = new Dictionary<string, string> {
                    { "Login", "This login is already exist" }
                }
            });

        var user = await _userManager.GetUserAsync(User);
        var changeLoginToken = await _userManager.GenerateUserTokenAsync(user, TokenOptions.DefaultProvider, "ChangeLogin");
        var tokenTransport = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changeLoginToken));
        var newLogin = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changeLoginDto.Login));

        var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
        var url = $"{host}/auth/confirm-change-login?userId={user.Id}&login={newLogin}&token={tokenTransport}";

        var email = _htmlParser.GetEmailPageAsString("ConfirmationChangeLogin.html", url);
        await _emailSender.SendAsync(user.Email, "Подтверждение смены логина", email);
        return Ok();
    }

    [HttpGet("confirm-change-login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmChangeLogin([FromQuery] string? userId, [FromQuery] string login, [FromQuery] string token) {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(login) || string.IsNullOrEmpty(token))
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //The confirmation link is invalid

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist
        var profile = await _dbContext.Profiles.FirstOrDefaultAsync(p => p.Id == user.Id);
        if (profile is null)
            return BadRequest(new ErrorResponse(400, "Profile not found"));

        var confirmToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var confirmLogin = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(login));
        var result = await _userManager.VerifyUserTokenAsync(user, TokenOptions.DefaultProvider, "ChangeLogin", confirmToken);
        if (!result) {
            return Unauthorized(new ErrorResponse {
                Status = 401,
                Title = "Unauthorized",
                Errors = new Dictionary<string, string> {
                        { "Token", "Invalid or expired token" }
                    }
            });
        }
        // var tokenARO = _configuration["AROIntegrationToken"];
        // bool loginChange = Gateway.ChangeLogin(tokenARO, user.Id, confirmLogin);
        // if (!loginChange) {
        //     return BadRequest(new ErrorResponse {
        //         Status = 400,
        //         Title = "BadRequest",
        //         Errors = new Dictionary<string, string> {
        //         { "LoginCreate", "Unable to change login in external system (ARO)." }
        //     }
        //     });
        // }
        using (var trans = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)) {
            try {

                profile.Login = confirmLogin;
                _dbContext.Profiles.Update(profile);
                await _dbContext.SaveChangesAsync();
                await _mediator.Publish(new UserNotification(new UserLoginChanged(user)));
                await trans.CommitAsync();
            } catch {
                await trans.RollbackAsync();
                throw;
            }
        }

        await _signInManager.RefreshSignInAsync(user);

        _logger.LogInformation("User with Id {@UserId} changed login", userId);
        // dev test
        //return Content("<script>window.location.href = 'http://localhost:4000/profile?action=confirmedLogin';</script>", "text/html");

        return Redirect("/app/profile?action=confirmedLogin");
    }

    [HttpPost("change-email")]
    [Authorize]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangeEmail([FromBody] EmailDto changeEmailDto) {
        var alreadyHas = await _userManager.FindByEmailAsync(changeEmailDto.Email);
        if (alreadyHas is not null)
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = new Dictionary<string, string> {
                    { "Email", "This email is already exist" }
                }
            });

        var user = await _userManager.GetUserAsync(User);
        var changeEmailToken = await _userManager.GenerateChangeEmailTokenAsync(user, changeEmailDto.Email);
        var tokenTransport = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changeEmailToken));
        var newEmail = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changeEmailDto.Email));

        var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
        var url = $"{host}/auth/confirm-change-email?userId={user.Id}&email={newEmail}&token={tokenTransport}";

        var email = _htmlParser.GetEmailPageAsString("ConfirmationChangeEmail.html", url);
        await _emailSender.SendAsync(changeEmailDto.Email, "Подтверждение почты", email);
        return Ok();
    }


    [HttpGet("confirm-change-email")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmChangeEmail([FromQuery] string? userId, [FromQuery] string email, [FromQuery] string token) {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //The confirmation link is invalid

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist

        var confirmToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var confirmEmail = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(email));
        var result = await _userManager.ChangeEmailAsync(user, confirmEmail, confirmToken);
        if (!result.Succeeded) {
            var errors = result.Errors.Select(err => $"{err.Code}:{err.Description}");
            return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString()));
            //TODO redirect to wrong front-end page
        }

        using (var trans = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)) {
            try {
                user.UserName = confirmEmail;
                await _userManager.UpdateAsync(user);

                await _mediator.Publish(new UserNotification(new UserEmailChanged(user)));
                await trans.CommitAsync();
            } catch {
                await trans.RollbackAsync();
                throw;
            }
        }

        await _signInManager.RefreshSignInAsync(user);

        _logger.LogInformation("User with Id {@UserId} changed email address", userId);
        // dev test
        //return Content("<script>window.location.href = 'http://localhost:4000/profile?action=confirmedEmail';</script>", "text/html");

        return Redirect("/app/profile?action=confirmedEmail");
    }


    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto) {
        var user = await _userManager.GetUserAsync(User);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized"));
        var isPassCorrect = await _userManager.CheckPasswordAsync(user, changePasswordDto.CurrentPassword);
        var validationResults = await Task.WhenAll(_userManager.PasswordValidators.Select(v => v.ValidateAsync(_userManager, user, changePasswordDto.NewPassword)));
        var errors = new Dictionary<string, string>();

        if (!isPassCorrect) {
            errors.Add("PasswordIncorrect", "Incorrect password");
        }
        var validationErrors = validationResults
            .SelectMany(r => r.Errors)
            .ToDictionary(err => err.Code, err => err.Description);

        foreach (var error in validationErrors) {
            errors.Add(error.Key, error.Value);
        }

        if (errors.Any()) {
            return BadRequest(new ErrorResponse {
                Status = 400,
                Title = "BadRequest",
                Errors = errors
            });
        }

        var currentPass = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changePasswordDto.CurrentPassword));
        var newPass = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(changePasswordDto.NewPassword));

        var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
        var url = $"{host}/auth/confirm-change-password?userId={user.Id}&currentPassword={currentPass}&newPassword={newPass}";

        var email = _htmlParser.GetEmailPageAsString("ConfirmationChangePassword.html", url);
        await _emailSender.SendAsync(user.Email, "Подтверждение смены пароля", email);

        return Ok();
    }

    [HttpGet("confirm-change-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmChangePassword([FromQuery] string? userId, [FromQuery] string currentPassword, [FromQuery] string newPassword) {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //The confirmation link is invalid

        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist

        var confirmCurrentPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(currentPassword));
        var confirmNewPassword = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(newPassword));
        var result = await _userManager.ChangePasswordAsync(user, confirmCurrentPassword, confirmNewPassword);
        if (!result.Succeeded) {
            var errors = result.Errors.Select(err => $"{err.Code}:{err.Description}");
            return BadRequest(new ErrorResponse(400, "BadRequest", result.ToString()));
            //TODO redirect to wrong front-end page
        }
        // var tokenARO = _configuration["AROIntegrationToken"];
        // bool passwordChanged = Gateway.ChangePassword(tokenARO, user.Id, confirmNewPassword);
        // if (!passwordChanged) {
        //     return BadRequest(new ErrorResponse {
        //         Status = 400,
        //         Title = "BadRequest",
        //         Errors = new Dictionary<string, string> {
        //         { "ExternalChange", "Unable to change password in external system (ARO)." }
        //     }
        //     });
        // }

        using (var trans = await _dbContext.Database.BeginTransactionAsync(IsolationLevel.ReadCommitted)) {
            try {

                await _userManager.UpdateAsync(user);
                await _mediator.Publish(new UserNotification(new UserPasswordChanged(user)));
                await trans.CommitAsync();
            } catch {
                await trans.RollbackAsync();
                throw;
            }
        }

        await _signInManager.RefreshSignInAsync(user);

        _logger.LogInformation("User with Id:{@UserId} change his password", user.Id);
        return Redirect("/app/profile?action=confirmedPassword");
    }


    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> ResetPassword([FromBody] EmailDto emailDto) {
        var user = await _userManager.FindByEmailAsync(emailDto.Email);
        if (user is null)
            return BadRequest(new ErrorResponse(400, "BadRequest"));

        string token;
        string tokenTransport;
        string url;
        string mail;
        var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

        var isEmailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
        if (!isEmailConfirmed) {
            token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            tokenTransport = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            url = $"{host}/auth/confirm-email?userId={user.Id}&token={tokenTransport}";
            mail = _htmlParser.GetEmailPageAsString("ConfirmationEmail.html", url);
            await _emailSender.SendAsync(user.Email, "Подтверждение почты", mail);
            return BadRequest(new ErrorResponse(400, "BadRequest")); //User's mail is not confirmed
        }

        token = await _userManager.GeneratePasswordResetTokenAsync(user);
        tokenTransport = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        url = $"{host}/auth/temporary-reset-password-redirect?userId={user.Id}&token={tokenTransport}";
        //TODO change to front-end page with query 

        mail = _htmlParser.GetEmailPageAsString("ConfirmationResetPassword.html", url);
        await _emailSender.SendAsync(emailDto.Email, "Подтверждение сброса пароля", mail);
        return Ok();
    }


    [HttpGet("temporary-reset-password-redirect")]
    [ApiExplorerSettings(IgnoreApi = true)]
    [AllowAnonymous]
    public async Task<ActionResult> ResetPasswordRedirect([FromQuery] string? userId, [FromQuery] string token) {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            throw new ArgumentException("The confirmation link is invalid");

        return Ok(new { UserId = userId, Token = token });
    }


    [HttpPost("confirm-reset-password")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> ConfirmResetPassword([FromBody] ResetPasswordDto resetPasswordDto) {
        var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
        if (user is null)
            return Unauthorized(new ErrorResponse(401, "Unauthorized")); //User is not exist

        var confirmToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(resetPasswordDto.Token));
        var result = await _userManager.ResetPasswordAsync(user, confirmToken, resetPasswordDto.NewPassword);
        if (!result.Succeeded) {
            var errors = result.Errors.Select(err => $"{err.Code}:{err.Description}");
            return Unauthorized(new ErrorResponse(401, "Unauthorized", result.ToString()));
            //TODO redirect to wrong front-end page
        }
        _logger.LogInformation("User with Id:{@UserId} reset his password", user.Id);
        return Ok();
    }



    // // TODO: important!
    // // сreate api/v1/viewer in Market project 
    // // and use it in viewerQuery/checkViewerQuery instead of this route
    // [HttpGet("current-user-info")]
    // [Authorize]
    // [ProducesResponseType(typeof(CurrentUserInfoDto), StatusCodes.Status200OK)]
    // [ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
    // public async Task<ActionResult<CurrentUserInfoDto>> GetCurrentUserInfo() {
    //     var user = await _userManager.GetUserAsync(User);
    //     if (user is null)
    //         return Unauthorized(new ErrorResponse(401, "Unauthorized"));

    //     var roles = await _userManager.GetRolesAsync(user);
    //     var profile = await _dbContext.Profiles.FirstOrDefaultAsync(profile => profile.Id == user.Id);
    //     var accounts = await _dbContext.Accounts
    //     .Where(a => a.AccountProfiles.Any(ap => ap.ProfileId == profile.Id && ap.Status == InvitationStatus.Accepted))
    //     .ToListAsync();

    //     var avatar = await _dbContext.Files.Where(a => a.Id == profile.AvatarId).FirstOrDefaultAsync();
    //     var avatarUrl = avatar != null ? $"/api/v1/files/{avatar.Id}" : null;

    //     return Ok(new CurrentUserInfoDto {
    //         Id = user.Id,
    //         UserName = user.UserName,
    //         Email = user.Email,
    //         Login = profile.Login,
    //         Avatar = avatarUrl,
    //         FirstName = profile.FirstName,
    //         LastName = profile.LastName,
    //         EmailConfirmed = user.EmailConfirmed,
    //         TwoFactorEnabled = user.TwoFactorEnabled,
    //         Lockout = user.LockoutEnd != null ? true : false,
    //         AccessFailedCount = user.AccessFailedCount,
    //         Roles = roles.ToList(),
    //         Accounts = accounts.Select(a => Accounts.From(a)).ToList(),
    //         ActiveAccountId = user.ActiveAccountId
    //     });
    // }
}
