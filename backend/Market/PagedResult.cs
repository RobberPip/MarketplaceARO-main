namespace Market;

// TODO плагинация
/// <summary>
/// Класс обертка для плагинации
/// </summary>
/// <typeparam name="T"></typeparam>
public class PagedResult<T> {
    public required List<T> Data { get; set; }
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}