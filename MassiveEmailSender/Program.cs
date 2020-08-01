using System;
using System.Collections.Generic;
using System.Diagnostics;
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


		static void Main()
		{
			try
			{
				Console.WriteLine("Massive Email Sender Utility");

				Console.Write("Are you want reset setting? [Yes/No] ");
				string input = Console.ReadLine();
				Console.WriteLine();

				if (input == "Yes") ResetSettings();


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

		private static void ResetSettings()
		{
			Console.Write("Reseting settings ... ");
			File.WriteAllText(Config.DefaultConfigFileName, "");
			File.WriteAllText(Config.DefaultEmailsTxtFileName, "");
			File.WriteAllText(DefaultEmailContentFileName, "");
			Console.WriteLine("OK!");

			Console.WriteLine();
			Console.WriteLine("Enter your email address");
			var tmp1 = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine("Enter email password");
			var tmp2 = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine("Enter email smtp server address");
			var tmp3 = Console.ReadLine();
			Console.WriteLine();

			Console.Write("Writting to file ... ");
			File.WriteAllText(Config.DefaultConfigFileName, 
				new Config(new MailAddress(tmp1), new MailAddress[0], tmp2, tmp3).ExportConfig().json);
			Console.WriteLine("OK!");

			Process notepad = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "notepad.exe",
					WorkingDirectory = Environment.CurrentDirectory
				}
			};

			Console.WriteLine();
			Console.WriteLine("Write all target emails to file throught line break");

			Console.Write("Opening ");
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(500);
				Console.Write('.');
			}
			Console.WriteLine(" Open!");

			notepad.StartInfo.Arguments = Config.DefaultEmailsTxtFileName;

			notepad.Start();
			notepad.WaitForExit();

			Console.WriteLine();
			Console.WriteLine("Write message to html file");
			Console.Write("Opening ");
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(500);
				Console.Write('.');
			}
			Console.WriteLine(" Open!");

			notepad.StartInfo.Arguments = DefaultEmailContentFileName;

			notepad.Start();
			notepad.WaitForExit();

			Console.WriteLine();
			Console.WriteLine("Configuration complite!");
			Console.WriteLine("Press any key to continue");
			Console.ReadLine();
			Console.WriteLine();

			
		}
	}
}
