using System.Collections.Generic;
using UnityEngine;

public class StabilityEntity : MonoBehaviour
{
	public class Support
	{
		public StabilityEntity parent;

		public socket sock;

		public float factor = 1f;

		public Support(StabilityEntity parent, socket sock, float factor)
		{
			this.parent = parent;
			this.sock = sock;
			this.factor = factor;
		}

		public StabilityEntity SupportEntity(StabilityEntity ignoreEntity = null)
		{
			StabilityEntity stabilityEntity = null;
			for (int i = 0; i < sock.connections.Count; i++)
			{
				if (sock.connections[i] == null || sock.connections[i].support == 0f)
				{
					continue;
				}
				StabilityEntity stabilityEntity2 = sock.connections[i].owner.stabilityEntity;
				if (stabilityEntity2 != null && stabilityEntity2 != parent && stabilityEntity2 != ignoreEntity && !stabilityEntity2.block.destroyed)
				{
					if (!stabilityEntity)
					{
						stabilityEntity = stabilityEntity2;
					}
					else if (stabilityEntity2.cachedDistanceFromGround <= stabilityEntity.cachedDistanceFromGround && (stabilityEntity2.cachedDistanceFromGround != stabilityEntity.cachedDistanceFromGround || !stabilityEntity2.block.roof))
					{
						stabilityEntity = stabilityEntity2;
					}
				}
			}
			return stabilityEntity;
		}
	}

	public class StabilityCheckWorkQueue
	{
		protected void RunJob(StabilityEntity entity)
		{
			entity.StabilityCheck();
		}

		protected bool ShouldAdd(StabilityEntity entity)
		{
			return true;
		}
	}

	public static StabilityCheckWorkQueue stabilityCheckQueue;

	public block block;

	private float stabilitySaved = -1f;

	public float stabilityCalc;

	public int cachedDistanceFromGround = int.MaxValue;

	public bool grounded;

	private int stabilityStrikes;

	private List<Support> supports;

	private static int strikes;

	private static float collapse;

	private static float accuracy;

	public static float stabilityqueue;

	static StabilityEntity()
	{
		strikes = 10;
		collapse = 0.05f;
		accuracy = 0.001f;
		stabilityqueue = 6f;
		stabilityCheckQueue = new StabilityCheckWorkQueue();
	}

	public void UpdateStability()
	{
		
	}

	public void StabilityCheck()
	{
		if (!(block == null) && !block.destroyed)
		{
			if (supports == null)
			{
				InitializeSupports();
			}
			bool flag = false;
			int num = DistanceFromGround();
			if (num != cachedDistanceFromGround)
			{
				cachedDistanceFromGround = num;
			}
			float num2 = SupportValue();
			if (Mathf.Abs(stabilityCalc - num2) > accuracy)
			{
				stabilityCalc = num2;
				flag = true;
			}
			if (flag)
			{
				UpdateConnectedStability();
				UpdateStability();
			}
			if (num2 >= collapse)
			{
				updateBlockState();
				stabilityStrikes = 0;
			}
			else if (stabilityStrikes >= strikes)
			{
				updateBlockState();
			}
			else
			{
				UpdateStability();
				stabilityStrikes++;
			}
		}
	}

	private void updateBlockState()
	{
		BuilderPart owner = block.owner;
		if (stabilityCalc != stabilitySaved)
		{
			stabilitySaved = stabilityCalc;
			owner.stability = stabilityCalc;
			if (Stability.inst.raidMode)
			{
				if (Stability.inst.SetMaterialCheck(owner))
				{
					owner.raidDestroy(Vector3.zero);
				}
			}
			else if (Stability.inst.colorView)
			{
				owner.SetStabilityMat();
			}
			if (owner.mouseEnter)
			{
				owner.UpdateInfo();
			}
		}
		else
		{
			stabilitySaved = stabilityCalc;
			owner.stability = stabilityCalc;
		}
	}

	public int DistanceFromGround(StabilityEntity ignoreEntity = null)
	{
		if (grounded || supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < supports.Count; i++)
		{
			StabilityEntity stabilityEntity = supports[i].SupportEntity();
			if (!(stabilityEntity == null) && !stabilityEntity.block.destroyed)
			{
				int num2 = stabilityEntity.CachedDistanceFromGround(ignoreEntity);
				if (num2 != int.MaxValue)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	public int CachedDistanceFromGround(StabilityEntity ignoreEntity = null)
	{
		if (grounded || supports == null)
		{
			return 1;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		int num = int.MaxValue;
		for (int i = 0; i < supports.Count; i++)
		{
			StabilityEntity stabilityEntity = supports[i].SupportEntity();
			if (!(stabilityEntity == null) && !stabilityEntity.block.destroyed)
			{
				int num2 = stabilityEntity.cachedDistanceFromGround;
				if (num2 != int.MaxValue)
				{
					num = Mathf.Min(num, num2 + 1);
				}
			}
		}
		return num;
	}

	public float SupportValue(StabilityEntity ignoreEntity = null)
	{
		if (grounded)
		{
			return 1f;
		}
		if (supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < supports.Count; i++)
		{
			Support support = supports[i];
			StabilityEntity stabilityEntity = support.SupportEntity();
			if (stabilityEntity != null)
			{
				float num2 = stabilityEntity.CachedSupportValue(ignoreEntity);
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	public float CachedSupportValue(StabilityEntity ignoreEntity = null)
	{
		if (grounded)
		{
			return 1f;
		}
		if (supports == null)
		{
			return 1f;
		}
		if (ignoreEntity == null)
		{
			ignoreEntity = this;
		}
		float num = 0f;
		for (int i = 0; i < supports.Count; i++)
		{
			Support support = supports[i];
			StabilityEntity stabilityEntity = support.SupportEntity(ignoreEntity);
			if (stabilityEntity != null)
			{
				float num2 = stabilityEntity.stabilityCalc;
				if (num2 != 0f)
				{
					num += num2 * support.factor;
				}
			}
		}
		return Mathf.Clamp01(num);
	}

	public void InitializeSupports()
	{
		supports = new List<Support>();
		if (grounded)
		{
			return;
		}
		for (int i = 0; i < block.sockets.Length; i++)
		{
			socket socket = block.sockets[i];
			if (socket.male && (socket.socketType == socket.Type.construction || socket.socketType == socket.Type.stability))
			{
				supports.Add(new Support(this, socket, socket.support));
			}
		}
	}

	public void UpdateConnectedStability()
	{
		for (int i = 0; i < block.sockets.Length; i++)
		{
			socket socket = block.sockets[i];
			if (!socket.female)
			{
				continue;
			}
			for (int j = 0; j < socket.connections.Count; j++)
			{
				StabilityEntity stabilityEntity = socket.connections[j].owner.stabilityEntity;
				if (stabilityEntity != null && !stabilityEntity.block.destroyed)
				{
					stabilityEntity.UpdateStability();
				}
			}
		}
	}

	public void ResetState()
	{
		stabilityCalc = 0f;
		cachedDistanceFromGround = int.MaxValue;
		supports = null;
		stabilityStrikes = 0;
	}
}
