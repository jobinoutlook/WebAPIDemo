using Microsoft.AspNetCore.Mvc;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PantsController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetPants()
        {
            return Ok("Reading all the Shirts");
        }

        [HttpGet("{id}")]
        public IActionResult GetPantById(int id)
        {
            return Ok($"Return reading shirt: {id}");
        }
    }
}
