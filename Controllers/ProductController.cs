using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NG_Core_Auth.Data;
using NG_Core_Auth.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NG_Core_Auth.Controllers
{
    [Route("api/[controller]")]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _db;

        public ProductController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: api/<controller>
        [HttpGet("[action]")]
        [Authorize(Policy = "RequireLoggedIn")]
        public IActionResult GetProducts()
        {
            return Ok(_db.Products.ToList());
        }

        [HttpPost("[action]")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> AddProduct([FromBody] ProductModel formdata)
        {
            var newProduct = new ProductModel
            {
                Name = formdata.Name,
                ImageUrl = formdata.ImageUrl,
                Description = formdata.Description,
                Price = formdata.Price,
                OutOfStock = formdata.OutOfStock
            };
            await _db.Products.AddAsync(newProduct);
            await _db.SaveChangesAsync();
            return Ok(new JsonResult("The Product was added successfully"));
        }

        [HttpPut("[action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id,[FromBody] ProductModel formdata)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var findProduct = _db.Products.FirstOrDefault(p => p.Id == id);
            if (findProduct==null)
            {
                return NotFound();
            }
            findProduct.Name = formdata.Name;
            findProduct.OutOfStock = formdata.OutOfStock;
            findProduct.Price = formdata.Price;
            findProduct.Description = formdata.Description;
            findProduct.ImageUrl = formdata.ImageUrl;
            _db.Entry(findProduct).State = EntityState.Modified;
            await _db.SaveChangesAsync();

            return Ok(new JsonResult("The Product with id  "+id+ "is updated"));
        }

        [HttpDelete("[action]/{id}")]
        [Authorize(Policy = "RequireAdministratorRole")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var findProduct = await _db.Products.FindAsync(id);

            if (findProduct==null)
            {
                return NotFound(findProduct);
            }
            _db.Products.Remove(findProduct);
            await _db.SaveChangesAsync();
            return Ok("The Product with id  " + id + " is deleted");
        }
    }
}
