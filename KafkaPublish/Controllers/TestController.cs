using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KafkaPublish.Controllers
{
    /// <summary>
    /// api kiểm tra xem có chạy được service không
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// kiểm tra xem có chạy được service không
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Service is running");
        }
    }
}
