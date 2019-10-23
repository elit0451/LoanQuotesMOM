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
        static void Main(string[] args)
        {
            new Program().Run();
        }

        private void CreateRequestQueue(string message)
        {
            var factory = new ConnectionFactory() { HostName = "10.211.55.2" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "requests",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
                
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: "",
                                     routingKey: "",
                                     basicProperties: null,
                                     body: body);

                Console.WriteLine(" [x] Requesting loan: {0}", message);
            }
        }

        private void Run()
        {
            User user = new User();
            List<string> loanProposals = new List<string>();

            Console.WriteLine("What is the amount of loan you need?");
            int requestedMoney = int.Parse(Console.ReadLine());

            JObject loan = new JObject();
            loan.Add("amount", requestedMoney);
            loan.Add("clientId", user.Id);

            CreateRequestQueue(loan.ToString());
            Console.ReadKey();
        }
    }
}
