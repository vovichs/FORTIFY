using System.Collections;
using UnityEngine;

public class HBHF : Device
{
	[Header("__________________________")]
	public Material matOn;

	public Material matOff;

	public AudioSource AS;

	public triggerCollider trig;

	private BuilderPart bp;

	private Coroutine checkCR;

	public LayerMask layerMask;

	private bool LOS;

	public Transform LOSt;

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
				swapMat();
				if (AS != null)
				{
					AS.Play();
				}
				trig.gameObject.SetActive(value: true);
			}
		}
		else
		{
			if (!on)
			{
				return;
			}
			on = false;
			swapMat();
			trig.gameObject.SetActive(value: false);
			toggleLOS(start: false);
		}
		if (LOS && inputPower > 1)
		{
			outputTo[0].power = 1;
		}
		else
		{
			outputTo[0].power = 0;
		}
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
		value = val;
	}

	public void swapMat()
	{
		if (!bp)
		{
			bp = GetComponent<BuilderPart>();
		}
		if (on)
		{
			bp.deploy.mat = matOn;
			bp.rend[0].sharedMaterial = matOn;
		}
		else
		{
			bp.deploy.mat = matOff;
			bp.rend[0].sharedMaterial = matOff;
		}
	}

	public void toggleLOS(bool start)
	{
		if (inputFrom[0].power < 1)
		{
			return;
		}
		if (start)
		{
			checkCR = StartCoroutine(checkLOS());
			return;
		}
		if (LOS)
		{
			LOS = false;
			newPowerThru();
		}
		if (checkCR != null)
		{
			StopCoroutine(checkCR);
		}
	}

	private IEnumerator checkLOS()
	{
		Transform player = BuilderSystem.inst._transform;
		while (true)
		{
			yield return new WaitForSeconds(0.5f);
			Vector3 direction = player.position - LOSt.position;
			if (!Physics.Raycast(LOSt.position, direction, out RaycastHit hitInfo, 3.34f, layerMask))
			{
				continue;
			}
			if (hitInfo.transform.gameObject.layer == 10)
			{
				if (!LOS)
				{
					LOS = true;
					newPowerThru();
				}
			}
			else if (LOS)
			{
				LOS = false;
				newPowerThru();
			}
		}
	}
}
