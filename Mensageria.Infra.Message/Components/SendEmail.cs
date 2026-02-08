using Mensageria.Domain.Events;
using Mensageria.Infra.Message.Helpers;
using System;
using System.IO;
using System.Net.Mail;

namespace Mensageria.Infra.Message.Components
{
    public class SendEmail
    {
        private readonly SmtpMailHelper _mailHelper;

        public SendEmail()
        {
            _mailHelper = new SmtpMailHelper();
        }

        public void SendArchiveByEmail(SendArchiveEvents sendArchiveEvents)
        {
            string htmlBody = $@"
                <html>
                    <body style='font-family: Arial, sans-serif; color: #333;'>
                        <div style='max-width: 600px; margin: 0 auto; border: 1px solid #e0e0e0; border-radius: 8px; overflow: hidden;'>
                            <div style='background-color: #007bff; padding: 20px; text-align: center;'>
                                <h2 style='color: white; margin: 0;'>Seu arquivo chegou!</h2>
                            </div>
                            <div style='padding: 20px;'>
                                <p>Olá,</p>
                                <p>O arquivo <strong>{sendArchiveEvents.ArchiveName}</strong> foi processado com sucesso.</p>
                                <p>Você pode encontrá-lo em anexo a este e-mail.</p>
                                <div style='margin-top: 30px; padding: 15px; background-color: #f8f9fa; text-align: center;'>
                                    <small>Enviado em: {DateTime.Now:dd/MM/yyyy HH:mm}</small>
                                </div>
                            </div>
                        </div>
                    </body>
                </html>";

            var mail = new MailMessage
            {
                Subject = $"Envio de Arquivo: {sendArchiveEvents.ArchiveName}",
                Body = htmlBody,
                IsBodyHtml = true
            };

            mail.To.Add(sendArchiveEvents.Email!);

            try
            {
                if (sendArchiveEvents.ArchiveContent != null)
                {
                    // Criamos o stream do blob (byte[])
                    var stream = new MemoryStream(sendArchiveEvents.ArchiveContent);

                    // IMPORTANTE: Reseta a posição para o início para que o SmtpClient consiga ler o conteúdo
                    stream.Position = 0;

                    var attachment = new Attachment(stream, sendArchiveEvents.ArchiveName, sendArchiveEvents.ArchiveType);
                    mail.Attachments.Add(attachment);
                }

                _mailHelper.Send(mail);
            }
            finally
            {
                // Limpeza obrigatória para evitar que arquivos fiquem presos em memória
                foreach (var attachment in mail.Attachments)
                {
                    attachment.ContentStream.Dispose();
                }
                mail.Dispose();
            }
        }
    }
}