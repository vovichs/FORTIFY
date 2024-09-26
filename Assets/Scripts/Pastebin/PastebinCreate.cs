using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace Pastebin
{
	internal class PastebinCreate
	{
		private static string postURL = "https://pastebin.com/api/api_post.php";

		private static string devKey = "7f5549b0d6b7176c82647974efae0260";

		public static string response;

		public static string Send(string saveData, string named)
		{
			if (string.IsNullOrEmpty(saveData.Trim()))
			{
				return "empty scene";
			}
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				return "offline?";
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection.Add("api_dev_key", devKey);
			nameValueCollection.Add("api_option", "paste");
			nameValueCollection.Add("api_paste_code", saveData);
			nameValueCollection.Add("api_paste_name", named);
			nameValueCollection.Add("api_paste_private", "0");
			nameValueCollection.Add("api_paste_expire_date", "N");
			if (Encoding.UTF8.GetByteCount(saveData) >= 512000)
			{
				return "over pastebin size limit";
			}
			WebClient webClient = new WebClient();
			string text = Encoding.UTF8.GetString(webClient.UploadValues(postURL, nameValueCollection));
			if (text == null)
			{
				return "pastebin response null";
			}
			Uri result = null;
			if (!Uri.TryCreate(text, UriKind.Absolute, out result))
			{
				string[] array = text.Split(',');
				text = "error " + array[1];
			}
			popUp.inst.message("link copied to clipboard");
			return text;
		}

		public static string Get(string key)
		{
			HttpWebResponse obj = (HttpWebResponse)(WebRequest.Create("https://pastebin.com/raw/" + key) as HttpWebRequest).GetResponse();
			WebHeaderCollection header = obj.Headers;
			using (StreamReader streamReader = new StreamReader(obj.GetResponseStream(), Encoding.UTF8))
			{
				return streamReader.ReadToEnd();
			}
		}
	}
}
