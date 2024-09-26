using UnityEngine;

public class cloudTransform : MonoBehaviour
{
	public float cloudSpeed = 100f;

	private void Update()
	{
		base.transform.Translate(Vector3.right * cloudSpeed * Time.deltaTime, Space.World);
	}
}
