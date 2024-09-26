using UnityEngine;

public class lockRotation : MonoBehaviour
{
	private float locked;

	private void Awake()
	{
		locked = base.transform.rotation.x;
	}

	private void LateUpdate()
	{
		base.transform.rotation = Quaternion.Euler(locked, base.transform.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z);
	}
}
