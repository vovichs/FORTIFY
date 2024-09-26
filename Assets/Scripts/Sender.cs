using Steamworks;
using System;
using UnityEngine;

public class Sender : MonoBehaviour
{
	public static BuilderPart sendObj;

	private static Transform camParent;

	private static Transform cam;

	public static CSteamID sendId;

	public static int sendInt;

	public static float sendFloat;

	public static bool placeSound = true;

	private int waitCount;

	private void Start()
	{
		cam = BuilderSystem.inst._transform;
		camParent = cam.transform.parent;
	}

	private void FixedUpdate()
	{
		if (Multiplayer.inst.connected)
		{
			waitCount++;
			if (waitCount > 2)
			{
				waitCount = 0;
				send(2);
			}
		}
	}

	public static void send(int id)
	{
		Packet.BuildPacket(id);
		switch (id)
		{
		case 2:
		{
			Packet.AddByte(Multiplayer.myIndex);
			Packet.AddFloat(camParent.position.x);
			Packet.AddFloat(camParent.position.y);
			Packet.AddFloat(camParent.position.z);
			Vector3 eulerAngles = camParent.eulerAngles;
			Packet.AddFloat(eulerAngles.x);
			Packet.AddFloat(eulerAngles.y);
			Packet.AddFloat(eulerAngles.z);
			Packet.AddFloat(cam.eulerAngles.x);
			break;
		}
		case 3:
		{
			Packet.AddInt(sendObj.id);
			Transform transform2 = sendObj._transform;
			Packet.AddFloat(transform2.position.x);
			Packet.AddFloat(transform2.position.y);
			Packet.AddFloat(transform2.position.z);
			Packet.AddFloat(transform2.eulerAngles.x);
			Packet.AddFloat(transform2.eulerAngles.y);
			Packet.AddFloat(transform2.eulerAngles.z);
			Packet.AddByte(sendObj.tier);
			Packet.AddFloat(sendObj.level);
			Packet.AddByte(sendObj.stage);
			break;
		}
		case 4:
		{
			int num3 = Multiplayer.syncObjList.IndexOf(sendObj);
			if (num3 == -1)
			{
				UnityEngine.Debug.Log("obj not in list");
				return;
			}
			Packet.AddInt(num3);
			Multiplayer.syncObjList.Remove(sendObj);
			if (placeSound)
			{
				Packet.AddInt(1);
			}
			else
			{
				Packet.AddInt(0);
			}
			break;
		}
		case 5:
		{
			int num5 = Multiplayer.syncObjList.IndexOf(sendObj);
			if (num5 == -1)
			{
				UnityEngine.Debug.Log("obj not in list");
				return;
			}
			Packet.AddInt(num5);
			Packet.AddInt(sendObj.tier);
			break;
		}
		case 7:
		{
			int num = Multiplayer.syncObjList.IndexOf(sendObj);
			if (num == -1)
			{
				UnityEngine.Debug.Log("obj not in list");
				return;
			}
			Packet.AddInt(num);
			Packet.AddFloat(sendFloat);
			break;
		}
		case 9:
		{
			int num4 = Multiplayer.syncObjList.IndexOf(sendObj);
			if (num4 == -1)
			{
				UnityEngine.Debug.Log("obj not in list");
				return;
			}
			Packet.AddInt(num4);
			Packet.AddInt(sendObj.stage);
			break;
		}
		case 10:
		{
			int num2 = Multiplayer.syncObjList.IndexOf(sendObj);
			if (num2 == -1)
			{
				UnityEngine.Debug.Log("obj not in list");
				return;
			}
			Packet.AddInt(num2);
			Transform transform3 = sendObj._transform;
			Packet.AddFloat(transform3.position.x);
			Packet.AddFloat(transform3.position.y);
			Packet.AddFloat(transform3.position.z);
			Packet.AddFloat(transform3.eulerAngles.x);
			Packet.AddFloat(transform3.eulerAngles.y);
			Packet.AddFloat(transform3.eulerAngles.z);
			Packet.AddFloat(sendObj.GetComponent<BuilderPart>().level);
			break;
		}
		case 11:
			Packet.AddInt(Multiplayer.myIndex);
			Packet.AddInt(sendInt);
			break;
		case 12:
			Packet.AddByte(Multiplayer.myIndex);
			if (sendObj != null)
			{
				Transform transform = sendObj._transform;
				Packet.AddFloat(transform.position.x);
				Packet.AddFloat(transform.position.y);
				Packet.AddFloat(transform.position.z);
				Packet.AddFloat(transform.eulerAngles.x);
				Packet.AddFloat(transform.eulerAngles.y);
				Packet.AddFloat(transform.eulerAngles.z);
			}
			else
			{
				Packet.AddFloat(0f);
				Packet.AddFloat(-100f);
				Packet.AddFloat(0f);
				Packet.AddFloat(0f);
				Packet.AddFloat(0f);
				Packet.AddFloat(0f);
			}
			break;
		}
		foreach (CSteamID connectId in Multiplayer.connectIds)
		{
			if (id == 2)
			{
				Packet.SendPacketUnreliable(connectId);
			}
			else
			{
				Packet.SendPacket(connectId);
			}
		}
	}

	public static void sendPart(BuilderPart bp, bool sendAll, bool placeSound)
	{
		Packet.BuildPacket(3);
		GameObject gameObject = bp.gameObject;
		Packet.AddInt(bp.id);
		Transform transform = bp._transform;
		Vector3 position = transform.position;
		Vector3 eulerAngles = transform.eulerAngles;
		Packet.AddFloat(position.x);
		Packet.AddFloat(position.y);
		Packet.AddFloat(position.z);
		Packet.AddFloat(eulerAngles.x);
		Packet.AddFloat(eulerAngles.y);
		Packet.AddFloat(eulerAngles.z);
		Packet.AddByte(bp.tier);
		Packet.AddFloat(bp.level);
		Packet.AddByte(bp.stage);
		if (bp.device != null)
		{
			if (bp.device.usesValue)
			{
				Packet.AddInt(bp.device.value);
			}
			if (bp.device is battery)
			{
				Packet.AddInt((bp.device as battery).chargePower);
			}
		}
		if ((bool)bp.door)
		{
			if (bp.door.open)
			{
				Packet.AddInt(1);
			}
			else
			{
				Packet.AddInt(0);
			}
		}
		if (sendAll)
		{
			foreach (CSteamID connectId in Multiplayer.connectIds)
			{
				Packet.SendPacket(connectId);
			}
		}
		else
		{
			Packet.SendPacket(sendId);
		}
		if (placeSound)
		{
			sendSound(transform.position, 0);
		}
	}

	public static void sendSound(Vector3 pos, int clip)
	{
		Packet.BuildPacket(13);
		Packet.AddFloat(pos.x);
		Packet.AddFloat(pos.y);
		Packet.AddFloat(pos.z);
		Packet.AddByte(clip);
		foreach (CSteamID connectId in Multiplayer.connectIds)
		{
			Packet.SendPacketUnreliable(connectId);
		}
	}

	public static void sendWire(int outSyncId, int outIndex, int inSyncId, int inIndex, Vector3[] points, bool powerThru, bool sendAll)
	{
		if (outSyncId != -1 && inSyncId != -1)
		{
			Packet.BuildPacket(14);
			Packet.AddInt(outSyncId);
			Packet.AddByte(outIndex);
			Packet.AddInt(inSyncId);
			Packet.AddByte(inIndex);
			Packet.AddByte(points.Length);
			foreach (Vector3 vector in points)
			{
				Packet.AddFloat(vector.x);
				Packet.AddFloat(vector.y);
				Packet.AddFloat(vector.z);
			}
			if (powerThru)
			{
				Packet.AddByte(1);
			}
			else
			{
				Packet.AddByte(0);
			}
			if (sendAll)
			{
				foreach (CSteamID connectId in Multiplayer.connectIds)
				{
					Packet.SendPacket(connectId);
				}
			}
			else
			{
				Packet.SendPacket(sendId);
			}
		}
	}

	public static void sendWireRemove(io io)
	{
		Packet.BuildPacket(15);
		int num = Multiplayer.syncObjList.IndexOf(io.dev.owner);
		if (num != -1)
		{
			Packet.AddInt(num);
			Packet.AddByte(io.transform.GetSiblingIndex());
			foreach (CSteamID connectId in Multiplayer.connectIds)
			{
				Packet.SendPacket(connectId);
			}
		}
	}

	public static void sendDeviceValue(BuilderPart bp, float val)
	{
		Packet.BuildPacket(16);
		int num = Multiplayer.syncObjList.IndexOf(bp);
		if (num != -1)
		{
			Packet.AddInt(num);
			Packet.AddFloat(val);
			foreach (CSteamID connectId in Multiplayer.connectIds)
			{
				Packet.SendPacket(connectId);
			}
		}
	}

	public static void sendDoorChange(BuilderPart bp, bool state)
	{
		Packet.BuildPacket(18);
		int num = Multiplayer.syncObjList.IndexOf(bp);
		if (num != -1)
		{
			Packet.AddInt(num);
			Packet.AddByte(Convert.ToInt32(state));
			foreach (CSteamID connectId in Multiplayer.connectIds)
			{
				Packet.SendPacket(connectId);
			}
		}
	}
}
