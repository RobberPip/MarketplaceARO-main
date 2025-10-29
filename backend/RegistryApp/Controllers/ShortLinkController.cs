using RegistryApp.Services;
using RegistryApp.Utils.CardShortLinks;
using Microsoft.AspNetCore.Mvc;

namespace RegistryApp.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]

    public class ShortLinkController : Controller {
        private readonly ShortLinkService _shortLinkService;

        public ShortLinkController(ShortLinkService shortLinkService) {
            _shortLinkService = shortLinkService;
        }

        [Route("cl/{data?}")]
        public IActionResult DefineCardShortLink(string? data) {
            var cardData = CardShortLinks.GetCardData(data);
            var softwareId = cardData.Value.SoftwareId;
            var vendorId = cardData.Value.DeveloperId;
            if (softwareId != null && softwareId != 0 && (vendorId == null || vendorId == 0)) {
                TempData["software_id"] = softwareId;
                return RedirectToAction("card", "softwares", new { decoded = data });
            }
            if (vendorId != null && vendorId != 0 && (softwareId == null || softwareId == 0)) {
                TempData["vendor_id"] = vendorId;
                return RedirectToAction("card", "vendor", new { decoded = data });
            }
            return View("NotFoundErrorView");
        }

        [Route("fl/{data?}")]
        public async Task<IActionResult> DefineFiltersShortLink(string? data) {
            if (data != null) {
                var responce = await _shortLinkService.GetFiltersStageAsync(data);
                if (responce.IsOkay) {
                    return View("FiltersShortLinkForm", responce.Data);
                }
                return View("NotFoundLink");
            }
            return View("NotFoundLink");
        }


        public async Task<IActionResult> GetFiltersShortLink() {
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";
            var requestParamsData = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());

            var responce = await _shortLinkService.GetFiltersShortLinkAsync(requestParamsData, host);
            if (responce.IsOkay)
                return Ok(responce.Data);
            else
                return StatusCode(500);
        }
    }
}
