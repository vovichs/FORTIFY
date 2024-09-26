using Steamworks;
using UnityEngine;

public class DLC : MonoBehaviour
{
	public static DLC inst;

	public static bool DLC_owned;

	public bool Override;

	public GameObject[] electricParts;

	public GameObject[] uiPanels;

	public GameObject[] dlcDisable;

	private void Awake()
	{
		if (inst != null)
		{
			setUI();
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		inst = this;
		Object.DontDestroyOnLoad(base.gameObject);
		if (SteamManager.Initialized)
		{
			DLC_owned = SteamApps.BIsDlcInstalled((AppId_t)1100550u);
		}
		setUI();
	}

	public void setUI()
	{
		if (!DLC_owned)
		{
			GameObject[] array = electricParts;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
			array = uiPanels;
			foreach (GameObject gameObject in array)
			{
				if (gameObject != null)
				{
					gameObject.SetActive(value: false);
				}
			}
		}
		else
		{
			GameObject[] array = dlcDisable;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].SetActive(value: false);
			}
		}
	}
}
