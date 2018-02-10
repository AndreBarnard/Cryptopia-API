using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net.Http.Formatting;
using System.Configuration;

namespace CryptopiaAPI
{
	public class Cryptopia
	{
		public static async System.Threading.Tasks.Task getBalanceAsync()
		{
			string apiKey = ConfigurationManager.AppSettings["apiKey"];
			string apiSecret = ConfigurationManager.AppSettings["apiSecret"];
			string requestUri = ConfigurationManager.AppSettings["requestUri"];
			var postData = new
			{
				//Currency = "BTC"
			};

			// Create Request
			var request = new HttpRequestMessage();
			request.Method = HttpMethod.Post;
			request.RequestUri = new Uri(requestUri);
			request.Content = new ObjectContent(typeof(object), postData, new JsonMediaTypeFormatter());

			// Authentication
			string requestContentBase64String = string.Empty;
			if (request.Content != null)
			{
				// Hash content to ensure message integrity
				using (var md5 = MD5.Create())
				{
					requestContentBase64String = Convert.ToBase64String(md5.ComputeHash(await request.Content.ReadAsByteArrayAsync()));
				}
			}

			//create random nonce for each request
			var nonce = Guid.NewGuid().ToString("N");

			//Creating the raw signature string
			var signature = Encoding.UTF8.GetBytes(string.Concat(apiKey, HttpMethod.Post, HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower()), nonce, requestContentBase64String));
			using (var hmac = new HMACSHA256(Convert.FromBase64String(apiSecret)))
			{
				request.Headers.Authorization = new AuthenticationHeaderValue("amx", $"{apiKey}:{Convert.ToBase64String(hmac.ComputeHash(signature))}:{nonce}");
			}
			
			// Send Request
			using (var client = new HttpClient())
			{
				var response = await client.SendAsync(request);
				if (response.IsSuccessStatusCode)
				{
					Console.WriteLine(await response.Content.ReadAsStringAsync());
					//{"Success":true,"Error":null,"Data":[{"CurrencyId":2,"Symbol":"DOT","Total":9646.07411016,"Available":9646.07411016,"Unconfirmed":0.0,"HeldForTrades":0.0,"PendingWithdraw":0.0,"Address":"1HEfio1kreDBgj5uCw4VHbEDSgc6YJXfTN","Status":"OK","StatusMessage":null}]}
				}
			}

		}
	}
}
