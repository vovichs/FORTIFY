using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class devicePanel : MonoBehaviour
{
	public static devicePanel inst;

	private Vector2 offset = new Vector2(40f, 40f);

	public TextMeshProUGUI text;

	public GameObject panel;

	public InputField[] inputs;

	public GameObject[] devicePanels;

	public Device dev;

	public setScreenPos panelIcon;

	public Text[] batteryText;

	public Slider batterySlider;

	public Text timerTime;

	public Text fuelTime;

	public Slider fuelSlider;

	public RectTransform canvas;

	public RectTransform deviceParent;

	public TMP_Dropdown weaponsDrop;

	private void Awake()
	{
		inst = this;
	}

	public void moveToMouse()
	{
		base.transform.position = UnityEngine.Input.mousePosition;
	}

	public void openPanel(int index, Device _dev)
	{
		if (dev == _dev)
		{
			return;
		}
		if (panel != null)
		{
			closePanel();
		}
		dev = _dev;
		ioTrigger.inst.ioTriggerCol.position = Vector3.zero;
		wiring.inst.enabled = false;
		wiring.inst.devPanel.text.enabled = false;
		panel = devicePanels[index];
		panel.SetActive(value: true);
		placeOver(deviceParent);
		dev.panelOpen = true;
		switch (index)
		{
		case 1:
			inputs[index].text = (dev as timerDevice).time.ToString();
			break;
		case 2:
			counterPanel();
			break;
		case 3:
			inputs[index].text = dev.value.ToString();
			break;
		case 4:
			inputs[index].text = dev.value.ToString();
			break;
		case 5:
			doorCtrlPanel();
			break;
		case 6:
			StartCoroutine((dev as battery).showTime());
			break;
		case 7:
			StartCoroutine((dev as fuelGenerator).panelUpdate());
			break;
		case 8:
			if (dev is autoTurret)
			{
				weaponsDrop.gameObject.SetActive(value: true);
				if (weaponsDrop.options.Count == 1)
				{
					fillWeaponsDrop();
				}
				setWeaponsDropValue();
			}
			else
			{
				weaponsDrop.gameObject.SetActive(value: false);
			}
			sentryPanel();
			break;
		case 9:
			inputs[index].text = dev.value.ToString();
			break;
		case 10:
			conveyorPanel();
			break;
		case 11:
			onOffToggle(get: true);
			break;
		case 12:
			inputs[index].text = dev.value.ToString();
			break;
		}
	}

	public void placeOver(RectTransform rt)
	{
		RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, UnityEngine.Input.mousePosition, null, out Vector2 localPoint);
		rt.localPosition = localPoint;
		Vector2 vector = canvas.rect.min - rt.rect.min;
		Vector2 vector2 = canvas.rect.max - rt.rect.max;
		localPoint.x = Mathf.Clamp(rt.localPosition.x, vector.x, vector2.x);
		localPoint.y = Mathf.Clamp(rt.localPosition.y, vector.y, vector2.y);
		rt.localPosition = localPoint;
	}

	public void setDeviceValue(int index)
	{
		if (dev == null)
		{
			closePanel();
			return;
		}
		if (dev is timerDevice)
		{
			timerSetTime();
		}
		else
		{
			int num = int.Parse(inputs[index].text);
			if (dev is power)
			{
				if (num < 0)
				{
					num = 0;
					inputs[index].text = num.ToString();
				}
			}
			else if (dev is branch)
			{
				if (num < 1)
				{
					num = 1;
					inputs[index].text = num.ToString();
				}
			}
			else if (dev is RF_broadcaster || dev is RF_receiver)
			{
				if (num < 1)
				{
					num = 1;
					inputs[index].text = num.ToString();
				}
			}
			else if (dev is counter)
			{
				if (num > 999)
				{
					num = 999;
					inputs[index].text = num.ToString();
				}
			}
			else if (dev is seismic)
			{
				if (num < 1)
				{
					num = 1;
				}
				else if (num > 30)
				{
					num = 30;
				}
				inputs[index].text = num.ToString();
			}
			dev.setValue(num, send: true);
		}
		closePanel();
	}

	public void closePanel()
	{
		if (dev != null)
		{
			dev.panelOpen = false;
		}
		dev.panelOpen = false;
		panel.SetActive(value: false);
		panel = null;
		dev = null;
		ioTrigger.inst.ioTriggerCol.position = Vector3.zero;
		wiring.inst.enabled = true;
	}

	public void branchAutoAmount()
	{
		inputs[3].text = (dev as branch).branchNeeds.ToString();
	}

	public void timerActivate()
	{
		if ((bool)dev)
		{
			timerSetTime();
			(dev as timerDevice).runTimer(send: true);
			closePanel();
		}
	}

	public void timerSetTime()
	{
		float num = float.Parse(inputs[1].text);
		if (num < 0.25f)
		{
			num = 0.25f;
			inputs[1].text = "0.25";
			popUp.inst.message("0.25 minimum");
		}
		dev.setValue(num, send: true);
	}

	public void timerStop()
	{
		(dev as timerDevice).stop();
	}

	public void counterMode()
	{
		if (dev == null)
		{
			closePanel();
			return;
		}
		if (dev.value == -1)
		{
			if (inputs[2].text.Length > 0)
			{
				dev.setValue(int.Parse(inputs[2].text), send: true);
			}
			else
			{
				dev.setValue(10, send: true);
			}
		}
		else
		{
			dev.setValue(-1, send: true);
		}
		counterPanel();
	}

	public void counterPanel()
	{
		Text component = panel.transform.GetChild(1).GetComponent<Text>();
		if (dev.value == -1)
		{
			component.text = "disable passthrough mode";
			inputs[2].interactable = false;
			inputs[2].text = "";
		}
		else
		{
			component.text = "enable passthrough mode";
			inputs[2].interactable = true;
			inputs[2].text = dev.value.ToString();
		}
	}

	public void doorCtrlPanel()
	{
		Text component = panel.transform.GetChild(1).GetComponent<Text>();
		Button component2 = panel.transform.GetChild(0).GetComponent<Button>();
		if ((dev as doorController).doorBP != null)
		{
			component2.interactable = false;
			component.text = "connected";
		}
		else
		{
			component2.interactable = true;
			component.text = "connect";
		}
	}

	public void doorCtrlConnect()
	{
		if (dev == null && dev is doorController)
		{
			closePanel();
			return;
		}
		StartCoroutine((dev as doorController).connectToDoor(manual: true));
		closePanel();
	}

	public void generatorTogglePower()
	{
		if (dev == null)
		{
			closePanel();
		}
		else
		{
			(dev as fuelGenerator).powerToggle();
		}
	}

	public void generatorFuelLvl()
	{
		if (dev == null)
		{
			closePanel();
		}
		else
		{
			(dev as fuelGenerator).setFuel((int)fuelSlider.value);
		}
	}

	public void setBatteryCharge()
	{
		if (dev == null)
		{
			closePanel();
		}
		else
		{
			(dev as battery).setCharge();
		}
	}

	public void sentryPanel()
	{
		Transform transform = panel.transform;
		if (dev is autoTurret)
		{
			autoTurret autoTurret = dev as autoTurret;
			for (int i = 0; i < 3; i++)
			{
				transform.GetChild(i).GetComponent<Toggle>().SetIsOnWithoutNotify(autoTurret.modes[i]);
			}
		}
		else
		{
			samSite samSite = dev as samSite;
			for (int j = 0; j < 3; j++)
			{
				transform.GetChild(j).GetComponent<Toggle>().SetIsOnWithoutNotify(samSite.modes[j]);
			}
		}
	}

	public void sentryToggle(int index)
	{
		if (dev is autoTurret)
		{
			autoTurret autoTurret = dev as autoTurret;
			autoTurret.modes[index] = !autoTurret.modes[index];
			autoTurret.newPowerThru();
		}
		else
		{
			samSite samSite = dev as samSite;
			samSite.modes[index] = !samSite.modes[index];
			samSite.newPowerThru();
		}
	}

	public void conveyorPanel()
	{
		conveyor conveyor = dev as conveyor;
		for (int i = 0; i < 2; i++)
		{
			panel.transform.GetChild(i).GetComponent<Toggle>().SetIsOnWithoutNotify(conveyor.modes[i]);
		}
		panel.transform.GetChild(2).GetComponent<Toggle>().SetIsOnWithoutNotify((conveyor.value != 0) ? true : false);
	}

	public void conveyorToggle(int index)
	{
		conveyor conveyor = dev as conveyor;
		if (index == 2)
		{
			conveyor.setValue(conveyor.getSwapValue(), send: true);
			return;
		}
		conveyor.modes[index] = !conveyor.modes[index];
		int num = 0;
		if (index == 0)
		{
			num = 1;
		}
		conveyor.modes[num] = !conveyor.modes[num];
		panel.transform.GetChild(num).GetComponent<Toggle>().SetIsOnWithoutNotify(!conveyor.modes[index]);
		conveyor.newPowerThru();
	}

	public void onOffToggle(bool get)
	{
		if (get)
		{
			panel.transform.GetChild(0).GetComponent<Toggle>().SetIsOnWithoutNotify((dev.value != 0) ? true : false);
		}
		else
		{
			dev.setValue(dev.getSwapValue(), send: true);
		}
	}

	public void seismicDetect(int power)
	{
		(dev as seismic).setTriggered(power);
	}

	public void setSentryWeapon()
	{
		if (!(dev == null))
		{
			string weapon = weaponsDrop.options[weaponsDrop.value].text;
			int id = RustCopyPaste.inst.weaponsList.Single((RustCopyPaste.weapon s) => s.name == weapon).id;
			dev.setValue(id, send: true);
		}
	}

	private void setWeaponsDropValue()
	{
		if (dev == null)
		{
			return;
		}
		string weaponNameFromID = RustCopyPaste.inst.GetWeaponNameFromID(dev.value);
		int num = 0;
		while (true)
		{
			if (num < weaponsDrop.options.Count)
			{
				if (weaponsDrop.options[num].text == weaponNameFromID)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		weaponsDrop.SetValueWithoutNotify(num);
	}

	private void fillWeaponsDrop()
	{
		weaponsDrop.options.Clear();
		foreach (RustCopyPaste.weapon weapons in RustCopyPaste.inst.weaponsList)
		{
			weaponsDrop.options.Add(new TMP_Dropdown.OptionData
			{
				text = weapons.name
			});
		}
	}
}
