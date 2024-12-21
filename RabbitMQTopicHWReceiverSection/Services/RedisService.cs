using RabbitMQTopicHWReceiverSection.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace RabbitMQTopicHWReceiverSection.Services
{
    public class RedisService : IRedisService
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly string nameDivision = "divisions";

        public RedisService()
        {
            _redis = ConnectionMultiplexer.Connect(
                new ConfigurationOptions
                {
                    EndPoints = { { "redis-16071.c114.us-east-1-4.ec2.redns.redis-cloud.com", 16071 } },
                    User = "default",
                    Password = "YJQ91T7S80iPSsnU5w6vD1Q7OBwrdK1x",

                }
            );

            _database = _redis.GetDatabase();
        }

        public void SaveRabbitMQDivisionList(string name, string parentName)
        {
            var json = _database.StringGet(nameDivision);

            var list = !string.IsNullOrEmpty(json)
                ? JsonSerializer.Deserialize<List<RabbitMQDivision>>(json)
                : new();

            var item = FindDivision(list, parentName);

            if (item == null)
            {
                list.Add(new RabbitMQDivision() { Name = name, RabbitMQDivisions = new() });
            }
            else
            {
                item.RabbitMQDivisions.Add(new RabbitMQDivision { Name = name, RabbitMQDivisions = new() });
            }

            var jsonData = JsonSerializer.Serialize(list);
            _database.StringSet(nameDivision, jsonData);
        }

        public List<RabbitMQDivision> GetRabbitMQDivisionList()
        {
            var jsonData = _database.StringGet(nameDivision);

            if (!jsonData.IsNullOrEmpty)
            {
                return JsonSerializer.Deserialize<List<RabbitMQDivision>>(jsonData);
            }
            return new List<RabbitMQDivision>();
        }

        private RabbitMQDivision FindDivision(List<RabbitMQDivision> list, string parentName)
        {
            if (string.IsNullOrEmpty(parentName))
                return null;

            var arr = parentName.Split('.', 2);

            if (list.Find(d => d.Name == arr[0]) is RabbitMQDivision division)
            {
                if (arr.Length == 1)
                    return division;

                return FindDivision(division.RabbitMQDivisions, arr[1]);
            }

            return null;
        }

      
    }
}
