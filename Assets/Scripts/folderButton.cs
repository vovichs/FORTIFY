using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class folderButton : MonoBehaviour
{
	public List<GameObject> fileButtons;

	public Transform _transform;

	public Toggle tog;

	public string path;

	public Text Text;

	public RectTransform arrow_rt;

	public bool minimized = true;

	public GameObject removeButt;

	public void minToggle()
	{
		minimized = !minimized;
		if (minimized)
		{
			arrow_rt.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
		}
		else
		{
			arrow_rt.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
		}
		foreach (GameObject fileButton in fileButtons)
		{
			if (fileButton != null)
			{
				fileButton.SetActive(!minimized);
			}
		}
	}

	public void pickFolder()
	{
		if (tog.isOn)
		{
			fileList.inst.changeFolder(path);
			tog.image.color = tog.colors.highlightedColor;
		}
		else
		{
			tog.image.color = Color.white;
		}
	}

	public void removeThis()
	{
		PlayerPrefs.DeleteKey("extFolder");
		fileList.inst.loadList();
	}
}
