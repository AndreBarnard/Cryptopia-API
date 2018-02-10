using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptopiaAPI
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				getbalAsync().Wait();
			}
			catch (Exception ex)
			{
			}

			Console.ReadLine();

		}


		public static async Task getbalAsync()
		{
			await Cryptopia.getBalanceAsync();
		}
	}
}
