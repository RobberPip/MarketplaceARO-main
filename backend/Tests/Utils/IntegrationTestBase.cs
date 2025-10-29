using DB;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Microsoft.AspNetCore.Hosting;

namespace Tests.Utils {
    public class SoftwaresMarketFactory : WebApplicationFactory<Program> {
        protected override void ConfigureWebHost(IWebHostBuilder builder) {
            builder.UseEnvironment("Testing");
        }
    }

    public abstract class IntegrationTestBase : IClassFixture<SoftwaresMarketFactory>, IDisposable {
        protected readonly HttpClient unauthHttpClient;
        protected readonly HttpClient authHttpClient;
        private const string _basePath = "http://localhost:5000";

        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithUsername("postgres")
            .WithPassword("32167")
            .WithCleanUp(true)
            .Build();

        public IntegrationTestBase(SoftwaresMarketFactory factory) {
            unauthHttpClient = factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    _dbContainer.StartAsync().Wait();
                    string connStr = _dbContainer.GetConnectionString();
                    services.ConfigureTestDb(connStr);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions {
                BaseAddress = new Uri(_basePath),
                AllowAutoRedirect = false,
            });

            authHttpClient = factory.WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    services.AddAuthentication(o => {
                        o.DefaultAuthenticateScheme = "Test";
                        o.DefaultChallengeScheme = "Test";
                    }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });

                    _dbContainer.StartAsync().Wait();
                    string connStr = _dbContainer.GetConnectionString();
                    services.ConfigureTestDb(connStr);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions {
                BaseAddress = new Uri(_basePath),
                AllowAutoRedirect = false,
            });
        }

        public void Dispose() {
            _dbContainer.DisposeAsync().ConfigureAwait(true);
        }
    }

    //TODO what's the fucking shit
    public static class TestWebHostExtension {
        public static void ConfigureTestDb(this IServiceCollection services, string connStr) {
            services.RemoveAll<DbContextOptions<AppDbContext>>();
            services.AddDbContext<AppDbContext>(opt => {
                opt.UseNpgsql(connStr);
                // TODO: https://github.com/dotnet/efcore/issues/35110#issuecomment-2488804270
                opt.ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
                opt.UseSeeding((context, _) => {
                    var identityData = TestData.GetIdentityTestData();
                    var appContext = context as AppDbContext;

                    if (!appContext.Users.ToList().Any())
                        appContext?.Users.Add(identityData.User);

                    if (!appContext.UserProfiles.ToList().Any())
                        appContext?.UserProfiles.Add(identityData.UserProfile);

                    if (!appContext.Roles.ToList().Any())
                        appContext?.Roles.AddRange(identityData.Roles);

                    if (!appContext.UserRoles.ToList().Any())
                        appContext?.UserRoles.AddRange(identityData.UserRoles);
                    appContext?.SaveChanges();
                });
            });
        }
    }
}
