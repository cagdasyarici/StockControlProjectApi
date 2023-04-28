using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StockControlProject.Entities.Entities;
using StockControlProject.Entities.Enums;
using StockControlProject.Service.Abstract;

namespace StockControlProject.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IGenericService<Order> _orderservice;
        private readonly IGenericService<OrderDetails> _odservice;
        private readonly IGenericService<Product> _productservice;

        public OrderController(IGenericService<Order> orderservice, IGenericService<OrderDetails> odservice, IGenericService<Product> productservice)
        {
            _orderservice = orderservice;
            _odservice = odservice;
            _productservice = productservice;
        }
        [HttpGet]
        public IActionResult TumSiparisleriGetir()
        {
            return Ok(_orderservice.GetAll(x=>x.User,x=>x.OrderDetails));
        }
        [HttpGet]
        public IActionResult AktifSiparisleriGetir()
        {
            return Ok(_orderservice.GetActive());
        }
        [HttpGet("{id}")]
        public IActionResult IdyeGoreSiparisDetaylari(int id)
        {
            return Ok(_odservice.GetAll(x=>x.OrderId==id,x=>x.Product));
        }
        [HttpGet("{id}")]
        public IActionResult IdyeGoreSiparisGetir(int id)
        {
            return Ok(_orderservice.GetById(id,x=>x.OrderDetails,x=>x.User));
        }
        [HttpGet]
        public IActionResult BekleyenSiparisleriGetir()
        {
            return Ok(_orderservice.GetDefault(x=>x.Status==Status.Pending));
        }
        [HttpGet]
        public IActionResult OnaylananSiparisleriGetir()
        {
            return Ok(_orderservice.GetDefault(x => x.Status == Status.Confirmed));
        }
        [HttpGet]
        public IActionResult ReddedilenSiparisleriGetir()
        {
            return Ok(_orderservice.GetDefault(x => x.Status == Status.Cancelled));
        }
        [HttpPost]
        public IActionResult SiparisEkle(int userId, [FromQuery] int[] productIDs, [FromQuery] short[] quantities)
        {
            Order yenisiparis = new Order();
            yenisiparis.UserId = userId;
            yenisiparis.Status=Status.Pending;
            yenisiparis.IsActive=true;//sipariş onaylandığında veya reddedildiğinde false'a çekilmeli adekugbe
            _orderservice.Add(yenisiparis);
            for(int i=0; i<productIDs.Length; i++)
            {
                OrderDetails yenisiparisdetay = new OrderDetails();
                yenisiparisdetay.OrderId = yenisiparis.Id;
                yenisiparisdetay.ProductId = productIDs[i];
                yenisiparisdetay.Quantity = quantities[i];
                yenisiparisdetay.UnitPrice = _productservice.GetById(productIDs[i]).UnitPrice;
                yenisiparisdetay.IsActive = true;
                _odservice.Add(yenisiparisdetay);
            }
            return Ok(yenisiparis);         
        }
        [HttpGet("{id}")]
        public IActionResult SiparisOnayla(int id)
        {
            Order confirmedOrder = _orderservice.GetById(id);
            if (confirmedOrder == null)
            {
                return NotFound();
            }
            else
            {
                List<OrderDetails> detaylar = _odservice.GetDefault(x => x.OrderId == confirmedOrder.Id).ToList();
                foreach(OrderDetails item in detaylar)
                {
                    Product productinOrder=_productservice.GetById(item.ProductId);
                    productinOrder.Stock -= item.Quantity;
                    _productservice.Update(productinOrder);
                }
                confirmedOrder.Status = Status.Confirmed;
                confirmedOrder.IsActive = false;
                _orderservice.Update(confirmedOrder);
                return Ok(confirmedOrder);
            }
        }
        [HttpGet("{id}")]
        public IActionResult SiparisReddet(int id)
        {
            Order cancelledOrder=_orderservice.GetById(id);
            if (cancelledOrder == null)
            {
                return NotFound();
            }
            else
            {
                cancelledOrder.Status = Status.Cancelled;
                cancelledOrder.IsActive = false;
                _orderservice.Update(cancelledOrder);
                return Ok(cancelledOrder);
            }
        }

    }
}
