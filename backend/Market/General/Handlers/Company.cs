using DB;
using Shared;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Db.Entities;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Dadata;

namespace Market;

public static partial class General {
    
    public class GetCompanyOut {
        /// <summary>ИНН</summary>
        public string? INN { get; set; }

        /// <summary>ОГРН</summary>
        public string? OGRN { get; set; }

        /// <summary>Сокращенное имя</summary>
        public string? ShortName { get; set; }

        /// <summary>Полное имя</summary>
        public string? FullName { get; set; }

        /// <summary>Дата ОГРН</summary>
        public DateTime? RegistrationDate { get; set; }

        /// <summary>Дата прекращения деятельности</summary>
        public DateTime? TerminationDate { get; set; }

        /// <summary>Адрес полный</summary>
        public string? FullAddress { get; set; }
        // TODO add fields
    }

    public static async Task<Results<
    Ok<List<GetCompanyOut>>,
    NotFound<ErrorResponse>,
    InternalServerError<ErrorResponse>
>> GetInfoCompanies(
    [FromServices] IConfiguration config,
    [FromQuery] string query
) {
        var apiKey = config["DaData:Token"];
        if (string.IsNullOrEmpty(apiKey)) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = "DaData API token is missing in configuration."
            });
        }

        if (string.IsNullOrWhiteSpace(query)) {
            return TypedResults.NotFound(new ErrorResponse {
                Detail = "Search query is required."
            });
        }

        try {
            var api = new SuggestClientAsync(apiKey);
            var result = await api.SuggestParty(query);

            if (result?.suggestions == null || result.suggestions.Count == 0) {
                return TypedResults.NotFound(new ErrorResponse {
                    Detail = "No companies found for the given query."
                });
            }

            var companies = result.suggestions
                .Where(s => s.data != null)
                .Select(s => new GetCompanyOut {
                    INN = s.data.inn,
                    OGRN = s.data.ogrn,
                    ShortName = s.value,
                    FullName = s.data.name?.full_with_opf,
                    RegistrationDate = s.data.state?.registration_date,
                    TerminationDate = s.data.state?.liquidation_date,
                    FullAddress = s.data.address?.value,
                })
                .ToList();

            return TypedResults.Ok(companies);
        } catch (Exception ex) {
            return TypedResults.InternalServerError(new ErrorResponse {
                Detail = $"Exception: {ex.Message}"
            });
        }
    }


    private static string? GetPropertyOrDefault(this JsonElement element, string propertyName) {
        return element.TryGetProperty(propertyName, out var prop) ? prop.GetString() : null;
    }

    private static DateTime? GetPropertyAsDate(this JsonElement element, string propertyName) {
        if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String &&
            DateTime.TryParse(prop.GetString(), out var dt)) {
            return dt;
        }
        return null;
    }
}
