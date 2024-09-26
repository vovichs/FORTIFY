using UnityEngine;

public class fluidPump : Device
{
	[Header("__________________________")]
	public Renderer _light;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (getInputPower(2) >= usage)
		{
			if (!on)
			{
				on = true;
				if ((bool)_light)
				{
					_light.sharedMaterial = wiring.inst.lightGreen;
				}
			}
		}
		else if (on)
		{
			on = false;
			if ((bool)_light)
			{
				_light.sharedMaterial = wiring.inst.lightOff;
			}
		}
		sendUsage();
	}

	public override void setValue(int val, bool send)
	{
	}
}
