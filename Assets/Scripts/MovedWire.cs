using UnityEngine;

public class MovedWire : UndoPart
{
	public wire w;

	public Vector3[] verts;

	public MovedWire(wire W, Vector3[] Verts)
	{
		w = W;
		verts = Verts;
	}
}
