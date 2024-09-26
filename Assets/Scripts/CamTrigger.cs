using System.Collections;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
	public CameraCtrl cc;

	private bool entered;

	private int count;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "climb")
		{
			count++;
			entered = true;
			if (cc.mode == CameraCtrl.Mode.ground)
			{
				StartCoroutine(waitCheck());
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (!(col.tag == "climb"))
		{
			return;
		}
		count--;
		if (count == 0)
		{
			entered = false;
			if (cc.mode == CameraCtrl.Mode.ground)
			{
				StartCoroutine(waitCheck());
			}
		}
	}

	private IEnumerator waitCheck()
	{
		yield return new WaitForFixedUpdate();
		cc.rb.useGravity = !entered;
		cc.climb = entered;
	}
}
