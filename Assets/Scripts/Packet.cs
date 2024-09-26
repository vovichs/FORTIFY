using Steamworks;
using System;
using System.Collections.Generic;
using System.Text;

public class Packet
{
	public static List<byte> buildPacketBytes = new List<byte>();

	public static byte[] packetBytes;

	private static int readFrom;

	private static byte[] getBytes(string s)
	{
		return Encoding.UTF8.GetBytes(s);
	}

	private static byte[] getBytes(float f)
	{
		return BitConverter.GetBytes(f);
	}

	private static byte[] getBytes(int I32)
	{
		return BitConverter.GetBytes(I32);
	}

	private static string getString(byte[] bytes)
	{
		return new string(Encoding.UTF8.GetChars(bytes));
	}

	private static int getInt(byte[] b)
	{
		return BitConverter.ToInt32(b, 0);
	}

	private static float getFloat(byte[] b)
	{
		return BitConverter.ToSingle(b, 0);
	}

	public static void BuildPacket(int packetId)
	{
		buildPacketBytes.Clear();
		AddByte(packetId);
	}

	public static void AddByte(int i)
	{
		buildPacketBytes.Add((byte)i);
	}

	public static void AddInt(int i)
	{
		byte[] bytes = getBytes(i);
		buildPacketBytes.AddRange(new List<byte>(bytes));
	}

	public static void AddFloat(float f)
	{
		byte[] bytes = getBytes(f);
		buildPacketBytes.AddRange(new List<byte>(bytes));
	}

	public static void AddString(string s)
	{
		byte[] bytes = getBytes(s);
		AddInt(s.Length);
		buildPacketBytes.AddRange(new List<byte>(bytes));
	}

	public static void ReadPacket(byte[] packet)
	{
		packetBytes = packet;
		readFrom = 0;
	}

	public static int ReadByte()
	{
		byte result = packetBytes[readFrom];
		readFrom++;
		return result;
	}

	public static int ReadInt()
	{
		byte[] array = new byte[4];
		Array.Copy(packetBytes, readFrom, array, 0, 4);
		int @int = getInt(array);
		readFrom += 4;
		return @int;
	}

	public static float ReadFloat()
	{
		byte[] array = new byte[4];
		Array.Copy(packetBytes, readFrom, array, 0, 4);
		float @float = getFloat(array);
		readFrom += 4;
		return @float;
	}

	public static string ReadString()
	{
		int num = ReadInt();
		string @string = getString(new List<byte>(packetBytes).GetRange(0, num).ToArray());
		readFrom += num;
		return @string;
	}

	public static void SendPacket(CSteamID receiver)
	{
		SteamNetworking.SendP2PPacket(receiver, buildPacketBytes.ToArray(), (uint)buildPacketBytes.Count, EP2PSend.k_EP2PSendReliable);
	}

	public static void SendPacketUnreliable(CSteamID receiver)
	{
		SteamNetworking.SendP2PPacket(receiver, buildPacketBytes.ToArray(), (uint)buildPacketBytes.Count, EP2PSend.k_EP2PSendUnreliable);
	}
}
