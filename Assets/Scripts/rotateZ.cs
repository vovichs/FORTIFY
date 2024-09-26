using UnityEngine;

public class rotateZ : MonoBehaviour
{
	private Transform t;

	public float speed;

	private void Awake()
	{
		t = base.transform;
	}

	private void Update()
	{
		base.transform.Rotate(0f, 0f, speed * Time.deltaTime);
	}
}
