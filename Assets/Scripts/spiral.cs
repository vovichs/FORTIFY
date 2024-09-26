using UnityEngine;

public class spiral : ConditionalCheck
{
	public MeshCollider landingCol;

	public override void RunCheck()
	{
		socket socketNamed = block.GetSocketNamed("stairs female");
		socket socketNamed2 = block.GetSocketNamed("floor female");
		if ((socketNamed != null && socketNamed.connections.Count > 0) || (socketNamed2 != null && socketNamed2.connections.Count > 0))
		{
			swapSpiralMesh(landing: false);
		}
		else
		{
			swapSpiralMesh(landing: true);
		}
	}

	public void swapSpiralMesh(bool landing)
	{
		MeshFilter component = GetComponent<MeshFilter>();
		if (block.tri)
		{
			if (landing)
			{
				if (!(component.sharedMesh == MGMT.inst.spiralTri[1]))
				{
					component.sharedMesh = MGMT.inst.spiralTri[1];
					landingCol.enabled = true;
				}
			}
			else if (!(component.sharedMesh == MGMT.inst.spiralTri[0]))
			{
				component.sharedMesh = MGMT.inst.spiralTri[0];
				landingCol.enabled = false;
			}
		}
		else if (landing)
		{
			if (!(component.sharedMesh == MGMT.inst.spiral[1]))
			{
				component.sharedMesh = MGMT.inst.spiral[1];
				landingCol.enabled = true;
			}
		}
		else if (!(component.sharedMesh == MGMT.inst.spiral[0]))
		{
			component.sharedMesh = MGMT.inst.spiral[0];
			landingCol.enabled = false;
		}
	}
}
