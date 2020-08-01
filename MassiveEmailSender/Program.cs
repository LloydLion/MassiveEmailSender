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
			Console.WriteLine("Massive Email Sender Utility");
			Console.Write("Check setting before start program ");

			for (int i = 0; i < 6; i++)
			{
				Thread.Sleep(500);
				Console.Write('.');
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Press any key to start utility");
			Console.ReadKey();

			Console.WriteLine();
			Console.WriteLine("Work started");

			try
			{
				Console.Write("Reading config ... ");
				Config config = Config.LoadConfigFromFile(Config.DefaultConfigFileName);
				config.SetAddressesFromTxtFile(Config.DefaultEmailsTxtFileName);
				Console.WriteLine("OK!");

				Console.Write("Reading message ... ");
				MailMessage message = new MailMessage
				{
					IsBodyHtml = true,
					Body = File.ReadAllText(DefaultEmailContentFileName),
					Subject = ""
				};
				Console.WriteLine("OK!");

				Console.Write("Creating client ... ");
				MailSender sender = new MailSender(config.SenderAddress, config.SenderPassword,
					config.SmtpServerAddress);
				Console.WriteLine("OK!");

				Console.Write("Sending emails ");


				var task =
					Task.Run(() => sender.SendToAll(config.TargetAdresses, message));

				for (int i = 0; !task.IsCompleted; i++)
				{
					Thread.Sleep(1000);
					Console.Write(".");
				}

				Console.WriteLine(" OK!");
			}
			catch(Exception ex)
			{
				Console.WriteLine("Unhandled Exception: ");
				Console.WriteLine("\tMessage: " + ex.Message);
				Console.WriteLine("\tStackTrace: " + ex.StackTrace);
				Console.WriteLine("\t" + "Source: " + ex.Source);
			}

			Console.WriteLine();
			Console.WriteLine("Program work endded. Press any key to close");
			Console.ReadKey();
		}
	}
}
