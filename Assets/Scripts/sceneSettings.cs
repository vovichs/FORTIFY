using UnityEngine;

public class sceneSettings : MonoBehaviour
{
	public enum eScene
	{
		island,
		flat,
		caves,
		icebergs
	}

	public static sceneSettings inst;

	public Material skybox;

	public Terrain terrain;

	public GameObject water;

	public bool rockPlace;

	public Material waterMat;

	public eScene scene;

	private void Awake()
	{
		inst = this;
		scene = (eScene)BuilderSystem.inst.sceneDropDn.value;
		if (skybox != null)
		{
			RenderSettings.skybox = skybox;
		}
		if (scene == eScene.caves)
		{
			RenderSettings.fog = false;
		}
		else
		{
			RenderSettings.fog = true;
		}
		BuilderSystem.rockPlacement = rockPlace;
	}
}
