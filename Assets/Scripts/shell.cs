using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class shell : MonoBehaviour
{
	public LayerMask usedMask;

	public GameObject exploLight;

	public Rigidbody rb;

	private void Start()
	{
		rb.AddForce(base.transform.forward * 18000f);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.gameObject.tag == "terrain" || col.gameObject.layer == 12)
		{
			Object.Instantiate(exploLight, base.transform.position, Quaternion.identity);
			GetComponent<SphereCollider>().enabled = false;
			explode();
		}
	}

	private void explode()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 2f, usedMask);
		foreach (Collider collider in array)
		{
			if (collider.gameObject.layer == 12)
			{
				Gravitize(collider.gameObject);
			}
		}
	}

	private void Gravitize(GameObject obj)
	{
		obj.GetComponent<NavMeshAgent>().isStopped = true;
		obj.GetComponent<NavMeshAgent>().enabled = false;
		Rigidbody rigidbody = obj.AddComponent<Rigidbody>();
		rigidbody.mass = 1f;
		rigidbody.useGravity = true;
		rigidbody.AddExplosionForce(1000f, base.transform.position, 2f, 3f);
		StartCoroutine(waitToDestroy(obj));
	}

	private IEnumerator waitToDestroy(GameObject obj)
	{
		yield return new WaitForSeconds(2f);
		UnityEngine.Object.Destroy(obj);
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
