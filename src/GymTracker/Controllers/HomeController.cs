using System;
using System.Threading.Tasks;
using GymTracker.Models;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace GymTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionsSerivce _sessions;
        private readonly ILogger<HomeController> _logger;
        private readonly string _accessKey;
        
        public HomeController(ISessionsSerivce sessions, ILogger<HomeController> logger, IOptions<Settings> config)
        {
            _sessions = sessions;
            _logger = logger;
            _accessKey = config.Value.AccessKey;
        }

        [HttpGet("")]
        public IActionResult Index(string accessKey)
        {
            if (accessKey != _accessKey)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpPost("")]
        public async Task<IActionResult> Post(long tzOffsetMinutes)
        {
            try
            {
                var offset = TimeSpan.FromMinutes(tzOffsetMinutes * -1);
                var timestamp = DateTimeOffset.UtcNow.ToOffset(offset);

                await _sessions.RecordSession(timestamp);
                TempData["Successful"] = true;
                return RedirectToAction("Index", new { accessKey = _accessKey });
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Uncaught exception during session creation");
                return RedirectToAction("Error");
            }
        }

        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
