using System.Collections;
using UnityEngine;

public class collisionExplode : MonoBehaviour
{
	private bool exp;

	public GameObject explosion;

	private void Start()
	{
		StartCoroutine(lifetime());
	}

	private void OnCollisionEnter(Collision col)
	{
		if (!exp)
		{
			float num = 5f;
			if (col.gameObject.tag == "terrain")
			{
				num = 15f;
			}
			if (col.relativeVelocity.magnitude > num)
			{
				Explode();
			}
		}
	}

	public void Explode()
	{
		exp = true;
		Object.Instantiate(explosion, base.transform.position, Quaternion.identity);
		sphereDamage(base.transform.position);
		GetComponent<Renderer>().enabled = false;
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void sphereDamage(Vector3 hitPos)
	{
		if (!Stability.inst.raidMode)
		{
			return;
		}
		Collider[] array = Physics.OverlapSphere(hitPos, 2f, Stability.inst.rocketMask);
		for (int i = 0; i < array.Length; i++)
		{
			BuilderPart component = array[i].GetComponent<BuilderPart>();
			if (!(component == null))
			{
				component.raidDestroy(hitPos);
			}
		}
	}

	private IEnumerator lifetime()
	{
		yield return new WaitForSeconds(10f);
		Explode();
	}
}
