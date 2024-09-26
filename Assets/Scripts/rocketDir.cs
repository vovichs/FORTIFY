using UnityEngine;

public class rocketDir : MonoBehaviour
{
	public Rigidbody rb;

	private Transform _transform;

	private void Start()
	{
		_transform = base.transform;
	}

	private void FixedUpdate()
	{
		_transform.rotation = Quaternion.LookRotation(rb.velocity);
	}
}
