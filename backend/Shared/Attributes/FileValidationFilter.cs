using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shared.Attributes {
    public class FileValidationFilter : ActionFilterAttribute {
        private readonly string[] _allowedFileTypes;
        private readonly long _allowedMaxSise;

        public FileValidationFilter(string[] allowedFileTypes, long allowedMaxSise) {
            _allowedFileTypes = allowedFileTypes;
            _allowedMaxSise = allowedMaxSise;
        }

        public override void OnActionExecuting(ActionExecutingContext context) {
            var param = context.ActionArguments.SingleOrDefault(p => p.Value is IFormFile);
            if (param.Value is not IFormFile file || file.Length == 0) {
                context.Result = new BadRequestObjectResult("File is null");
                return;
            }
            if (!IsFileSizeValid(file)) {
                var mbSize = _allowedMaxSise / 1024 / 1024;
                context.Result = new BadRequestObjectResult($"File shouldn't be more than the maximum allowed size ({mbSize}MB)");
                return;
            }
            if (!IsFileTypeValid(file)) {
                var allowedExtensionsMessage = string.Join(", ", _allowedFileTypes).Replace(".", "").ToLower();
                context.Result = new BadRequestObjectResult($"Invalid file type. Please upload {allowedExtensionsMessage} file.");
                return;
            }
        }

        private bool IsFileSizeValid(IFormFile file) => file.Length <= _allowedMaxSise;

        private bool IsFileTypeValid(IFormFile file) {
            var ext = Path.GetExtension(file.FileName).Replace(".", "").ToLower();
            return !string.IsNullOrEmpty(ext) && _allowedFileTypes.Contains(ext);
        }
    }
}
