using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Outbox_Pattern.DTOs;
using Outbox_Pattern.Services.Interface;

namespace Outbox_Pattern.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService) => _orderService = orderService;


        [HttpPost("create-orders")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var result = await _orderService.CreateOrderAsync(request);

            if (!result.IsSuccess)
            {
                return StatusCode(result.Message!.StatusCode, result.Message);
            }

            return Ok(result.Message);
        }

        [HttpGet("get-orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();

            return Ok(orders);
        }
    }
}
