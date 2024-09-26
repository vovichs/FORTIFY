using System.IO;
using UnityEngine;

public class bannerTexture : MonoBehaviour
{
	private static bool loaded;

	public Material banner;

	public Texture box;

	private void Awake()
	{
		if (!loaded)
		{
			string path = Application.dataPath + "/banner.png";
			if (File.Exists(path))
			{
				byte[] data = File.ReadAllBytes(path);
				Texture2D texture2D = new Texture2D(128, 384, TextureFormat.RGB24, mipChain: false);
				texture2D.LoadImage(data);
				texture2D.Apply();
				banner.SetTexture("_MainTex", texture2D);
			}
			else
			{
				banner.SetTexture("_MainTex", box);
			}
			loaded = true;
		}
	}
}
