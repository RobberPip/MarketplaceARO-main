using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DB;

namespace Market.Pages;

public class IndexModel(Ctx ctx, AppDbContext db, ILogger<IndexModel> logger) : PageModel {
    public bool IsAuthenticated => ctx.User.Identity?.IsAuthenticated ?? false;

    public async void OnGet() {
        logger.LogInformation("Hello");
    }
}

