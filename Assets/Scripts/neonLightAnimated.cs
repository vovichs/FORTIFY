using System.Collections;
using UnityEngine;

public class neonLightAnimated : electricLight
{
	[Header("__________________________")]
	public Material matBase;

	public Material matOn1;

	public Material matOn2;

	private static Material[] matsOff;

	private static Material[] matsOn1;

	private static Material[] matsOn2;

	private Coroutine CR;

	private void Awake()
	{
		if (matsOn1 == null)
		{
			matsOff = new Material[2]
			{
				matBase,
				matOff
			};
			matsOn1 = new Material[2]
			{
				matBase,
				matOn1
			};
			matsOn2 = new Material[2]
			{
				matBase,
				matOn2
			};
		}
	}

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
				CR = StartCoroutine(animate());
			}
		}
		else if (CR != null)
		{
			StopCoroutine(CR);
			CR = null;
			on = false;
			bp.rend[0].sharedMaterials = matsOff;
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

	private void changeMat(Material newMat)
	{
		bp.rend[0].sharedMaterials = matsOn1;
	}

	public override void setValue(int val, bool send)
	{
	}

	private IEnumerator animate()
	{
		on = true;
		int count = 0;
		while (on)
		{
			if (count == 0)
			{
				bp.rend[0].sharedMaterials = matsOn1;
			}
			else
			{
				bp.rend[0].sharedMaterials = matsOn2;
			}
			yield return new WaitForSeconds(1f);
			count++;
			if (count == 2)
			{
				count = 0;
			}
		}
	}

	private void OnEnable()
	{
		if (CR != null)
		{
			CR = StartCoroutine(animate());
		}
	}
}
