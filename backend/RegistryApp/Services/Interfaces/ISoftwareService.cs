using RegistryApp.Domain.Enums;
using RegistryApp.Models;

namespace RegistryApp.Services {
    public interface ISoftwareService {
        public Task<BaseResponse<string[]>> GetPossibleAttributesValueAsync(string attrName);
        public Task<BaseResponse<string[]>> GetAllSoftwaresAttributesValueAsync(string attrName);
        public Task<BaseResponse<IEnumerable<SoftwareVM>>> GetAndProcessingAsync(int pageSize, Dictionary<string, string>? dataParams = null, string? userId = null);
        public Task<BaseResponse<IEnumerable<SoftwareVM>>> GetBySortAsync(SortBy sort, int amount);
        public Task<BaseResponse<int>> GetSoftwaresCountAsync(Dictionary<string, string>? dataParams = null);
        public Task<BaseResponse<SoftwareVM>> GetByIdAsync(int? id, string hostname,string? userId = null);
    }
}
