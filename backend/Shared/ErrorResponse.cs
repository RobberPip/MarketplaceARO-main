using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace Shared {
    public class ErrorResponse : ProblemDetails {
        [JsonIgnore]
        public string? Type { get; set; }

        [JsonIgnore]
        public string? Instance { get; set; }

        [JsonIgnore]
        public IDictionary<string, object?> Extensions { get; set; }
        public Dictionary<string, string> Errors { get; set; }

        public ErrorResponse() { }
        // TODO ValidationProblem
        public ErrorResponse(int status, string? title, string? detail = null, Dictionary<string, string> errors = null) {
            Status = status;
            Title = title;
            Detail = detail;
            Errors = errors;
        }
    }
}
