using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MorbositesBotApi.Services;
using System.IO;
using System.Threading.Tasks;

namespace MorbositesBotApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MorbositesController : ControllerBase
    {
        private readonly IMorbositeService _morbositeService;
        private readonly IWebHostEnvironment _environment;

        public MorbositesController(IMorbositeService morbositeService, IWebHostEnvironment environment)
        {
            _morbositeService = morbositeService;
            _environment = environment;
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetAllMorbosites(long? chatId)
        {
            var morbosites = await _morbositeService.GetMorbositesAsync(chatId);
            return Ok(morbosites);
        }

        [HttpGet("Download")]
        public IActionResult DownloadDatabase()
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "Morbosites.db");
            var fileBytes = System.IO.File.ReadAllBytes(path);
            return File(fileBytes, "application/x-msdownload", "Morbosites.db");
        }

        [HttpPost("Upload")]
        public IActionResult UploadDatabase(IFormFile database)
        {
            using (var stream = new FileStream(Path.Combine(Directory.GetCurrentDirectory(), database.FileName), FileMode.Create))
            {
                database.CopyTo(stream);
            }

            return NoContent();
        }
    }
}
