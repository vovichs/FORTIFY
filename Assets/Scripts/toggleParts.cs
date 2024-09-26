using System;
using UnityEngine;
using UnityEngine.UI;

public class toggleParts : MonoBehaviour
{
	[Flags]
	public enum partGroups
	{
		None = 0x0,
		Electric = 0x1,
		Water = 0x2,
		Storage = 0x3,
		fun = 0x4
	}

	public static int togGroupId = 1;

	public int groupId;

	public Toggle toggle;

	private Color normal;

	private Color pressed;

	public string overrideName;

	public Toggle includeTog;

	public partInfo info;

	public partGroups group;

	private void Awake()
	{
		togGroupId = 1;
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(OnToggleValueChanged);
		ColorBlock colors = toggle.colors;
		normal = colors.normalColor;
		pressed = colors.pressedColor;
		if (toggle.isOn)
		{
			colors.normalColor = pressed;
			toggle.colors = colors;
		}
	}

	private void OnToggleValueChanged(bool isOn)
	{
		ColorBlock colors = toggle.colors;
		if (isOn && !BuilderSystem.placeMode)
		{
			BuilderUI.inst.lastUsed[groupId] = base.name;
		}
		if ((bool)includeTog && includeTog.isOn)
		{
			if (isOn)
			{
				if (!BuilderSystem.placeMode)
				{
					togGroupId = groupId;
				}
				if (!pickIncludePart())
				{
					return;
				}
				includeTog.transform.parent.GetComponent<Toggle>().isOn = true;
				colors.normalColor = normal;
			}
			else
			{
				colors.normalColor = normal;
			}
		}
		else if (isOn)
		{
			if (overrideName.Length > 1)
			{
				BuilderSystem.inst.ChangePart(overrideName);
			}
			else
			{
				BuilderSystem.inst.ChangePart(base.name);
			}
			colors.normalColor = pressed;
			if (!BuilderSystem.placeMode)
			{
				togGroupId = groupId;
			}
			if (includeTog != null)
			{
				pickIncludePart();
			}
		}
		else
		{
			colors.normalColor = normal;
		}
		toggle.colors = colors;
	}

	public bool pickIncludePart()
	{
		GameObject gameObject = base.gameObject;
		if (togGroupId == 4)
		{
			if (IncludePartWith.inst.windowPart == gameObject.name)
			{
				switchToPart();
				return false;
			}
			IncludePartWith.inst.windowPart = gameObject.name;
			IncludePartWith.inst.rectImages[0].transform.position = gameObject.transform.position;
		}
		else if (togGroupId == 5)
		{
			if (IncludePartWith.inst.doorPart == gameObject.name)
			{
				switchToPart();
				return false;
			}
			IncludePartWith.inst.doorPart = gameObject.name;
			IncludePartWith.inst.rectImages[1].transform.position = gameObject.transform.position;
		}
		else if (togGroupId == 8)
		{
			if (IncludePartWith.inst.wallFramePart == gameObject.name)
			{
				switchToPart();
				return false;
			}
			IncludePartWith.inst.wallFramePart = gameObject.name;
			IncludePartWith.inst.rectImages[2].transform.position = gameObject.transform.position;
		}
		else if (togGroupId == 9)
		{
			if (IncludePartWith.inst.floorFramePart == gameObject.name)
			{
				switchToPart();
				return false;
			}
			IncludePartWith.inst.floorFramePart = gameObject.name;
			IncludePartWith.inst.rectImages[3].transform.position = gameObject.transform.position;
		}
		else if (togGroupId == 0)
		{
			if (IncludePartWith.inst.floorFrameTriPart == gameObject.name)
			{
				switchToPart();
				return false;
			}
			IncludePartWith.inst.floorFrameTriPart = gameObject.name;
			IncludePartWith.inst.rectImages[4].transform.position = gameObject.transform.position;
		}
		return true;
	}

	private void switchToPart()
	{
		includeTog.isOn = false;
		BuilderSystem.inst.ChangePart(base.name);
		toggle.isOn = true;
		ColorBlock colors = toggle.colors;
		colors.normalColor = pressed;
		toggle.colors = colors;
		togGroupId = groupId;
	}
}
