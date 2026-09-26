using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Eventing.Reader;
using System.Security.Cryptography.X509Certificates;
using WebAPIDemo.Attributes;
using WebAPIDemo.Data;
using WebAPIDemo.Filters;
using WebAPIDemo.Filters.ActionFilters.V2;
using WebAPIDemo.Filters.AuthFilters;
using WebAPIDemo.Models;
using WebAPIDemo.Models.Repositories;

namespace WebAPIDemo.Controllers.V2
{
    [ApiController]
    [ApiVersion("2.0")]
    [ApiExplorerSettings(GroupName = "v2")]
    [Route("api/[controller]")]
    [JwtAuthFilter]
    public class ShirtsController: ControllerBase
    {
        ApplicationDbContext db;
        public ShirtsController(ApplicationDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirts()
        {
            return Ok(db.Shirts.ToList());
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("read", "true")]
        public IActionResult GetShirtById(int id)
        {
                       
            return Ok(HttpContext.Items["shirt"]);
        }

        [HttpPost]
        [TypeFilter(typeof(Shirt_ValidateShirtCreateFilterAttribute))]
        [Shirt_EnsureDescriptionIsPresentFilter]
        [RequiredClaim("write", "true")]    
        public IActionResult CreateShirt([FromBody]Shirt shirt)
        {
            //if (shirt == null) return BadRequest();

            db.Shirts.Add(shirt);
            db.SaveChanges();

            return CreatedAtAction(nameof(GetShirtById), new { id = shirt.ShirtId },
                                    shirt);
        }

        [HttpPut("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [Shirt_ValidateUpdateShirtFilter]
        [TypeFilter(typeof(Shirt_HandleUpdateExceptionsFilterAttribute))]
        [Shirt_EnsureDescriptionIsPresentFilter]
        [RequiredClaim("write", "true")]
        public IActionResult UpdateShirt(int id,Shirt shirt)
        {
            var shirt_update = HttpContext.Items["shirt"] as Shirt;
            shirt_update?.Brand = shirt.Brand;
            shirt_update?.Price = shirt.Price;
            shirt_update?.Size = shirt.Size;
            shirt_update?.Color = shirt.Color;
            shirt_update?.Gender = shirt.Gender;

            db.SaveChanges();

            return NoContent();

         }

        [HttpDelete("{id}")]
        [TypeFilter(typeof(Shirt_ValidateShirtIdFilterAttribute))]
        [RequiredClaim("delete", "true")]
        public IActionResult DeleteShirt(int id)
        {
            var shirt = HttpContext.Items["shirt"] as Shirt;
            db.Shirts.Remove(shirt);
            db.SaveChanges();

            return Ok(shirt);
        }
    }
}
