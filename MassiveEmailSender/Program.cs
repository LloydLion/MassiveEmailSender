using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassiveEmailSender
{
	class Program
	{
		public const string DefaultEmailContentFileName = "content.html";
		public const int CountOfThreads = 4;


		static void Main(string[] args)
		{
			try
			{

				Config config = Config.LoadConfigFromFile(Config.DefaultConfigFileName);
				config.SetAddressesFromTxtFile(Config.DefaultEmailsTxtFileName);

				MailMessage message = new MailMessage
				{
					IsBodyHtml = true,
					Body = File.ReadAllText(DefaultEmailContentFileName),
					Subject = ""
				};

				MailSender sender = new MailSender(config.SenderAddress, config.SenderPassword,
					config.SmtpServerAddress);

				sender.SendToAll(config.TargetAdresses, message);


			}
			catch(Exception ex)
			{
				Console.WriteLine("Unhandled Exception: ");
				Console.WriteLine(ex.Message);
				Console.WriteLine(ex.StackTrace);
			}

			Console.WriteLine(new string('-', 30));
			Console.WriteLine("Program work endded. Press any key to close");
			Console.ReadKey();
		}
	}
}
