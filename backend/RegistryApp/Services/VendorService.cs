using RegistryApp.DB;
using RegistryApp.Domain.Models;
using RegistryApp.Models;

namespace RegistryApp.Services {
    public class VendorService : IVendorService {
        private readonly PostgreSqlAdapter _dbProvider;

        public VendorService(PostgreSqlAdapter dbProvider) {
            _dbProvider = dbProvider;
        }

        public async Task<BaseResponse<IEnumerable<VendorVM>>> GetAllVendorsAsync() {
            var vendors = await _dbProvider.GetAllVendorsAsync();
            if (vendors != null) {
                return new BaseResponse<IEnumerable<VendorVM>>() {
                    IsOkay = true,
                    Data = vendors.Select(vendor => new VendorVM(vendor))
                };
            }
            return new BaseResponse<IEnumerable<VendorVM>>() {
                IsOkay = true,
                Description = "[VendorService][GetAllVendorsAsync()]"
            };
        }

        public async Task<BaseResponse<string[]>> GetAllVendorsNameAsync() {
            var vendors = await GetAllVendorsAsync();
            if (vendors.IsOkay) {
                return new BaseResponse<string[]>() {
                    IsOkay = true,
                    Data = vendors.Data?.OrderBy(v => v.Name).Select(v => v.Name).ToArray(),
                };
            }
            return new BaseResponse<string[]>() {
                IsOkay = true,
                Description = "[VendorService][GetAllVendorsNameAsync()]"
            };
        }

        public async Task<BaseResponse<VendorVM>> GetByIdAsync(int? id, string hostname) {
            if (id != null) {
                try {
                    var vendor = await _dbProvider.GetVendorByIdAsync((long)id);
                    if (vendor != null) {
                        int countInReestr = await GetCountFilteredSoftwaresByVendor(vendor.Name, ("IsHasReestr", true));
                        return new BaseResponse<VendorVM> {
                            IsOkay = true,
                            Data = new VendorVM(vendor, hostname, countInReestr)
                        };
                    }
                    return new BaseResponse<VendorVM> {
                        IsOkay = false,
                        Description = $"[VendorService][GetByIdAsync()] is null"
                    };
                } catch (Exception ex) {
                    return new BaseResponse<VendorVM>() {
                        IsOkay = false,
                        Description = $"[VendorService][GetByIdAsync()] with Exception:\n{ex.Message}"
                    };
                }
            }
            return new BaseResponse<VendorVM>() {
                IsOkay = false,
                Description = $"[VendorService][GetByIdAsync()] passed Id is null"
            };
        }

        private async Task<int> GetCountFilteredSoftwaresByVendor(string vendorName, (string property, object value)? filterCondition) {
            var filterArr = new List<(string, object)>() { ("VendorName", vendorName) };
            if (filterCondition != null)
                filterArr.Add(((string, object))filterCondition);

            var filtersForDb = new Filters().GetFilters(filterArr.ToArray());
            var softwares = await _dbProvider.GetSoftwareFilterAsync(filtersForDb);

            return softwares.Count();
        }
    }
}
