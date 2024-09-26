using System;
using System.Collections;
using UnityEngine;

public class battery : Device
{
	public enum Size
	{
		small,
		medium,
		large
	}

	[Header("__________________________")]
	private bool drained;

	public Size size;

	public int chargePower;

	private int chargeMax;

	private int outputMax;

	private int chargeRateMax;

	public int activeUsage;

	private string time;

	private Coroutine dischargeCR;

	private bool charging;

	private void Awake()
	{
		if (size == Size.small)
		{
			chargePower = 1200;
			chargeMax = 24000;
			outputMax = 15;
		}
		else if (size == Size.medium)
		{
			chargePower = 6000;
			chargeMax = 540000;
			outputMax = 50;
		}
		else
		{
			chargePower = 12000;
			chargeMax = 1440000;
			outputMax = 100;
		}
		chargeRateMax = outputMax * 4;
	}

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (inputFrom[0].connectedTo != null)
		{
			inputFrom[0].power = inputFrom[0].connectedTo.power;
			if (inputFrom[0].power > 0)
			{
				if (!charging)
				{
					StartCoroutine(charge());
				}
				usage = inputFrom[0].power;
			}
			else
			{
				usage = 0;
			}
			sendUsage(activeUsage, 0, 0);
		}
		else
		{
			inputFrom[0].power = 0;
		}
		if (inputFrom[0].power == 0 && charging)
		{
			charging = false;
		}
		if (drained)
		{
			outputTo[0].power = 0;
		}
		else
		{
			outputTo[0].power = outputMax;
		}
		if (outputTo[0].connectedTo != null)
		{
			if (outputTo[0].connectedTo.dev is blocker && (outputTo[0].connectedTo.dev as blocker).blocked)
			{
				stopDischarge();
			}
			else
			{
				startDischarge();
			}
			outputPower(0, PID);
		}
		else
		{
			stopDischarge();
			activeUsage = 0;
		}
	}

	private IEnumerator charge()
	{
		charging = true;
		while (charging)
		{
			if (drained && chargePower > activeUsage)
			{
				drained = false;
				newPowerThru();
			}
			yield return new WaitForSeconds(1f);
			if (chargePower >= chargeMax)
			{
				chargePower = chargeMax;
			}
			else
			{
				chargePower += Mathf.Min(chargeRateMax, Mathf.RoundToInt((float)inputFrom[0].power * 0.8f));
			}
		}
	}

	private IEnumerator discharge()
	{
		while (chargePower > 0)
		{
			yield return new WaitForSeconds(1f);
			chargePower -= activeUsage;
		}
		drained = true;
		newPowerThru();
		dischargeCR = null;
	}

	public IEnumerator showTime()
	{
		string maxString = "/" + chargeMax / 60 + " rWm";
		time = "";
		while (panelOpen)
		{
			string str = "";
			if (chargePower < activeUsage)
			{
				time = "0s";
			}
			else if (activeUsage > 0 || time.Length == 0)
			{
				int num = chargePower / Mathf.Max(activeUsage, 1);
				if (num >= 3600)
				{
					str += Mathf.Floor(num / 3600).ToString("0h ");
				}
				if (num >= 60)
				{
					str += Mathf.Floor(num / 60 % 60).ToString("0m ");
				}
				str = (time = str + Mathf.Floor(num % 60).ToString("0s"));
			}
			devicePanel.inst.batteryText[0].text = time;
			devicePanel.inst.batteryText[1].text = "Active Usage: " + activeUsage.ToString();
			devicePanel.inst.batteryText[2].text = (chargePower / 60).ToString() + maxString;
			float num2 = (float)chargePower / (float)chargeMax;
			devicePanel.inst.batterySlider.value = (float)Math.Round(num2, 2);
			yield return new WaitForSeconds(1f);
		}
	}

	public void setActiveUsage(int amount)
	{
		activeUsage = Mathf.Min(amount, outputMax);
	}

	public void setCharge()
	{
		chargePower = (int)(devicePanel.inst.batterySlider.value * (float)chargeMax);
		if (chargePower > 0 && drained)
		{
			drained = false;
			newPowerThru();
		}
		time = "";
		if (BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, chargePower);
		}
	}

	public void startDischarge()
	{
		if (!drained && dischargeCR == null)
		{
			dischargeCR = StartCoroutine(discharge());
		}
	}

	public void stopDischarge()
	{
		if (dischargeCR != null)
		{
			StopCoroutine(dischargeCR);
			dischargeCR = null;
		}
	}

	private void OnEnable()
	{
		if (dischargeCR != null)
		{
			StartCoroutine(discharge());
		}
		if (charging)
		{
			StartCoroutine(charge());
		}
	}

	public override void setValue(int val, bool send)
	{
		chargePower = val;
		if (chargePower > 0 && drained)
		{
			drained = false;
			newPowerThru();
		}
		time = "";
	}

	private void OnDestroy()
	{
		panelOpen = false;
	}
}
