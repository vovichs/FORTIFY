using System.Collections;
using UnityEngine;

public class timerDevice : Device
{
	[Header("__________________________")]
	public float time;

	public float t;

	private bool lightGreen;

	private bool togglePower;

	private int loopCount;

	public Renderer[] lights;

	private Coroutine timerCR;

	public bool timerOn;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			loopCount++;
			if (loopCount > 2)
			{
				loopCount = 0;
				return;
			}
		}
		else
		{
			PID = _PID;
		}
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		if (inputPower > 0)
		{
			on = true;
			if (timerOn)
			{
				return;
			}
			if (inputPower2 > 0 && !togglePower)
			{
				togglePower = true;
				timerCR = StartCoroutine(toggleTimer());
				return;
			}
			togglePower = false;
			lightState();
		}
		else
		{
			if (!on)
			{
				return;
			}
			on = false;
			lightsOff(lights);
			if (timerOn)
			{
				StopCoroutine(timerCR);
				timerCR = null;
				timerOn = false;
			}
			togglePower = false;
		}
		outputTo[0].power = 0;
		if ((bool)outputTo[0].connectedTo)
		{
			outputPower(0, PID);
		}
		sendUsage();
	}

	public void runTimer(bool send)
	{
		if (inputFrom[0].power != 0)
		{
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDeviceValue(owner, -1f);
			}
			if (!timerOn)
			{
				timerCR = StartCoroutine(timer());
			}
		}
	}

	public IEnumerator toggleTimer()
	{
		yield return new WaitForSeconds(0.5f);
		if (!timerOn)
		{
			timerCR = StartCoroutine(timer());
		}
	}

	private IEnumerator timer()
	{
		timerOn = true;
		StartCoroutine(playSound(GetComponent<AudioSource>()));
		lightState();
		sendOutputPower(inputFrom[0].power);
		t = time;
		devicePanel.inst.timerTime.text = time.ToString();
		while (t > 0f)
		{
			yield return new WaitForSeconds(1f);
			t -= 1f;
		}
		timerOn = false;
		sendOutputPower(0);
		lightState();
		timerCR = null;
	}

	private void sendOutputPower(int amount)
	{
		outputTo[0].power = amount;
		if ((bool)outputTo[0].connectedTo)
		{
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			outputPower(0, PID);
		}
	}

	public override void setValue(float val, bool send)
	{
		if (val == -1f)
		{
			runTimer(send: false);
			return;
		}
		time = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, time);
		}
	}

	private IEnumerator toggleCheckDelay()
	{
		yield return new WaitForSeconds(time);
		newPowerThru();
	}

	public void stop()
	{
		t = 0f;
	}

	private void lightState()
	{
		if (inputFrom[0].power == 0)
		{
			lightsOff(lights);
		}
		if (timerOn && !lightGreen)
		{
			lightGreen = true;
			lights[0].sharedMaterial = wiring.inst.lightGreen;
			lights[1].sharedMaterial = wiring.inst.lightOff;
		}
		else
		{
			lightGreen = false;
			lights[0].sharedMaterial = wiring.inst.lightOff;
			lights[1].sharedMaterial = wiring.inst.lightRed;
		}
	}

	private void OnDisable()
	{
		if (timerOn)
		{
			if (timerCR != null)
			{
				StopCoroutine(timerCR);
			}
			timerOn = false;
			timerCR = null;
			outputTo[0].power = 0;
			lightState();
		}
	}

	private void OnDestroy()
	{
		panelOpen = false;
	}
}
