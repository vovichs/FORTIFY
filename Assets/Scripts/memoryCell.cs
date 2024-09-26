using UnityEngine;

public class memoryCell : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private bool toggle;

	private bool powered;

	private int savedOutput0 = -1;

	private int savedOutput1 = -1;

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
		int inputPower4 = getInputPower(3);
		if (inputPower == 0)
		{
			if (powered)
			{
				powered = false;
				lightsOff(lights);
				outputTo[0].power = 0;
				outputTo[1].power = 0;
			}
		}
		else if (!powered)
		{
			powered = true;
			lights[1].sharedMaterial = wiring.inst.lightGreen;
		}
		if (inputPower4 > 0)
		{
			if (!toggle)
			{
				toggle = true;
				if (value == 0)
				{
					value = 1;
				}
				else
				{
					value = 0;
				}
			}
		}
		else
		{
			toggle = false;
		}
		if (inputPower3 > 0 && value == 1)
		{
			value = 0;
		}
		if (inputPower2 > 0 && value == 0)
		{
			value = 1;
		}
		if (powered)
		{
			power = inputPower - usage;
			if (value == 1)
			{
				if (outputTo[0].power == 0)
				{
					lights[0].sharedMaterial = wiring.inst.lightGreen;
				}
				outputTo[0].power = power;
				outputTo[1].power = 0;
			}
			else
			{
				if (outputTo[1].power == 0)
				{
					lights[0].sharedMaterial = wiring.inst.lightRed;
				}
				outputTo[1].power = power;
				outputTo[0].power = 0;
			}
		}
		bool flag = false;
		if (outputTo[0].connectedTo != null)
		{
			if (savedOutput0 != outputTo[0].power)
			{
				savedOutput0 = outputTo[0].power;
				outputPower(0, PID);
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			savedOutput0 = -1;
		}
		if (outputTo[1].connectedTo != null)
		{
			if (savedOutput1 != outputTo[1].power)
			{
				savedOutput1 = outputTo[1].power;
				outputPower(1, PID);
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			savedOutput1 = -1;
		}
		if (outputTo[0].connectedTo == null && outputTo[1].connectedTo == null)
		{
			flag = true;
		}
		if (flag)
		{
			sendUsage(-1, 0, 0);
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
