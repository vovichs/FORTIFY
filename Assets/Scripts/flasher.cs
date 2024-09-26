using System.Collections;
using UnityEngine;

public class flasher : electricLight
{
	[Header("__________________________")]
	public Renderer rend;

	private Coroutine CR;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower >= usage)
		{
			if (CR == null)
			{
				CR = StartCoroutine(flashing());
			}
		}
		else if (CR != null)
		{
			StopCoroutine(CR);
			CR = null;
			on = false;
			_light.enabled = false;
			rend.sharedMaterial = matOff;
		}
		if (inputPower > usage)
		{
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
	}

	private IEnumerator flashing()
	{
		on = true;
		bool state = false;
		int count = 0;
		while (on)
		{
			float seconds;
			if (count == 6)
			{
				count = 0;
				seconds = 1f;
			}
			else
			{
				seconds = 0.05f;
			}
			_light.enabled = state;
			if (state)
			{
				rend.sharedMaterial = matOn;
			}
			else
			{
				rend.sharedMaterial = matOff;
			}
			yield return new WaitForSeconds(seconds);
			count++;
			state = !state;
		}
	}

	private void OnEnable()
	{
		if (CR != null)
		{
			CR = StartCoroutine(flashing());
		}
	}
}
