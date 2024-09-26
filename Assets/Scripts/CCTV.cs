using UnityEngine;

public class CCTV : Device
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
		if (getInputPower(0) >= usage)
		{
			if (!on)
			{
				on = true;
				if ((bool)_light)
				{
					_light.sharedMaterial = wiring.inst.lightRed;
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
