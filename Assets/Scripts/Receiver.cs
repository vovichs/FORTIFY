using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
	private BuilderSystem cs;

	public GameObject playerPrefab;

	public List<OtherPlayer> players = new List<OtherPlayer>();

	public List<Material> partColors = new List<Material>();

	public GameObject invitePanel;

	private void Awake()
	{
		cs = BuilderSystem.inst;
	}

	private void Update()
	{
		uint pcubMsgSize;
		while (SteamNetworking.IsP2PPacketAvailable(out pcubMsgSize))
		{
			readPacket(pcubMsgSize);
		}
	}

	public void playerSpawn(CSteamID steamid)
	{
		if (getPlayerIndex(steamid) <= -1)
		{
			OtherPlayer component = UnityEngine.Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity).GetComponent<OtherPlayer>();
			component.playerSetup(steamid);
			players.Add(component);
		}
	}

	public void playerDestroy(CSteamID id)
	{
		int playerIndex = getPlayerIndex(id);
		if (playerIndex > -1)
		{
			players[playerIndex].DestroyPlayer();
			players[playerIndex].nameText.enabled = false;
			players.RemoveAt(playerIndex);
		}
	}

	public void playerDestroyAll()
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != null)
			{
				players[i].DestroyPlayer();
			}
		}
		players.Clear();
	}

	public int getPlayerIndex(CSteamID id)
	{
		for (int i = 0; i < players.Count; i++)
		{
			if (players[i] != null && players[i].steamid == id)
			{
				return i;
			}
		}
		return -1;
	}

	private BuilderPart getSyncObject(int index)
	{
		return Multiplayer.syncObjList[index];
	}

	private void readPacket(uint size)
	{
		byte[] array = new byte[size];
		if (!SteamNetworking.ReadP2PPacket(array, size, out uint _, out CSteamID _))
		{
			return;
		}
		Packet.packetBytes = null;
		Packet.ReadPacket(array);
		switch (Packet.ReadByte())
		{
		case 1:
			popUp.inst.message("scene reseting");
			cs.ClearScene();
			cs.SceneLoader(Packet.ReadInt(), null);
			if (CameraCtrl.inst.mode != CameraCtrl.Mode.mount)
			{
				CameraCtrl.resetTransform();
			}
			BuilderSystem.disableInput = false;
			break;
		case 2:
		{
			int num3 = Packet.ReadByte();
			if (num3 < players.Count && !(players[num3] == null))
			{
				players[num3].newPos = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
				players[num3].newRot = Quaternion.Euler(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
				players[num3].headTiltX = Packet.ReadFloat();
			}
			break;
		}
		case 3:
		{
			GameObject gameObject = MGMT.inst.prefabList[Packet.ReadInt()];
			Vector3 position = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
			Vector3 euler = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, position, Quaternion.Euler(euler));
			gameObject2.name = gameObject.name;
			BuilderPart component = gameObject2.GetComponent<BuilderPart>();
			component.tier = Packet.ReadByte();
			component.level = Packet.ReadFloat();
			component.stage = Packet.ReadByte();
			if (component.device != null && component.device.usesValue)
			{
				component.device.value = Packet.ReadInt();
			}
			if ((bool)component.door && Packet.ReadInt() == 1)
			{
				StartCoroutine(component.changeDoorMeshState(state: false, audio: false, send: false));
			}
			cs.PlacedSetup(component, send: false, notLoaded: true, sound: false, useCodeTog: false);
			break;
		}
		case 4:
		{
			BuilderPart syncObject9 = getSyncObject(Packet.ReadInt());
			if ((bool)syncObject9.block)
			{
				syncObject9.block.destroyed = true;
				syncObject9.block.UpdateNeighborLinks(removed: true);
			}
			Multiplayer.syncObjList.Remove(syncObject9);
			cs.destroyPart(syncObject9, Packet.ReadInt() == 1, receive: true);
			break;
		}
		case 5:
		{
			BuilderPart syncObject6 = getSyncObject(Packet.ReadInt());
			if ((bool)syncObject6.block)
			{
				syncObject6.block.changeTier(Packet.ReadInt(), received: true);
			}
			break;
		}
		case 6:
			SteamMatchmaking.SetLobbyMemberData((CSteamID)Multiplayer.inst.lobbyID, "colorIndex", Packet.ReadInt().ToString());
			break;
		case 7:
		{
			BuilderPart syncObject8 = getSyncObject(Packet.ReadInt());
			if (syncObject8.center != null)
			{
				syncObject8._transform.RotateAround(syncObject8.center.position, Vector3.up, Packet.ReadFloat());
			}
			else
			{
				syncObject8._transform.Rotate(Vector3.up, Packet.ReadFloat());
			}
			if ((bool)syncObject8.block)
			{
				syncObject8.block.UpdateNeighborLinks(removed: false);
			}
			break;
		}
		case 9:
		{
			BuilderPart syncObject7 = getSyncObject(Packet.ReadInt());
			int num6 = syncObject7.stage = Packet.ReadInt();
			syncObject7.SetMaterial();
			break;
		}
		case 10:
		{
			BuilderPart syncObject4 = getSyncObject(Packet.ReadInt());
			syncObject4._transform.position = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
			syncObject4._transform.rotation = Quaternion.Euler(new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat()));
			syncObject4.level = Packet.ReadFloat();
			if ((bool)syncObject4.block)
			{
				syncObject4.block.UpdateNeighborLinks(removed: false);
			}
			break;
		}
		case 11:
		{
			int num4 = Packet.ReadInt();
			if (num4 < players.Count)
			{
				UnityEngine.Object.Destroy(players[num4].part.gameObject);
				BuilderPart component2 = UnityEngine.Object.Instantiate(MGMT.inst.prefabList[Packet.ReadInt()], new Vector3(0f, -1000f, 0f), Quaternion.identity).GetComponent<BuilderPart>();
				component2._transform.GetChild(0).gameObject.SetActive(value: false);
				component2.rend[0].sharedMaterial = partColors[num4];
				players[num4].part = component2._transform;
			}
			break;
		}
		case 12:
		{
			int num2 = Packet.ReadByte();
			if (num2 < players.Count)
			{
				players[num2].part.position = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
				players[num2].part.rotation = Quaternion.Euler(new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat()));
			}
			break;
		}
		case 13:
		{
			Vector3 pos = new Vector3(Packet.ReadFloat(), Packet.ReadFloat(), Packet.ReadFloat());
			AudioPlayer.inst.playAtPoint(pos, Packet.ReadByte());
			break;
		}
		case 14:
		{
			BuilderPart syncObject5 = getSyncObject(Packet.ReadInt());
			if (!syncObject5.device)
			{
				return;
			}
			io outputIO = syncObject5.device.outputTo[Packet.ReadByte()];
			syncObject5 = getSyncObject(Packet.ReadInt());
			if (!syncObject5.device)
			{
				return;
			}
			io inputIO = syncObject5.device.inputFrom[Packet.ReadByte()];
			int num5 = Packet.ReadByte();
			Vector3[] array2 = new Vector3[num5];
			for (int i = 0; i < num5; i++)
			{
				array2[i].x = Packet.ReadFloat();
				array2[i].y = Packet.ReadFloat();
				array2[i].z = Packet.ReadFloat();
			}
			if (Packet.ReadByte() == 1)
			{
				wiring.inst.wiredConnect(outputIO, inputIO, array2, 0, powerThru: true, send: false);
				AudioPlayer.inst.playAtPoint(array2[0], 2);
			}
			else
			{
				wiring.inst.wiredConnect(outputIO, inputIO, array2, 0, powerThru: false, send: false);
			}
			break;
		}
		case 15:
		{
			BuilderPart syncObject3 = getSyncObject(Packet.ReadInt());
			if (!(syncObject3 == null))
			{
				syncObject3._transform.GetChild(Packet.ReadByte()).GetComponent<io>().sendDisconnect(destroyed: false);
			}
			break;
		}
		case 16:
		{
			BuilderPart syncObject2 = getSyncObject(Packet.ReadInt());
			if (syncObject2 != null && (bool)syncObject2.device)
			{
				float num = Packet.ReadFloat();
				if (syncObject2.device is timerDevice)
				{
					syncObject2.device.setValue(num, send: false);
				}
				else
				{
					syncObject2.device.setValue((int)num, send: false);
				}
			}
			break;
		}
		case 17:
			foreach (wire wire in BuilderSystem.wireList)
			{
				if (wire.output.dev.powerSource)
				{
					wire.output.dev.newPowerThru();
				}
			}
			break;
		case 18:
		{
			BuilderPart syncObject = getSyncObject(Packet.ReadInt());
			if (!(syncObject == null))
			{
				bool state = false;
				if (Packet.ReadByte() == 1)
				{
					state = true;
				}
				StartCoroutine(syncObject.changeDoorMeshState(state, audio: true, send: false));
			}
			break;
		}
		}
		Packet.packetBytes = null;
	}
}
