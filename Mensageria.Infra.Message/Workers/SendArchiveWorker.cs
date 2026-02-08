using Mensageria.Domain.Events;
using Mensageria.Infra.Message.Components;
using Mensageria.Infra.Message.Settings;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mensageria.Infra.Message.Workers
{
    public class SendArchiveWorker : BackgroundService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public SendArchiveWorker()
        {
            var factory = new ConnectionFactory()
            {
                HostName = RabbitMQSettings.Host,
                UserName = RabbitMQSettings.UserName,
                Password = RabbitMQSettings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: RabbitMQSettings.QueueName,
                exclusive: false, durable: true, autoDelete: false, arguments: null);

            // Diz ao RabbitMQ para não mandar mais de 1 mensagem por vez para este worker
            _channel.BasicQos(0, 1, false);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, args) =>
            {
                try
                {
                    var content = Encoding.UTF8.GetString(args.Body.ToArray());
                    var sendArchiveEvents = JsonConvert.DeserializeObject<SendArchiveEvents>(content);

                    if (sendArchiveEvents != null)
                    {
                        var sendEmail = new SendEmail();
                        sendEmail.SendArchiveByEmail(sendArchiveEvents);
                    }

                    // Se o e-mail enviou sem erros, confirmamos a mensagem (Ack)
                    _channel.BasicAck(args.DeliveryTag, false);
                    Console.WriteLine($"[Sucesso] E-mail enviado para: {sendArchiveEvents?.Email}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Erro] Falha ao processar mensagem: {ex.Message}");
                    // Em caso de erro, a mensagem volta para a fila (Nack) para tentar novamente
                    _channel.BasicNack(args.DeliveryTag, false, true);
                }
            };

            _channel.BasicConsume(
                queue: RabbitMQSettings.QueueName,
                autoAck: false, // Confirmação manual via BasicAck
                consumer: consumer);

            return Task.CompletedTask;
        }

        public override void Dispose()
        {
            if (_channel.IsOpen) _channel.Close();
            if (_connection.IsOpen) _connection.Close();
            base.Dispose();
        }
    }
}