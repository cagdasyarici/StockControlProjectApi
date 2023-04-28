using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IGenericService<Category> _service;
        public CategoryController(IGenericService<Category> service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult TumKategorileriGetir()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet]
        public IActionResult AktifKategorileriGetir()
        {
            return Ok(_service.GetActive());
        }
        [HttpGet("{id}")]
        public IActionResult IdyeGoreKategoriGetir(int id) 
        {
            return Ok(_service.GetById(id));
        }
        [HttpPost]
        public IActionResult KategoriEkle(Category category)
        {
            _service.Add(category);
            return CreatedAtAction("IdyeGoreKategoriGetir", new { id = category.Id }, category);
        }
        [HttpPut("{id}")]
        public IActionResult KategoryGuncelle(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest();
            try
            {
                _service.Update(category);
                return Ok(category);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExist(id))
                    return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult KategoriSil(int id)
        {
            var category = _service.GetById(id);
            if (category == null) return NotFound();
            try
            {
                _service.Remove(category);
                return Ok("Kategori silindi");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public IActionResult KategorileriAktiflestir(int id)
        {
            var category = _service.GetById(id);
            if(category==null)
                return NotFound();
            try
            {
                _service.Activate(id);
                return Ok(category);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        private bool CategoryExist(int id)
        {
            return _service.Any(x => x.Id == id);
        }
    }
}
