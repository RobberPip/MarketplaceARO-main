

// namespace Market;

// public class Software {
//     public Guid Id { get; set; }
// }


// public class GetSoftwareOut {
//     public Guid Id { get; set; }
//     public required string Name { get; set; }
//     public required string Description { get; set; }
//     public required double Rating { get; set; }
//     public Guid ProfileId { get; set; }
//     public required string Version { get; set; }
//     public required DateTime DataUpdate { get; set; }
//     public SoftwareAttributes Attributes { get; set; }
//     public required int Price { get; set; }
//     public required Status Status { get; set; }
// }

// //TODO Атрибуты ПО
// public class SoftwareAttributes {
//     public List<string> BasicSoftware { get; set; }
//     public List<string> ClassificationStage { get; set; }
// }
// [JsonConverter(typeof(JsonStringEnumConverter))]
// public enum Status {
//     Approved,
//     Pending,
//     Rejected
// }

// public static class Products {
//     public static void AddProductsEndpoints(this WebApplication app) {
//         var router = app.MapGroup("/")
//           // .RequireAuthorization()
//           .WithOpenApi()
//           .WithTags(["Market"]);

//         // TODO: delete
//         router.MapGet("/api/v1/software/{id}", GetSoftware).WithOpenApi(op => {
//             op.Parameters[0].Examples.Add("ExampleId", new OpenApiExample {
//                 Value = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6")
//             });
//             return op;
//         });
//         var routerAuth = app.MapGroup("/")
//             .RequireAuthorization()
//             .WithOpenApi()
//             .WithTags(["Market"]);
//         routerAuth.MapGet("/api/v1/products", GetUserProducts)
//             .WithOpenApi(op => {
//                 op.Parameters[0].Name = "profileId";
//                 op.Parameters[0].Description = "Получить все продукты пользователя";
//                 op.Parameters[0].Examples.Add("ExampleId", new OpenApiExample {
//                     Value = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6") // пример значения для profile
//                 });

//                 return op;
//             });

//     }


//             LikeLegacySoftwareIn args,
//             AppDbContext db,
//             Ctx ctx
//         ) {
//         var exists = await db.Database.SqlQueryRaw<int>("SELECT id from software where id = {0}", args.SoftwareId).AnyAsync();
//         if (!exists) {
//             return TypedResults.NotFound(new ErrorResponse { Detail = "Software not found." });
//         }

//         using var transaction = db.Database.BeginTransaction();

//         try {
//             var currentReview = await db.Reviews.Where(r => r.AuthorId == ctx.UserId && r.LegacySoftwareId == args.SoftwareId).FirstOrDefaultAsync();

//             if (currentReview == null) {
//                 var review = new Review {
//                     AuthorId = ctx.UserId,
//                     Like = args.Like,
//                     LegacySoftwareId = args.SoftwareId
//                 };
//                 db.Reviews.Add(review);
//             } else {
//                 currentReview.Like = args.Like;
//             }

//             await db.SaveChangesAsync();

//             var sql = @"
//             UPDATE ""software""
//             SET
//                 like_count = (
//                     SELECT COUNT(*)
//                     FROM ""Reviews""
//                     WHERE ""LegacySoftwareId"" = {0} AND ""Like"" = true
//                 )
//             WHERE id = {0};";

//             await db.Database.ExecuteSqlRawAsync(sql, args.SoftwareId);
//             await transaction.CommitAsync();

//             var res = await db.Database.SqlQueryRaw<LikeLegacySoftwareOut>(
//                 @"SELECT s.id as SoftwareId, s.rating as Rating, s.like_count as LikeCount FROM software AS s WHERE s.id = {0}",
//                 args.SoftwareId
//             ).ToListAsync();

//             return TypedResults.Ok(new LikeLegacySoftwareOut {
//                 SoftwareId = args.SoftwareId,
//                 LikeCount = res[0].LikeCount
//             });

//         } catch (Exception e) {
//             // TODO
//             Console.WriteLine(e);
//             return TypedResults.InternalServerError(new ErrorResponse { });
//         }
//     }


//     static Results<Ok<GetSoftwareOut>, NotFound> GetSoftware(Guid id, AppDbContext db) {
//         return TypedResults.Ok(new GetSoftwareOut {
//             Id = id,
//             Name = "CADLib Модель и Архив",
//             Description = "Информационная система для поддержки жизненного цикла объектов капитального строительства и технологического оборудования промышленных предприятий, которая обеспечивает управление процессом проектирования, проверку 3D-моделей, информационную поддержку в процессе строительства и эксплуатации зданий, сооружений и оборудования. Программный продукт предназначен для организации среды общих данных, коллективного доступа и управления инженерными данными информационной модели, обеспечивает структурирование, хранение, визуализацию, проверку коллизий информационных моделей.",
//             Rating = 3,
//             Version = "1.0.0",
//             DataUpdate = DateTime.Today,
//             Price = 143000,
//             Status = Status.Pending,
//             Attributes = new SoftwareAttributes() {
//                 BasicSoftware = new List<string> { "Renga", "Revit" },
//                 ClassificationStage = new List<string> {
//                     "Строительство", "Проектирование"
//                 }
//             }
//         });
//     }
//     static Results<Ok<PagedResult<GetSoftwareOut>>, NotFound> GetUserProducts(Guid profileId, AppDbContext db) {
//         var products = new List<GetSoftwareOut>
//         {
//             new GetSoftwareOut
//             {
//                 ProfileId = profileId,
//                 Name = "CADLib Модель и Архив",
//                 Description = "Продукт помогает проверять BIM коллизии",
//                 Rating = 3,
//                 Version = "1.0.0",
//                 DataUpdate = DateTime.Today,
//                 Price = 143000,
//                 Status = Status.Pending,
//                 Attributes = new SoftwareAttributes() {
//                     BasicSoftware = new List<string>
//                     {
//                         "Atr1", "Atr2", "Atr3", "Atr4", "Atr5",
//                         "Atr6", "Atr7", "Atr8", "Atr9", "Atr10",
//                         "Atr11", "Atr12", "Atr13", "Atr14", "Atr15",
//                         "Atr16", "Atr17", "Atr18", "Atr19", "Atr20"
//                     },
//                     ClassificationStage = new List<string> {
//                         "Строительство", "Проектирование"
//                     }
//                 }
//             },
//             new GetSoftwareOut
//             {
//                 ProfileId = profileId,
//                 Name = "КОМПАС 3D",
//                 Description = "Продукт помогает с проектированием моделей",
//                 Rating = 4,
//                 Version = "1.0.0",
//                 DataUpdate = DateTime.Today,
//                 Price = 50000,
//                 Status = Status.Approved,
//                 Attributes = new SoftwareAttributes() {
//                     BasicSoftware = new List<string>{"Renga","Revit"},
//                     ClassificationStage = new List<string> {
//                         "Строительство", "Проектирование"
//                     }
//                 }
//             },
//             new GetSoftwareOut
//             {
//                 ProfileId = profileId,
//                 Name = "PILOT-BIM",
//                 Description = "Продукт помогает с проектированием моделей",
//                 Rating = 4,
//                 Version = "1.0.0",
//                 DataUpdate = DateTime.Today,
//                 Price = 65000,
//                 Status = Status.Rejected,
//                 Attributes = new SoftwareAttributes() {
//                     BasicSoftware = new List<string>{"Pilot"},
//                     ClassificationStage = new List<string> {
//                         "Строительство", "Проектирование"
//                     }
//                 }
//             }
//         };

//         var pagedResult = new PagedResult<GetSoftwareOut> {
//             Data = products,
//             TotalCount = products.Count,
//             Page = 1,
//             PageSize = products.Count
//         };

//         return TypedResults.Ok(pagedResult);
//     }
// }
