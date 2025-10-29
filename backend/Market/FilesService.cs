using DB;
using Shared;
using Microsoft.EntityFrameworkCore;
using Db.Entities;
using System.IO;

namespace Market {
    public class FilesService(AppDbContext db) {
        private static readonly string[] AllowedExtensions = { "jpg", "jpeg", "png" };
        private readonly AppDbContext _db = db;

        public async Task<Result<DB.Entities.File, ErrorResponse>> CreateOrUpdateAvatarAsync<T>(
            Ctx ctx, IFormFile imageFile) where T : class, IAvatarOwner {
            if (imageFile == null || !AllowedExtensions
                .Contains(Path.GetExtension(imageFile.FileName).TrimStart('.').ToLower())) {
                return Result<DB.Entities.File, ErrorResponse>.Fail(new ErrorResponse { Detail = "Invalid file type. Only jpg, jpeg, and png are allowed." });
            }

            if (imageFile.Length > 3 * 1024 * 1024) {
                return Result<DB.Entities.File, ErrorResponse>.Fail(new ErrorResponse { Detail = "File size exceeds the 3MB limit." });
            }

            var entity = await ctx.GetEntityAsync<T>();
            if (entity == null) {
                return Result<DB.Entities.File, ErrorResponse>.Fail(new ErrorResponse { Detail = $"{typeof(T).Name} not found." });
            }

            byte[] fileBytes;
            using (var ms = new MemoryStream()) {
                await imageFile.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            var newAvatar = new DB.Entities.File {
                Name = imageFile.FileName,
                Content = fileBytes,
                ContentType = Path.GetExtension(imageFile.FileName).TrimStart('.').ToLower()
            };

            var avatar = await _db.Files.FirstOrDefaultAsync(a => a.Id == entity.AvatarId);

            if (avatar != null) {
                avatar.Name = newAvatar.Name;
                avatar.Content = newAvatar.Content;
                avatar.ContentType = newAvatar.ContentType;
                avatar.UpdatedAt = DateTime.UtcNow;
            } else {
                _db.Files.Add(newAvatar);
                await _db.SaveChangesAsync();
            }

            entity.AvatarId = avatar?.Id ?? newAvatar.Id;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<DB.Entities.File, ErrorResponse>.Ok(avatar ?? newAvatar);
        }

        public async Task<Result<bool, ErrorResponse>> DeleteAvatarAsync<T>(Ctx ctx) where T : class, IAvatarOwner {
            var entity = await ctx.GetEntityAsync<T>();
            if (entity == null) {
                return Result<bool, ErrorResponse>.Fail(new ErrorResponse { Detail = $"{typeof(T).Name} not found." });
            }

            var avatar = await _db.Files.FirstOrDefaultAsync(f => f.Id == entity.AvatarId);
            if (avatar == null) {
                return Result<bool, ErrorResponse>.Fail(new ErrorResponse { Detail = "Avatar not found." });
            }

            _db.Files.Remove(avatar);
            entity.AvatarId = null;
            entity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return Result<bool, ErrorResponse>.Ok(true);
        }

        public async Task<List<DB.Entities.File>> AddImageFilesAsync(IEnumerable<IFormFile> imageFiles) {
            var files = new List<DB.Entities.File>();

            foreach (var imageFile in imageFiles) {
                byte[] fileBytes;
                using (var ms = new MemoryStream()) {
                    await imageFile.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                }

                var file = new DB.Entities.File {
                    Name = imageFile.FileName,
                    Content = fileBytes,
                    ContentType = Path.GetExtension(imageFile.FileName).TrimStart('.').ToLower(),
                    UpdatedAt = DateTime.UtcNow
                };

                _db.Files.Add(file);
                files.Add(file);
            }

            await _db.SaveChangesAsync();

            return files;
        }

        public class Result<TSuccess, TError> {
            public bool IsFailed { get; private set; }
            public TError? Error { get; private set; }
            public TSuccess? Success { get; private set; }

            public static Result<TSuccess, TError> Fail(TError error) =>
                new Result<TSuccess, TError> { IsFailed = true, Error = error };

            public static Result<TSuccess, TError> Ok(TSuccess success) =>
                new Result<TSuccess, TError> { IsFailed = false, Success = success };
        }
    }
}