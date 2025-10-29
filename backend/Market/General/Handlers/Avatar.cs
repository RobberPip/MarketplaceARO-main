using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Db.Entities;

namespace Market;

public static partial class General {
    public class AvatarOut {
        public string? Url { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public static AvatarOut From(DB.Entities.File source) => new() {
            Url = $"/api/v1/files/{source.Id}",
            UpdatedAt = source.UpdatedAt
        };
    }
    public static async Task<Results<NoContent, BadRequest<ErrorResponse>, InternalServerError<ErrorResponse>>> CreateAvatar<T>(
                AppDbContext db,
                Ctx ctx,
                IFormFile imageFile
            ) where T : class, IAvatarOwner {
        var filesService = new FilesService(db);
        var result = await filesService.CreateOrUpdateAvatarAsync<T>(ctx, imageFile);
        if (result.IsFailed)
            return TypedResults.BadRequest(result.Error);

        return TypedResults.NoContent();
    }

    public static async Task<Results<NoContent, NotFound<ErrorResponse>>> DeleteAvatar<T>(
        AppDbContext db,
        Ctx ctx
    ) where T : class, IAvatarOwner {
        var filesService = new FilesService(db);
        var result = await filesService.DeleteAvatarAsync<T>(ctx);
        if (result.IsFailed)
            return TypedResults.NotFound(result.Error);

        return TypedResults.NoContent();
    }

}

