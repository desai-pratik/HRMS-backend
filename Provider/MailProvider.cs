using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Net.Mail;

namespace Crypt.Providers
{
    public class MailConfig
    {
        [Key] public int Id { get; set; }
        [Required] public string SenderName { get; set; } = "HR";
        [Required] public string EmailId { get; set; } = "divya.giri@kautilyam.com";
        [Required] public string Password { get; set; } = "Gd2812@2003";
        [Required] public int Port { get; set; } = 587;
        [Required] public string Host { get; set; } = "in-v3.mailjet.com";
        [Required] public bool EnableSsl { get; set; } = false;
        [Required] public bool IsDefault { get; set; } = true;
        public string NetCredentialKey { get; set; } = "4529fcad24f3baba633fc71cbbf70b29";
        public string NetCredentialPassword { get; set; } = "d060126e1376bdfd4c15f7a891be914a";

        [NotMapped] public bool IsBodyHtml { get; set; } = true;
        [NotMapped] public string Subject { get; set; }
        [NotMapped] public string ReceiverEmail { get; set; }
        [NotMapped] public string MailBody { get; set; }
    }
    public class Receiver
    {
        public string To { get; set; }
        public string Body { get; set; }
    }

    public static class MailProvider
    {
        private static SmtpClient GetSmtpClient(MailConfig mailConfig)
        {
            return new SmtpClient(mailConfig.Host, mailConfig.Port)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = mailConfig.EnableSsl,
                Credentials = new NetworkCredential(
                    string.IsNullOrWhiteSpace(mailConfig.NetCredentialKey) ? mailConfig.EmailId : mailConfig.NetCredentialKey,
                    string.IsNullOrWhiteSpace(mailConfig.NetCredentialPassword) ? mailConfig.Password : mailConfig.NetCredentialPassword),
            };
        }
        public static async Task<bool> SendMail(MailConfig mailConfig)
        {
            MailMessage mailMessage = new() { From = new MailAddress(mailConfig.EmailId, mailConfig.SenderName), Subject = mailConfig.Subject, IsBodyHtml = mailConfig.IsBodyHtml };

            mailMessage.To.Add(new MailAddress(mailConfig.ReceiverEmail));
            mailMessage.Body = mailConfig.MailBody;
            SmtpClient client = GetSmtpClient(mailConfig);
            await client.SendMailAsync(mailMessage);

            return true;
        }
    }
}
