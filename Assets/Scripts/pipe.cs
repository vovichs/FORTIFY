using System.Collections.Generic;
using UnityEngine;

public class pipe : wire
{
	[Header("__________________________")]
	public MeshFilter mf;

	public MeshRenderer rend;

	public Transform _transform;

	public Transform colliderParent;

	public GameObject pipeCollider;

	private float handleLength = 0.1f;

	private int subdivs = 5;

	private static int sides;

	private float pipeRadius = 0.015f;

	private float ioSegLength = 0.03f;

	private bool placing;

	private static List<Vector3> linePoints;

	private static List<Vector3> pipePoints;

	private static List<Vector3> handles;

	private static List<Vector3> pipeSegments;

	private static float[] angles;

	private static Vector3[] points;

	static pipe()
	{
		sides = 6;
		linePoints = new List<Vector3>();
		pipePoints = new List<Vector3>();
		handles = new List<Vector3>();
		pipeSegments = new List<Vector3>();
		angles = new float[sides];
		for (int i = 0; i < sides; i++)
		{
			angles[i] = Mathf.InverseLerp(0f, sides - 1, i) * 360f;
		}
	}

	private void OnDestroy()
	{
		destroyMesh();
		BuilderSystem.wireList.Remove(this);
	}

	public override void setMat(int ind, bool circuit)
	{
		colorIndex = ind;
		if (wiring.inst.wireFlow)
		{
			rend.sharedMaterial = wiring.inst.pipeMatColorsFlow[ind];
		}
		else
		{
			rend.sharedMaterial = wiring.inst.pipeMatColors[ind];
		}
		if (circuit)
		{
			circuitColor(ind);
		}
	}

	public override void highlightMat(bool state)
	{
		if (lockGlow && !state)
		{
			return;
		}
		if (state)
		{
			if (wiring.inst.wireFlow)
			{
				rend.sharedMaterial = wiring.inst.wireMatGlowFlow;
			}
			else
			{
				rend.sharedMaterial = wiring.inst.wireMatGlow;
			}
		}
		else if (wiring.inst.wireFlow)
		{
			rend.sharedMaterial = wiring.inst.pipeMatColorsFlow[colorIndex];
		}
		else
		{
			rend.sharedMaterial = wiring.inst.pipeMatColors[colorIndex];
		}
	}

	public void buildPipe(bool _placing, bool undo)
	{
		placing = _placing;
		if (!placing)
		{
			foreach (Transform item in colliderParent)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		getPipePoints(undo);
		if (linePoints.Count > 1)
		{
			BuildMesh();
		}
	}

	public void getPipePoints(bool undo)
	{
		points = new Vector3[lr.positionCount];
		lr.GetPositions(points);
		linePoints = new List<Vector3>(points);
		if (undo)
		{
			linePoints.RemoveAt(endIndex());
		}
		if (linePoints.Count < 2)
		{
			destroyMesh();
			return;
		}
		if (placing)
		{
			if (linePoints.Count < 3)
			{
				pipePoints = linePoints;
				return;
			}
		}
		else
		{
			Vector3 forward = input.transform.forward;
			Vector3 item = linePoints[0] + forward * ioSegLength;
			linePoints.Insert(1, item);
			forward = output.transform.forward;
			item = linePoints[endIndex()] + forward * ioSegLength;
			linePoints.Insert(endIndex(), item);
		}
		handles.Clear();
		for (int i = 1; i < linePoints.Count; i++)
		{
			if (i == 1)
			{
				handles.Add(linePoints[0]);
				continue;
			}
			if (i == endIndex())
			{
				handles.Add(linePoints[i]);
				continue;
			}
			Vector3 value = linePoints[i] - linePoints[i - 1];
			Vector3 vector = (linePoints[i] + linePoints[i - 1]) / 2f;
			float magnitude = value.magnitude;
			if (!placing && magnitude > 0.02f)
			{
				Quaternion rot = Quaternion.LookRotation(value.normalized);
				addPipeCollider(vector, rot, magnitude);
			}
			if (handleLength > magnitude * 0.5f)
			{
				handles.Add(vector);
				handles.Add(vector);
				continue;
			}
			Vector3 a = Vector3.Normalize(value);
			Vector3 item2 = linePoints[i - 1] + a * handleLength;
			Vector3 item3 = linePoints[i] - a * handleLength;
			handles.Add(item2);
			handles.Add(item3);
		}
		pipePoints.Clear();
		int num = 0;
		for (int j = 0; j < linePoints.Count; j++)
		{
			if (j != 0)
			{
				if (j == endIndex())
				{
					pipePoints.Add(linePoints[j]);
					continue;
				}
				GetCurveSegments(handles[num], linePoints[j], handles[num + 1]);
				num += 2;
			}
		}
	}

	private void GetCurveSegments(Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3[] array = new Vector3[subdivs];
		for (int i = 0; i < subdivs; i++)
		{
			float num = Mathf.Clamp01((float)i / (float)subdivs);
			float num2 = 1f - num;
			array[i] = num2 * num2 * num2 * p1 + 3f * num2 * num2 * num * p2 + 3f * num2 * num * num * p2 + num * num * num * p3;
		}
		pipePoints.AddRange(array);
	}

	private int endIndex()
	{
		return linePoints.Count - 1;
	}

	private void BuildMesh()
	{
		destroyMesh();
		Mesh mesh = new Mesh();
		mesh.name = "pipeMesh";
		mesh.vertices = pipeVerts();
		mesh.triangles = pipeTris();
		mesh.uv = pipeUVs();
		mesh.RecalculateNormals();
		mf.sharedMesh = mesh;
	}

	private Vector3[] pipeVerts()
	{
		Vector3[] array = new Vector3[pipePoints.Count * sides];
		int num = 0;
		Vector3 rhs = Vector3.down;
		int num2 = pipePoints.Count - 1;
		for (int num3 = num2; num3 >= 0; num3--)
		{
			Vector3 zero = Vector3.zero;
			zero = ((num3 != 0 && num3 != num2) ? GetRingNormal(num3) : GetRingDirection(num3));
			Vector3 b = _transform.InverseTransformPoint(pipePoints[num3]);
			Quaternion lhs = Quaternion.LookRotation(zero);
			Vector3[] array2 = new Vector3[sides];
			int num4 = 0;
			float num5 = -1f;
			for (int i = 0; i < sides; i++)
			{
				Quaternion rotation = lhs * Quaternion.Euler(Vector3.forward * angles[i]);
				array2[i] = rotation * Vector3.down;
				if (num3 < num2)
				{
					float num6 = Vector3.Dot(array2[i], rhs);
					if (num6 > num5)
					{
						num4 = i;
						num5 = num6;
					}
				}
			}
			rhs = array2[num4];
			for (int j = 0; j < sides; j++)
			{
				array[num] = array2[offset(j, num4)] * pipeRadius + b;
				num++;
			}
		}
		return array;
	}

	private int offset(int num, int offset)
	{
		num += offset;
		if (num > 5)
		{
			return -(5 - num);
		}
		return num;
	}

	private int[] pipeTris()
	{
		int count = pipePoints.Count;
		int[] array = new int[(count - 1) * sides * 2 * 3];
		array[0] = sides - 1;
		array[1] = sides;
		array[2] = 0;
		int num = 3;
		int num2 = 0;
		while (num < array.Length - 6)
		{
			array[num] = num2 + sides;
			array[num + 1] = num2 + 1;
			array[num + 2] = num2;
			array[num + 3] = num2 + sides + 1;
			array[num + 4] = num2 + 1;
			array[num + 5] = num2 + sides;
			num += 6;
			num2++;
		}
		array[array.Length - 3] = sides * count - 1;
		array[array.Length - 2] = sides * (count - 1);
		array[array.Length - 1] = sides * (count - 1) - 1;
		return array;
	}

	private Vector2[] pipeUVs()
	{
		Vector2[] array = new Vector2[pipePoints.Count * sides];
		float num = 0f;
		int num2 = array.Length - 1;
		for (int i = 0; i < pipePoints.Count; i++)
		{
			if (i > 0)
			{
				num += Vector3.Distance(pipePoints[i], pipePoints[i - 1]);
			}
			int num3 = 0;
			while (num3 < sides)
			{
				array[num2] = new Vector2(num, 0f);
				array[num2 - 1] = new Vector2(num, 1f);
				num3 += 2;
				num2 -= 2;
			}
		}
		return array;
	}

	private Vector3 GetRingDirection(int i)
	{
		if (i > 0)
		{
			return (pipePoints[i] - pipePoints[i - 1]).normalized;
		}
		if (i < pipePoints.Count - 1)
		{
			return (pipePoints[i + 1] - pipePoints[i]).normalized;
		}
		return pipePoints[i].normalized;
	}

	private Vector3 GetRingNormal(int i)
	{
		Vector3 normalized = (pipePoints[i] - pipePoints[i - 1]).normalized;
		Vector3 normalized2 = (pipePoints[i + 1] - pipePoints[i]).normalized;
		return normalized + normalized2;
	}

	public override void showWire(bool state)
	{
		rend.enabled = state;
	}

	public void destroyMesh()
	{
		if (mf.sharedMesh != null)
		{
			UnityEngine.Object.DestroyImmediate(mf.sharedMesh, allowDestroyingAssets: true);
		}
	}

	private void addPipeCollider(Vector3 pos, Quaternion rot, float length)
	{
		Object.Instantiate(pipeCollider, pos, rot, colliderParent).GetComponent<BoxCollider>().size = new Vector3(0.03f, 0.03f, length);
	}
}
