using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace LoanQuotesMOMClient
{
    class Program
    {
        private List<JObject> _loanProposals;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            User user = new User();
            _loanProposals = new List<JObject>();

            Console.WriteLine("What is the amount of loan you need?");
            int requestedMoney = int.Parse(Console.ReadLine());

            JObject loan = new JObject();
            loan.Add("amount", requestedMoney);
            loan.Add("clientId", user.Id.ToString());

            CreateRequestQueue(loan.ToString());
            ReceiveOffer(user.Id);
            Console.ReadKey();

            foreach (JObject proposal in _loanProposals)
            {
                Guid bankId = Guid.Parse(proposal["bankId"].Value<string>());
                string offer = proposal["offer"].Value<string>();

                Console.WriteLine("Bank {0} - {1};", bankId.ToString(), offer);
            }

            Console.ReadKey();
        }

        private void CreateRequestQueue(string message)
        {
            var factory = new ConnectionFactory() { HostName = "10.211.55.2" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("requests", type: ExchangeType.Fanout);

                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "requests",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Requesting loan: {0}", message);
            }
        }

        private void ReceiveOffer(Guid userIdentifier)
        {
            var factory = new ConnectionFactory() { HostName = "10.211.55.2" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            var userId = userIdentifier.ToString();

            channel.ExchangeDeclare(exchange: "proposals",
                                        type: "direct");
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName,
                                  exchange: "proposals",
                                  routingKey: userId);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                Console.WriteLine("Received offers:");
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                JObject proposal = JsonConvert.DeserializeObject<JObject>(message);
                _loanProposals.Add(proposal);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}
