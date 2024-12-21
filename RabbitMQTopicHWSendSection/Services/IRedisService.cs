using RabbitMQTopicHWSendSection.Models;

namespace RabbitMQTopicHWSendSection.Services
{
    public interface IRedisService
    {
        void SaveRabbitMQDivisionList(string name, string parentName);
        List<RabbitMQDivision> GetRabbitMQDivisionList();
    }
}
