using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;

namespace MainApp.Client.Clients.ClientClient;
public class ClientEmail
{
	private string _address_from="svaolimpiadny@yandex.ru";
	private string _address_from_password= "cli1VA23rDMmMf@g";
	public async Task SendEmailAsync(string _address_to, string subject, string text)
	{
		using var message = new MimeMessage();

		message.From.Add(new MailboxAddress("Система мониторинга и диагностики", _address_from));
		message.To.Add(new MailboxAddress("Персонал", _address_to));
		message.Subject = subject;
		message.Body = new TextPart(MimeKit.Text.TextFormat.Html) {
			Text = text
		};

		using(var client = new SmtpClient())
		{
			await client.ConnectAsync("smtp.yandex.com", 465, true);
			await client.AuthenticateAsync(_address_from, _address_from_password);
			await client.SendAsync(message);
			await client.DisconnectAsync(true);
		}
	}
}
