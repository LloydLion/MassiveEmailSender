using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MassiveEmailSender
{
	class Program
	{
		public const string DefaultEmailContentFileName = "content.html";


		static void Main(string[] args)
		{
			Config config = Config.LoadConfigFromFile(Config.DefaultConfigFileName);
			config.SetAddressesFromTxtFile(Config.DefaultEmailsTxtFileName);

			SendAll(config, File.ReadAllText(DefaultEmailContentFileName));

			Console.WriteLine("Program work endded. Press any key to close");
			Console.ReadKey();
		}

		static void SendAll(Config config, string text)
		{
			var sender = new SmtpClient(config.SmtpServerAddress)
			{
				EnableSsl = true,
				UseDefaultCredentials = false,
				Credentials = new NetworkCredential(config.SenderAdress.Address, config.SenderPassword)
			};



			Console.WriteLine("Sending from " + config.SenderAdress.Address);
			Console.WriteLine();

			for (int i = 0; i < config.TargetAdresses.Length; i++)
			{
				var target = config.TargetAdresses[i];
				var message = new MailMessage(config.SenderAdress, target)
				{
					Subject = " ",
					IsBodyHtml = true,
					Body = text
				};

				Console.Write("Sending to " + target.Address);
				sender.Send(message);
				Console.WriteLine(" ... OK!");
			}

			Console.WriteLine(new string('-', 40));
		}
	}
}
