namespace RabbitMQTopicHWSendSection.Services
{
    public interface IRabbitMQService
    {
        Task<bool> SendMessageTopic(string routingKey, string message);
    }
}
