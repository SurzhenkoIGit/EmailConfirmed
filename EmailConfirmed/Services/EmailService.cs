using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Net;

namespace EmailConfirmed
{
    public class EmailService
    {
        
		public async Task SendAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Администрация сайта", "surzhenko_i@krk-finance.ru"));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.mail.ru", 465, true);
                await client.AuthenticateAsync("surzhenko_i@krk-finance.ru", "w4kuApPuSYRd7EquWLzd");
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }

        }

        public async Task SendEmailAsync(string userEmail, string subject, string message)
        {
            await SendAsync(userEmail, subject, message);
        }

        public async Task SendEmailTwoFactorCode(string userEmail, string code)
        {
            await SendAsync(userEmail, "Код подтверждения", code);
        }
	}
}
