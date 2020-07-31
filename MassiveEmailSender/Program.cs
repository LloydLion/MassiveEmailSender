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
			Config config = Config.LoadConfigFromFile(Config.DefaultConfigFileName);
			config.SetAddressesFromTxtFile(Config.DefaultEmailsTxtFileName);

			MailMessage message = new MailMessage
			{
				IsBodyHtml = true,
				Body = File.ReadAllText(DefaultEmailContentFileName),
				Subject = ""
			};



			var threadsAdresses = new MailAddress[CountOfThreads][];
			MailAddress[] tmp = 
				new MailAddress[config.TargetAdresses.Length - config.TargetAdresses.Length % CountOfThreads];
			MailAddress[] tmp2 = new MailAddress[config.TargetAdresses.Length % CountOfThreads];

			Array.Copy(config.TargetAdresses, tmp,+
				config.TargetAdresses.Length - config.TargetAdresses.Length % CountOfThreads);

			Array.Copy(config.TargetAdresses,
				config.TargetAdresses.Length - config.TargetAdresses.Length % CountOfThreads,
				tmp2, 0, config.TargetAdresses.Length % CountOfThreads);

			ThreadHandler(config, message, tmp2, "Main");

			var threadAddressesCount = tmp.Length / CountOfThreads;
			for (int i = 0; i < CountOfThreads; i++)
			{
				threadsAdresses[i] = new MailAddress[threadAddressesCount];
				Array.Copy(tmp, threadAddressesCount * i, threadsAdresses[i], 0, threadAddressesCount);
			}

			Task[] tmp3 = new Task[CountOfThreads];
			for (int j = 0; j < CountOfThreads; j++)
			{
				var jcache = j;
				tmp3[j] = Task.Run(() => ThreadHandler(config, message, threadsAdresses[jcache], "Thread " + jcache));
			}

			for (int h = 0; h < CountOfThreads; h++)
			{
				tmp3[h].Wait();
			}

			Console.WriteLine(new string('-', 30));
			Console.WriteLine("Program work endded. Press any key to close");
			Console.ReadKey();
		}

		static void ThreadHandler(Config config, MailMessage message, MailAddress[] addresses, string threadID)
		{
			MailSender sender = new MailSender(config.SenderAddress, config.SenderPassword,
				config.SmtpServerAddress);

			sender.SendToAll(addresses, message, threadID);

			Console.WriteLine("[" + threadID + "]: " + "Work completed! Wait for all!");
		}
	}
}
