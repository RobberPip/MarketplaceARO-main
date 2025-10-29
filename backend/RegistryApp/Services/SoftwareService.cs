using RegistryApp.DB;
using RegistryApp.Domain.Enums;
using RegistryApp.Domain.Models;
using RegistryApp.Models;

namespace RegistryApp.Services {
    public class SoftwareService : ISoftwareService {
        private readonly PostgreSqlAdapter _dbProvider;

        public SoftwareService(PostgreSqlAdapter dbProvider) {
            _dbProvider = dbProvider;
        }

        public async Task<BaseResponse<string[]>> GetPossibleAttributesValueAsync(string attrName) {
            try {
                var data = await _dbProvider.GetByNameAttributesPossibleValuesAsync(attrName);
                if (data != null) {
                    return new BaseResponse<string[]>() {
                        IsOkay = true,
                        Data = data
                    };
                }
                return new BaseResponse<string[]>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetPossibleAttributesValue()]"
                };
            } catch (Exception ex) {
                return new BaseResponse<string[]>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetPossibleAttributesValue()] with Exception:\n{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<string[]>> GetAllSoftwaresAttributesValueAsync(string attrName) {
            try {
                var softwares = await _dbProvider.GetAllSoftwareAsync();
                var data = softwares.SelectMany(s => s.Attributes
                                    .Where(pair => pair.Key == attrName && pair.Value is string[])
                                    .Select(pair => pair.Value as string[])).SelectMany(s => s.Select(s => s.Trim()))
                                    .Distinct().OrderBy(s => s).ToArray();
                if (data != null) {
                    return new BaseResponse<string[]>() {
                        IsOkay = true,
                        Data = data
                    };
                }
                return new BaseResponse<string[]>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetAllSoftwaresAttributesValueAsync()]"
                };
            } catch (Exception ex) {
                return new BaseResponse<string[]>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetAllSoftwaresAttributesValueAsync()] with Exception:\n{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<SoftwareVM>>> GetBySortAsync(SortBy sort, int amount) {
            try {
                var softwares = await _dbProvider.GetAllSoftwareSortedAsync(sort, amount);
                if (softwares != null) {
                    return new BaseResponse<IEnumerable<SoftwareVM>>() {
                        IsOkay = true,
                        Data = softwares.Select(soft => new SoftwareVM(soft))
                    };
                }
                return new BaseResponse<IEnumerable<SoftwareVM>>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetBySortAsync()] is null"
                };
            } catch (Exception ex) {
                return new BaseResponse<IEnumerable<SoftwareVM>>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetBySortAsync()] with Exception:\n{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<IEnumerable<SoftwareVM>>> GetAndProcessingAsync(int pageSize, Dictionary<string, string>? dataParams, string? userId = null) {
            try {
                if (dataParams == null) {
                    var softwares = await _dbProvider.GetAllSoftwareSortedAsync(SortBy.LikeCount, 1, pageSize, userId);
                    if (softwares != null) {
                        return new BaseResponse<IEnumerable<SoftwareVM>>() {
                            IsOkay = true,
                            Data = softwares.Select(soft => new SoftwareVM(soft))
                        };
                    }
                    return new BaseResponse<IEnumerable<SoftwareVM>>() {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetAllSoftwareSortedAsync()] is null"
                    };
                } else {
                    List<Software> softwares;
                    var sort = DefineSortType(dataParams);
                    var pageNum = DefinePageRequest(dataParams, "current_page");

                    var filtersRawData = ParseRequestDataForFilter(dataParams);
                    if (filtersRawData.Length != 0) {
                        var filtersForDb = new Filters().GetFilters(filtersRawData);
                        softwares = await _dbProvider.GetSoftwareFilterSortedAsync(filtersForDb, sort, pageNum, pageSize, userId);
                    } else {
                        softwares = await _dbProvider.GetAllSoftwareSortedAsync(sort, pageNum, pageSize, userId);
                    }

                    if (softwares != null) {
                        return new BaseResponse<IEnumerable<SoftwareVM>>() {
                            IsOkay = true,
                            Data = softwares.Select(soft => new SoftwareVM(soft))
                        };
                    }
                    return new BaseResponse<IEnumerable<SoftwareVM>>() {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetSoftwareFilterSortedAsync()] is null"
                    };
                }
            } catch (Exception ex) {
                return new BaseResponse<IEnumerable<SoftwareVM>>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetAndProcessingAsync()] with Exception:\n{ex.Message}"
                };
            }
        }

        public async Task<BaseResponse<SoftwareVM>> GetByIdAsync(int? id, string hostname, string? userId = null) {
            if (id != null) {
                try {
                    var software = await _dbProvider.GetSoftwareByIdAsync((long)id, userId);
                    if (software != null) {
                        return new BaseResponse<SoftwareVM> {
                            IsOkay = true,
                            Data = new SoftwareVM(software, hostname)
                        };
                    }
                    return new BaseResponse<SoftwareVM> {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetByIdAsync()] is null"
                    };
                } catch (Exception ex) {
                    return new BaseResponse<SoftwareVM>() {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetByIdAsync()] with Exception:\n{ex.Message}"
                    };
                }
            }
            return new BaseResponse<SoftwareVM>() {
                IsOkay = false,
                Description = $"[SoftwareService][GetByIdAsync()] passed Id is null"
            };
        }

        public async Task<BaseResponse<int>> GetSoftwaresCountAsync(Dictionary<string, string>? dataParams = null) {
            try {
                if (dataParams != null) {
                    List<Software> softwares;
                    var filtersRawData = ParseRequestDataForFilter(dataParams);
                    if (filtersRawData.Length != 0) {
                        var filtersForDb = new Filters().GetFilters(filtersRawData);
                        softwares = await _dbProvider.GetSoftwareFilterAsync(filtersForDb);
                    } else {
                        softwares = await _dbProvider.GetAllSoftwareAsync();
                    }

                    if (softwares != null) {
                        return new BaseResponse<int>() {
                            IsOkay = true,
                            Data = softwares.Count()
                        };
                    }
                    return new BaseResponse<int>() {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetSoftwaresCount()] is null"
                    };
                } else {
                    var allSoftwares = await _dbProvider.GetAllSoftwareAsync();
                    if (allSoftwares != null) {
                        return new BaseResponse<int>() {
                            IsOkay = true,
                            Data = allSoftwares.Count()
                        };
                    }
                    return new BaseResponse<int>() {
                        IsOkay = false,
                        Description = $"[SoftwareService][GetSoftwaresCount()] is null"
                    };
                }
            } catch (Exception ex) {
                return new BaseResponse<int>() {
                    IsOkay = false,
                    Description = $"[SoftwareService][GetSoftwaresCount()] with Exception:\n{ex.Message}"
                };
            }
        }


        public static int DefinePageRequest(Dictionary<string, string>? pairs, string key) {
            int page;
            if (pairs != null && pairs.ContainsKey(key)) {
                if (int.TryParse(pairs[key], out page)) {
                    return page;
                }
            }
            return 1;
        }
        private static SortBy DefineSortType(Dictionary<string, string>? pairs) {
            int sort;
            if (pairs != null && pairs.ContainsKey("sort_type")) {
                if (int.TryParse(pairs["sort_type"], out sort)) {
                    if (sort == 1)
                        return SortBy.Date;
                    if (sort == 2)
                        return SortBy.Name;
                }
            }
            return SortBy.LikeCount;
        }
        private static bool DefineTagFilter(Dictionary<string, string>? pairs, string key) {
            if (pairs != null && pairs.ContainsKey(key)) {
                if (pairs[key] == "1")
                    return true;
            }
            return false; ;
        }

        private (string property, object value)[] ParseRequestDataForFilter(Dictionary<string, string> pairs) {
            var filterArr = new List<(string, object)>();

            var filterBySoftwareNames = pairs.GetValueOrDefault("search_request");
            if (!string.IsNullOrEmpty(filterBySoftwareNames))
                filterArr.Add(("search", filterBySoftwareNames));
            //filterArr.Add(("SoftwareName", filterBySoftwareNames));

            var filterByCompany = pairs.GetValueOrDefault("company_filter");
            if (!string.IsNullOrEmpty(filterByCompany))
                filterArr.Add(("VendorName", filterByCompany));

            var filterByActivity = pairs.GetValueOrDefault("activity_filter");
            if (!string.IsNullOrEmpty(filterByActivity))
                filterArr.Add(("Classification_Activity", filterByActivity));

            var filterByTypeSoftware = pairs.GetValueOrDefault("softwareType_filter");
            if (!string.IsNullOrEmpty(filterByTypeSoftware))
                filterArr.Add(("Classification_Type", filterByTypeSoftware));

            var filterByMainProduct = pairs.GetValueOrDefault("mainproduct_filter");
            if (!string.IsNullOrEmpty(filterByMainProduct))
                filterArr.Add(("basic_software", filterByMainProduct));

            var filterByStage = pairs.GetValueOrDefault("stage_filter");
            if (!string.IsNullOrEmpty(filterByStage))
                filterArr.Add(("Classification_Stage", filterByStage));

            var filterByContent = pairs.GetValueOrDefault("content_filter");
            if (!string.IsNullOrEmpty(filterByContent))
                filterArr.Add(("Classification_Content", filterByContent));

            var filterByCIM = DefineTagFilter(pairs, "workwithCIM_filter");
            if (filterByCIM)
                filterArr.Add(("Compatibility_CIM", true));

            var filterByNotim = DefineTagFilter(pairs, "isNotimMember_filter");
            if (filterByNotim)
                filterArr.Add(("IsHasNotim", true));

            var filterByRegReestr = DefineTagFilter(pairs, "regReestr_filter");
            if (filterByRegReestr)
                filterArr.Add(("IsHasReestr", true));

            return filterArr.ToArray();
        }
    }
}