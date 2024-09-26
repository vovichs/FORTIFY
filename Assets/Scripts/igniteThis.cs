using System.Collections;
using UnityEngine;

public class igniteThis : MonoBehaviour
{
	public GameObject firework;

	public int num = 10;

	private bool wait;

	public void ignite()
	{
		if (!wait)
		{
			StartCoroutine(timedIgnite());
		}
	}

	private IEnumerator timedIgnite()
	{
		wait = true;
		int count = 0;
		Transform t = base.transform;
		for (; count < num; count++)
		{
			yield return new WaitForSeconds(2f);
			Object.Instantiate(firework, t.position, Quaternion.identity);
		}
		wait = false;
	}

	private void OnDisable()
	{
		wait = false;
	}
}
