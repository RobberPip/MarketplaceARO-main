using System.Security.Claims;
using RegistryApp.Services;
using RegistryApp.Utils.Helpers;
using RegistryApp.Models.SoftwareFilters;
using Htmx;
using Microsoft.AspNetCore.Mvc;
using RegistryApp.Models;

namespace ARO.RegistrySoftware.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]

    public class SoftwaresController : Controller {
        private readonly ISoftwareService _softwareService;
        private readonly IVendorService _vendorService;

        private const int _softwareViewSize = 30; // software items limit for loading to view  

        public SoftwaresController(ISoftwareService softwareService, IVendorService vendorService) {
            _softwareService = softwareService;
            _vendorService = vendorService;
        }

        public async Task<IActionResult> Index() {
            ViewBag.IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Request.Method == "POST" && Request.Form != null) {
                var requestParamsData = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
                var response = await _softwareService.GetAndProcessingAsync(_softwareViewSize, requestParamsData,userId);
                if (Request.IsHtmx()) {
                    if (response.IsOkay) {
                        if (await IntializeViewDataDictionary(requestParamsData)) {
                            IntializeSeoMetaData();
                            return PartialView("_GetAllSoftwares", response?.Data?.ToArray());
                        } else {
                            return PartialView("_ErrorSoftwares");
                        }
                    } else {
                        return PartialView("_ErrorSoftwares");
                    }
                }

                if (response.IsOkay) {
                    IntializeSeoMetaData();
                    if (await IntializeViewDataDictionary(requestParamsData))
                        return View("SoftwaresIndex", response?.Data?.ToArray());

                    return View("ErrorView");
                }
                return View("ErrorView");
            } else {
                var responseSoftwaresColl = await _softwareService.GetAndProcessingAsync(_softwareViewSize, userId:userId);
                if (responseSoftwaresColl.IsOkay) {
                    if (await IntializeViewDataDictionary()) {
                        IntializeSeoMetaData();
                        return View("SoftwaresIndex", responseSoftwaresColl?.Data?.ToArray());

                    }
                    return View("ErrorView");
                }
                return View("ErrorView");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Card() {
            ViewBag.IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

            var requestParamsData = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
            var requestElId = AttributesParser.TryDefineInteger(requestParamsData, "el_id");
            var responce = await _softwareService.GetByIdAsync(requestElId, host,userId:userId);
            if (responce.IsOkay) {
                IntializeSeoMetaData(responce?.Data?.Name ?? "Карточка програмного продукта",
                    responce?.Data?.Description ?? "Описание программного продукта");
                return View("SoftwareCard", responce?.Data);
            }
            return View("ErrorView");
        }
        
        [HttpGet]
        [Route("softwares/card/{decoded?}")]
        public async Task<IActionResult> Card(string? decoded) {
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
            ViewBag.IsAuthenticated = HttpContext.User.Identity?.IsAuthenticated ?? false;
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var softwareId = (int)TempData["software_id"];
            var responce = await _softwareService.GetByIdAsync(softwareId, host,userId:userId);
            if (responce.IsOkay) {
                IntializeSeoMetaData(responce?.Data?.Name ?? "Карточка програмного продукта",
                    responce?.Data?.Description ?? "Описание программного продукта");
                return View("SoftwareCard", responce?.Data);
            }
            return View("ErrorView");
        }


        private async Task<bool> IntializeViewDataDictionary(Dictionary<string, string>? pairs = null) {
            ViewData["OobUpdate"] = DefineOobUpdater(pairs);
            ViewData["SortType"] = DefineCheckerForViewData(pairs, "sort_type");
            ViewData["PageRequest"] = SoftwareService.DefinePageRequest(pairs, "current_page");
            ViewData["HasFilters"] = false;
            var filterChipsColl = new List<FilterChipVM>();

            #region Classification by Vendor
            var vendors = await _vendorService.GetAllVendorsNameAsync();
            if (!vendors.IsOkay)
                return false;

            ViewData["VendorNames"] = vendors.Data;
            var checkedVendors = GetCheckedFiltersForViewData(pairs, "company_filter");
            if (checkedVendors.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["VendorNames_Selected"] = checkedVendors;
            filterChipsColl.AddRange(checkedVendors.Select(v => new FilterChipVM("company_filter", v)));
            #endregion
            #region Classification by Activity
            var activityClassification = await _softwareService.GetPossibleAttributesValueAsync("Classification_Activity");
            if (!activityClassification.IsOkay)
                return false;

            ViewData["ClassificationActivity"] = activityClassification.Data;
            var checkedActivityClassification = GetCheckedFiltersForViewData(pairs, "activity_filter");
            if (checkedActivityClassification.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["ClassificationActivity_Selected"] = checkedActivityClassification;
            filterChipsColl.AddRange(checkedActivityClassification.Select(v => new FilterChipVM("activity_filter", v)));
            #endregion
            #region Classification by software type
            var softwareTypeClassification = await _softwareService.GetPossibleAttributesValueAsync("Classification_Type");
            if (!softwareTypeClassification.IsOkay)
                return false;

            ViewData["ClassificationSoftwareType"] = softwareTypeClassification.Data;
            var checkedSoftwareTypesClassification = GetCheckedFiltersForViewData(pairs, "softwareType_filter");
            if (checkedSoftwareTypesClassification.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["ClassificationSoftwareType_Selected"] = checkedSoftwareTypesClassification;
            filterChipsColl.AddRange(checkedSoftwareTypesClassification.Select(v => new FilterChipVM("softwareType_filter", v)));
            #endregion
            #region Classification by MainProduct
            var mainProductClassification = await _softwareService.GetAllSoftwaresAttributesValueAsync("basic_software");
            if (!mainProductClassification.IsOkay)
                return false;

            ViewData["MainProduct"] = mainProductClassification.Data;
            var checkedMainProductClassification = GetCheckedFiltersForViewData(pairs, "mainproduct_filter");
            if (checkedMainProductClassification.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["MainProduct_Selected"] = checkedMainProductClassification;
            filterChipsColl.AddRange(checkedMainProductClassification.Select(v => new FilterChipVM("mainproduct_filter", v)));
            #endregion

            #region Classification by Stage
            var stageClassification = await _softwareService.GetPossibleAttributesValueAsync("Classification_Stage");
            if (!stageClassification.IsOkay)
                return false;

            ViewData["ClassificationLifeStage"] = stageClassification.Data;
            var checkedStageClassification = GetCheckedFiltersForViewData(pairs, "stage_filter");
            if (checkedStageClassification.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["ClassificationLifeStage_Selected"] = checkedStageClassification;
            filterChipsColl.AddRange(checkedStageClassification.Select(v => new FilterChipVM("stage_filter", v)));
            #endregion
            #region Classification by Content
            var contentClassification = await _softwareService.GetPossibleAttributesValueAsync("Classification_Content");
            if (!contentClassification.IsOkay)
                return false;

            ViewData["ClassificationContent"] = contentClassification.Data;
            var checkedContentClassification = GetCheckedFiltersForViewData(pairs, "content_filter");
            if (checkedContentClassification.Count() != 0)
                ViewData["HasFilters"] = true;

            ViewData["ClassificationContent_Selected"] = checkedContentClassification;
            filterChipsColl.AddRange(checkedContentClassification.Select(v => new FilterChipVM("content_filter", v)));
            #endregion

            #region Classification for single checkboxes
            int workWithCimFilter = DefineCheckerForViewData(pairs, "workwithCIM_filter");
            if (workWithCimFilter != 0) {
                ViewData["HasFilters"] = true;
                filterChipsColl.Add(new FilterChipVM("workwithCIM_filter", "Работает с ЦИМ", true));
            }
            ViewData["WorkWitnCIM_Checked"] = workWithCimFilter;

            int regReestrFilter = DefineCheckerForViewData(pairs, "regReestr_filter");
            if (regReestrFilter != 0) {
                ViewData["HasFilters"] = true;
                filterChipsColl.Add(new FilterChipVM("regReestr_filter", "В реестре российского ПО", true));
            }
            ViewData["RegReestr_Checked"] = regReestrFilter;

            int isNotimMemberFilter = DefineCheckerForViewData(pairs, "isNotimMember_filter");
            if (isNotimMemberFilter != 0) {
                ViewData["HasFilters"] = true;
                filterChipsColl.Add(new FilterChipVM("isNotimMember_filter", "Вендор состоит в НОТИМ", true));
            }
            ViewData["IsNotimMember_Checked"] = isNotimMemberFilter;
            #endregion

            ViewData["SearchRequest"] = GetSearchRequestForViewData(pairs, "search_request");

            var softwaresCount = await _softwareService.GetSoftwaresCountAsync(pairs);
            if (!softwaresCount.IsOkay)
                return false;
            ViewData["SoftwaresCount"] = softwaresCount.Data;

            ViewData["FilterChips"] = filterChipsColl;
            return true;
        }

        private void IntializeSeoMetaData(string? title = null, string? description = null) {
            ViewBag.MetaTitle = title ?? "Перечень цифровых продуктов для строительства";
            ViewBag.MetaDescription = description ?? "Перечень более девятиста отечественных программных продуктов, " +
                "средств автоматизации и цифровизации строительной сферы, чтобы каждый участник процесса смог определить потребность в том или ином ПО";
        }

        private string GetSearchRequestForViewData(Dictionary<string, string>? pairs, string key) {
            if (pairs != null && pairs.ContainsKey(key)) {
                var checkedData = pairs[key];
                if (!string.IsNullOrEmpty(checkedData))
                    return checkedData;
            }
            return string.Empty;
        }

        private List<string> GetCheckedFiltersForViewData(Dictionary<string, string>? pairs, string key) {
            if (pairs != null && pairs.ContainsKey(key)) {
                var checkedData = pairs[key];
                if (!string.IsNullOrEmpty(checkedData))
                    return checkedData.Split(';').ToList();
            }
            return new List<string>();
        }

        private int DefineCheckerForViewData(Dictionary<string, string>? pairs, string key) {
            int checkPos;
            if (pairs != null && pairs.ContainsKey(key)) {
                if (int.TryParse(pairs[key], out checkPos)) {
                    if (checkPos == 1)
                        return 1;
                }
            }
            return 0;
        }

        private bool DefineOobUpdater(Dictionary<string, string>? pairs) {
            int checkPos;
            if (pairs != null && pairs.ContainsKey("oob_update")) {
                if (int.TryParse(pairs["oob_update"], out checkPos)) {
                    if (checkPos == 1)
                        return true;
                }
            }
            return false;
        }
    }
}