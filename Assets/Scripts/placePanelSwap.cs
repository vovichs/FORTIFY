using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class placePanelSwap : MonoBehaviour
{
	public static placePanelSwap inst;

	public Toggle pickTog;

	public int mode;

	public GameObject buildCanvas;

	public GameObject placeCanvas;

	public GameObject placeList;

	public GameObject[] panels;

	public GameObject goBack;

	public Texture2D pickCursor;

	private void Awake()
	{
		inst = this;
	}

	public void modeSwap()
	{
		BuilderUI.inst.swapActiveSelf(buildCanvas);
		BuilderUI.inst.swapActiveSelf(placeCanvas);
		if (BuilderSystem.placeMode)
		{
			pickTog.gameObject.SetActive(value: true);
			Toggle toggle = panels[mode].GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
			if ((bool)toggle)
			{
				BuilderSystem.inst.ChangePart(toggle.name);
			}
		}
		else
		{
			Toggle toggle2 = buildCanvas.GetComponent<ToggleGroup>().ActiveToggles().FirstOrDefault();
			BuilderSystem.inst.ChangePart(toggle2.name);
		}
	}

	public void partPickerToggle()
	{
		if (pickTog.isOn)
		{
			Cursor.SetCursor(pickCursor, new Vector2(0f, 64f), CursorMode.Auto);
		}
		else
		{
			Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
		}
	}

	public void partPicker(BuilderPart bp)
	{
		pickTog.isOn = false;
		int num = (!bp.deploy) ? 2 : ((bp.deploy.frameFloor || bp.deploy.frameWall || ((bool)bp.door && bp.door.type != door.Type.gate) || bp.deploy.windowPart) ? 2 : 0);
		if (num == 2)
		{
			if (BuilderSystem.placeMode)
			{
				BuilderUI.inst.modeToggle.isOn = !BuilderUI.inst.modeToggle.isOn;
			}
			Transform transform = buildCanvas.transform;
			int num2 = 0;
			Transform child;
			while (true)
			{
				if (num2 < transform.childCount)
				{
					child = transform.GetChild(num2);
					if (bp.name == child.name)
					{
						break;
					}
					num2++;
					continue;
				}
				return;
			}
			child.GetComponent<Toggle>().isOn = true;
			return;
		}
		if (!BuilderSystem.placeMode)
		{
			BuilderUI.inst.modeToggle.isOn = !BuilderUI.inst.modeToggle.isOn;
		}
		if (num != mode)
		{
			GameObject[] array = panels;
			foreach (GameObject obj in array)
			{
				BuilderUI.inst.swapActiveSelf(obj);
			}
			if (mode == 0)
			{
				goBack.SetActive(value: false);
			}
			else
			{
				goBack.SetActive(value: true);
			}
		}
		mode = num;
		if (mode != 0)
		{
			return;
		}
		Transform transform2 = placeList.transform;
		int num3 = 0;
		Transform child2;
		while (true)
		{
			if (num3 < transform2.childCount)
			{
				child2 = transform2.GetChild(num3);
				if (bp.name == child2.name)
				{
					break;
				}
				num3++;
				continue;
			}
			return;
		}
		child2.gameObject.SetActive(value: true);
		child2.GetComponent<Toggle>().isOn = true;
	}
}
