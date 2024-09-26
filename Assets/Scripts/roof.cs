using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class roof : ConditionalCheck
{
	public enum AngleType
	{
		Concave120 = -120,
		Concave90 = -90,
		Concave60 = -60,
		Concave30 = -30,
		None = -1,
		Straight = 0,
		Convex60 = 60,
		Convex90 = 90,
		Convex120 = 120
	}

	public enum ShapeType
	{
		Any = -1,
		Square,
		Triangle
	}

	public enum Side
	{
		Right,
		Left
	}

	public MeshFilter meshFilter;

	public Mesh[] roofMesh;

	private int[] meshArr = new int[9]
	{
		1,
		0,
		0,
		0,
		0,
		0,
		0,
		0,
		0
	};

	private int[] newMeshArr;

	public socket socketLeft;

	public socket socketRight;

	public override void RunCheck()
	{
		if (block.tri)
		{
			newMeshArr = new int[8]
			{
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0
			};
			newMeshArr[4] = (RightCheck(AngleType.Straight, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[1] = (LeftCheck(AngleType.Concave30, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[0] = (LeftCheck(AngleType.Concave30, ShapeType.Square) ? 1 : 0);
			newMeshArr[3] = (RightCheck(AngleType.Concave30, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[4] += (RightCheck(AngleType.Concave30, ShapeType.Square) ? 1 : 0);
			newMeshArr[0] += (LeftCheck(AngleType.Concave60, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[1] += (LeftCheck(AngleType.Concave60, ShapeType.Square) ? 1 : 0);
			newMeshArr[4] += (RightCheck(AngleType.Concave60, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[3] += (RightCheck(AngleType.Concave60, ShapeType.Square) ? 1 : 0);
			newMeshArr[0] += (LeftCheck(AngleType.Concave120, ShapeType.Triangle) ? 1 : 0);
			bool flag = RightCheck(AngleType.Concave120, ShapeType.Triangle);
			newMeshArr[4] += (flag ? 1 : 0);
			newMeshArr[5] = (flag ? 1 : 0);
			newMeshArr[6] = (LeftCheck(AngleType.None, ShapeType.Any) ? 1 : 0);
			newMeshArr[7] = (RightCheck(AngleType.None, ShapeType.Any) ? 1 : 0);
		}
		else
		{
			newMeshArr = new int[9]
			{
				0,
				0,
				0,
				1,
				0,
				0,
				0,
				0,
				0
			};
			if (FindSocket("wall female").connections.Count == 0)
			{
				newMeshArr[4] = (LeftCheck(AngleType.Convex90, ShapeType.Any) ? 1 : 0);
				newMeshArr[2] = (RightCheck(AngleType.Convex90, ShapeType.Any) ? 1 : 0);
				if (newMeshArr[4] > 0 || newMeshArr[2] > 0)
				{
					newMeshArr[3] = 0;
				}
			}
			newMeshArr[1] = (LeftCheck(AngleType.Concave60, ShapeType.Square) ? 1 : 0);
			newMeshArr[0] = (LeftCheck(AngleType.Concave60, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[6] = (RightCheck(AngleType.Concave60, ShapeType.Triangle) ? 1 : 0);
			newMeshArr[0] += (LeftCheck(AngleType.Concave90, ShapeType.Square) ? 1 : 0);
			newMeshArr[6] += (RightCheck(AngleType.Concave90, ShapeType.Square) ? 1 : 0);
			newMeshArr[1] += (LeftCheck(AngleType.Concave120, ShapeType.Square) ? 1 : 0);
			newMeshArr[5] = (RightCheck(AngleType.Concave120, ShapeType.Square) ? 1 : 0);
			newMeshArr[7] = (LeftCheck(AngleType.None, ShapeType.Any) ? 1 : 0);
			newMeshArr[8] = (RightCheck(AngleType.None, ShapeType.Any) ? 1 : 0);
		}
		if (!newMeshArr.SequenceEqual(meshArr))
		{
			meshArr = newMeshArr;
			setMesh();
		}
	}

	private bool LeftCheck(AngleType angle, ShapeType shape)
	{
		if (angle == AngleType.None)
		{
			for (int i = 0; i < socketLeft.connections.Count; i++)
			{
				if (socketLeft.connections[i].name == "neighbor 3")
				{
					return false;
				}
			}
			return true;
		}
		bool result = false;
		for (int j = 0; j < socketLeft.connections.Count; j++)
		{
			socket socket = socketLeft.connections[j];
			if (!socket)
			{
				return result;
			}
			block owner = socket.owner;
			if (!owner.roof || socket.name != "neighbor 3" || (shape == ShapeType.Square && owner.tri) || (shape == ShapeType.Triangle && !owner.tri) || owner.tier != block.tier)
			{
				continue;
			}
			float num = Vector3.SignedAngle(block.owner._transform.right, owner.owner._transform.right, Vector3.up);
			if (num < (float)(angle - 10))
			{
				if (IsConvex(angle))
				{
					return false;
				}
			}
			else if (num <= (float)(angle + 10))
			{
				result = true;
			}
			else if (IsConvex(angle))
			{
				return false;
			}
		}
		return result;
	}

	private bool RightCheck(AngleType angle, ShapeType shape)
	{
		if (angle == AngleType.None)
		{
			for (int i = 0; i < socketRight.connections.Count; i++)
			{
				if (socketRight.connections[i].name == "neighbor 4")
				{
					return false;
				}
			}
			return true;
		}
		bool result = false;
		for (int j = 0; j < socketRight.connections.Count; j++)
		{
			socket socket = socketRight.connections[j];
			if (!socket)
			{
				return result;
			}
			block owner = socket.owner;
			if (!owner.roof || socket.name != "neighbor 4" || (shape == ShapeType.Square && owner.tri) || (shape == ShapeType.Triangle && !owner.tri) || owner.tier != block.tier)
			{
				continue;
			}
			float num = 0f - Vector3.SignedAngle(block.owner._transform.right, owner.owner._transform.right, Vector3.up);
			if (num < (float)(angle - 10))
			{
				if (IsConvex(angle))
				{
					return false;
				}
			}
			else if (num <= (float)(angle + 10))
			{
				result = true;
			}
			else if (IsConvex(angle))
			{
				return false;
			}
		}
		return result;
	}

	public new void setMesh()
	{
		List<CombineInstance> list = new List<CombineInstance>();
		bool flag = false;
		Transform transform = block.owner._transform;
		int num = roofMesh.Length;
		while (num-- > 0)
		{
			if (num != 0 || flag)
			{
				if (meshArr[num] == 0)
				{
					continue;
				}
				flag = true;
			}
			CombineInstance item = default(CombineInstance);
			item.mesh = roofMesh[num];
			item.transform = transform.worldToLocalMatrix * transform.localToWorldMatrix;
			list.Add(item);
		}
		meshFilter.mesh = new Mesh();
		meshFilter.mesh.CombineMeshes(list.ToArray());
		MeshCollider component = GetComponent<MeshCollider>();
		component.sharedMesh = meshFilter.sharedMesh;
		component.convex = false;
	}

	private bool IsConvex(AngleType angle)
	{
		return angle > (AngleType)10;
	}

	public socket FindSocket(string socketName)
	{
		for (int i = 0; i < block.sockets.Length; i++)
		{
			if (block.sockets[i].name == socketName)
			{
				return block.sockets[i];
			}
		}
		return null;
	}
}
