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

		public void SendToAll(MailAddress[] addresses, MailMessage message, string threadID)
		{
			message.From = from;

			for (int i = 0; i < addresses.Length; i++)
			{
				message.To.Clear();
				message.To.Add(addresses[i]);
    
				try
				{
					client.Send(message);
				}
				catch(Exception)
				{
					Console.WriteLine("[" + threadID + "]: " +
						"Sending To " + addresses[i].Address + " ... FAILED!!");

					Thread.Sleep(1000);
					SendToAll(addresses, message, threadID);
				}

				Console.WriteLine("[" + threadID + "]: " +
					"Sending To " + addresses[i].Address + " ... OK!" );
			}
		}
	}
}
