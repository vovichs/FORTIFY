using UnityEngine;

namespace UnityStandardAssets.Water
{
	[ExecuteInEditMode]
	public class WaterBase : MonoBehaviour
	{
		public Material sharedMaterial;

		public static bool edgeBlend = true;

		public void Start()
		{
			if ((bool)sharedMaterial)
			{
				UpdateShader();
			}
		}

		public static void UpdateShader()
		{
			if (!SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.Depth))
			{
				edgeBlend = false;
			}
			if (edgeBlend && QualitySettings.GetQualityLevel() > 0)
			{
				Shader.EnableKeyword("WATER_EDGEBLEND_ON");
				Shader.DisableKeyword("WATER_EDGEBLEND_OFF");
				if ((bool)Camera.main)
				{
					Camera.main.depthTextureMode |= DepthTextureMode.Depth;
				}
			}
			else
			{
				Shader.EnableKeyword("WATER_EDGEBLEND_OFF");
				Shader.DisableKeyword("WATER_EDGEBLEND_ON");
			}
		}

		public void WaterTileBeingRendered(Transform tr, Camera currentCam)
		{
			if ((bool)currentCam && edgeBlend)
			{
				currentCam.depthTextureMode |= DepthTextureMode.Depth;
			}
		}
	}
}
