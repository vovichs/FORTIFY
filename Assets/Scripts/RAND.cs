using UnityEngine;

public class RAND : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private bool set;

	private bool reset;

	private bool powered;

	private int loopCount;

	public override void powerThru(int _PID)
	{
		int num = 0;
		if (PID == _PID)
		{
			loopCount++;
			if (loopCount > 1)
			{
				loopCount = 0;
				return;
			}
		}
		else
		{
			PID = _PID;
		}
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		int inputPower3 = getInputPower(2);
		if (inputPower == 0)
		{
			if (powered)
			{
				powered = false;
				lightsOff(lights);
				outputTo[0].power = 0;
			}
		}
		else
		{
			if (!powered)
			{
				powered = true;
				if (on)
				{
					lights[1].sharedMaterial = wiring.inst.lightGreen;
				}
				else
				{
					lights[1].sharedMaterial = wiring.inst.lightRed;
				}
			}
			else
			{
				if (inputPower2 > 0)
				{
					if (!set)
					{
						num = Random.Range(1, 3);
						set = true;
					}
				}
				else
				{
					set = false;
				}
				if (inputPower3 > 0)
				{
					if (!reset)
					{
						num = 2;
						reset = true;
					}
				}
				else
				{
					reset = false;
				}
			}
			switch (num)
			{
			case 1:
				if (!on)
				{
					on = true;
					lights[1].sharedMaterial = wiring.inst.lightGreen;
				}
				break;
			case 2:
				if (on)
				{
					on = false;
					lights[1].sharedMaterial = wiring.inst.lightRed;
				}
				break;
			}
		}
		if (on && inputPower > usage)
		{
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}
}
