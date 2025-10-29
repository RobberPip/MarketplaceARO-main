using System.Security.Claims;
using DB;
using DB.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared.Services;

namespace Auth;

public static class AuthExtensions {
    public static void AddAuthServices(this IServiceCollection services, IWebHostEnvironment environment) {
        services.AddAuthentication();

        services.AddAuthorization(options => {
            options.AddPolicy("EmailConfirmed", policy => {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context => {
                    var claim = context.User.FindFirst("email_confirmed");
                    return claim != null && claim.Value == "true";
                });
            });
        });


        services.AddIdentity<User, IdentityRole<Guid>>(opt => {
            if (environment.IsDevelopment() || environment.IsEnvironment("Testing")) {
                opt.Password.RequireDigit = false;
                opt.Password.RequiredLength = 4;
                opt.SignIn.RequireConfirmedEmail = false;
            } else {
                opt.Password.RequiredLength = 8;
                opt.Password.RequireDigit = true;
                opt.Password.RequiredUniqueChars = 3;
                opt.SignIn.RequireConfirmedEmail = false;

            }

            opt.User.RequireUniqueEmail = true;
            opt.SignIn.RequireConfirmedPhoneNumber = false;
            opt.Password.RequireUppercase = false;
            opt.Password.RequireLowercase = false;
            opt.Password.RequireNonAlphanumeric = false;
            opt.Lockout.MaxFailedAccessAttempts = 10;
            opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
            opt.Tokens.EmailConfirmationTokenProvider = "EmailConfirmationTokenProvider";
            opt.Tokens.ChangeEmailTokenProvider = "EmailConfirmationTokenProvider";
            opt.Tokens.PasswordResetTokenProvider = "ResetPasswordTokenProvider";
        }).AddRoles<IdentityRole<Guid>>()
        .AddUserManager<ApplicationUserManager>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddTokenProvider<EmailConfirmationTokenProvider<User>>("EmailConfirmationTokenProvider")
        .AddTokenProvider<ResetPasswordTokenProvider<User>>("ResetPasswordTokenProvider")
        .AddDefaultTokenProviders();
        services.AddSingleton<IPasswordValidator<User>, CustomPasswordValidator>();

        services.ConfigureApplicationCookie(opt => {
            opt.Cookie.Name = "auth";
            opt.Cookie.SameSite = SameSiteMode.Lax;
            opt.ExpireTimeSpan = TimeSpan.FromDays(7);
            opt.SlidingExpiration = true;
            opt.Cookie.HttpOnly = true;

            opt.Events.OnRedirectToLogin = redirectContext => {
                redirectContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            };

            // TODO: add response body
            opt.Events.OnRedirectToAccessDenied = redirectContext => {
                redirectContext.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            };
        });
        services.AddScoped<IUserClaimsPrincipalFactory<User>, CustomUserClaimsPrincipalFactory>();
        services.AddScoped<SignInManager<User>, ApplicationSignInManager>();
        services.AddTransient<IEmailSender<User>, AuthEmailSender>();
    }

    public static void AddAuthEndpoints(this WebApplication app) {
        app.AddAuthV2Endpoints();
    }
}

public class CustomPasswordValidator : IPasswordValidator<User> {
    public Task<IdentityResult> ValidateAsync(UserManager<User> manager, User user, string? password) {
        if (password == null || !password.Any(char.IsLetter)) {
            return Task.FromResult(IdentityResult.Failed(new IdentityError {
                Code = "PasswordRequiresLetter",
                Description = "Password must contain at least one letter."
            }));
        }

        return Task.FromResult(IdentityResult.Success);
    }
}

public class EmailConfirmationTokenProvider<TUser>(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<EmailConfirmationTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<TUser>> logger) : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : class {
}
public class EmailConfirmationTokenProviderOptions : DataProtectionTokenProviderOptions {
    public EmailConfirmationTokenProviderOptions() {
        Name = "EmailConfirmationTokenProvider";
        TokenLifespan = TimeSpan.FromHours(4);
    }
}


public class ResetPasswordTokenProvider<TUser>(
    IDataProtectionProvider dataProtectionProvider,
    IOptions<ResetPasswordTokenProviderOptions> options,
    ILogger<DataProtectorTokenProvider<TUser>> logger) : DataProtectorTokenProvider<TUser>(dataProtectionProvider, options, logger) where TUser : class {
}

public class ResetPasswordTokenProviderOptions : DataProtectionTokenProviderOptions {
    public ResetPasswordTokenProviderOptions() {
        Name = "ResetPasswordTokenProvider";
        TokenLifespan = TimeSpan.FromMinutes(15);
    }
}

public class ApplicationUserManager(
    IUserStore<User> store,
    IOptions<IdentityOptions> optionsAccessor,
    IPasswordHasher<User> passwordHasher,
    IEnumerable<IUserValidator<User>> userValidators,
    IEnumerable<IPasswordValidator<User>> passwordValidators,
    ILookupNormalizer keyNormalizer,
    IdentityErrorDescriber errors,
    IServiceProvider services,
    ILogger<UserManager<User>> logger
    ) : UserManager<User>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger) {
    public override async Task<IdentityResult> CreateAsync(User user, string password) {
        var result = await base.CreateAsync(user, password);

        return result;
    }
}

public class CustomUserClaimsPrincipalFactory :
    UserClaimsPrincipalFactory<User, IdentityRole<Guid>> {
    public CustomUserClaimsPrincipalFactory(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(userManager, roleManager, optionsAccessor) { }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user) {
        var identity = await base.GenerateClaimsAsync(user);
        var isConfirmed = await UserManager.IsEmailConfirmedAsync(user);
        identity.AddClaim(new Claim("email_confirmed", isConfirmed.ToString().ToLower()));
        return identity;
    }
}

public class ApplicationSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<User> confirmation) : SignInManager<User>(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) {

    public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure) {
        var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);

        return result;
    }
}
public class AuthEmailSender(ILogger<AuthEmailSender> logger, Shared.Services.EmailSender sender, IHtmlParser htmlParser, IHttpContextAccessor httpContextAccessor) : IEmailSender<User> {
    private readonly IHtmlParser _htmlParser = htmlParser;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink) {
        // TODO: use Shared.Services.EmailSender
        logger.LogInformation("SendConfirmationLinkAsync {@Email} {@ConfirmationLink}", email, confirmationLink);
        return Task.CompletedTask;
    }

    public async Task SendPasswordResetCodeAsync(User user, string email, string resetCode) {
        var request = _httpContextAccessor.HttpContext?.Request;
        if (request == null) throw new InvalidOperationException("HttpContext is not available.");

        var host = $"{request.Scheme}://{request.Host.ToUriComponent()}";
        var url = $"{host}/app/resetpass?step=reset&email={user.Email}&token={resetCode}"; //TODO React ссылка
        var _email = _htmlParser?.GetEmailPageAsString("ConfirmationResetPassword.html", url);

        if (_email == null) throw new InvalidOperationException("Email body cannot be null.");
        if (string.IsNullOrEmpty(user.Email)) throw new InvalidOperationException("User email cannot be null or empty.");


        await sender.SendAsync(user.Email, "Подтверждение восстановления пароля", _email);
        logger.LogInformation("SendPasswordResetCodeAsync {@Email} {@ResetCode}", email, resetCode);
    }

    public Task SendPasswordResetLinkAsync(User user, string email, string resetLink) {
        // TODO: use Shared.Services.EmailSender
        logger.LogInformation("SendPasswordResetLinkAsync {@Email} {@ResetLink}", email, resetLink);
        return Task.CompletedTask;
    }
}
