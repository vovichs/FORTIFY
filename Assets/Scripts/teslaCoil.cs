using UnityEngine;

public class teslaCoil : Device
{
	[Header("__________________________")]
	public ParticleSystem ps;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower >= 1)
		{
			usage = Mathf.Min(35, inputPower);
			if (!on)
			{
				ps.Play();
				on = true;
			}
		}
		else if (on)
		{
			ps.Stop();
			on = false;
		}
		sendUsage();
	}

	private void OnEnable()
	{
		if (on)
		{
			ps.Play();
		}
	}

	public override void setValue(int val, bool send)
	{
	}

	public override void deviceHideState(bool hide)
	{
		
	}
}
