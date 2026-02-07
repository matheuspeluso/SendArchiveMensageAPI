using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Mensageria.Domain.Events;
using Mensageria.Domain.Interfaces.Messages;
using Mensageria.Infra.Message.Settings;
using RabbitMQ.Client;

namespace Mensageria.Infra.Message.Publishers
{
    /// <summary>
    /// Classe para implementar envio de dados para servidor de mensageria RabbitMQ
    /// </summary>
    public class SendArchivePublisher : ISendArchiveMessage
    {
        public void Publisher(SendArchiveEvents ArchivePayload)
        {
            var factory = new ConnectionFactory()
            { 
                HostName = RabbitMQSettings.Host,
                UserName = RabbitMQSettings.UserName,
                Password = RabbitMQSettings.Password
            };

            ////Conectando com o servidor da mensageria
            using var conn = factory.CreateConnection();
            using var channel = conn.CreateModel();

            //criar fila no servidor de mensageria
            channel.QueueDeclare(queue: RabbitMQSettings.QueueName, 
                exclusive: false, durable: true, autoDelete: false, arguments: null);

            //Seriaalizando os dados
            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(ArchivePayload));

            //gravando os dados na fila
            channel.BasicPublish(exchange: "", routingKey: RabbitMQSettings.QueueName, 
                body: message, basicProperties: null);
        }
    }
}
