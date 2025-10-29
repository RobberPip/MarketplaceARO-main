using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Admin {
    [Route("config")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class AppConfigurationController : ControllerBase {
        private readonly IConfiguration _configuration;

        public AppConfigurationController(IConfiguration configuration) {
            _configuration = configuration;
        }


        [HttpGet("email-flow")]

        public ActionResult GetEmailFlow() {
            var data = _configuration.GetSection("EmailFlow").Value;
            return Ok(data);
        }

        [HttpPost("email-flow")]
        public ActionResult SetEmailFlow([FromBody] bool flow) {
            _configuration["EmailFlow"] = flow.ToString();
            return Ok();
        }
    }
}
