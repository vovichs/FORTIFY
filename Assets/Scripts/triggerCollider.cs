using UnityEngine;

public class triggerCollider : MonoBehaviour
{
	public bool on;

	public Device dev;

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.layer != 10)
		{
			return;
		}
		on = true;
		if (dev is triggerDevice)
		{
			if (dev.inputFrom[0].power > 0)
			{
				if ((bool)(dev as triggerDevice).laser)
				{
					(dev as triggerDevice).laserRaycast(set: false);
				}
				dev.newPowerThru();
			}
		}
		else if (dev is pressurePad)
		{
			if (dev.inputFrom[0].power > 0)
			{
				dev.newPowerThru();
			}
			else
			{
				StartCoroutine((dev as pressurePad).padTimed(1f));
			}
		}
		else if (dev is HBHF)
		{
			(dev as HBHF).toggleLOS(start: true);
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.layer != 10)
		{
			return;
		}
		on = false;
		if (dev is triggerDevice)
		{
			if (dev.inputFrom[0].power > 0)
			{
				if ((bool)(dev as triggerDevice).laser)
				{
					(dev as triggerDevice).laserRaycast(set: false);
				}
				dev.newPowerThru();
			}
		}
		else if (dev is pressurePad)
		{
			if (dev.inputFrom[0].power > 0)
			{
				dev.newPowerThru();
			}
		}
		else if (dev is HBHF)
		{
			(dev as HBHF).toggleLOS(start: false);
		}
	}

	private void OnDisable()
	{
		on = false;
	}
}
