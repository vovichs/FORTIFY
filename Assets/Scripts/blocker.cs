using System.Collections;
using UnityEngine;

public class blocker : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private Coroutine waitCR;

	public bool blocked;

	private bool waitRun;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		if (inputPower > 0)
		{
			if (inputPower2 > 0)
			{
				blocked = true;
				outputTo[0].power = 0;
				lights[0].sharedMaterial = wiring.inst.lightRed;
			}
			else
			{
				blocked = false;
				lights[0].sharedMaterial = wiring.inst.lightGreen;
				outputTo[0].power = inputFrom[0].power;
			}
		}
		else
		{
			blocked = false;
			lightsOff(lights);
			outputTo[0].power = 0;
		}
		if (inputPower2 > 0)
		{
			usage = 0;
		}
		else
		{
			usage = 1;
		}
		if (outputTo[0].connectedTo != null)
		{
			if (savedOutput != outputTo[0].power)
			{
				savedOutput = outputTo[0].power;
				StartCoroutine(wait());
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
		}
		else
		{
			savedOutput = -1;
			sendUsage(-1, 0, 0);
		}
	}

	private IEnumerator wait()
	{
		waitRun = true;
		int power = outputTo[0].power;
		yield return new WaitForSeconds(0.1f);
		outputTo[0].power = power;
		wiring.inst.newPID++;
		PID = wiring.inst.newPID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
		waitRun = false;
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
		if (inputFrom[1].power > 0)
		{
			for (int i = 0; i < 2; i++)
			{
				if (inputFrom[i].connectedTo != null)
				{
					inputFrom[i].connectedTo.dev.sendUsage(i, inputFrom[i].connectedTo.index, UID);
				}
			}
			return;
		}
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
		for (int j = 0; j < inputFrom.Length; j++)
		{
			io io = inputFrom[j];
			if (!io.noUsage && io.type == 0 && io.connectedTo != null)
			{
				io.connectedTo.dev.sendUsage(num, io.connectedTo.index, UID);
			}
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
