using UnityEngine;

public class ramp : ConditionalCheck
{
	private bool high = true;

	public GameObject deployCol;

	public MeshFilter mf;

	public Collider[] cols;

	[ContextMenu("set colliders")]
	private void setCollidersEditor()
	{
		cols = block.owner._transform.GetChild(0).GetComponents<Collider>();
	}

	public override void RunCheck()
	{
		socket socketNamed = block.GetSocketNamed("male mid 1");
		high = (socketNamed.connections.Count == 0);
		setMesh();
		cols[1].enabled = high;
	}

	public override void setMesh()
	{
		Mesh mesh = null;
		int tier = block.owner.tier;
		mesh = ((!high) ? MGMT.inst.rampLow[tier] : MGMT.inst.rampHigh[tier]);
		if (!(mf.sharedMesh == mesh))
		{
			mf.sharedMesh = mesh;
			(block.owner.col as MeshCollider).sharedMesh = mesh;
		}
	}
}
