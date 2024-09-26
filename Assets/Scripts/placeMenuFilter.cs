using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class placeMenuFilter : MonoBehaviour
{
	public static placeMenuFilter inst;

	private static bool start = true;

	public TMP_InputField search;

	public Transform listParent;

	public GameObject label;

	public TextMeshProUGUI labelText;

	public TextMeshProUGUI descText;

	public static bool displayInfo = true;

	public Toggle infoTog;

	private toggleParts[] partList;

	public List<int> favorites;

	private bool favChange;

	public int lastIndex;

	public Scrollbar sbar;

	private void Awake()
	{
		inst = this;
		if (start)
		{
			start = false;
			if (PlayerPrefs.HasKey("hideDesc") && PlayerPrefs.GetInt("hideDesc") == 1)
			{
				displayInfo = false;
			}
			if (PlayerPrefs.HasKey("favorites"))
			{
				string[] array = PlayerPrefs.GetString("favorites").Split('_');
				for (int i = 0; i < array.Length - 1; i++)
				{
					favorites.Add(int.Parse(array[i]));
				}
			}
		}
		if (!displayInfo)
		{
			infoTog.isOn = false;
			displayInfo = false;
		}
	}

	public void displayInfoState()
	{
		displayInfo = !displayInfo;
	}

	private void Start()
	{
		partList = new toggleParts[listParent.childCount];
		bool flag = favorites.Count > 0;
		int num = 0;
		foreach (Transform item in listParent)
		{
			toggleParts component = item.GetComponent<toggleParts>();
			GameObject[] prefabList = MGMT.inst.prefabList;
			foreach (GameObject gameObject in prefabList)
			{
				if (gameObject != null && item.name == gameObject.name)
				{
					component.info = gameObject.GetComponent<BuilderPart>().info;
					if (flag)
					{
						component.info.favorite = favorites.Contains(component.info.id);
					}
					break;
				}
			}
			if (!component.info)
			{
				MonoBehaviour.print(item.name);
				break;
			}
			item.GetComponent<mouseOverLabel>().info = component.info;
			partList[num] = component;
			num++;
		}
	}

	public void searchFilter()
	{
		string text = search.text.ToLower();
		toggleParts[] array;
		if (text == "")
		{
			array = partList;
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = array[i].gameObject;
				if (!gameObject.activeSelf)
				{
					gameObject.SetActive(value: true);
				}
			}
			return;
		}
		array = partList;
		foreach (toggleParts toggleParts in array)
		{
			if (!toggleParts.toggle.isOn)
			{
				toggleParts.gameObject.SetActive(toggleParts.info.named.ToLower().Contains(text));
			}
		}
	}

	public void typeFilter(int index)
	{
		toggleParts[] array;
		if (index == lastIndex)
		{
			lastIndex = -1;
			array = partList;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].gameObject.SetActive(value: true);
			}
			return;
		}
		lastIndex = index;
		sbar.value = 1f;
		switch (index)
		{
		case 0:
			searchFilter();
			return;
		case 5:
			array = partList;
			foreach (toggleParts toggleParts in array)
			{
				if (toggleParts.info.favorite)
				{
					toggleParts.gameObject.SetActive(value: true);
				}
				else if (!toggleParts.toggle.isOn)
				{
					toggleParts.gameObject.SetActive(value: false);
				}
			}
			return;
		}
		array = partList;
		foreach (toggleParts toggleParts2 in array)
		{
			if (toggleParts2.group.HasFlag((toggleParts.partGroups)index))
			{
				toggleParts2.gameObject.SetActive(value: true);
			}
			else if (!toggleParts2.toggle.isOn)
			{
				toggleParts2.gameObject.SetActive(value: false);
			}
		}
	}

	public void searchToggle()
	{
		if (!search.gameObject.activeSelf)
		{
			search.gameObject.SetActive(value: true);
			StartCoroutine(WaitForFocus());
		}
		else
		{
			search.gameObject.SetActive(value: false);
		}
	}

	public IEnumerator WaitForFocus()
	{
		yield return 0;
		search.ActivateInputField();
		search.Select();
		BuilderUI.inst.disableInput(state: true);
	}

	public void toggleFavorite()
	{
		favChange = true;
		partInfo info = BuilderSystem.inst.BPinfo.info;
		info.favorite = !info.favorite;
	}

	private void OnApplicationQuit()
	{
		int value = Convert.ToInt32(!displayInfo);
		PlayerPrefs.SetInt("hideDesc", value);
		if (!favChange)
		{
			return;
		}
		StringBuilder stringBuilder = new StringBuilder();
		toggleParts[] array = partList;
		foreach (toggleParts toggleParts in array)
		{
			if (toggleParts.info.favorite)
			{
				stringBuilder.Append(toggleParts.info.id.ToString() + "_");
			}
		}
		PlayerPrefs.SetString("favorites", stringBuilder.ToString());
	}
}
