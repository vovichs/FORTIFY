using System.Collections;
using UnityEngine;

public class branch : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private int savedOutput0;

	private int savedOutput1;

	private bool powered;

	private bool wait;

	public int branchNeeds;

	public override void powerThru(int _PID)
	{
		if (PID == _PID || wait)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower > 0)
		{
			if (!powered)
			{
				lightsOff(lights);
				powered = true;
			}
			lights[0].sharedMaterial = wiring.inst.lightGreen;
			if (inputPower <= value)
			{
				outputTo[1].power = inputPower;
				outputTo[0].power = 0;
			}
			else
			{
				outputTo[1].power = value;
				outputTo[0].power = inputPower - value;
			}
		}
		else
		{
			if (powered)
			{
				lightsOff(lights);
				powered = false;
			}
			outputTo[0].power = 0;
			outputTo[1].power = 0;
		}
		bool flag = false;
		if (outputTo[0].connectedTo != null)
		{
			if (savedOutput0 != outputTo[0].power)
			{
				savedOutput0 = outputTo[0].power;
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
			savedOutput0 = -1;
		}
		if (outputTo[1].connectedTo != null)
		{
			if (savedOutput1 != outputTo[1].power)
			{
				savedOutput1 = outputTo[1].power;
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(1, PID);
			}
			else
			{
				flag = true;
			}
		}
		else
		{
			savedOutput1 = -1;
		}
		if (outputTo[0].connectedTo == null && outputTo[1].connectedTo == null)
		{
			flag = true;
		}
		if (flag)
		{
			sendUsage(-1, 0, 0);
		}
	}

	private IEnumerator waitForOff()
	{
		wait = true;
		yield return new WaitForSeconds(0.05f);
		wiring.inst.newPID++;
		PID = wiring.inst.newPID;
		outputPower(1, PID);
		wait = false;
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, val);
		}
		if (PID > 0)
		{
			newPowerThru();
		}
	}
}
