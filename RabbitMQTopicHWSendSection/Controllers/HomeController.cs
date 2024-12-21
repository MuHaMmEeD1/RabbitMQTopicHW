using Microsoft.AspNetCore.Mvc;
using RabbitMQTopicHWSendSection.Models;
using RabbitMQTopicHWSendSection.Services;

namespace RabbitMQTopicHWSendSection.Controllers
{
    public class HomeController : Controller
    {
        private List<RabbitMQDivision> divisions;

        private readonly IRedisService _redisService;
        private readonly IRabbitMQService _rabbitMQService;
        public HomeController(IRedisService redisService, IRabbitMQService rabbitMQService)
        {
            _redisService = redisService;
            _rabbitMQService = rabbitMQService;
        }

        public IActionResult Index()
        {
            divisions = _redisService.GetRabbitMQDivisionList();
            return View(divisions);
        }

        [HttpPost]
        public IActionResult AddRabbitMQDivision(string name, string parentName)
        {
            if(name != null)
            {
            _redisService.SaveRabbitMQDivisionList(name , parentName);

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SendMessageRabbitMQ([FromBody] SendMessageRequestModel model)
        {
            
            var _ = await _rabbitMQService.SendMessageTopic(model.DivisionName, model.Message);
            return Ok(new { message = "Message sent successfully!" });

        }

    }
}
