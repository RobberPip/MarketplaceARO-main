using System.Runtime.CompilerServices;
using DB;
using Microsoft.EntityFrameworkCore;

namespace Market;

public static class Files {

    public static void AddFilesEndpoints(this WebApplication app) {

        var router = app.MapGroup("/")
          .WithOpenApi()
          .WithTags(["Files"]);

        router.MapGet("/api/v1/files/{fileId}", GetFile);
    }

    async static Task<IResult> GetFile(Guid fileId, AppDbContext db, Ctx ctx) {
        var profile = await ctx.GetCurrentProfile();
        if (profile is null)
            return Results.NotFound();

        var file = await db.Files.Where(a => a.Id == fileId)
          .FirstOrDefaultAsync();

        if (file == null) {
            return Results.NotFound();
        }

        // TODO: handle mime!
        return Results.Stream(new MemoryStream(file.Content!), "image/" + file.ContentType);
    }
}
