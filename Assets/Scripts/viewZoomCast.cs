using UnityEngine;

public class viewZoomCast : MonoBehaviour
{
	public Transform parent;

	private Vector3 dir;

	private Vector3 pos;

	private float z;

	private bool blocked;

	private Transform _transform;

	private void Start()
	{
		_transform = base.transform;
		dir = _transform.InverseTransformDirection(_transform.position - parent.position);
		pos = _transform.localPosition;
	}

	private void Update()
	{
		if (Physics.Raycast(parent.position, parent.TransformDirection(dir), out RaycastHit hitInfo, 0.7f, 1))
		{
			blocked = true;
			float num = 0.9f - hitInfo.distance;
			_transform.Translate(Vector3.forward * (0f - (z - num)));
			z = num;
		}
		else if (blocked)
		{
			blocked = false;
			z = 0f;
			_transform.localPosition = pos;
		}
	}
}
