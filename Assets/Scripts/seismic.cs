using System.Collections;
using UnityEngine;

public class seismic : Device
{
	[Header("__________________________")]
	public Renderer[] lights;

	private int trigPower = 3;

	private bool trig;

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
				on = true;
				lights[0].sharedMaterial = wiring.inst.lightGreen;
				lights[1].sharedMaterial = wiring.inst.lightRed;
			}
		}
		else if (on)
		{
			on = false;
			lightsOff(lights);
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
		value = val;
	}

	public void setTriggered(int power)
	{
		if (power != -1)
		{
			trigPower = power;
		}
		StartCoroutine(triggered());
	}

	private IEnumerator triggered()
	{
		if (on && !trig && (bool)outputTo[0].connectedTo)
		{
			trig = true;
			lights[1].sharedMaterial = wiring.inst.lightGreen;
			outputTo[0].power = Mathf.Min(getInputPower(0), trigPower);
			PID = ++wiring.inst.newPID;
			standardOutput(0);
			yield return new WaitForSeconds(2f);
			lights[1].sharedMaterial = wiring.inst.lightRed;
			outputTo[0].power = 0;
			newPowerThru();
			trig = false;
		}
	}
}
