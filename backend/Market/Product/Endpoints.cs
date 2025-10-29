using DB;
using SharpGrip.FluentValidation.AutoValidation.Endpoints.Extensions;
using DB.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Market.Product;

public static partial class Product {

    public static void AddProductsEndpoints(this WebApplication app) {

        var router = app.MapGroup("/")
            .AddFluentValidationAutoValidation()
            .WithOpenApi()
            .WithTags(["Product"]);
        router.MapGet("/api/v1/products/options", GetProductOptions).RequireAuthorization();
        router.MapGet("/api/v1/product/{id}", GetProduct);
        router.MapGet("/api/v1/products", GetProducts);
        router.MapGet("/api/v1/account/products", GetAccountProducts).RequireAuthorization();
        router.MapGet("/api/v1/account/product/{id}", GetProduct).RequireAuthorization();
        router.MapPost("/api/v1/product", AddProductDraft);
        router.MapPost("/api/v1/product/images/{id}", AddProductImages).RequireAuthorization().DisableAntiforgery();
        router.MapPatch("/api/v1/product/{id}", UpdateProduct);
        router.MapPost("/api/v1/like-software", RateLegacySoftware).RequireAuthorization();
        router.MapDelete("/api/v1/product/{id}", DeleteProduct).RequireAuthorization();
    }
    static async Task<Results<Ok<LikeLegacySoftwareOut>, NotFound<ErrorResponse>, InternalServerError<ErrorResponse>>> RateLegacySoftware(
           LikeLegacySoftwareIn args,
           AppDbContext db,
           Ctx ctx
       ) {
        var exists = await db.Database.SqlQueryRaw<int>("SELECT id from software where id = {0}", args.SoftwareId).AnyAsync();
        if (!exists) {
            return TypedResults.NotFound(new ErrorResponse { Detail = "Software not found." });
        }

        using var transaction = db.Database.BeginTransaction();

        try {
            var currentReview = await db.Reviews.Where(r => r.AuthorId == ctx.UserId && r.LegacySoftwareId == args.SoftwareId).FirstOrDefaultAsync();

            if (currentReview == null) {
                var review = new Review {
                    AuthorId = ctx.UserId,
                    Like = args.Like,
                    LegacySoftwareId = args.SoftwareId
                };
                db.Reviews.Add(review);
            } else {
                currentReview.Like = args.Like;
            }

            await db.SaveChangesAsync();

            var sql = @"
                UPDATE ""software""
                SET
                    like_count = (
                        SELECT COUNT(*)
                        FROM ""Reviews""
                        WHERE ""LegacySoftwareId"" = {0} AND ""Like"" = true
                    )
                WHERE id = {0};";

            await db.Database.ExecuteSqlRawAsync(sql, args.SoftwareId);
            await transaction.CommitAsync();

            var res = await db.Database.SqlQueryRaw<LikeLegacySoftwareOut>(
                @"SELECT s.id as SoftwareId, s.rating as Rating, s.like_count as LikeCount FROM software AS s WHERE s.id = {0}",
                args.SoftwareId
            ).ToListAsync();

            return TypedResults.Ok(new LikeLegacySoftwareOut {
                SoftwareId = args.SoftwareId,
                LikeCount = res[0].LikeCount
            });

        } catch (Exception e) {
            // TODO
            Console.WriteLine(e);
            return TypedResults.InternalServerError(new ErrorResponse { });
        }
    }

    public class LikeLegacySoftwareIn {
        public int SoftwareId { get; set; }
        public bool Like { get; set; }
    }

    public class LikeLegacySoftwareOut {
        public int SoftwareId { get; set; }
        public int LikeCount { get; set; }
    }
}