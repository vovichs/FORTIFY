using UnityEngine;

public class crafter : Device
{
	[Header("__________________________")]
	public Renderer _light;

	private bool state;

	private bool inputTogState;

	public bool[] modes = new bool[2]
	{
		true,
		false
	};

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(1);
		int inputPower2 = getInputPower(5);
		int inputPower3 = getInputPower(4);
		int inputPower4 = getInputPower(2);
		if (inputPower >= 0)
		{
			if (inputPower2 > 0)
			{
				inputTogState = false;
			}
			if (inputPower3 > 0)
			{
				inputTogState = true;
			}
			if ((bool)inputFrom[2].connectedTo)
			{
				inputTogState = (inputPower4 > 0);
			}
		}
		setState(inputTogState);
		if (circuitEndCheck())
		{
			sendUsage(-1, 0, 0);
		}
	}

	private void setState(bool inputTogState)
	{
		if (inputFrom[1].power == 0)
		{
			inputTogState = false;
		}
		if (!inputTogState && state)
		{
			state = false;
			on = false;
			_light.sharedMaterial = wiring.inst.lightOff;
		}
		if (inputTogState && !state)
		{
			state = true;
			on = true;
			_light.sharedMaterial = wiring.inst.lightGreen;
		}
		value = (state ? 1 : 0);
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(GetComponent<BuilderPart>(), val);
		}
		if (value == 1)
		{
			inputTogState = true;
		}
		else
		{
			inputTogState = false;
		}
		if (PID > 0 && inputFrom[1].power > 0)
		{
			newPowerThru();
		}
	}
}
