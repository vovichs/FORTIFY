using System.Collections;
using UnityEngine;

public class digitalClock : Device
{
	[Header("__________________________")]
	private Coroutine timerCR;

	public BuilderPart bp;

	public Material[] mats;

	private float time = 5f;

	private bool timerOn;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (getInputPower(0) == 0)
		{
			if (on)
			{
				on = false;
				bp.rend[0].sharedMaterial = mats[0];
				bp.deploy.mat = mats[0];
				if (timerOn)
				{
					StopCoroutine(timerCR);
					timerCR = null;
					timerOn = false;
				}
				outputTo[0].power = 0;
			}
		}
		else
		{
			if (!on)
			{
				on = true;
				bp.rend[0].sharedMaterial = mats[1];
				bp.deploy.mat = mats[1];
			}
			if (!timerOn)
			{
				outputTo[0].power = 0;
			}
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

	public override void setValue(int val, bool send)
	{
		if (val == -1)
		{
			runTimer(send: false);
		}
	}

	public void runTimer(bool send)
	{
		if (!timerOn)
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
		outputTo[0].power = inputFrom[0].power - usage;
		timerOn = true;
		PID = ++wiring.inst.newPID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
		yield return new WaitForSeconds(seconds);
		timerOn = false;
		newPowerThru();
	}

	private void OnEnable()
	{
		timerOn = false;
	}
}
