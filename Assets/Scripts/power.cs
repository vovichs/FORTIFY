public class power : Device
{
	public int activeUsage;

	private void OnEnable()
	{
		for (int i = 0; i < outputTo.Length; i++)
		{
			outputTo[i].power = power;
		}
	}

	public override void powerThru(int _PID)
	{
		PID = _PID;
		power = value;
		for (int i = 0; i < outputTo.Length; i++)
		{
			outputTo[i].power = value;
			if (i > 0)
			{
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
			}
			if (outputTo[i].connectedTo != null)
			{
				outputPower(i, PID);
			}
		}
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		power = value;
		newPowerThru();
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, val);
		}
	}
}
