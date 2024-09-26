using UnityEngine;

public class found : ConditionalCheck
{
	public PlacePartFoundation[] foundPlaceCols;

	public MeshFilter mf;

	public override void RunCheck()
	{
		if (Stability.inst.raidMode)
		{
			return;
		}
		for (int i = 0; i < foundPlaceCols.Length; i++)
		{
			PlacePartFoundation placePartFoundation = foundPlaceCols[i];
			bool flag = true;
			for (int j = 0; j < placePartFoundation.sockets.Length; j++)
			{
				socket socket = placePartFoundation.sockets[j];
				for (int k = 0; k < socket.connections.Count; k++)
				{
					if (socket.connections[k] != null && socket.connections[k].partType == socket.Part.foundation)
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					break;
				}
			}
			if (placePartFoundation.gameObject.activeSelf != flag)
			{
				placePartFoundation.gameObject.SetActive(flag);
			}
		}
	}

	public override void setMesh()
	{
		Mesh mesh = null;
		bool flag = block.tier < 2;
		mesh = (flag ? ((!block.tri) ? MGMT.inst.foundWood : MGMT.inst.foundTriWood) : ((!block.tri) ? MGMT.inst.found : MGMT.inst.foundTri));
		if (!(mf.sharedMesh == mesh))
		{
			mf.sharedMesh = mesh;
			changeCol(flag);
		}
	}

	private void changeCol(bool wood)
	{
		if (block.tri)
		{
			(block.owner.col as MeshCollider).convex = !wood;
			int num = wood ? 1 : 0;
			(block.owner.col as MeshCollider).sharedMesh = MGMT.inst.foundTriCols[num];
			return;
		}
		block.owner.col.enabled = false;
		if (block.owner.col is MeshCollider)
		{
			block.owner.col = GetComponent<BoxCollider>();
		}
		else
		{
			block.owner.col = GetComponent<MeshCollider>();
		}
		block.owner.col.enabled = true;
	}
}
