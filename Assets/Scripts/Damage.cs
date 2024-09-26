using System;
using System.Collections.Generic;

public class Damage
{
	public enum DamageType
	{
		Heat,
		Bullet,
		Blunt,
		Stab,
		Explosion
	}

	[Serializable]
	public class DamageTypeEntry
	{
		public DamageType type;

		public float amount;
	}

	public class DamageTypeList
	{
		public float[] types = new float[5];

		public void Add(DamageType index, float amount)
		{
			Set(index, Get(index) + amount);
		}

		public void Add(List<DamageTypeEntry> entries)
		{
			foreach (DamageTypeEntry entry in entries)
			{
				Add(entry.type, entry.amount);
			}
		}

		public void Clear()
		{
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = 0f;
			}
		}

		public float Get(DamageType index)
		{
			return types[(int)index];
		}

		public bool Has(DamageType index)
		{
			return Get(index) > 0f;
		}

		public void Scale(DamageType index, float amount)
		{
			Set(index, Get(index) * amount);
		}

		public void ScaleAll(float amount)
		{
			for (int i = 0; i < types.Length; i++)
			{
				Scale((DamageType)i, amount);
			}
		}

		public void Set(DamageType index, float amount)
		{
			types[(int)index] = amount;
		}

		public float Total()
		{
			float num = 0f;
			for (int i = 0; i < types.Length; i++)
			{
				num += types[i];
			}
			return num;
		}
	}

	public static int damageTypeCount;

	public static float[,] tierProtection;

	static Damage()
	{
		tierProtection = new float[5, 5]
		{
			{
				0.5f,
				0.8f,
				0.8f,
				0.8f,
				0f
			},
			{
				0.9f,
				0.97f,
				0.98f,
				0.9f,
				0.1f
			},
			{
				1f,
				0.99f,
				0.98f,
				0.95f,
				0.5f
			},
			{
				1f,
				0.9999f,
				0.99f,
				0.98f,
				0.5f
			},
			{
				1f,
				0.9999f,
				0.99f,
				0.98f,
				0.5f
			}
		};
		damageTypeCount = Enum.GetValues(typeof(DamageType)).Length;
	}
}
