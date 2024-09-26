using UnityEngine;

public class camAlign : MonoBehaviour
{
	public Transform _transform;

	private void Update()
	{
		_transform.rotation = camAlignStatic.camRot;
	}
}
