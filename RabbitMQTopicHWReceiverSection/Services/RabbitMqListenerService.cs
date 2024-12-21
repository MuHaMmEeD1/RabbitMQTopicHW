using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using RabbitMQTopicHWReceiverSection.Models;
using System.Text;
using RabbitMQTopicHWReceiverSection.StaticClasses;

public class RabbitMqListenerService : BackgroundService
{
    private string _routingKey = "";
    private readonly List<RabbitMessageModel> _messages = new List<RabbitMessageModel>();

    public void SetRoutingKey(string routingKey)
    {
        _routingKey = routingKey;
    }

    public List<RabbitMessageModel> GetMessages()
    {
        return _messages.ToList();
    }

    public async Task StartListening()
    {
        await ExecuteAsync(CancellationToken.None);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri("amqps://gbpuukwl:AQ7-D2W_b8-chIj2ObgijZqTY7ctECPd@ostrich.lmq.cloudamqp.com/gbpuukwl")
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        string exchangeName = "main_folder";
        await channel.ExchangeDeclareAsync(exchange: exchangeName, type: ExchangeType.Direct);

        string queueName = (await channel.QueueDeclareAsync()).QueueName;
        await channel.QueueBindAsync(queue: queueName, exchange: exchangeName, routingKey: _routingKey+".#");

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            _messages.Add(new RabbitMessageModel
            {
                Message = message,
                ReceivedAt = DateTime.Now
            });



            Console.WriteLine($"msg->: {message}");

            MessageStatic.Messages.Add(message);

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
