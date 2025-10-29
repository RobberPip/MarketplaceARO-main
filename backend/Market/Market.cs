using DB;
using Market.Account;
using Market.Company;
using Market.Profile;
using Market.Product;
using Market.ServiceExtensions;

namespace Market;

public static class Market {

    public static void AddMarketServices(this IServiceCollection services) {
        services.AddScoped(provider => {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var dbContext = provider.GetRequiredService<AppDbContext>();
            var invitationService = provider.GetRequiredService<InvitationService>();
            return new Ctx(httpContextAccessor.HttpContext!, dbContext, invitationService);
        });
    }

    public static void AddMarketEndpoints(this WebApplication app) {
        app.AddProfileEndpoints();
        app.AddAccountEndpoints();
        app.AddFilesEndpoints();
        app.AddCompaniesEndpoints();
        app.AddProductsEndpoints();
        app.AddGeneralEndpoints();
    }
}
