using System.Text.Json;
using DB;
using DB.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Shared;
using static ProductQueryValidator;

namespace Market.Product;

public class AddProductDraftIn {
    public required string Name { get; set; }
    public required Guid TypeId { get; set; }
}
public class UpdateProductIn {
    public string? Description { get; set; }
    public string? Website { get; set; }
    public List<Guid> Rightholders { get; set; } = new();
    public Dictionary<string, JsonElement> ProductData { get; set; } = new();
}
public static partial class Product {
    public static async Task<Results<
            Ok,
            BadRequest<ErrorResponse>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
        >>
        DeleteProduct(AppDbContext db, Guid id, Ctx ctx) {

        var product = await db.Products
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Product not found."
            });
        var activeAccount = await ctx.GetCurrentAccount();
        if (activeAccount == null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Account not found."
            });
        }

        if (product.OwnerId != activeAccount.Id) {
            return TypedResults.BadRequest(new ErrorResponse {
                Detail = "Access denied. Account not associated with this product."
            });
        }
        if (product.Status == ProductStatus.Draft) {
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return TypedResults.Ok();
        }

        return TypedResults.BadRequest(new ErrorResponse());
    }

    public static async Task<Results<
        Ok<GetProductOut>,
        BadRequest<ErrorResponse>,
        NotFound<ErrorResponse>,
        InternalServerError<ErrorResponse>
    >>
    AddProductDraft(AppDbContext db, AddProductDraftIn input, Ctx ctx) {
        try {
            var activeAccount = await ctx.GetCurrentAccount();
            if (activeAccount is null) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "Account not found."
                });
            }
            var product = new DB.Entities.Product {
                Id = Guid.NewGuid(),
                Name = input.Name,
                TypeId = input.TypeId,
                Status = ProductStatus.Draft,
                OwnerId = activeAccount.Id,
                CreatedAt = DateTime.UtcNow
            };
            product.RightholdersAccounts.Add(activeAccount);
            db.Products.Add(product);
            try {
                await db.SaveChangesAsync();
            } catch (Exception ex) {
                var innerEx = ex.InnerException?.Message ?? ex.Message;
                return TypedResults.InternalServerError(new ErrorResponse {
                    Detail = "Error occurred while adding product: " + innerEx
                });
            }


            var productWithIncludes = await db.Products
            .Include(p => p.Type)
            .Include(p => p.RightholdersAccounts)
            .Include(p => p.ProductData)
            .FirstOrDefaultAsync(p => p.Id == product.Id);

            if (productWithIncludes == null) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "Product not found after saving."
                });
            }

            var output = GetProductOut.From(productWithIncludes, null!);

            return TypedResults.Ok(output);
        } catch (Exception ex) {
            return TypedResults.InternalServerError(
                new ErrorResponse { Detail = "Ошибка при добавлении продукта: " + ex.Message }
            );
        }
    }
    public static async Task<Results<
            Ok<GetProductOut>,
            BadRequest<ErrorResponse>,
            NotFound<ErrorResponse>,
            InternalServerError<ErrorResponse>
        >>
    UpdateProduct(AppDbContext db, Guid id, UpdateProductIn input, Ctx ctx) {
        try {
            var product = await db.Products
                .Include(p => p.RightholdersAccounts)
                .Include(p => p.ProductData)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "Product not found."
                });
            var activeAccount = await ctx.GetCurrentAccount();
            if (activeAccount == null) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "Account not found."
                });
            }
            if (product.OwnerId != activeAccount.Id) {
                return TypedResults.BadRequest(new ErrorResponse {
                    Detail = "Access denied. Account not associated with this product."
                });
            }

            var attributeNames = input.ProductData.Keys.ToList();
            var metas = await db.AttributeMeta
                .Where(m => attributeNames.Contains(m.Name))
                .ToListAsync();

            var validator = new ProductQueryValidator();
            validator.SetMetas(metas);

            var validationResult = await validator.ValidateAsync(input.ProductData);
            if (!validationResult.IsValid)
                return TypedResults.BadRequest(new ErrorResponse {
                    Detail = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage))
                });


            product.Description = input.Description ?? product.Description;
            product.Website = input.Website ?? product.Website;


            product.RightholdersAccounts.Clear();
            product.RightholdersAccounts.Add(activeAccount);

            if (input.Rightholders.Count > 0) {
                var rightIds = input.Rightholders.Distinct().ToList();
                var accounts = await db.Accounts
                    .Where(a => rightIds.Contains(a.Id))
                    .ToListAsync();

                if (accounts.Count != rightIds.Count) {
                    var missing = rightIds.Except(accounts.Select(a => a.Id)).ToList();
                    return TypedResults.BadRequest(new ErrorResponse {
                        Detail = $"Неизвестные правообладатели: {string.Join(", ", missing)}"
                    });
                }

                foreach (var a in accounts)
                    product.RightholdersAccounts.Add(a);
            }

            if (input.ProductData != null && input.ProductData.Any()) {
                if (product.ProductData == null) {
                    var newProductData = new ProductData {
                        Id = Guid.NewGuid(),
                        Data = new List<ProductDataEntry>()
                    };
                    db.ProductsData.Add(newProductData);
                    product.ProductData = newProductData;
                } else {
                    product.ProductData.Data.Clear();
                }

                foreach (var kv in input.ProductData) {
                    var meta = metas.FirstOrDefault(m => m.Name == kv.Key);
                    if (meta == null) continue;

                    object value;

                    if (kv.Value.ValueKind == JsonValueKind.Array) {
                        value = kv.Value.EnumerateArray()
                            .Select(v => ProductFilterHelper.TryCast(v.GetString()!, meta, out var vv) ? vv : v.GetString()!)
                            .ToArray();
                    } else {
                        var raw = kv.Value.GetString() ?? kv.Value.ToString()!;
                        value = ProductFilterHelper.TryCast(raw, meta, out var vv) ? vv : raw;
                    }

                    product.ProductData.Data.Add(new ProductDataEntry {
                        AttributeId = meta.Id,
                        Value = value
                    });
                }
                db.Entry(product.ProductData).Property(pd => pd.Data).IsModified = true;
            } else {
                if (product.ProductData != null) {
                    var pdToRemove = product.ProductData;
                    product.ProductData = null;
                    db.ProductsData.Remove(pdToRemove);
                }
            }

            try {
                await db.SaveChangesAsync();
            } catch (Exception ex) {
                var innerEx = ex.InnerException?.Message ?? ex.Message;
                throw new Exception("Ошибка сохранения: " + innerEx, ex);
            }


            var output = GetProductOut.From(product, metas);

            return TypedResults.Ok(output);

        } catch (Exception ex) {
            return TypedResults.InternalServerError(
                new ErrorResponse { Detail = "Ошибка при обновлении продукта: " + ex.Message }
            );
        }
    }

    public static async Task<IResult> AddProductImages(
    AppDbContext db,
    Ctx ctx,
    Guid id,
    IFormFileCollection files
) {
        var activeAccount = await ctx.GetCurrentAccount();
        if (activeAccount == null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Account not found."
            });
        }
        var filesList = files?.ToList() ?? new List<IFormFile>();

        if (filesList.Count > 10) {
            return TypedResults.BadRequest(new ErrorResponse {
                Detail = "Maximum 10 files allowed."
            });
        }

        var product = await db.Products
            .Include(p => p.Files)
            .FirstOrDefaultAsync(p => p.Id == id);
        if (product.Status == ProductStatus.Pending) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Product Pending."
            });
        }
        if (product == null) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Product not found."
            });
        }

        var filesService = new FilesService(db);
        var filesImg = await filesService.AddImageFilesAsync(filesList);

        product.Files?.Clear();
        product.Files?.AddRange(filesImg);

        await db.SaveChangesAsync();

        return TypedResults.Ok(filesImg);
    }

}