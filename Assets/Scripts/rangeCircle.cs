using UnityEngine;

public class rangeCircle : MonoBehaviour
{
	public BuilderPart bp;

	public bool flat;

	public Transform _transform;

	public MeshFilter meshFilter;

	public Renderer rend;

	public int matIndex;

	public void movedPos(Vector3 newPos)
	{
		if (flat)
		{
			_transform.position = new Vector3(newPos.x, 0.05f, newPos.z);
			return;
		}
		_transform.position = newPos;
		cupboardRange.inst.updateVerts(this, 0f);
	}

	public void destroy()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
