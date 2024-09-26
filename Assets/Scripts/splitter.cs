using UnityEngine;

public class splitter : Device
{
	private bool powered;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower > 0)
		{
			powered = true;
			int num = 0;
			for (int i = 0; i < 3; i++)
			{
				if ((bool)base.outputTo[i].connectedTo)
				{
					num++;
				}
			}
			if (num > 0)
			{
				int power = Mathf.FloorToInt(inputPower / num);
				int num2 = inputPower % num;
				io[] outputTo = base.outputTo;
				foreach (io io in outputTo)
				{
					if ((bool)io.connectedTo)
					{
						io.power = power;
						if (num2 > 0)
						{
							io.power++;
							num2--;
						}
					}
					else
					{
						io.power = 0;
					}
				}
			}
			else
			{
				io[] outputTo = base.outputTo;
				for (int j = 0; j < outputTo.Length; j++)
				{
					outputTo[j].power = 0;
				}
			}
		}
		else
		{
			if (!powered)
			{
				return;
			}
			powered = false;
			io[] outputTo = base.outputTo;
			for (int j = 0; j < outputTo.Length; j++)
			{
				outputTo[j].power = 0;
			}
		}
		for (int k = 0; k < base.outputTo.Length; k++)
		{
			if (base.outputTo[k].connectedTo != null)
			{
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(k, PID);
			}
		}
		if (circuitEndCheck())
		{
			sendUsage(-1, 0, 0);
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
