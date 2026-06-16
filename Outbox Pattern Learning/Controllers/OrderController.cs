using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Outbox_Pattern_Learning.DTOs;
using Outbox_Pattern_Learning.Services.Interfaces;

namespace Outbox_Pattern_Learning.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);
            return Ok(result);
        }
    }
}
