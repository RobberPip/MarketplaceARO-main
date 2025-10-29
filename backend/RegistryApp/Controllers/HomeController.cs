using RegistryApp.Domain.Enums;
using RegistryApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace ARO.RegistrySoftware.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]

    public class HomeController : Controller {
        private readonly ISoftwareService _softwareService;

        public HomeController(ISoftwareService softwareService) {
            _softwareService = softwareService;
        }

        public async Task<IActionResult> Index() {
            var responseSoftwaresColl = await _softwareService.GetBySortAsync(SortBy.Date, 2);
            var responseSofwaresCount = await _softwareService.GetSoftwaresCountAsync();
            if (responseSoftwaresColl.IsOkay && responseSofwaresCount.IsOkay) {
                IntializeSeoMetaData();
                ViewData["SoftwaresRoundCount"] = responseSofwaresCount.Data / 10 * 10;
                return View(responseSoftwaresColl.Data);
            }
            return View("ErrorView");
        }

        private void IntializeSeoMetaData() {
            ViewBag.MetaTitle = "Перечень цифровых продуктов для строительства";
            ViewBag.MetaDescription = "Информационный ресурс, объединяющий более чем девятисот отечественных программных продуктов, " +
                "средств автоматизации и цифровизации строительной сферы, чтобы каждый участник процесса смог определить потребность в том или ином ПО";
        }
    }
}