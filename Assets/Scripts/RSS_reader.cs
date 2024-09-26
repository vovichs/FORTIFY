using System.Collections;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class RSS_reader : MonoBehaviour
{
	private string url = "https://steamcommunity.com/games/505040/rss";

	public Text updateText;

	private string link;

	private void Awake()
	{
		StartCoroutine(News());
	}

	public IEnumerator News()
	{
		UnityWebRequest.Get(url);
		using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
		{
			yield return webRequest.SendWebRequest();
			if (webRequest.isNetworkError || webRequest.isHttpError)
			{
				updateText.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(webRequest.downloadHandler.text);
				XmlNodeList xmlNodeList = xmlDocument.SelectNodes("rss/channel/item/title");
				XmlNodeList xmlNodeList2 = xmlDocument.SelectNodes("rss/channel/item/link");
				updateText.text = xmlNodeList[0].InnerText;
				link = xmlNodeList2[0].InnerText;
			}
		}
	}

	public void openLink()
	{
		if (link != "")
		{
			Application.OpenURL(link);
		}
	}
}
