using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace RabbitMQTopicHWSendSection.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly ConnectionFactory _factory;

        public RabbitMQService()
        {
            _factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://gbpuukwl:AQ7-D2W_b8-chIj2ObgijZqTY7ctECPd@ostrich.lmq.cloudamqp.com/gbpuukwl")
            };
        }

        public async Task<bool> SendMessageTopic(string routingKey, string message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            string exchangeName = "main_folder";

            try
            {
                await channel.ExchangeDeclareAsync(
                    exchange: exchangeName,
                    type: ExchangeType.Topic,
                    durable: true,
                    autoDelete: false,
                    passive: true
                );
            }
            catch (RabbitMQ.Client.Exceptions.OperationInterruptedException ex)
            {
                if (ex.ShutdownReason.ReplyCode == 404)
                {
                    await channel.ExchangeDeclareAsync(
                        exchange: exchangeName,
                        type: ExchangeType.Topic,
                        durable: true,
                        autoDelete: false
                    );
                }
                else
                {
                    throw;
                }
            }

            var body = System.Text.Encoding.UTF8.GetBytes(message);

            await channel.BasicPublishAsync(
                exchange: exchangeName,
                routingKey: routingKey + ".#",
                mandatory: false,
                body: body
            );

            Console.WriteLine($"Sent Message -> {message}");

            return true;
        }


    }
}
