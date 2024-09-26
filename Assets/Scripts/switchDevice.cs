using UnityEngine;

public class switchDevice : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private bool toggleOn;

	private bool toggleOff;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		int inputPower3 = getInputPower(2);
		if (inputPower > 0)
		{
			if (inputPower2 > 0)
			{
				if (!toggleOn)
				{
					toggleOn = true;
					value = 1;
				}
			}
			else
			{
				toggleOn = false;
			}
			if (inputPower3 > 0)
			{
				if (!toggleOff)
				{
					toggleOff = true;
					value = 0;
				}
			}
			else
			{
				toggleOff = false;
			}
		}
		bool flag = false;
		if (inputPower > 0 && value == 1)
		{
			on = true;
			if (outputTo[0].power == 0 && outputTo[0].connectedTo != null)
			{
				flag = true;
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			on = false;
			if (outputTo[0].power > 0 && outputTo[0].connectedTo != null)
			{
				flag = true;
			}
			outputTo[0].power = 0;
		}
		setLights();
		if ((bool)outputTo[0].connectedTo)
		{
			if (savedOutput != outputTo[0].power)
			{
				savedOutput = outputTo[0].power;
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(0, PID);
				flag = false;
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			savedOutput = -1;
			flag = true;
		}
		if (flag)
		{
			sendUsage(-1, 0, 0);
		}
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, val);
		}
		if (PID > 0 && inputFrom[0].power > 0)
		{
			newPowerThru();
			StartCoroutine(playSound(GetComponent<AudioSource>()));
		}
	}

	private void setLights()
	{
		if (inputFrom[0].power == 0)
		{
			lightsOff(lights);
		}
		else if (value == 0)
		{
			lights[0].sharedMaterial = wiring.inst.lightRed;
			lights[1].sharedMaterial = wiring.inst.lightOff;
		}
		else
		{
			lights[0].sharedMaterial = wiring.inst.lightOff;
			lights[1].sharedMaterial = wiring.inst.lightGreen;
		}
	}
}
