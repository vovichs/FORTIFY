using UnityEngine;

public class PlacingLine : MonoBehaviour
{
	public LineRenderer lr;

	public Transform _transform;

	public CapsuleCollider col;

	public Material[] mats;

	private int overlapCount;

	public bool blocked;

	private Vector3 segment;

	private Vector3 pos1;

	private Vector3 pos2;

	private Quaternion rot;

	private void FixedUpdate()
	{
		if (overlapCount > 0 || Physics.Raycast(pos1, _transform.forward, segment.magnitude, 256))
		{
			if (!blocked)
			{
				blocked = true;
			}
			lr.sharedMaterial = mats[0];
		}
		else
		{
			if (blocked)
			{
				blocked = false;
			}
			lr.sharedMaterial = mats[1];
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "block")
		{
			overlapCount++;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "block")
		{
			overlapCount--;
		}
	}

	public bool updateLine(Vector3 p1, Vector3 p2)
	{
		if (p1 != Vector3.zero)
		{
			lr.SetPosition(0, p1);
		}
		if (p2 != Vector3.zero)
		{
			lr.SetPosition(1, p2);
		}
		updateCollider();
		return Vector3.Distance(pos2, pos1) < 0.0166f;
	}

	private void updateCollider()
	{
		pos1 = lr.GetPosition(0);
		pos2 = lr.GetPosition(1);
		segment = pos2 - pos1;
		Vector3 position = (pos2 + pos1) / 2f;
		Quaternion rotation = Quaternion.LookRotation(segment.normalized);
		_transform.SetPositionAndRotation(position, rotation);
		col.height = segment.magnitude;
	}

	private void OnEnable()
	{
		overlapCount = 0;
		blocked = false;
	}
}
