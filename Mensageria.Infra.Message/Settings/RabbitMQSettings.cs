using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.Infra.Message.Settings
{
    public class RabbitMQSettings
    {
        public static string Host => "localhost";
        public static string QueueName => "sendArchive";
        public static string UserName => "DevMatheus";
        public static string Password => "FuscaAzul";
    }
}
