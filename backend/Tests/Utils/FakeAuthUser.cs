using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Tests.Utils {
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions> {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder) : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync() {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, AdminTestUserSettings.UserId),
                new Claim(ClaimTypes.Name, AdminTestUserSettings.UserName),
                new Claim(ClaimTypes.Email, AdminTestUserSettings.UserEmail),
                new Claim(ClaimTypes.Role, AdminTestUserSettings.UserRole)
            };
            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);

            var ticket = new AuthenticationTicket(principal, "Test");
            var result = AuthenticateResult.Success(ticket);
            return Task.FromResult(result);
        }
    }


    internal static class AdminTestUserSettings {
        // look at the test identity data
        internal const string UserId = "5450721d-b3bd-4342-9aac-6fe315911a04";
        internal const string UserName = "test_user123@test.com";
        internal const string UserEmail = "test_user123@test.com";
        internal const string UserRole = "Admin";
    }
}
