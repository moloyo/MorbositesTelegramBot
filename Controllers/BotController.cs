using Microsoft.AspNetCore.Mvc;
using MorbositesBotApi.Services;
using System.Threading.Tasks;

namespace MorbositesBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private readonly IBotService _botService;

        public BotController(IBotService botService)
        {
            _botService = botService;
        }

        [HttpGet("Start")]
        public async Task<IActionResult> StartBot()
        {
            await _botService.StartBotAsync();
            return Ok("Bot Started");
        }

        [HttpGet("Stop")]
        public async Task<IActionResult> StopBot()
        {
            await _botService.StopBotAsync();
            return Ok("Bot Stopped");
        }
    }
}
