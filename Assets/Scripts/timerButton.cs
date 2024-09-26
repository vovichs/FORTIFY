using System.Collections;
using UnityEngine;

public class timerButton : Device
{
	[Header("__________________________")]
	private Coroutine timerCR;

	public bool button;

	public float time;

	public override void powerThru(int _PID)
	{
		if (PID != _PID)
		{
			PID = _PID;
			if (getInputPower(0) == 0)
			{
				outputTo[0].power = 0;
			}
			else if (!on)
			{
				outputTo[0].power = 0;
			}
			if (outputTo[0].connectedTo != null)
			{
				PID = ++wiring.inst.newPID;
				outputPower(0, PID);
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
		}
	}

	public override void setValue(int val, bool send)
	{
		if (val == -1)
		{
			runTimer(send: false);
		}
	}

	public void runTimer(bool send)
	{
		if (!on)
		{
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDeviceValue(owner, -1f);
			}
			StartCoroutine(playSound(GetComponent<AudioSource>()));
			timerCR = StartCoroutine(timer(inputFrom[0].power > 0));
		}
	}

	private IEnumerator timer(bool power)
	{
		float seconds = time;
		if (button)
		{
			if (power)
			{
				outputTo[0].power = inputFrom[0].power;
			}
			else
			{
				seconds = 1f;
				outputTo[0].power = 2;
			}
		}
		else
		{
			if (!power)
			{
				yield break;
			}
			seconds = 1f;
			outputTo[0].power = 1;
		}
		on = true;
		PID = ++wiring.inst.newPID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
		yield return new WaitForSeconds(seconds);
		on = false;
		newPowerThru();
	}

	private void OnEnable()
	{
		on = false;
	}
}
