using UnityEngine;

public class boomBox : Device
{
	[Header("__________________________")]
	public AudioSource AS;

	public static AudioSource master;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		if (inputPower >= usage)
		{
			if (inputPower2 > 0 && value == 0)
			{
				value = 1;
				if (!master)
				{
					master = AS;
				}
				else
				{
					AS.time = master.time;
				}
				AS.Play();
			}
			else if (inputPower2 == 0 && value == 1)
			{
				value = 0;
				if (master == AS)
				{
					master = null;
				}
				if (AS.isPlaying)
				{
					AS.Stop();
				}
			}
		}
		else
		{
			value = 0;
			if (master == AS)
			{
				master = null;
			}
			if (AS.isPlaying)
			{
				AS.Stop();
			}
		}
		on = (value == 1);
		bool flag = false;
		if (on)
		{
			if (outputTo[0].power == 0 && outputTo[0].connectedTo != null)
			{
				flag = true;
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			if (outputTo[0].power > 0 && outputTo[0].connectedTo != null)
			{
				flag = true;
			}
			outputTo[0].power = 0;
		}
		if (outputTo[0].connectedTo != null)
		{
			if (savedOutput != outputTo[0].power)
			{
				savedOutput = outputTo[0].power;
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(0, PID);
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			savedOutput = -1;
			flag = true;
		}
		if (flag)
		{
			sendUsage(-1, 0, 0);
		}
	}

	public override void sendUsage(int circuitUsage, int ioIndex, int _UID)
	{
		if (_UID == 0)
		{
			wiring.inst.newUID++;
			UID = wiring.inst.newUID;
		}
		else
		{
			if (_UID == UID)
			{
				return;
			}
			UID = _UID;
		}
		int num = 0;
		if (value == 0)
		{
			num = 0;
			savedUsage = 0;
		}
		else
		{
			if (outputTo.Length != 0 && circuitUsage != -1)
			{
				if (outputTo[0].connectedTo == null)
				{
					savedUsage = 0;
				}
				else
				{
					savedUsage = circuitUsage;
				}
			}
			num = usage + savedUsage;
		}
		for (int i = 0; i < inputFrom.Length; i++)
		{
			if (i != 1)
			{
				io io = inputFrom[i];
				if (!io.noUsage && io.type == 0 && io.connectedTo != null)
				{
					io.connectedTo.dev.sendUsage(num, io.connectedTo.index, UID);
				}
			}
		}
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, val);
		}
		if (PID > 0 && inputFrom[0].power > 0)
		{
			newPowerThru();
			StartCoroutine(playSound(GetComponent<AudioSource>()));
		}
	}
}
