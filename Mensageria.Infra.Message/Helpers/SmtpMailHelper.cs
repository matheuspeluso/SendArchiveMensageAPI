using System;
using System.Net.Mail;

namespace Mensageria.Infra.Message.Helpers
{
    public class SmtpMailHelper
    {
        public void Send(MailMessage mailMessage)
        {
            var email = Environment.GetEnvironmentVariable("EMAIL_USERNAME");
            var passwordEmail = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(passwordEmail))
                throw new ApplicationException("Credenciais 'EMAIL_USERNAME' ou 'EMAIL_PASSWORD' não configuradas.");

            // Configura o remetente com o e-mail das variáveis de ambiente
            mailMessage.From = new MailAddress(email, "Sistema de Arquivos");

            using (var smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.Credentials = new System.Net.NetworkCredential(email, passwordEmail);
                smtp.EnableSsl = true;
                smtp.Send(mailMessage);
            }
        }
    }
}