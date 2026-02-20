using System.Net.Mail;

namespace Mensageria.Infra.Message.Helpers
{
    public class SmtpMailHelper
    {
        public void Send(MailMessage mailMessage)
        {
            // Remetente fake (MailHog aceita qualquer um)
            mailMessage.From = new MailAddress(
                "no-reply@mensageria.local",
                "Sistema de Arquivos"
            );

            using (var smtp = new SmtpClient("localhost", 1025))
            {
                smtp.UseDefaultCredentials = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.EnableSsl = false;

                smtp.Send(mailMessage);
            }
        }
    }
}