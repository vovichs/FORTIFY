using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class fuelGenerator : Device
{
	[Header("__________________________")]
	private int fuel = 250;

	private int fuelMax = 500;

	private float burnRate = 15f;

	private Coroutine powerOnCR;

	public AudioSource AS;

	public bool powered;

	private bool start;

	private bool stop;

	private bool output;

	private bool toggled;

	public int secondsLeft;

	public override void powerThru(int _PID)
	{
		bool flag = false;
		bool flag2 = false;
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (outputTo[0].connectedTo != null)
		{
			output = true;
		}
		else
		{
			if (powered)
			{
				StartCoroutine(powerOff());
			}
			output = false;
		}
		if (toggled)
		{
			toggled = false;
			if (output)
			{
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(0, PID);
			}
			return;
		}
		if (inputFrom[0].connectedTo != null && inputFrom[0].connectedTo.power > 0)
		{
			if (!start && fuel > 0 && !powered)
			{
				start = true;
				flag = true;
			}
		}
		else
		{
			start = false;
		}
		if (inputFrom[1].connectedTo != null && inputFrom[1].connectedTo.power > 0)
		{
			if (!stop && powered)
			{
				stop = true;
				flag2 = true;
			}
		}
		else
		{
			stop = false;
		}
		if (inputFrom[0].connectedTo != null || inputFrom[1].connectedTo != null)
		{
			sendUsage();
		}
		if (flag2)
		{
			StartCoroutine(powerOff());
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			if (output)
			{
				outputPower(0, PID);
			}
		}
		else if (flag)
		{
			powerOnCR = StartCoroutine(powerOn());
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			if (output)
			{
				outputPower(0, PID);
			}
		}
	}

	public void powerToggle()
	{
		toggled = true;
		if (!powered)
		{
			if (powerOnCR == null && fuel > 0)
			{
				powerOnCR = StartCoroutine(powerOn());
				newPowerThru();
			}
		}
		else if (powerOnCR != null)
		{
			StartCoroutine(powerOff());
			newPowerThru();
		}
	}

	private IEnumerator powerOn()
	{
		if (panelOpen)
		{
			devicePanel.inst.panel.transform.GetChild(1).GetComponent<Text>().text = "TURN OFF";
		}
		AS.enabled = true;
		AS.Play();
		powered = true;
		outputTo[0].power = 40;
		while (fuel > 0 && output)
		{
			yield return new WaitForSeconds(burnRate);
			fuel--;
		}
		powerOnCR = null;
		StartCoroutine(powerOff());
		newPowerThru();
	}

	private IEnumerator powerOff()
	{
		if (powerOnCR != null)
		{
			StopCoroutine(powerOnCR);
			powerOnCR = null;
		}
		outputTo[0].power = 0;
		powered = false;
		yield return new WaitForSeconds(0.1f);
		if (panelOpen)
		{
			devicePanel.inst.panel.transform.GetChild(1).GetComponent<Text>().text = "TURN ON";
		}
		AS.Stop();
		AS.enabled = false;
	}

	public IEnumerator panelUpdate()
	{
		Text component = devicePanel.inst.panel.transform.GetChild(1).GetComponent<Text>();
		if (powered)
		{
			component.text = "TURN OFF";
		}
		else
		{
			component.text = "TURN ON";
		}
		secondsLeft = Mathf.RoundToInt((float)fuel * burnRate);
		while (panelOpen)
		{
			string str = "";
			if (fuel == 0)
			{
				str = "0s";
			}
			else
			{
				if (secondsLeft >= 60)
				{
					str += Mathf.Floor(secondsLeft / 60).ToString("0m ");
				}
				str += Mathf.Floor(secondsLeft % 60).ToString("0s");
			}
			devicePanel.inst.fuelTime.text = str;
			devicePanel.inst.fuelSlider.value = fuel;
			yield return new WaitForSeconds(1f);
			if (powered && secondsLeft > 0)
			{
				secondsLeft--;
			}
		}
	}

	public void setFuel(int amount)
	{
		fuel = amount;
		secondsLeft = Mathf.RoundToInt((float)fuel * burnRate);
	}

	private void OnEnable()
	{
		if (powerOnCR != null)
		{
			powerOnCR = StartCoroutine(powerOn());
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
