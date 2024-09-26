using UnityEngine;

public class combiner : Device
{
	public Renderer[] lights;

	public override void powerThru(int _PID)
	{
		if (PID != _PID)
		{
			PID = _PID;
			int num = getInputPower(0);
			int num2 = getInputPower(1);
			if (num > 0 && !inputFrom[0].connectedTo.dev.combine)
			{
				num = 0;
				inputFrom[0].power = 0;
			}
			if (num2 > 0 && !inputFrom[1].connectedTo.dev.combine)
			{
				num2 = 0;
				inputFrom[1].power = 0;
			}
			power = num + num2;
			if (power > 0)
			{
				lights[0].sharedMaterial = wiring.inst.lightGreen;
				lights[1].sharedMaterial = wiring.inst.lightGreen;
			}
			else
			{
				lightsOff(lights);
			}
			outputTo[0].power = power;
			standardOutput(0);
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
		for (int i = 0; i < inputFrom.Length; i++)
		{
			io io = inputFrom[i];
			if (!io.noUsage && io.type == 0 && io.connectedTo != null && io.connectedTo.dev.combine)
			{
				io.connectedTo.dev.sendUsage(num, io.connectedTo.index, UID);
			}
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
