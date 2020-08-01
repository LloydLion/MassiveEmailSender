using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MassiveEmailSender
{
	class LocaleDictionary
	{
		private const string DefaultLocation = ".\\Language\\";

		private readonly ResXResourceSet manager;


		public LocaleDictionary(CultureInfo language)
		{
			if (language.Name != "en-US") manager = LoadDictionary(language);
		}

		public string GetString(string key)
		{
			var clone = (string)key.Clone();

			if (key.StartsWith(" ")) key = @"\\\\\4" + key.Substring(1);
			if (key.EndsWith(" ")) key = key.Substring(0, key.Length - 1) + @"\\\\\4";

			try
			{
				return manager?.GetString(key) ?? clone;
			}
			catch(MissingManifestResourceException)
			{
				return clone;
			}
		}

		private ResXResourceSet LoadDictionary(CultureInfo language)
		{
			return new ResXResourceSet(DefaultLocation + language + ".resx");
		}

	}
}
