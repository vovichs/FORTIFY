using UnityEngine;

public class alarm : Device
{
	[Header("__________________________")]
	public AudioSource AS;

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
				AS.Play();
				on = true;
			}
		}
		else if (on)
		{
			AS.Stop();
			on = false;
		}
		sendUsage();
	}

	public override void setValue(int val, bool send)
	{
	}
}
