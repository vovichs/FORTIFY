using System.Collections;
using UnityEngine;

public class igniter : Device
{
	[Header("__________________________")]
	public ParticleSystem ps;

	public LayerMask mask;

	private bool wait;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (getInputPower(0) >= usage)
		{
			if (!on)
			{
				on = true;
				ps.Play();
				if (!wait)
				{
					StartCoroutine(igniteAreaCheck());
				}
			}
		}
		else if (on)
		{
			ps.Stop();
			on = false;
		}
		sendUsage();
	}

	private IEnumerator igniteAreaCheck()
	{
		wait = true;
		yield return new WaitForSeconds(1f);
		if (inputFrom[0].power > 0)
		{
			Collider[] array = Physics.OverlapSphere(base.transform.position, 1f, mask);
			foreach (Collider collider in array)
			{
				if (collider.tag == "deploy")
				{
					BuilderPart component = collider.GetComponent<BuilderPart>();
					if ((bool)component.deploy.ignitable)
					{
						component.deploy.ignitable.ignite();
					}
				}
			}
			yield return new WaitForSeconds(2f);
		}
		wait = false;
	}

	private void OnEnable()
	{
		if (on)
		{
			ps.Play();
		}
	}

	public override void deviceHideState(bool hide)
	{
		
	}
}
