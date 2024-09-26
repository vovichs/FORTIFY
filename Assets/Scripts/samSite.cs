using UnityEngine;

public class samSite : Device
{
	[Header("__________________________")]
	public Renderer _light;

	public bool[] modes = new bool[3];

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		getInputPower(1);
		if (inputPower >= usage)
		{
			if (!on)
			{
				on = true;
				_light.sharedMaterial = wiring.inst.lightGreen;
			}
		}
		else if (on)
		{
			on = false;
			_light.sharedMaterial = wiring.inst.lightOff;
		}
		for (int i = 0; i < outputTo.Length; i++)
		{
			if (outputTo[i].connectedTo == null)
			{
				continue;
			}
			if (i == 3)
			{
				if (on && inputPower >= usage + 1)
				{
					outputTo[i].power = inputPower - usage;
				}
				else
				{
					outputTo[i].power = 0;
				}
			}
			else
			{
				if (modes[i] && on && inputPower >= usage + 1)
				{
					outputTo[i].power = 1;
				}
				else
				{
					outputTo[i].power = 0;
				}
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
			}
			outputPower(i, PID);
		}
		if (circuitEndCheck())
		{
			sendUsage(-1, 0, 0);
		}
	}
}
