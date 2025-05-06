using Microsoft.AspNetCore.Mvc;

namespace WebPracticalTask.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class StringHandlerController : ControllerBase
    {
        [HttpGet]
        public void GetString(string text)
        {

        }
    }
}
