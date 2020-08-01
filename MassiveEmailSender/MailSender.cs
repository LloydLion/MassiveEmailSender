using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassiveEmailSender
{
	class MailSender
	{
		private readonly SmtpClient client;
		private readonly MailAddress from;

		public MailSender(MailAddress from, string password, string server)
		{
			this.from = from;

			client = new SmtpClient(server)
			{
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(from.Address, password),
				EnableSsl = true
			};
		}

		public void SendToAll(MailAddress[] addresses, MailMessage message)
		{
			message.To.Clear();
			message.From = from;

			foreach (var item in addresses) message.Bcc.Add(item);

			client.Send(message);
		}
	}
}
