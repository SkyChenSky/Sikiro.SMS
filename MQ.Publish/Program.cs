using System;
using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace MQ.Publish
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                HostName = "10.1.20.140",
                UserName = "admin",
                Password = "admin@ucsmy"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var queueName = "Queue.SMS.Test";
                var exchangeName = "Exchange.SMS.Test";
                var key = "Route.SMS.Test";

                DeclareDelayQueue(channel, exchangeName, queueName, key);

                DeclareReallyConsumeQueue(channel, exchangeName, queueName, key);

                var body = Encoding.UTF8.GetBytes("info: test dely publish!");
                channel.BasicPublish(exchangeName + ".Delay", key, null, body);
            }
        }

        private static void DeclareDelayQueue(IModel channel, string exchangeName, string queueName, string key)
        {
            var retryDic = new Dictionary<string, object>
            {
                {"x-dead-letter-exchange", exchangeName+".dl"},
                {"x-dead-letter-routing-key", key},
                {"x-message-ttl", 30000}
            };

            var ex = exchangeName + ".Delay";
            var qu = queueName + ".Delay";
            channel.ExchangeDeclare(ex, "topic");
            channel.QueueDeclare(qu, false, false, false, retryDic);
            channel.QueueBind(qu, ex, key);
        }

        private static void DeclareReallyConsumeQueue(IModel channel, string exchangeName, string queueName, string key)
        {
            var ex = exchangeName + ".dl";
            channel.ExchangeDeclare(ex, "topic");
            channel.QueueDeclare(queueName, false, false, false);
            channel.QueueBind(queueName, ex, key);
        }
    }
}
