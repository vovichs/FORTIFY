using System.Collections.Generic;
using UnityEngine;

public class Proximity : MonoBehaviour
{
	private struct ProximityInfo
	{
		public bool hit;

		public bool connection;

		public Line line;

		public float sqrDist;
	}

	private struct Line
	{
		public Vector3 point0;

		public Vector3 point1;

		public Line(Vector3 point0, Vector3 point1)
		{
			this.point0 = point0;
			this.point1 = point1;
		}

		public Line(Vector3 origin, Vector3 direction, float length)
		{
			point0 = origin;
			point1 = origin + direction * length;
		}

		public Vector3 ClosestPoint(Vector3 pos)
		{
			Vector3 a = point1 - point0;
			float magnitude = a.magnitude;
			Vector3 vector = a / magnitude;
			return point0 + Mathf.Clamp(Vector3.Dot(pos - point0, vector), 0f, magnitude) * vector;
		}

		public float Distance(Vector3 pos)
		{
			return (pos - ClosestPoint(pos)).magnitude;
		}

		public float SqrDistance(Vector3 pos)
		{
			return (pos - ClosestPoint(pos)).sqrMagnitude;
		}
	}

	public static Proximity inst;

	public LayerMask mask;

	private Vector3 position;

	private Vector3 position2;

	private List<BuilderPart> list = new List<BuilderPart>();

	private float range = 1.16f;

	public void Awake()
	{
		inst = this;
	}

	public bool Check(BuilderPart bp)
	{
		if (!placeOptions.proximityCheck)
		{
			return true;
		}
		block block = bp.block;
		if (!bp.origin)
		{
			position = bp._transform.position;
		}
		else
		{
			position = bp.origin.position;
		}
		list.Clear();
		Collider[] array = Physics.OverlapSphere(position, range, mask, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			BuilderPart component = array[i].transform.root.GetComponent<BuilderPart>();
			if (component != bp && (component.wall || component.found))
			{
				list.Add(component);
			}
		}
		for (int j = 0; j < list.Count; j++)
		{
			block block2 = list[j].block;
			if (!list[j].origin)
			{
				position2 = list[j]._transform.position;
			}
			else
			{
				position2 = list[j].origin.position;
			}
			ProximityInfo proximity = GetProximity(block, block2);
			ProximityInfo proximity2 = GetProximity(block2, block);
			ProximityInfo proximityInfo = default(ProximityInfo);
			proximityInfo.hit = (proximity.hit || proximity2.hit);
			proximityInfo.connection = (proximity.connection || proximity2.connection);
			ProximityInfo proximityInfo2 = proximityInfo;
			if (proximity.sqrDist > proximity2.sqrDist)
			{
				proximityInfo2.line = proximity2.line;
				proximityInfo2.sqrDist = proximity2.sqrDist;
			}
			else
			{
				proximityInfo2.line = proximity.line;
				proximityInfo2.sqrDist = proximity.sqrDist;
			}
			if (!proximityInfo2.hit)
			{
				continue;
			}
			Vector3 vector = proximityInfo2.line.point1 - proximityInfo2.line.point0;
			float num = Magnitude2D(vector);
			if (Mathf.Abs(vector.y) <= 0.4966f && num <= 0.4966f)
			{
				if (bp.found)
				{
					return false;
				}
				if (num > 0.1f)
				{
					return false;
				}
			}
		}
		return true;
	}

	private ProximityInfo GetProximity(block block1, block block2)
	{
		ProximityInfo proximityInfo = default(ProximityInfo);
		proximityInfo.hit = false;
		proximityInfo.connection = false;
		proximityInfo.line = default(Line);
		proximityInfo.sqrDist = 1000f;
		ProximityInfo proximityInfo2 = proximityInfo;
		for (int i = 0; i < block1.sockets.Length; i++)
		{
			if (block1.sockets[i].socketType != 0 || block1.sockets[i].partType != socket.Part.wall)
			{
				continue;
			}
			Vector3 point = block1.sockets[i]._transform.position;
			for (int j = 0; j < block2.proximities.Length; j++)
			{
				Line line = new Line(point, block2.proximities[j].position);
				float sqrMagnitude = (line.point1 - line.point0).sqrMagnitude;
				if (Magnitude2D(line.point1 - line.point0) < 0.005f)
				{
					proximityInfo2 = default(ProximityInfo);
					proximityInfo2.connection = true;
					return proximityInfo2;
				}
				if (sqrMagnitude < proximityInfo2.sqrDist)
				{
					proximityInfo2.hit = true;
					proximityInfo2.line = line;
					proximityInfo2.sqrDist = sqrMagnitude;
				}
			}
		}
		return proximityInfo2;
	}

	private float Magnitude2D(Vector3 v)
	{
		return Mathf.Sqrt(v.x * v.x + v.z * v.z);
	}
}
