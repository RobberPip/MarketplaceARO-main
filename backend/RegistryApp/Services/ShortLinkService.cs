using RegistryApp.DB;
using RegistryApp.Utils.Encoder;
using RegistryApp.Utils.Helpers;
using RegistryApp.Models;
using RegistryApp.Models.SoftwareFilters;

namespace RegistryApp.Services {
    public class ShortLinkService {
        private readonly PostgreSqlAdapter _dbProvider;
        public ShortLinkService(PostgreSqlAdapter dbProvider) {
            _dbProvider = dbProvider;
        }

        public async Task<BaseResponse<string>> GetFiltersShortLinkAsync(Dictionary<string, string> pairs, string host) {
            try {
                var filtersDto = new FilterStageVM(pairs);
                var json = JsonWorker.SerializeObject(filtersDto);
                var hash = json.GetHashSHA256();
                var random = new Random().Next(0, int.MaxValue);

                var uniqueId = await _dbProvider.GetFilterShortLinkUniqueId(random, hash, json);
                var buffer = BitConverter.GetBytes(uniqueId);
                var encodeLink = Base32.ToBase32String(buffer);

                return new BaseResponse<string>() {
                    IsOkay = true,
                    Data = $"{host}/fl/{encodeLink}"
                };
            } catch (Exception ex) {
                return new BaseResponse<string>() {
                    IsOkay = false,
                    Description = $"[ShortLinkService][GetFiltersShortLink()] with Exception:\n{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<FilterStageVM>> GetFiltersStageAsync(string encodedData) {
            try {
                var buffer = Base32.FromBase32String(encodedData);
                var decodedNum = BitConverter.ToInt32(buffer, 0);

                var jsonData = await _dbProvider.TryGetFilterStageByUniqueId(decodedNum);
                if (jsonData == null) {
                    return new BaseResponse<FilterStageVM>() {
                        IsOkay = false,
                        Description = "Link is obsolete or does not exist"
                    };
                }
                var filterModel = JsonWorker.DeSerializeObject<FilterStageVM>(jsonData);
                return new BaseResponse<FilterStageVM>() {
                    IsOkay = true,
                    Data = filterModel
                };
            } catch (Exception ex) {
                return new BaseResponse<FilterStageVM>() {
                    IsOkay = false,
                    Description = $"[ShortLinkService][GetFiltersStageAsync()] with Exception:\n{ex.Message}"
                };
            }
        }
    }
}
