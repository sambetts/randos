using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SecurityInfoController : ControllerBase
    {

        private readonly ILogger<SecurityInfoController> _logger;

        public SecurityInfoController(ILogger<SecurityInfoController> logger)
        {
            _logger = logger;
        }

        // GET: /SecurityInfo
        [HttpGet]
        public ActionResult Get()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            _logger.LogInformation("claims: {claims}", claims);

            return new JsonResult(claims);
        }
    }
}
