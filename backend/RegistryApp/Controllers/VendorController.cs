using RegistryApp.Services;
using RegistryApp.Utils.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ARO.RegistrySoftware.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]

    public class VendorController : Controller {
        private readonly IVendorService _vendorService;

        public VendorController(IVendorService vendorService) {
            _vendorService = vendorService;
        }

        [HttpPost]
        public async Task<IActionResult> Card() {
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

            var requestParamsData = Request.Form.ToDictionary(k => k.Key, v => v.Value.ToString());
            var requestElId = AttributesParser.TryDefineInteger(requestParamsData, "el_id");

            var responce = await _vendorService.GetByIdAsync(requestElId, host);
            if (responce.IsOkay) {
                IntializeSeoMetaData(responce?.Data?.Name ?? "Карточка вендора", responce?.Data?.Description ?? "Описание вендора");
                return View("VendorCard", responce?.Data);
            }
            return View("ErrorView");
        }

        [HttpGet]
        [Route("vendor/card/{decoded?}")]
        public async Task<IActionResult> Card(string? decoded) {
            //var info = Request.Headers;
            var host = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

            var vendorId = (int)TempData["vendor_id"];
            var responce = await _vendorService.GetByIdAsync(vendorId, host);
            if (responce.IsOkay) {
                IntializeSeoMetaData(responce?.Data?.Name ?? "Карточка вендора",
                    responce?.Data?.Description ?? "Описание вендора");
                return View("VendorCard", responce?.Data);
            }
            return View("ErrorView");
        }

        private void IntializeSeoMetaData(string title, string description) {
            ViewBag.MetaTitle = title;
            ViewBag.MetaDescription = description;
        }
    }
}