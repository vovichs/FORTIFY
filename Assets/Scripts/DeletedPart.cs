using UnityEngine;

public class DeletedPart : UndoPart
{
	public int id;

	public int instId;

	public Quaternion rot;

	public Vector3 pos;

	public float floor;

	public int tier;

	public string code;

	public int stage;

	public bool check;

	public int val;

	public DeletedPart(int Id, int InstId, Quaternion Rot, Vector3 Pos, float Floor, int Stage, string Code, bool Check, int Val)
	{
		id = Id;
		instId = InstId;
		rot = Rot;
		pos = Pos;
		floor = Floor;
		stage = Stage;
		code = Code;
		check = Check;
		val = Val;
	}

	public DeletedPart(int Id, int InstId, Quaternion Rot, Vector3 Pos, float Floor, int Tier, int Stage)
	{
		id = Id;
		instId = InstId;
		rot = Rot;
		pos = Pos;
		floor = Floor;
		tier = Tier;
		stage = Stage;
	}
}
