using UnityEngine;

public class MovedPart : UndoPart
{
	public BuilderPart bp;

	public Quaternion rot;

	public Vector3 pos;

	public float lvl;

	public int instId;

	public MovedPart(BuilderPart Bp, Quaternion Rot, Vector3 Pos, float Lvl, int InstId)
	{
		bp = Bp;
		rot = Rot;
		pos = Pos;
		lvl = Lvl;
		instId = InstId;
	}

	public MovedPart(BuilderPart Bp)
	{
		bp = Bp;
		rot = bp._transform.rotation;
		pos = bp._transform.position;
		lvl = bp.level;
		instId = bp.instId;
	}
}
