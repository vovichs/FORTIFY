using System.Collections;
using UnityEngine;

public class electricLight : Device
{
	[Header("__________________________")]
	public Light _light;

	public GameObject lightObj;

	public Material matOn;

	public Material matOff;

	public BuilderPart bp;

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
			if (!on)
			{
				on = true;
				if ((bool)_light || (bool)lightObj)
				{
					toggleLight(state: true);
				}
				changeMat(matOn);
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			if (on)
			{
				on = false;
				if ((bool)_light || (bool)lightObj)
				{
					toggleLight(state: false);
				}
				changeMat(matOff);
			}
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	private void changeMat(Material newMat)
	{
		if (bp.rend[0].sharedMaterials.Length == 1)
		{
			bp.rend[0].sharedMaterial = newMat;
			bp.deploy.mat = newMat;
		}
		else
		{
			Material[] sharedMaterials = bp.rend[0].sharedMaterials;
			sharedMaterials[1] = newMat;
			bp.rend[0].sharedMaterials = sharedMaterials;
		}
	}

	private IEnumerator slowOn()
	{
		yield return new WaitForSeconds(0.1f);
		toggleLight(state: false);
		bp.rend[0].sharedMaterial = matOn;
		bp.deploy.mat = matOn;
	}

	private IEnumerator slowOff()
	{
		yield return new WaitForSeconds(0.1f);
		toggleLight(state: false);
		bp.rend[0].sharedMaterial = matOff;
		bp.deploy.mat = matOff;
	}

	private void toggleLight(bool state)
	{
		if ((bool)lightObj)
		{
			lightObj.SetActive(state);
		}
		else
		{
			_light.enabled = state;
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
