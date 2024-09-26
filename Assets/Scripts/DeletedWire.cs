using UnityEngine;

public class DeletedWire : UndoPart
{
	public int outId;

	public int outIndex;

	public int inId;

	public int inIndex;

	public Vector3[] verts;

	public int colorId;

	public DeletedWire(int OutId, int OutIndex, int InId, int InIndex, Vector3[] Verts, int ColorId)
	{
		outId = OutId;
		outIndex = OutIndex;
		inId = InId;
		inIndex = InIndex;
		verts = Verts;
		colorId = ColorId;
	}
}
