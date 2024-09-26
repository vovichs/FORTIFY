using System.Collections;
using UnityEngine;

public class pressurePad : Device
{
	[Header("__________________________")]
	public triggerCollider trig;

	public AudioSource AS;

	private bool timerOn;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (trig.on && inputPower > 0)
		{
			if (AS != null && !AS.isPlaying)
			{
				AS.Play();
			}
			power = inputPower;
		}
		else
		{
			power = 0;
		}
		outputTo[0].power = power;
		if (outputTo[0].connectedTo != null)
		{
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			outputPower(0, PID);
		}
		else
		{
			sendUsage(-1, 0, 0);
		}
	}

	private void OnMouseDown()
	{
		StartCoroutine(padTimed(1f));
	}

	public override void setValue(int val, bool send)
	{
		value = val;
	}

	public IEnumerator padTimed(float time)
	{
		if (!timerOn)
		{
			timerOn = true;
			if (AS != null && !AS.isPlaying)
			{
				AS.Play();
			}
			outputTo[0].power = 1;
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			if (outputTo[0].connectedTo != null)
			{
				outputPower(0, PID);
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
			yield return new WaitForSeconds(time);
			outputTo[0].power = 0;
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			if (outputTo[0].connectedTo != null)
			{
				outputPower(0, PID);
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
			timerOn = false;
		}
	}
}
