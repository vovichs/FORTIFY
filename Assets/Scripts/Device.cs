using System.Collections;
using UnityEngine;

public abstract class Device : MonoBehaviour
{
	public bool on;

	public int usage;

	[HideInInspector]
	public int[] outUsage;

	[HideInInspector]
	public int savedUsage;

	public int power;

	public int panel_id;

	public bool usesValue;

	public bool powerSource;

	public bool combine;

	public int value;

	public bool selfLoop;

	[HideInInspector]
	public int id;

	[HideInInspector]
	public int savedOutput = -1;

	[HideInInspector]
	public int usageInputIndex = -1;

	[HideInInspector]
	public int PID;

	[HideInInspector]
	public int UID;

	[Header("SET INDEX FOR EACH IO")]
	public GameObject[] hideObjs;

	public io[] inputFrom;

	public io[] outputTo;

	[HideInInspector]
	public BuilderPart owner;

	[HideInInspector]
	public bool mouseOver;

	[HideInInspector]
	public bool panelOpen;

	public bool showInfoAlways;

	public static bool disabled;

	public static bool noUsage;

	private void OnMouseEnter()
	{
		if ((!wiring.inst.on && !showInfoAlways) || BuilderUI.inst.mouseOverUI || Vector3.Distance(owner._transform.position, CameraCtrl.inst.tf.position) > 50f)
		{
			return;
		}
		mouseOver = true;
		devicePanel devPanel = wiring.inst.devPanel;
		if (devPanel.panel == null)
		{
			Vector3 center = owner.col.bounds.center;
			devPanel.transform.position = CameraCtrl.inst.cam.WorldToScreenPoint(center);
			devPanel.text.enabled = true;
			devPanel.text.text = setMouseOverText();
			StartCoroutine(updateMouseOverText());
			if (panel_id > 0 && wiring.inst.on)
			{
				devPanel.panelIcon.enabled = true;
			}
		}
	}

	private void OnMouseUp()
	{
		if (BuilderSystem.editMode || wiring.inst.io1Type != wiring.connected.none || BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		if (this is timerButton)
		{
			(this as timerButton).runTimer(send: true);
		}
		else if (this is digitalClock)
		{
			(this as digitalClock).runTimer(send: true);
		}
		else if (this is switchDevice)
		{
			setValue(getSwapValue(), send: true);
		}
		else if (wiring.inst.on)
		{
			if (panel_id > 0)
			{
				wiring.inst.devPanel.openPanel(panel_id, this);
			}
		}
		else if (this is seismic)
		{
			(this as seismic).setTriggered(-1);
		}
		else if (this is timerDevice)
		{
			(this as timerDevice).runTimer(send: true);
		}
		else if (this is triggerDevice)
		{
			triggerDevice triggerDevice = this as triggerDevice;
			if ((bool)triggerDevice.laser)
			{
				triggerDevice.laserRaycast(set: true);
			}
		}
	}

	private void OnMouseExit()
	{
		if (mouseOver)
		{
			wiring.inst.devPanel.panelIcon.enabled = false;
		}
		wiring.inst.devPanel.text.enabled = false;
		mouseOver = false;
	}

	public abstract void powerThru(int _PID);

	public void newPowerThru()
	{
		if (!disabled)
		{
			savedOutput = -2;
			wiring.inst.newPID++;
			powerThru(wiring.inst.newPID);
		}
	}

	public void sendUsage()
	{
		sendUsage(0, 0, 0);
	}

	public virtual void sendUsage(int circuitUsage, int ioIndex, int _UID)
	{
		if (noUsage)
		{
			return;
		}
		if (_UID == 0)
		{
			wiring.inst.newUID++;
			UID = wiring.inst.newUID;
		}
		else
		{
			if (_UID == UID)
			{
				return;
			}
			UID = _UID;
		}
		if (this is battery)
		{
			(this as battery).setActiveUsage(circuitUsage);
		}
		int num = 0;
		if (this is branch || this is splitter || this is memoryCell || this is autoTurret || this is samSite || this is power)
		{
			if (circuitUsage != -1)
			{
				if (outputTo[ioIndex].connectedTo == null)
				{
					outUsage[ioIndex] = 0;
				}
				else
				{
					outUsage[ioIndex] = circuitUsage;
				}
			}
			num = (on ? usage : 0);
			for (int i = 0; i < outUsage.Length; i++)
			{
				num += outUsage[i];
			}
			if (this is power)
			{
				(this as power).activeUsage = num;
			}
			else if (this is branch)
			{
				(this as branch).branchNeeds = num;
			}
		}
		else if (this is RF_receiver)
		{
			num = (savedUsage = (on ? (usage + circuitUsage) : 0));
		}
		else if (this is timerDevice)
		{
			num = (savedUsage = ((!(this as timerDevice).timerOn) ? usage : (usage + circuitUsage)));
		}
		else
		{
			if (outputTo.Length != 0 && circuitUsage != -1)
			{
				if (outputTo[0].connectedTo == null)
				{
					savedUsage = 0;
				}
				else
				{
					savedUsage = circuitUsage;
				}
			}
			num = (on ? usage : 0);
			num += savedUsage;
		}
		for (int j = 0; j < inputFrom.Length; j++)
		{
			io io = inputFrom[j];
			if (io.type == 0 && io.connectedTo != null && !io.noUsage)
			{
				if (usageInputIndex > -1 && j != usageInputIndex)
				{
					io.connectedTo.dev.sendUsage(0, io.connectedTo.index, UID);
				}
				else
				{
					io.connectedTo.dev.sendUsage(num, io.connectedTo.index, UID);
				}
			}
		}
	}

	public void standardOutput(int i)
	{
		if (outputTo[i].connectedTo != null)
		{
			if (savedOutput != outputTo[i].power)
			{
				savedOutput = outputTo[i].power;
				outputPower(i, PID);
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
		}
		else
		{
			savedOutput = -1;
			sendUsage(-1, 0, 0);
		}
	}

	public bool circuitEndCheck()
	{
		for (int i = 0; i < outputTo.Length; i++)
		{
			if (outputTo[i].type == 0 && outputTo[i].connectedTo != null)
			{
				return false;
			}
		}
		return true;
	}

	public void outputPower(int ind, int _PID)
	{
		outputTo[ind].connectedTo.dev.powerThru(_PID);
		if (wiring.inst.labelsOn)
		{
			outputTo[ind].wire.updateLabel(outputTo[ind].power);
		}
	}

	[ContextMenu("getUsage")]
	public void getUsage()
	{
		Device[] array = new Device[3]
		{
			this,
			null,
			null
		};
		Device[] array2 = new Device[3];
		int num = 0;
		bool flag = true;
		while (flag)
		{
			Device[] array3 = array;
			foreach (Device device in array3)
			{
				flag = false;
				if (device == null)
				{
					continue;
				}
				for (int j = 0; j < device.outputTo.Length; j++)
				{
					if (device.outputTo[j] != null && device.outputTo[j].connectedTo != null && device.outputTo[j].connectedTo.power > 0)
					{
						array2[j] = device.outputTo[j].connectedTo.dev;
						num += device.usage;
						flag = true;
					}
				}
			}
			array = array2;
		}
		MonoBehaviour.print(num);
	}

	public virtual void setValue(int val, bool send)
	{
	}

	public virtual void setValue(float val, bool send)
	{
	}

	public IEnumerator disconnectAll()
	{
		if (this is elevator)
		{
			MGMT.inst.StartCoroutine(MGMT.inst.elevatorStackChange(this as elevator));
		}
		yield return null;
		if (mouseOver)
		{
			wiring.inst.devPanel.text.enabled = false;
			wiring.inst.devPanel.panelIcon.enabled = false;
		}
		for (int i = 0; i < inputFrom.Length; i++)
		{
			inputFrom[i].sendDisconnect(destroyed: true);
		}
		for (int j = 0; j < outputTo.Length; j++)
		{
			if (outputTo[j].wire != null)
			{
				UnityEngine.Object.Destroy(outputTo[j].wire.gameObject);
			}
			outputTo[j].sendDisconnect(destroyed: true);
		}
	}

	public int getInputPower(int index)
	{
		io io = inputFrom[index];
		if (io.connectedTo != null && io.connectedTo.power > 0)
		{
			io.power = io.connectedTo.power;
		}
		else
		{
			io.power = 0;
		}
		return io.power;
	}

	public void showIOs(bool placing)
	{
		int layer = 18;
		if (placing)
		{
			layer = 2;
		}
		int ioMode = wiring.inst.ioMode;
		io[] array;
		if (wiring.inst.io1Type != 0)
		{
			array = inputFrom;
			foreach (io io in array)
			{
				if (placing || io.type == ioMode)
				{
					io.gameObject.SetActive(value: true);
					io.gameObject.layer = layer;
				}
			}
		}
		if (wiring.inst.io1Type == wiring.connected.output)
		{
			return;
		}
		array = outputTo;
		foreach (io io2 in array)
		{
			if (placing || io2.type == ioMode)
			{
				io2.gameObject.SetActive(value: true);
				io2.gameObject.layer = layer;
			}
		}
	}

	public void hideIOs(bool listRemove)
	{
		if (listRemove)
		{
			ioTrigger.inst.displaying.Remove(this);
		}
		io[] array = inputFrom;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
		array = outputTo;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.SetActive(value: false);
		}
	}

	public IEnumerator WaitToHideIOs()
	{
		yield return new WaitForSeconds(1f);
		if (!ioTrigger.inst.displaying.Contains(this))
		{
			hideIOs(listRemove: true);
		}
	}

	public virtual void lightsOff(Renderer[] lights)
	{
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].sharedMaterial = wiring.inst.lightOff;
		}
	}

	public virtual void deviceHideState(bool hide)
	{
		GameObject[] array = hideObjs;
		foreach (GameObject gameObject in array)
		{
			if (gameObject != null)
			{
				gameObject.SetActive(hide);
			}
		}
	}

	public void clearConnectedTo()
	{
		savedOutput = -1;
		io[] array = inputFrom;
		foreach (io obj in array)
		{
			obj.connectedTo = null;
			obj.power = 0;
			obj.wire = null;
		}
		array = outputTo;
		foreach (io obj2 in array)
		{
			obj2.connectedTo = null;
			obj2.power = 0;
			obj2.wire = null;
		}
		if (this is doorController)
		{
			(this as doorController).doorBP = null;
		}
		newPowerThru();
	}

	private IEnumerator waitNewPowerThru()
	{
		yield return null;
		newPowerThru();
	}

	private IEnumerator updateMouseOverText()
	{
		devicePanel dp = wiring.inst.devPanel;
		while (mouseOver && dp.panel == null)
		{
			yield return new WaitForSeconds(1f);
			if (mouseOver)
			{
				dp.text.text = setMouseOverText();
			}
		}
	}

	private string setMouseOverText()
	{
		string text = owner.info.named;
		if (this is timerDevice)
		{
			timerDevice timerDevice = this as timerDevice;
			text = "timer " + timerDevice.time.ToString() + "s\n";
			if (timerDevice.timerOn)
			{
				text = text + timerDevice.t.ToString() + "s";
			}
		}
		else if (this is battery)
		{
			text = text + "\n usage: " + (this as battery).activeUsage.ToString();
		}
		else if (this is autoTurret)
		{
			autoTurret autoTurret = this as autoTurret;
			text = text + "\n" + autoTurret.turretsInRange.Count + " in range";
			if (on)
			{
				text = text + " - " + autoTurret.interfCount.ToString() + " interference";
			}
			text = text + "\n" + autoTurret.itemName;
		}
		else if (this is power)
		{
			text = text + "\n usage " + outUsage[0] + ", " + outUsage[1] + ", " + outUsage[2];
		}
		return text;
	}

	public IEnumerator playSound(AudioSource AS)
	{
		if ((bool)AS)
		{
			AS.enabled = true;
			AS.Play();
			yield return new WaitForSeconds(AS.clip.length);
			AS.enabled = false;
		}
	}

	public int getSwapValue()
	{
		if (value == 0)
		{
			return 1;
		}
		return 0;
	}

	public bool isElectric()
	{
		io[] array = inputFrom;
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].type == 0)
			{
				return true;
			}
		}
		return false;
	}
}
