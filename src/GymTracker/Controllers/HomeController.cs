using System;
using System.Threading.Tasks;
using GymTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GymTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionsSerivce _sessions;
        private readonly ILogger<HomeController> _logger;
        
        public HomeController(ISessionsSerivce sessions, ILogger<HomeController> logger)
        {
            _sessions = sessions;
            _logger = logger;
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("")]
        public async Task<IActionResult> Post()
        {
            try
            {
                await _sessions.RecordSession(DateTimeOffset.UtcNow);
                TempData["Successful"] = true;
                return RedirectToAction("Index");
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
