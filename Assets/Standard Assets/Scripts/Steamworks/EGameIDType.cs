using System;

namespace Steamworks
{
	[Serializable]
	public struct CGameID : IEquatable<CGameID>, IComparable<CGameID>
	{
		public enum EGameIDType
		{
			k_EGameIDTypeApp,
			k_EGameIDTypeGameMod,
			k_EGameIDTypeShortcut,
			k_EGameIDTypeP2P
		}

		public ulong m_GameID;

		public CGameID(ulong GameID)
		{
			m_GameID = GameID;
		}

		public CGameID(AppId_t nAppID)
		{
			m_GameID = 0uL;
			SetAppID(nAppID);
		}

		public CGameID(AppId_t nAppID, uint nModID)
		{
			m_GameID = 0uL;
			SetAppID(nAppID);
			SetType(EGameIDType.k_EGameIDTypeGameMod);
			SetModID(nModID);
		}

		public bool IsSteamApp()
		{
			return Type() == EGameIDType.k_EGameIDTypeApp;
		}

		public bool IsMod()
		{
			return Type() == EGameIDType.k_EGameIDTypeGameMod;
		}

		public bool IsShortcut()
		{
			return Type() == EGameIDType.k_EGameIDTypeShortcut;
		}

		public bool IsP2PFile()
		{
			return Type() == EGameIDType.k_EGameIDTypeP2P;
		}

		public AppId_t AppID()
		{
			return new AppId_t((uint)(m_GameID & 0xFFFFFF));
		}

		public EGameIDType Type()
		{
			return (EGameIDType)((m_GameID >> 24) & 0xFF);
		}

		public uint ModID()
		{
			return (uint)((m_GameID >> 32) & uint.MaxValue);
		}

		public bool IsValid()
		{
			switch (Type())
			{
			case EGameIDType.k_EGameIDTypeApp:
				return AppID() != AppId_t.Invalid;
			case EGameIDType.k_EGameIDTypeGameMod:
				if (AppID() != AppId_t.Invalid)
				{
					return ((int)ModID() & int.MinValue) != 0;
				}
				return false;
			case EGameIDType.k_EGameIDTypeShortcut:
				return ((int)ModID() & int.MinValue) != 0;
			case EGameIDType.k_EGameIDTypeP2P:
				if (AppID() == AppId_t.Invalid)
				{
					return ((int)ModID() & int.MinValue) != 0;
				}
				return false;
			default:
				return false;
			}
		}

		public void Reset()
		{
			m_GameID = 0uL;
		}

		public void Set(ulong GameID)
		{
			m_GameID = GameID;
		}

		private void SetAppID(AppId_t other)
		{
			m_GameID = (ulong)(((long)m_GameID & -16777216L) | ((long)(uint)other & 16777215L));
		}

		private void SetType(EGameIDType other)
		{
			m_GameID = (ulong)(((long)m_GameID & -4278190081L) | (((long)other & 255L) << 24));
		}

		private void SetModID(uint other)
		{
			m_GameID = (ulong)((long)(m_GameID & uint.MaxValue) | (((long)other & 4294967295L) << 32));
		}

		public override string ToString()
		{
			return m_GameID.ToString();
		}

		public override bool Equals(object other)
		{
			if (other is CGameID)
			{
				return this == (CGameID)other;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_GameID.GetHashCode();
		}

		public static bool operator ==(CGameID x, CGameID y)
		{
			return x.m_GameID == y.m_GameID;
		}

		public static bool operator !=(CGameID x, CGameID y)
		{
			return !(x == y);
		}

		public static explicit operator CGameID(ulong value)
		{
			return new CGameID(value);
		}

		public static explicit operator ulong(CGameID that)
		{
			return that.m_GameID;
		}

		public bool Equals(CGameID other)
		{
			return m_GameID == other.m_GameID;
		}

		public int CompareTo(CGameID other)
		{
			return m_GameID.CompareTo(other.m_GameID);
		}
	}
}
