using UnityEngine;

public class speaker : Device
{
	public bool wireAudio;

	public AudioSource AS;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputFrom[0].connectedTo != null)
		{
			Device dev = inputFrom[0].connectedTo.dev;
			if (dev is boomBox)
			{
				wireAudio = true;
			}
			else if (dev is speaker && (dev as speaker).wireAudio)
			{
				wireAudio = true;
			}
			else
			{
				wireAudio = false;
			}
		}
		else
		{
			wireAudio = false;
		}
		if (inputPower >= usage)
		{
			on = true;
			outputTo[0].power = inputFrom[0].power - usage;
			if (wireAudio && !AS.isPlaying && (bool)boomBox.master)
			{
				AS.time = boomBox.master.time;
				AS.Play();
			}
		}
		else
		{
			on = false;
			outputTo[0].power = 0;
			if (AS.isPlaying)
			{
				AS.Stop();
			}
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}
}
