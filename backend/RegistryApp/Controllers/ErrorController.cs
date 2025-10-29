using Microsoft.AspNetCore.Mvc;

namespace RegistryApp.Controllers {
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ErrorController : Controller {
        public new IActionResult NotFound() {
            Response.StatusCode = 404;
            return View();
        }
    }
}
