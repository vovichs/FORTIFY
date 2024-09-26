using System.Collections.Generic;
using UnityEngine;

public class socket : MonoBehaviour
{
	public enum Type
	{
		construction,
		neighbor,
		stability
	}

	public enum Part
	{
		_,
		foundation,
		stairs,
		wall,
		floor
	}

	public Type socketType;

	public Part partType;

	public block owner;

	public Transform _transform;

	public float support = 1f;

	public List<socket> connections = new List<socket>(8);

	public bool male;

	public bool female;

	public bool monogamous;

	public bool connectCheck(socket other, Vector3 thisPos)
	{
		if (compatibleCheck(other))
		{
			return distanceCheck(other, thisPos);
		}
		return false;
	}

	public bool compatibleCheck(socket other)
	{
		if (other == null)
		{
			return false;
		}
		if (socketType != other.socketType)
		{
			return false;
		}
		if (socketType == Type.construction)
		{
			if (other.partType != partType)
			{
				return false;
			}
			if (!other.male && !male)
			{
				return false;
			}
			if (!other.female && !female)
			{
				return false;
			}
		}
		return true;
	}

	public bool distanceCheck(socket other, Vector3 thisPos)
	{
		if (socketType == Type.stability)
		{
			Vector3 position = other._transform.position;
			if (Vector2.Distance(v2(position), v2(thisPos)) <= 0.01f)
			{
				if (!other.owner.halfHeight && !owner.halfHeight)
				{
					return Mathf.Abs(position.y - thisPos.y) < 0.51f;
				}
				return Mathf.Abs(position.y - thisPos.y) < 0.26f;
			}
			return false;
		}
		return Vector3.Distance(other._transform.position, _transform.position) <= 0.01f;
	}

	public bool CheckSocketOccupied()
	{
		if (connections.Count > 0)
		{
			return true;
		}
		return false;
	}

	private Vector2 v2(Vector3 pos)
	{
		return new Vector2(pos.x, pos.z);
	}
}
