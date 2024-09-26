using UnityEngine;
using UnityEngine.UI;

public class counter : Device
{
	[Header("__________________________")]
	public GameObject display;

	public Text text;

	private bool add;

	private bool sub;

	private bool clear;

	private int count;

	private int displayNum = -1;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		int inputPower3 = getInputPower(2);
		int inputPower4 = getInputPower(3);
		if (inputPower > 0)
		{
			if (!on)
			{
				on = true;
				if (!display.activeSelf)
				{
					display.SetActive(value: true);
				}
			}
			if (value == -1)
			{
				power = inputPower;
			}
			else
			{
				if (inputPower2 > 0)
				{
					if (add)
					{
						count++;
						add = false;
					}
				}
				else
				{
					add = true;
				}
				if (inputPower3 > 0)
				{
					if (sub)
					{
						if (count > 0)
						{
							count--;
						}
						sub = false;
					}
				}
				else
				{
					sub = true;
				}
				if (inputPower4 > 0)
				{
					if (clear)
					{
						count = 0;
						clear = false;
					}
				}
				else
				{
					clear = true;
				}
				if (count >= value)
				{
					power = inputPower;
				}
				else
				{
					power = 0;
				}
			}
			setDisplay();
		}
		else if (on)
		{
			on = false;
			if (display.activeSelf)
			{
				display.SetActive(value: false);
			}
			power = 0;
		}
		outputTo[0].power = power;
		standardOutput(0);
	}

	private void setDisplay()
	{
		int num = (value != -1) ? count : inputFrom[0].power;
		if (num != displayNum)
		{
			displayNum = num;
			text.text = displayNum.ToString();
		}
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
