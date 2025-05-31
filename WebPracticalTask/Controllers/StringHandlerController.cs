using Microsoft.AspNetCore.Mvc;
using WebPracticalTask.ProgramLogic;

namespace WebPracticalTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StringHandlerController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetString(string text, string sort) 
        {
            var result = await Logics.StartLogic(text, sort);

            Logics.text = "\0";
            Logics.finalText = "\0";
            Logics.sortSelection = "\0";
            Logics.finalMessage = "\0";
            Sorting.finalTextIndex.Clear();
            if (result.ToString().Contains("Произошла ошибка:"))
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
    }
}
