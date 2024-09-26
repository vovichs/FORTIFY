using UnityEngine;

public class basic_matChange : Device
{
	[Header("__________________________")]
	public Material matOn;

	public Material matOff;

	public BuilderPart bp;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower >= usage)
		{
			if (!on)
			{
				on = true;
				bp.rend[0].sharedMaterial = matOn;
				bp.deploy.mat = matOn;
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			if (on)
			{
				on = false;
				bp.rend[0].sharedMaterial = matOff;
				bp.deploy.mat = matOff;
			}
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}
}
