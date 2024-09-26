public class basic_passthru : Device
{
	public override void powerThru(int _PID)
	{
		if (PID != _PID)
		{
			PID = _PID;
			if (getInputPower(0) >= usage)
			{
				on = true;
				outputTo[0].power = inputFrom[0].power - usage;
			}
			else
			{
				on = false;
				outputTo[0].power = 0;
			}
			standardOutput(0);
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
