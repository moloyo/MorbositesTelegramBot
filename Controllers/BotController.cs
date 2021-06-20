using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MorbositesBotApi.Services;
using System.Threading.Tasks;

namespace MorbositesBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotService _botService;
        private readonly ILogger<BotController> _logger;

        public BotController(IBotService botService, ILogger<BotController> logger)
        {
            _botService = botService;
            _logger = logger;
        }

        [HttpGet("Start")]
        public async Task<IActionResult> StartBot()
        {
            await _botService.StartBotAsync();
            _logger.LogInformation("Bot Started");
            return Ok("Bot Started");
        }

        [HttpGet("Stop")]
        public async Task<IActionResult> StopBot()
        {
            await _botService.StopBotAsync();
            _logger.LogInformation("Bot Stopped");
            return Ok("Bot Stopped");
        }
    }
}
