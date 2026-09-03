using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;
using WebAPIDemo.Data;
using WebAPIDemo.Filters;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShirtsController: ControllerBase
    {
        ApplicationDbContext db;
        public ShirtsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList());
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult GetShirtById(int id)
        {
                       
            return Ok(HttpContext.Items["shirt"]);
        }

        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidateShirtCreateFilterAttribute))]
        public IActionResult CreateShirt([FromBody]Shirt shirt)
        {
            //if (shirt == null) return BadRequest();

            var existingshirt = ShirtRepository.GetShirtByProperties(shirt.Brand, shirt.Gender, shirt.Color, shirt.Size);
            if(existingshirt!=null)return BadRequest();

            ShirtRepository.AddShirt(shirt);

            return CreatedAtAction(nameof(GetShirtById), new { id = shirt.ShirtId },
                                    shirt);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [Shirt_HandleUpdateExceptionsFilter]
        public IActionResult UpdateShirt(int id,Shirt shirt)
        {
            return Ok($"Updating shirt: {id}");
        }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        public IActionResult DeleteShirt(int id)
        {
            return Ok($"Deleting shirt: {id}");
        }
    }
}
