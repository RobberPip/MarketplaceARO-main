using System.Globalization;
using System.Text.Json;
using DB;
using DB.Entities;
using FluentValidation;
using FluentValidation.Results;
using Market.Account;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared;
using static Market.General;
using static ProductQueryValidator;

namespace Market.Product;

public class GetProductOut {
    public Guid Id { get; set; }
    public List<GetAttributesDataOut> ProductData { get; set; } = new();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? Website { get; set; }
    public List<AvatarOut> Images { get; set; } = new();
    public List<GetAccountSummaryOut> Rightholders { get; set; } = new();
    public required ProductStatus Status { get; set; }
    public required ProductTypeOut? Type { get; set; }
    public string CreatedAt { get; set; }
    public string? UpdatedAt { get; set; }
    public static GetProductOut From(DB.Entities.Product source, IEnumerable<AttributeMeta> metas) => new() {
        Id = source.Id,
        Name = source.Name,
        ProductData = source.ProductData != null
        ? GetAttributesDataOut.From(source.ProductData, metas).ToList()
        : new List<GetAttributesDataOut>(),
        Status = source.Status,
        Type = source.Type != null ? ProductTypeOut.From(source.Type) : null,
        Images = source.Files != null ? source.Files.Select(i => AvatarOut.From(i)).ToList() : new List<AvatarOut>(),
        Description = source.Description,
        Website = source.Website,
        Rightholders = source.RightholdersAccounts.Select(rh => new GetAccountSummaryOut {
            Id = rh.Id,
            ShortName = rh.ShortName
        }).ToList(),
        CreatedAt = source.CreatedAt.ToString("dd.MM.yyyy"),
        UpdatedAt = source.UpdatedAt?.ToString("dd.MM.yyyy")
    };
}
public class ProductOptionsOut {
    public required List<ProductTypeOut> ProductTypes { get; set; } = [];
}
public class ProductTypeOut {
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? ShortName { get; set; }
    public string? CombinedName { get; set; }
    public static ProductTypeOut From(ProductType? source) => new() {
        Id = source.Id,
        FullName = source.FullName,
        ShortName = source.ShortName,
        CombinedName = source.CombinedName
    };
}

public class GetAttributesDataOut {
    public Guid AttributeId { get; set; }
    public string AttributeName { get; set; } = default!;
    public string? DataType { get; set; }
    public object Value { get; set; } = default!;


    public static IEnumerable<GetAttributesDataOut> From(ProductData data, IEnumerable<AttributeMeta> metas) {
        foreach (var entry in data.Data) {
            var meta = metas.FirstOrDefault(m => m.Id == entry.AttributeId);
            yield return new GetAttributesDataOut {
                AttributeId = entry.AttributeId,
                AttributeName = meta?.Name ?? "Unknown",
                DataType = meta?.DataType,
                Value = entry.Value
            };
        }
    }
}


public static partial class Product {

    public static async Task<Results<
            Ok<GetProductOut>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
        >>
        GetProduct(AppDbContext db, Guid id) {
        var product = await db.Products
        .Include(p => p.ProductData)
        .Include(p => p.RightholdersAccounts)
        .Include(p => p.Files)
        .FirstOrDefaultAsync(a => a.Id == id);

        if (product == null)
            return TypedResults.NotFound(new ErrorResponse { Detail = "Product not found" });

        var attributeIds = product.ProductData != null
            ? product.ProductData.Data.Select(e => e.AttributeId).Distinct().ToList()
            : new List<Guid>();

        var metas = await db.AttributeMeta
            .Where(m => attributeIds.Contains(m.Id))
            .ToListAsync();


        return TypedResults.Ok(GetProductOut.From(product, metas));
    }
    /// <summary>
    /// Публичный метод для поиска в реестре
    /// </summary>
    /// <param name="db"></param>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<IResult> GetProducts(AppDbContext db, HttpRequest request) {
        // 1. Загружаем метаданные
        var metas = await db.AttributeMeta.ToListAsync();

        // 2. Собираем фильтр из query
        var filter = request.Query.ToDictionary(k => k.Key, v => v.Value.ToArray());

        if (!filter.Any())
            return TypedResults.Ok(new List<object>());

        // 3. Начинаем запрос к продуктам с подгрузкой ProductData
        IQueryable<DB.Entities.Product> query = db.Products.Include(p => p.ProductData);

        // 4. Обрабатываем каждый атрибут
        foreach (var (key, values) in filter) {
            var meta = metas.FirstOrDefault(m => m.Name == key);
            if (meta == null) continue;

            // Логическое И для всех значений одного атрибута
            foreach (var v in values) {
                var jsonString = JsonSerializer.Serialize(new[] { new ProductDataEntry { AttributeId = meta.Id, Value = v } });
                var jsonArray = JsonSerializer.Serialize(new[] { new ProductDataEntry { AttributeId = meta.Id, Value = new[] { v } } });

                query = query.Where(p =>
                    p.ProductData != null &&
                    (EF.Functions.JsonContains(p.ProductData.Data, jsonString) ||
                     EF.Functions.JsonContains(p.ProductData.Data, jsonArray))
                );
            }
        }

        // 5. Выполняем запрос
        var products = await query.ToListAsync();

        if (!products.Any())
            return TypedResults.NotFound(new ErrorResponse { Detail = "Products not found" });

        // 6. Формируем метаданные для результата
        var metasForResult = metas
            .Where(m =>
                products.Any(p => p.ProductData != null &&
                    p.ProductData.Data.Any(e => e.AttributeId == m.Id)
                )
            )
            .ToList();

        // 7. Подготовка результата
        var result = products.Select(p => GetProductOut.From(p, metasForResult)).ToList();

        return TypedResults.Ok(result);
    }

    public static async Task<Results<
            Ok<List<GetProductOut>>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
        >> GetAccountProducts(AppDbContext db, HttpRequest request, Ctx ctx) {
        var account = await ctx.GetCurrentAccount();
        if (account is null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Account not found."
            });
        }
        IQueryable<DB.Entities.Product> products = db.Products.Where(p => p.OwnerId == account.Id);

        if (!products.Any())
            return TypedResults.NotFound(new ErrorResponse { Detail = "Products not found" });


        var result = products
            .Select(p => GetProductOut.From(p, null))
            .ToList();

        return TypedResults.Ok(result);
    }
    public static async Task<ProductOptionsOut> GetProductOptions(AppDbContext db) {
        var types = await db.ProductTypes
        .ToListAsync();

        var productTypesOut = types.Select(ProductTypeOut.From).ToList();

        return new ProductOptionsOut {
            ProductTypes = productTypesOut
        };
    }
}