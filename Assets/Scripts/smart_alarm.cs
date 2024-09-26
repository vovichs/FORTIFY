public class smart_alarm : Device
{
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
				popUp.inst.message("Your base is under attack!");
				on = true;
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			on = false;
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}
}
