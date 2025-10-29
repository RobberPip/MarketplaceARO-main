using RegistryApp.Models;

namespace RegistryApp.Services {
    public interface IVendorService {
        public Task<BaseResponse<IEnumerable<VendorVM>>> GetAllVendorsAsync();
        public Task<BaseResponse<string[]>> GetAllVendorsNameAsync();
        public Task<BaseResponse<VendorVM>> GetByIdAsync(int? id, string hostname);
    }
}
