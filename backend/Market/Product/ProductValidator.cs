using System.Globalization;
using FluentValidation;
using DB.Entities;
using System.Text.Json;

public class ProductQueryValidator : AbstractValidator<Dictionary<string, JsonElement>> {
    private List<AttributeMeta> _metas = new();

    public void SetMetas(List<AttributeMeta> metas) {
        _metas = metas;

        RuleFor(x => x).Custom((dict, context) => {
            foreach (var kv in dict) {
                var meta = _metas.FirstOrDefault(m => m.Name == kv.Key);
                if (meta == null) {
                    context.AddFailure($"Неизвестный фильтр {kv.Key}");
                    continue;
                }

                // получаем все значения как IEnumerable<string>
                IEnumerable<string> values;
                if (kv.Value.ValueKind == JsonValueKind.Array) {
                    values = kv.Value.EnumerateArray().Select(v => v.GetString() ?? string.Empty);
                } else {
                    values = new[] { kv.Value.GetString() ?? kv.Value.ToString() ?? string.Empty };
                }

                // проверяем каждое значение
                foreach (var value in values) {
                    if ((!ProductFilterHelper.TryCast(value, meta, out _)) ||
                        (meta.AvailableValues.Any() && !meta.AvailableValues.Contains(value))) {
                        context.AddFailure($"Некорректное или недопустимое значение {value} для фильтра {kv.Key}");
                    }
                }
            }
        });
    }

    public static class ProductFilterHelper {
        public static bool TryCast(string raw, AttributeMeta meta, out object result) {
            result = raw;
            try {
                result = meta.DataType?.ToLower() switch {
                    "int" => int.Parse(raw, CultureInfo.InvariantCulture),
                    "decimal" => decimal.Parse(raw.Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture),
                    "bool" => bool.Parse(raw),
                    "datetime" => DateTime.Parse(raw, CultureInfo.InvariantCulture),
                    _ => raw
                };
                return true;
            } catch {
                return false;
            }
        }
    }
}