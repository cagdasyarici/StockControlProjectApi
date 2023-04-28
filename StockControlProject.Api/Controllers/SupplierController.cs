using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SupplierController : ControllerBase
    {
        private readonly IGenericService<Supplier> _service;
        public SupplierController(IGenericService<Supplier> service)
        {
            _service = service;
        }
        [HttpGet]
        public IActionResult TumSaglayıcılariGetir()
        {
            return Ok(_service.GetAll());
        }
        [HttpGet]
        public IActionResult AktifSaglayıcılariGetir()
        {
            return Ok(_service.GetActive());
        }
        [HttpGet("{id}")]
        public IActionResult IdyeGoreSaglayıcıGetir(int id)
        {
            return Ok(_service.GetById(id));
        }
        [HttpPost]
        public IActionResult SaglayıcıEkle(Supplier supplier)
        {
            _service.Add(supplier);
            return CreatedAtAction("IdyeGoreKategoriGetir", new { id = supplier.Id }, supplier);
        }
        [HttpPut("{id}")]
        public IActionResult SaglayiciGuncelle(int id, Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();
            try
            {
                _service.Update(supplier);
                return Ok(supplier);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SupplierExist(id))
                    return NotFound();
            }
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult SaglayiciSil(int id)
        {
            var supplier = _service.GetById(id);
            if (supplier == null) return NotFound();
            try
            {
                _service.Remove(supplier);
                return Ok("Sağlayıcı silindi");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("{id}")]
        public IActionResult SaglayicilariAktiflestir(int id)
        {
            var supplier = _service.GetById(id);
            if (supplier == null)
                return NotFound();
            try
            {
                _service.Activate(id);
                return Ok(supplier);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        private bool SupplierExist(int id)
        {
            return _service.Any(x => x.Id == id);
        }
    }
}
