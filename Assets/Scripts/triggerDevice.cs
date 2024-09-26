using System.Collections;
using UnityEngine;

public class triggerDevice : Device
{
	[Header("__________________________")]
	public triggerCollider trig;

	public AudioSource AS;

	public LineRenderer laser;

	public LayerMask lm;

	public float dist = 4f;

	public override void powerThru(int _PID)
	{
		if (PID == 0 && laser != null)
		{
			StartCoroutine(laserWait());
		}
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower > 0)
		{
			if (power == 0 && (bool)laser)
			{
				on = true;
				laser.enabled = true;
				trig.gameObject.SetActive(value: true);
			}
			power = inputPower;
		}
		else
		{
			if (power > 0)
			{
				if (!laser && trig.on)
				{
					StartCoroutine(padTimed(1f));
					return;
				}
				if ((bool)laser)
				{
					on = false;
					laser.enabled = false;
					trig.gameObject.SetActive(value: false);
				}
			}
			power = 0;
		}
		if (trig.on && power > 0)
		{
			if (AS != null && !AS.isPlaying)
			{
				AS.Play();
			}
			outputTo[0].power = power;
		}
		else
		{
			outputTo[0].power = 0;
		}
		if (outputTo[0].connectedTo != null)
		{
			wiring.inst.newPID++;
			PID = wiring.inst.newPID;
			outputPower(0, PID);
		}
		else
		{
			sendUsage(-1, 0, 0);
		}
	}

	private void OnMouseDown()
	{
		if ((bool)laser)
		{
			laserRaycast(set: true);
		}
		else
		{
			StartCoroutine(padTimed(3f));
		}
	}

	public override void setValue(int val, bool send)
	{
		value = val;
	}

	private IEnumerator laserWait()
	{
		yield return null;
		laserRaycast(set: true);
	}

	public void laserRaycast(bool set)
	{
		if (inputFrom[0].power >= 1)
		{
			float num = dist;
			if (set)
			{
				num = 4f;
			}
			Transform transform = trig.transform;
			if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, num, lm))
			{
				Vector3 position = transform.InverseTransformPoint(hitInfo.point);
				laser.SetPosition(1, position);
				num = hitInfo.distance;
			}
			else
			{
				laser.SetPosition(1, new Vector3(0f, 0f, num));
			}
			if (set)
			{
				BoxCollider component = trig.GetComponent<BoxCollider>();
				component.size = new Vector3(component.size.x, component.size.y, num);
				component.center = new Vector3(0f, 0f, num / 2f);
				dist = num;
			}
		}
	}

	public IEnumerator padTimed(float time)
	{
		if (AS != null && !AS.isPlaying)
		{
			AS.Play();
		}
		outputTo[0].power = 1;
		wiring.inst.newPID++;
		PID = wiring.inst.newPID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
		else
		{
			sendUsage(-1, 0, 0);
		}
		yield return new WaitForSeconds(time);
		outputTo[0].power = 0;
		wiring.inst.newPID++;
		PID = wiring.inst.newPID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
		else
		{
			sendUsage(-1, 0, 0);
		}
	}
}
