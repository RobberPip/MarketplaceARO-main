namespace RegistryApp.Models {
    public class BaseResponse<T> {
        public bool IsOkay { get; set; }
        public string? Description { get; set; }
        public T? Data { get; set; }
    }
}
