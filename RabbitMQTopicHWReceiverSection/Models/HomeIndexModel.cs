namespace RabbitMQTopicHWReceiverSection.Models
{
    public class HomeIndexModel
    {
        public List<RabbitMQDivision> RabbitMQDivisions { get; set; }
        public string RoutingKey { get; set; }
        public List<RabbitMessageModel> Messages { get; set; } = new List<RabbitMessageModel>();
    }

}
