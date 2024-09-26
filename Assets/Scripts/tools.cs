using UnityEngine;

public class tools
{
	public enum mode
	{
		add,
		remove,
		check
	}

	public static Collider[] hit;

	public static mode _mode;

	public static int blockNeighborCheck(Vector3 pos, float radius)
	{
		hit = new Collider[46];
		return Physics.OverlapSphereNonAlloc(pos, radius, hit, 16384, QueryTriggerInteraction.Collide);
	}

	public static int sphereCheck(Vector3 pos, float radius)
	{
		hit = new Collider[46];
		return Physics.OverlapSphereNonAlloc(pos, radius, hit, 1);
	}

	public static int deployOnCheck(BuilderPart bp, Vector3 pos, float radius)
	{
		hit = new Collider[1];
		bp.gameObject.layer = 2;
		int result = Physics.OverlapSphereNonAlloc(pos, radius, hit, 1);
		bp.setRaycastLayer();
		return result;
	}

	private static void getColliderFromDir(BuilderPart bp, Vector3 dir, bool state)
	{
		float num = Mathf.RoundToInt(Mathf.Atan2(dir.x, dir.z) * 57.29578f);
		int num2 = -1;
		if (bp.tri)
		{
			if (num == 90f)
			{
				num2 = 0;
			}
			else if (num == -150f)
			{
				num2 = 1;
			}
			else if (num == -30f)
			{
				num2 = 2;
			}
		}
		else if (num == -90f)
		{
			num2 = 0;
		}
		else if (num == 90f)
		{
			num2 = 1;
		}
		else if (num == 0f)
		{
			num2 = 2;
		}
		else if (Mathf.Abs(num) == 180f)
		{
			num2 = 3;
		}
		if (num2 > -1)
		{
			bp.block.baseCols[num2].gameObject.SetActive(!state);
		}
	}

	public static float FlatDistance(Vector3 pos1, Vector3 pos2)
	{
		pos1.y = pos1.z;
		pos2.y = pos2.z;
		return Vector2.Distance(pos1, pos2);
	}

	public static float VerticalDistance(Vector3 pos1, Vector3 pos2)
	{
		return Mathf.Abs(pos1.y - pos2.y);
	}

	private static Vector2 getV2(Vector3 pos)
	{
		return new Vector2(pos.x, pos.z);
	}
}
