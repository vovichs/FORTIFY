using UnityEngine;

public class conveyor : Device
{
	[Header("__________________________")]
	public Renderer _light;

	private bool inputState;

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
		int inputPower2 = getInputPower(2);
		int inputPower3 = getInputPower(3);
		if (inputPower >= usage)
		{
			if (inputPower2 > 0)
			{
				inputState = true;
			}
			if (inputPower3 > 0)
			{
				inputState = false;
			}
		}
		setState();
		for (int i = 1; i < 4; i++)
		{
			if (!(outputTo[i].connectedTo != null))
			{
				continue;
			}
			if (i == 1)
			{
				if (inputPower > 0 && inputPower >= usage + 1)
				{
					outputTo[i].power = inputPower - usage;
				}
				else
				{
					outputTo[i].power = 0;
				}
			}
			else if (modes[i - 2] && valueToBool() && inputPower >= usage + 1)
			{
				outputTo[i].power = 1;
			}
			else
			{
				outputTo[i].power = 0;
			}
			outputPower(i, PID);
		}
		if (circuitEndCheck())
		{
			sendUsage(-1, 0, 0);
		}
	}

	private void setState()
	{
		bool flag = valueToBool();
		if (inputFrom[1].power == 0)
		{
			if (flag)
			{
				flag = false;
				_light.sharedMaterial = wiring.inst.lightOff;
				on = false;
			}
		}
		else
		{
			if (!inputState && flag)
			{
				flag = false;
			}
			if (inputState && !flag)
			{
				flag = true;
			}
			if (flag)
			{
				on = true;
				_light.sharedMaterial = wiring.inst.lightGreen;
			}
			else
			{
				on = false;
				_light.sharedMaterial = wiring.inst.lightRed;
			}
		}
		value = (flag ? 1 : 0);
	}

	private bool valueToBool()
	{
		if (value == 0)
		{
			return false;
		}
		return true;
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, val);
		}
		inputState = (value == 1);
		if (PID > 0 && inputFrom[1].power > 0)
		{
			newPowerThru();
		}
	}
}
