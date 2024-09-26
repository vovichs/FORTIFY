using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosives : MonoBehaviour
{
	public enum Type
	{
		splash,
		direct,
		sphere
	}

	public Type type;

	public List<Damage.DamageTypeEntry> damageTypes = new List<Damage.DamageTypeEntry>();

	public Rigidbody rb;

	private bool hit;

	public GameObject explosion;

	public ParticleSystem ps;

	public AudioSource AS;

	public Transform center;

	public float radius;

	public float minRadius;

	public float speedMulti = 1f;

	private static Vector3[] splashVectors;

	static explosives()
	{
		int num = 720;
		splashVectors = new Vector3[num];
		float num2 = (float)Math.PI * (3f - Mathf.Sqrt(5f));
		float num3 = 2f / (float)num;
		for (int i = 0; i < num; i++)
		{
			float num4 = (float)i * num3 - 1f + num3 / 2f;
			float num5 = Mathf.Sqrt(1f - num4 * num4);
			float f = (float)i * num2;
			float x = Mathf.Cos(f) * num5;
			float z = Mathf.Sin(f) * num5;
			splashVectors[i] = new Vector3(x, num4, z);
		}
	}

	private void Start()
	{
		rb.AddForce(base.transform.forward * (16000f * speedMulti));
		StartCoroutine(lifetime());
	}

	private void OnCollisionEnter(Collision col)
	{
		if (hit)
		{
			return;
		}
		ContactPoint contactPoint = col.contacts[0];
		hit = true;
		Vector3 position = base.transform.position;
		UnityEngine.Object.Instantiate(explosion, position, Quaternion.identity);
		if (type != Type.direct || !(col.gameObject.tag == "terrain"))
		{
			if (type == Type.splash)
			{
				splashDamage(center.position, contactPoint.point);
			}
			if (type == Type.direct)
			{
				directDamage(position, col.gameObject);
			}
			if (type == Type.sphere)
			{
				sphereDamage(position);
			}
		}
		GetComponent<Renderer>().enabled = false;
		rb.detectCollisions = false;
		if (ps != null)
		{
			ps.Stop();
		}
		if (AS != null)
		{
			AS.Stop();
		}
		StartCoroutine(waitToDestroy());
	}

	public void splashDamage(Vector3 centerPos, Vector3 hitpos)
	{
		if (!Stability.inst.raidMode)
		{
			return;
		}
		List<BuilderPart> list = new List<BuilderPart>();
		for (int i = 0; i < splashVectors.Length; i++)
		{
			if (Physics.Raycast(centerPos, splashVectors[i], out RaycastHit hitInfo, 2f, 1))
			{
				BuilderPart component = hitInfo.transform.GetComponent<BuilderPart>();
				if ((bool)component && !list.Contains(component))
				{
					list.Add(component);
				}
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			BuilderPart builderPart = list[j];
			float num = 100f;
			Vector3 zero = Vector3.zero;
			Collider[] components = builderPart.GetComponents<Collider>();
			for (int k = 0; k < components.Length; k++)
			{
				Vector3 a = components[k].ClosestPointOnBounds(hitpos);
				float num2 = Vector3.Distance(a, hitpos);
				if (num2 < num)
				{
					num = num2;
				}
			}
			float num3 = Mathf.Clamp01(num - minRadius) / (radius - minRadius);
			if (num3 <= 1f)
			{
				Damage.DamageTypeList damageTypeList = new Damage.DamageTypeList();
				damageTypeList.Add(damageTypes);
				damageTypeList.ScaleAll(1f - num3);
				builderPart.applyDamage(damageTypeList, hitpos);
			}
		}
	}

	public void directDamage(Vector3 hitPos, GameObject hit)
	{
		if (Stability.inst.raidMode)
		{
			BuilderPart component = hit.GetComponent<BuilderPart>();
			if ((bool)component)
			{
				Damage.DamageTypeList damageTypeList = new Damage.DamageTypeList();
				damageTypeList.Add(damageTypes);
				component.applyDamage(damageTypeList, hitPos);
			}
		}
	}

	public void sphereDamage(Vector3 hitPos)
	{
		Collider[] array = Physics.OverlapSphere(hitPos, 1.5f, Stability.inst.rocketMask);
		foreach (Collider collider in array)
		{
			if (collider.gameObject.layer != 15)
			{
				BuilderPart component = collider.GetComponent<BuilderPart>();
				if ((bool)component)
				{
					component.raidDestroy(hitPos);
				}
			}
		}
	}

	private IEnumerator waitToDestroy()
	{
		yield return new WaitForSeconds(5f);
		UnityEngine.Object.Destroy(base.gameObject);
	}

	private IEnumerator lifetime()
	{
		yield return new WaitForSeconds(8f);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
