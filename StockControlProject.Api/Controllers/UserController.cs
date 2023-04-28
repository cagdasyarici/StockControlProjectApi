using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockControlProject.Entities.Entities;
using StockControlProject.Service.Abstract;

namespace StockControlProject.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGenericService<User> _service;

        public UserController(IGenericService<User> service)
        {
            _service = service;
        }

        [HttpGet]
        public IActionResult TumKullanicilariGetir() 
        {
            return Ok(_service.GetAll());
        }

        [HttpGet]
        public IActionResult AktifKullanicilariGetir()
        {
            return Ok(_service.GetActive());
        }

        [HttpGet("{id}")]
        public IActionResult IdyeGoreKullaniciGetir(int id)
        {
            return Ok(_service.GetById(id));
        }

        [HttpPost]
        public IActionResult KullaniciEkle(User user)
        {
            _service.Add(user);
            return CreatedAtAction("IdyeGoreKullaniciGetir", new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public IActionResult KullaniciGuncelle(int id, User user)
        {
            if (id != user.Id)
                return BadRequest();
            try
            {
                _service.Update(user);
                return Ok(user);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExist(id))
                    return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult KullaniciSil(int id)
        {
            var user= _service.GetById(id);
            if (user == null) return NotFound();
            try
            {
                _service.Remove(user);
                return Ok("Kullanıcı silindi");
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult KullaniciAktiflestir(int id)
        {
            var user = _service.GetById(id);
            if (user == null)
                return NotFound();
            try
            {
                _service.Activate(id);
                return Ok(user);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        private bool UserExist(int id)
        {
            return _service.Any(x => x.Id == id);
        }
    }
}
