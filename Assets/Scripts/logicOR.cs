using UnityEngine;

public class logicOR : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		if (inputPower == 0 && inputPower2 == 0)
		{
			lightsOff(lights);
			power = 0;
		}
		else if (inputPower > 0 || inputPower2 > 0)
		{
			if (inputPower > 0)
			{
				lights[0].sharedMaterial = wiring.inst.lightGreen;
			}
			else
			{
				lights[0].sharedMaterial = wiring.inst.lightRed;
			}
			if (inputPower2 > 0)
			{
				lights[1].sharedMaterial = wiring.inst.lightGreen;
			}
			else
			{
				lights[1].sharedMaterial = wiring.inst.lightRed;
			}
			if (inputPower2 > inputPower)
			{
				power = inputPower2;
				usageInputIndex = 1;
			}
			else
			{
				power = inputPower;
				usageInputIndex = 0;
			}
		}
		else
		{
			power = 0;
		}
		outputTo[0].power = power;
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}
}
