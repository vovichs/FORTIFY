using UnityEngine;

public class wall : ConditionalCheck
{
	public enum State
	{
		full,
		right,
		left
	}

	public State state;

	public MeshFilter mf;

	public socket wallFemale;

	public socket floorTop;

	public socket floorMid;

	public socket stability1;

	public socket stability2;

	public static Vector3 boxColSize;

	public override void RunCheck()
	{
		BuilderPart owner = block.owner;
		if (boxColSize == Vector3.zero)
		{
			boxColSize = (owner.col as BoxCollider).size;
		}
		socket socketFromType = GetSocketFromType(block, socket.Type.neighbor);
		if (socketFromType != null && socketFromType.connections.Count > 0)
		{
			if (WallNeighborCheck(block, socketFromType, right: true))
			{
				if (state != State.right)
				{
					state = State.right;
					setWallMesh(owner, 1);
				}
			}
			else if (WallNeighborCheck(block, socketFromType, right: false))
			{
				if (state != State.left)
				{
					state = State.left;
					setWallMesh(owner, 2);
				}
			}
			else if (state != 0)
			{
				state = State.full;
				setWallMesh(owner, 0);
			}
		}
		else if (state != 0)
		{
			state = State.full;
			setWallMesh(owner, 0);
		}
	}

	private bool WallNeighborCheck(block block, socket socket, bool right)
	{
		if (wallFemale.CheckSocketOccupied())
		{
			return false;
		}
		if (floorTop.CheckSocketOccupied())
		{
			return false;
		}
		if (floorMid.CheckSocketOccupied())
		{
			return false;
		}
		if (right)
		{
			if (stability1.CheckSocketOccupied())
			{
				return false;
			}
		}
		else if (stability2.CheckSocketOccupied())
		{
			return false;
		}
		for (int i = 0; i < socket.connections.Count; i++)
		{
			block owner = socket.connections[i].owner;
			if (!owner || !owner.roof)
			{
				continue;
			}
			Vector3 right2 = owner.owner._transform.right;
			Vector3 forward = block.owner._transform.forward;
			if (right)
			{
				if (!owner.tri && Vector3.Angle(forward, right2) < 10f)
				{
					return true;
				}
				if (owner.tri && Vector3.Angle(forward, right2) < 40f)
				{
					return true;
				}
			}
			else
			{
				if (!owner.tri && Vector3.Angle(forward, -right2) < 10f)
				{
					return true;
				}
				if (owner.tri && Vector3.Angle(forward, -right2) < 40f)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void setWallMesh(BuilderPart bp, int meshIndex)
	{
		mf.sharedMesh = MGMT.inst.wallMesh[meshIndex];
		UnityEngine.Object.Destroy(bp.col);
		if (meshIndex == 0)
		{
			bp.col = bp.gameObject.AddComponent<BoxCollider>();
			(bp.col as BoxCollider).size = boxColSize;
		}
		else
		{
			bp.col = bp.gameObject.AddComponent<MeshCollider>();
			(bp.col as MeshCollider).convex = true;
		}
	}

	private bool CheckNamedSocketOccupied(block block, string named)
	{
		for (int i = 0; i < block.sockets.Length; i++)
		{
			if (block.sockets[i].name == named)
			{
				if (block.sockets[i].connections.Count > 0)
				{
					return true;
				}
				return false;
			}
		}
		return false;
	}

	private socket GetSocketFromType(block block, socket.Type type)
	{
		for (int i = 0; i < block.sockets.Length; i++)
		{
			if (block.sockets[i].socketType == type)
			{
				return block.sockets[i];
			}
		}
		return null;
	}
}
