using MassiveEmailSender.Language;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
		private static LocaleDictionary locale;

		static void Main()
		{
			try
			{
				locale = new LocaleDictionary(CultureInfo.CurrentUICulture);

				Console.WriteLine(locale.GetString("Massive Email Sender Utility"));

				Console.Write(locale.GetString("Are you want reset setting? [Yes/No] "));
				string input = Console.ReadLine();
				Console.WriteLine();

				if (input == locale.GetString("Yes")) ResetSettings();


				Console.Write(locale.GetString("Check setting before program start "));

				for (int i = 0; i < 6; i++)
				{
					Thread.Sleep(500);
					Console.Write('.');
				}

				Console.WriteLine();
				Console.WriteLine();
				Console.WriteLine(locale.GetString("Press any key to start utility"));
				Console.ReadKey();

				Console.WriteLine();

				Console.Write(locale.GetString("Reading config ... "));
				Config config = Config.LoadConfigFromFile(Config.DefaultConfigFileName);
				config.SetAddressesFromTxtFile(Config.DefaultEmailsTxtFileName);
				Console.WriteLine(locale.GetString("OK!"));

				Console.Write(locale.GetString("Reading message ... "));
				MailMessage message = new MailMessage
				{
					IsBodyHtml = true,
					Body = File.ReadAllText(DefaultEmailContentFileName),
					Subject = ""
				};
				Console.WriteLine(locale.GetString("OK!"));


				Console.Write(locale.GetString("Creating client ... "));
				MailSender sender = new MailSender(config.SenderAddress, config.SenderPassword,
					config.SmtpServerAddress);
				Console.WriteLine(locale.GetString("OK!"));

				Console.Write(locale.GetString("Sending emails "));


				var task =
					Task.Run(() => sender.SendToAll(config.TargetAdresses, message));

				for (int i = 0; !task.IsCompleted; i++)
				{
					Thread.Sleep(1000);
					Console.Write('.');
				}

				Console.WriteLine(" " + locale.GetString("OK!"));
			}
			catch(Exception ex)
			{
				ShowExceptionInfo(ex);
			}

			Console.WriteLine();
			Console.WriteLine(locale.GetString("Program work endded. Press any key to close"));
			Console.ReadKey();
		}

		private static void ResetSettings()
		{
			Console.Write(locale.GetString("Reseting settings ... "));
			File.WriteAllText(Config.DefaultConfigFileName, "");
			File.WriteAllText(Config.DefaultEmailsTxtFileName, "");
			File.WriteAllText(DefaultEmailContentFileName, "");
			Console.WriteLine(locale.GetString("OK!"));

			Console.WriteLine();
			Console.WriteLine(locale.GetString("Enter your email address"));
			var tmp1 = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine(locale.GetString("Enter password from email"));
			var tmp2 = Console.ReadLine();
			Console.WriteLine();
			Console.WriteLine(locale.GetString("Enter email smtp server address"));
			var tmp3 = Console.ReadLine();
			Console.WriteLine();

			Console.Write(locale.GetString("Writting to file ... "));
			File.WriteAllText(Config.DefaultConfigFileName, 
				new Config(new MailAddress(tmp1), new MailAddress[0], tmp2, tmp3).ExportConfig().json);
			Console.WriteLine(locale.GetString("OK!"));

			Process notepad = new Process()
			{
				StartInfo = new ProcessStartInfo()
				{
					FileName = "notepad.exe",
					WorkingDirectory = Environment.CurrentDirectory
				}
			};

			Console.WriteLine();
			Console.WriteLine(locale.GetString("Write all target emails to file throught line break"));

			Console.Write(locale.GetString("Opening "));
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(500);
				Console.Write('.');
			}
			Console.WriteLine(locale.GetString(" Open!"));

			notepad.StartInfo.Arguments = Config.DefaultEmailsTxtFileName;

			notepad.Start();
			notepad.WaitForExit();

			Console.WriteLine();
			Console.WriteLine(locale.GetString("Write message to html file"));
			Console.Write(locale.GetString("Opening "));
			for (int i = 0; i < 5; i++)
			{
				Thread.Sleep(500);
				Console.Write('.');
			}
			Console.WriteLine(locale.GetString(" Open!"));

			notepad.StartInfo.Arguments = DefaultEmailContentFileName;

			notepad.Start();
			notepad.WaitForExit();

			Console.WriteLine();
			Console.WriteLine(locale.GetString("Configuration complite!"));
			Console.WriteLine(locale.GetString("Press any key to continue"));
			Console.ReadLine();
			Console.WriteLine();

			
		}

		private static void ShowExceptionInfo(Exception ex)
		{
			Console.WriteLine("Unhandled Exception: ");
			Console.WriteLine("\tMessage: " + ex.Message);
			Console.WriteLine("\tStackTrace: " + ex.StackTrace);
			Console.WriteLine("\tSource: " + ex.Source);

			if (ex.InnerException != null)
			{
				Console.WriteLine("\n\n\tInnerException: ");
				ShowExceptionInfo(ex.InnerException);
			}
		}
	}
}
