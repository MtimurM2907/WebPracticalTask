using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using WebPracticalTask.ProgramLogic;

namespace WebPracticalTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StringHandlerController : ControllerBase
    {
        private readonly RequestLimiterService _limiterService;

        public StringHandlerController(RequestLimiterService limiterService)
        {
            _limiterService = limiterService;
        }


        [HttpGet]
        public async Task<IActionResult> GetString(string text, string sort, [FromServices] IOptions<BlacklistSettings> blacklistOptions) 
        {
            var blacklist = blacklistOptions.Value.BlackList;
            if (blacklist != null && blacklist.Any(bannedWord => text.Contains(bannedWord, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Текст содержит запрещенные слова");
            }

            var result = await Logics.StartLogic(text, sort);

            Logics.text = "\0";
            Logics.finalText = "\0";
            Logics.sortSelection = "\0";
            Logics.finalMessage = "\0";
            Sorting.finalTextIndex.Clear();
            if (result.Contains("Произошла ошибка:"))
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

        [HttpGet("status")]
        public IActionResult GetServiceStatus()
        {
            var status = _limiterService.GetStatus();
            return Ok($"Current: {status.current}, Limit: {status.limit}");
        }
    }
}
