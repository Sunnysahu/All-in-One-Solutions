using MassTransit;
using MassTransit_Using_RabbitMQ.Contracts.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MassTransit_Using_RabbitMQ.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndPoint;

        public OrderController(IPublishEndpoint publishEndPoint) => _publishEndPoint = publishEndPoint;

        [HttpPost]
        public async Task<IActionResult> CreateOrder()
        {
            var orderEvent = new OrderCreatedEvent
            {
                OrderId = Random.Shared.Next(1, 1000),
                ProductName = "Laptop",
                Quantity = 1,
                Price = 55000
            };

            //await Task.Delay(2000);
            await _publishEndPoint.Publish(orderEvent);

            return Ok(new
            {
                Message = "OrderQueued Successfully"
            });
        }
    }
}
