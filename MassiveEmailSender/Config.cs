using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace MassiveEmailSender
{
	class Config
	{
		public const string DefaultConfigFileName = "emal_config.json";
		public const string DefaultEmailsTxtFileName = "emails.txt";


		public MailAddress SenderAddress { get; private set; }
		public string SenderPassword { get; private set; }
		public string SmtpServerAddress { get; private set; }
		public MailAddress[] TargetAdresses { get; private set; }


		public static Config LoadConfigFromFile(string fileName)
		{
			return LoadConfigFromString(File.ReadAllText(fileName));
		}

		public static Config LoadConfigFromString(string value)
		{
			var model = JsonConvert.DeserializeObject<ParseModel>(value);

			return new Config(new MailAddress(model.SenderAddress),
				new MailAddress[0], model.SenderPassword, model.SmtpServerAddress);
		}

		public void SetAddressesFromTxtFile(string path)
		{
			TargetAdresses = File.ReadAllText(path).Replace("\r\n", "\u1234")
				.Split('\u1234').Select((s) => new MailAddress(s)).ToArray();
		}

		public (string json, string txt) ExportConfig()
		{
			var pm = new ParseModel
			{
				SenderAddress = SenderAddress.Address,
				SenderPassword = SenderPassword,
				SmtpServerAddress = SmtpServerAddress
			};

			return (JsonConvert.SerializeObject(pm),
				string.Join("\r\n", TargetAdresses.Select((s) => s.Address)));
		}

		public Config(MailAddress sender, MailAddress[] target, string password, string smtpServerAddress)
		{
			SenderAddress = sender;
			TargetAdresses = target;
			SenderPassword = password;
			SmtpServerAddress = smtpServerAddress;
		}


		private class ParseModel
		{
			public string SenderAddress { get; set; }
			public string SenderPassword { get; set; }
			public string SmtpServerAddress { get; set; }
		}
	}
}
