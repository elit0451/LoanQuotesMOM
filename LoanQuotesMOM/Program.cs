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
            bool running = true;
            User user = new User();
            _loanProposals = new List<JObject>();

            ReceiveOffer(user.Id);

            while (running)
            {
                Console.Clear();
                Console.WriteLine("1 - Ask for a loan");
                Console.WriteLine("2 - View current offers");
                Console.WriteLine("0 - Exit");
                Console.WriteLine("Choose an option: ");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        AskForLoan(user);
                        break;
                    case "2":
                        ViewCurrentOffers();
                        break;
                    case "0":
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Option not valid.");
                        Console.ReadKey();
                        break;
                }
                
            }
        }

        private void AskForLoan(User user)
        {
            Console.Clear();
            Console.WriteLine("What is the amount of loan you need?");
            int requestedMoney = int.Parse(Console.ReadLine());

            JObject loan = new JObject();
            loan.Add("amount", requestedMoney);
            loan.Add("clientId", user.Id.ToString());

            CreateRequestQueue(loan.ToString());
        }

        private void ViewCurrentOffers()
        {
            Console.Clear();
            foreach (JObject proposal in _loanProposals)
            {
                Guid bankId = Guid.Parse(proposal["bankId"].Value<string>());
                string offer = proposal["offer"].Value<string>();

                Console.WriteLine("Bank {0} - {1};", bankId.ToString(), offer);
            }

            Console.WriteLine("\nChoose an offer: ");
            string offerSelected = Console.ReadLine();

            _loanProposals.Clear();
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
