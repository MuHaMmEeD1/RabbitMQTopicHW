using Microsoft.AspNetCore.Mvc;
using RabbitMQTopicHWReceiverSection.Models;
using RabbitMQTopicHWReceiverSection.Services;
using RabbitMQTopicHWReceiverSection.StaticClasses;

namespace RabbitMQTopicHWReceiverSection.Controllers
{
    public class HomeController : Controller
    {
        private readonly RabbitMqListenerService _rabbitMqListenerService;
        private readonly IRedisService _redisService;


        public HomeController(RabbitMqListenerService rabbitMqListenerService, IRedisService redisService)
        {
            _rabbitMqListenerService = rabbitMqListenerService;
            _redisService = redisService;
        }

        public IActionResult Index()
        {
            string routingKey = TempData["routingKey"] as string;

          

            Console.WriteLine(routingKey);

            Thread.Sleep(500);

            HomeIndexModel model = new HomeIndexModel()
            {
                RabbitMQDivisions = _redisService.GetRabbitMQDivisionList(),
                RoutingKey = routingKey == null ? "" : routingKey,
                Messages = _rabbitMqListenerService.GetMessages()
            };

           

            return View(model);
        }



        [HttpPost]
        public IActionResult StartListener([FromBody] IndexPostRKeyModel model)
        {
            _rabbitMqListenerService.SetRoutingKey(model.RoutingKey);


            if (model.RoutingKey != "") {
              
                Task.Run(() => { 
                    _rabbitMqListenerService.StartListening();
                     });
            }

            TempData["routingKey"] = model.RoutingKey;

          

            return RedirectToAction("Index");
        }
    }
}
