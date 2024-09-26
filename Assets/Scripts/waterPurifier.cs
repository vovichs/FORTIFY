using UnityEngine;

public class waterPurifier : Device
{
	[Header("__________________________")]
	public bool poweredType;

	public override void powerThru(int _PID)
	{
		if (poweredType && PID != _PID)
		{
			PID = _PID;
			int inputPower = getInputPower(1);
			on = (inputPower >= usage);
			sendUsage();
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
