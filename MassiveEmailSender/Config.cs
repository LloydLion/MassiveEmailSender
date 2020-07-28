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


		public MailAddress SenderAdress { get; private set; }
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

			return new Config(new MailAddress(model.SenderAdress),
				new MailAddress[0], model.SenderPassword, model.SmtpServerAddress);
		}

		/*public string ConvertToString()
		{
			return JsonConvert.SerializeObject(this);
		}*/

		public void SetAddressesFromTxtFile(string path)
		{
			TargetAdresses = File.ReadAllText(path).Replace("\r\n", "\u1234")
				.Split('\u1234').Select((s) => new MailAddress(s)).ToArray();
		}

		public Config(MailAddress sender, MailAddress[] target, string password, string smtpServerAddress)
		{
			SenderAdress = sender;
			TargetAdresses = target;
			SenderPassword = password;
			SmtpServerAddress = smtpServerAddress;
		}


		private class ParseModel
		{
			public string SenderAdress { get; set; }
			public string SenderPassword { get; set; }
			public string SmtpServerAddress { get; set; }
		}
	}
}
