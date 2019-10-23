using System;
using RabbitMQ.Client;
using System.Text;
using System.Linq;
using RabbitMQ.Client.Events;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace LoanQuotesMOMBank
{
    public class Program
    {
        private Guid _bankId;
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void Run()
        {
            _bankId = Guid.NewGuid();

            CreateRequestQueue();

            Console.WriteLine("Waiting on requests");
            Console.ReadKey();
        }

        private void CreateRequestQueue()
        {
            var factory = new ConnectionFactory() { HostName = "10.211.55.2" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare("requests", type: ExchangeType.Fanout);
            var queueName = channel.QueueDeclare().QueueName;

            channel.QueueBind(queue: queueName, exchange: "requests", routingKey: "");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                ProcessRequests(Encoding.UTF8.GetString(ea.Body));
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: true,
                                 consumer: consumer);
        }

        private void ProcessRequests(string body)
        {
            JObject loanRequest = JsonConvert.DeserializeObject<JObject>(body);

            Guid clientId = Guid.Parse(loanRequest["clientId"].Value<string>());
            int loanValue = loanRequest["amount"].Value<int>();
            int amountOfYears = new Random().Next(1, 10);
            int interestRate = new Random().Next(1, 10);
            string offer = "We offer you " + loanValue + " at " + interestRate + "% interest for only " + amountOfYears + " years!";
            JObject proposal = new JObject();

            proposal.Add("bankId", _bankId.ToString());
            proposal.Add("offer", offer);

            SendNewOffer(proposal.ToString(), clientId);
        }

        private void SendNewOffer(string offer, Guid clientId)
        {
            var factory = new ConnectionFactory() { HostName = "10.211.55.2" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(exchange: "proposals",
                                        type: "direct");

                var body = Encoding.UTF8.GetBytes(offer);
                channel.BasicPublish(exchange: "proposals",
                                     routingKey: clientId.ToString(),
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", clientId.ToString(), offer);
            }
        }
    }
}
