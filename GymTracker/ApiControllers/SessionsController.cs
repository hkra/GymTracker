using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using GymTracker.Services;
using System;
using Microsoft.Extensions.Logging;

namespace GymTracker.Controllers
{
    public class SessionsController : Controller
    {
        private readonly ISessionsSerivce _sessions;
        private readonly ILogger<SessionsController> _logger;
        public SessionsController(ISessionsSerivce sessions, ILogger<SessionsController> logger)
        {
            _sessions = sessions;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post()
        {
            try
            {
                await _sessions.RecordSession(DateTimeOffset.UtcNow);
                return StatusCode(201);
            }
            catch (Exception e)
            {
                _logger.LogError(0, e, "Uncaught exception during session creation");
                return StatusCode(500);
            }
        }
    }
}
