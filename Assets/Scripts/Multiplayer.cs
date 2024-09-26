using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Multiplayer : MonoBehaviour
{
	public class PlayerColor
	{
		public int place;

		public Color color;

		public PlayerColor(int Place, Color Color)
		{
			place = Place;
			color = Color;
		}
	}

	public static Multiplayer inst;

	private BuilderSystem cs;

	public bool connected;

	public Sender _Sender;

	public Receiver _Receiver;

	public static List<BuilderPart> syncObjList = new List<BuilderPart>();

	public static List<CSteamID> connectIds = new List<CSteamID>();

	private List<CSteamID> friendIds = new List<CSteamID>();

	public CSteamID userId;

	public static int myIndex = 0;

	public static bool host;

	public ulong lobbyID;

	public Dropdown dropdown;

	public Toggle multiToggle;

	public GameObject P2Pbutton;

	public GameObject connectPanel;

	public GameObject invitePanel;

	public Toggle stabilityTog;

	public Toggle hostToggle;

	public Button joinButton;

	public GameObject syncToggle;

	public BuilderHideFloors hf;

	public List<Color> colors = new List<Color>();

	public List<PlayerColor> adjustColors = new List<PlayerColor>();

	public List<Text> playerNames = new List<Text>();

	public static bool hideSync;

	public bool online;

	private int max = 4;

	public static bool CmdConnect;

	private Callback<P2PSessionRequest_t> p2PSessionRequestCallback;

	private Callback<LobbyCreated_t> Callback_lobbyCreated;

	private Callback<LobbyEnter_t> Callback_lobbyEnter;

	private Callback<GameLobbyJoinRequested_t> Callback_joinRequest;

	private Callback<LobbyChatUpdate_t> Callback_LobbyUpdate;

	private Callback<LobbyDataUpdate_t> Callback_LobbyDataUpdate;

	private void Awake()
	{
		inst = this;
	}

	private void Start()
	{
		cs = BuilderSystem.inst;
		if (SteamManager.Initialized)
		{
			p2PSessionRequestCallback = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
			Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
			Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
			Callback_joinRequest = Callback<GameLobbyJoinRequested_t>.Create(OnJoinRequest);
			Callback_LobbyUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyUpdate);
			Callback_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
			userId = SteamUser.GetSteamID();
			multiToggle.interactable = true;
			if (CmdConnect)
			{
				return;
			}
			string[] commandLineArgs = Environment.GetCommandLineArgs();
			int num = 0;
			while (true)
			{
				if (num < commandLineArgs.Length)
				{
					if (CmdConnect)
					{
						break;
					}
					if (commandLineArgs[num] == "+connect_lobby")
					{
						CmdConnect = true;
					}
					num++;
					continue;
				}
				return;
			}
			joinAtStart(commandLineArgs[num]);
		}
		else
		{
			UnityEngine.Debug.Log("steamManager not initialized");
		}
	}

	public void hostLobby()
	{
		SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, max);
	}

	private void OnLobbyCreated(LobbyCreated_t result)
	{
		if (result.m_eResult == EResult.k_EResultOK)
		{
			myIndex = 0;
			lobbyID = result.m_ulSteamIDLobby;
			host = true;
			_Receiver.players.Add(null);
			syncObjList = new List<BuilderPart>(cs.bpList);
			GetFriendList();
			invitePanel.SetActive(value: true);
			adjustColors.Clear();
			for (int i = 0; i < colors.Count; i++)
			{
				adjustColors.Add(new PlayerColor(i, colors[i]));
			}
			SteamMatchmaking.SetLobbyMemberData((CSteamID)result.m_ulSteamIDLobby, "colorIndex", "0");
			SteamMatchmaking.SetLobbyMemberData((CSteamID)result.m_ulSteamIDLobby, "avatar", Options.avatarIndex.ToString());
			popUp.inst.message("lobby started, invite players");
		}
		else
		{
			leaveLobby();
			popUp.inst.message("failed to create lobby");
		}
	}

	private void OnLobbyEntered(LobbyEnter_t result)
	{
		if (result.m_EChatRoomEnterResponse != 1)
		{
			popUp.inst.message("failed to join lobby");
			return;
		}
		StartCoroutine(waitToSend());
		if (!host)
		{
			lobbyID = result.m_ulSteamIDLobby;
			BuilderSystem.disableInput = true;
			getLobbyMembers();
		}
		MPsettingsOn();
	}

	private IEnumerator waitToSend()
	{
		yield return new WaitForSeconds(1f);
		SteamMatchmaking.SetLobbyMemberData((CSteamID)lobbyID, "avatar", Options.avatarIndex.ToString());
	}

	public void getLobbyMembers()
	{
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyID);
		if (numLobbyMembers == 0)
		{
			popUp.inst.message("no lobby data");
			return;
		}
		connected = true;
		popUp.inst.message("connected");
		myIndex = numLobbyMembers - 1;
		for (int i = 0; i < numLobbyMembers; i++)
		{
			CSteamID lobbyMemberByIndex = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)lobbyID, i);
			if (lobbyMemberByIndex != userId)
			{
				connectIds.Add(lobbyMemberByIndex);
				_Receiver.playerSpawn(lobbyMemberByIndex);
				getPlayerLobbyData(lobbyMemberByIndex, i);
				SteamNetworking.AcceptP2PSessionWithUser(lobbyMemberByIndex);
			}
			else
			{
				_Receiver.players.Add(null);
			}
		}
	}

	private void OnLobbyUpdate(LobbyChatUpdate_t result)
	{
		CSteamID cSteamID = (CSteamID)result.m_ulSteamIDUserChanged;
		if (cSteamID == userId)
		{
			return;
		}
		if (result.m_rgfChatMemberStateChange == 1)
		{
			connected = true;
			if (host)
			{
				sendResetCmd(cs.currentScene.buildIndex, (ulong)cSteamID);
				Sender.sendId = cSteamID;
				sendScene(sendAll: false);
				sendNewColor(cSteamID);
				PlayerColor item = adjustColors[1];
				adjustColors.RemoveAt(1);
				adjustColors.Add(item);
			}
			connectIds.Add(cSteamID);
			_Receiver.playerSpawn(cSteamID);
			popUp.inst.message(SteamFriends.GetFriendPersonaName(cSteamID) + " joined");
			return;
		}
		bool flag = false;
		if (result.m_rgfChatMemberStateChange == 2)
		{
			flag = true;
		}
		else if (result.m_rgfChatMemberStateChange == 4)
		{
			flag = true;
		}
		if (!flag)
		{
			return;
		}
		int playerIndex = getPlayerIndex(cSteamID);
		if (playerIndex != -1)
		{
			if (!host && playerIndex == 0)
			{
				leaveLobby();
				popUp.inst.message("host left - lobby closed");
				return;
			}
			popUp.inst.message(SteamFriends.GetFriendPersonaName(cSteamID) + " left");
			_Receiver.playerDestroy(cSteamID);
			connectIds.Remove(cSteamID);
		}
		int numLobbyMembers = SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyID);
		if (!host)
		{
			int num = 0;
			while (true)
			{
				if (num < numLobbyMembers)
				{
					if (SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)lobbyID, num) == userId)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			myIndex = num;
			Sender.sendInt = cs.BPinfo.id;
			Sender.send(11);
		}
		else if (numLobbyMembers == 1)
		{
			connected = false;
		}
	}

	private void OnLobbyDataUpdate(LobbyDataUpdate_t result)
	{
		CSteamID cSteamID = (CSteamID)result.m_ulSteamIDMember;
		if (!(cSteamID == userId) && (ulong)cSteamID != lobbyID)
		{
			int playerIndex = _Receiver.getPlayerIndex(cSteamID);
			if (playerIndex != -1)
			{
				getPlayerLobbyData(cSteamID, playerIndex);
			}
		}
	}

	public void getPlayerLobbyData(CSteamID id, int index)
	{
		if (index >= _Receiver.players.Count)
		{
			return;
		}
		string text = SteamMatchmaking.GetLobbyMemberData((CSteamID)lobbyID, id, "avatar");
		if (text == null || text == "")
		{
			text = "0";
		}
		_Receiver.players[index].avatarMeshSetup(int.Parse(text));
		if (!(_Receiver.players[index].meshChild == null))
		{
			string lobbyMemberData = SteamMatchmaking.GetLobbyMemberData((CSteamID)lobbyID, id, "colorIndex");
			if (lobbyMemberData != null && lobbyMemberData != "")
			{
				_Receiver.players[index].setColor(int.Parse(lobbyMemberData));
			}
			string lobbyMemberData2 = SteamMatchmaking.GetLobbyMemberData((CSteamID)lobbyID, id, "mount");
			if (lobbyMemberData2 != null && lobbyMemberData2 != "")
			{
				bool mount = int.Parse(lobbyMemberData2) == 1;
				_Receiver.players[index].miniCopterState(mount);
			}
		}
	}

	public void leaveLobby()
	{
		SteamMatchmaking.LeaveLobby((CSteamID)lobbyID);
		_Receiver.playerDestroyAll();
		connected = false;
		MPsettingsOff();
	}

	public void sendLobbyInvite()
	{
		if (SteamMatchmaking.GetNumLobbyMembers((CSteamID)lobbyID) == max)
		{
			popUp.inst.message(max + " player limit");
		}
		else if (host && dropdown.options.Count > 0)
		{
			SteamMatchmaking.InviteUserToLobby((CSteamID)lobbyID, friendIds[dropdown.value]);
		}
	}

	private void OnJoinRequest(GameLobbyJoinRequested_t result)
	{
		if (!BuilderSystem.multiplayer)
		{
			SteamMatchmaking.JoinLobby(result.m_steamIDLobby);
			multiToggle.isOn = true;
		}
	}

	private void joinAtStart(string stringId)
	{
		SteamMatchmaking.JoinLobby(new CSteamID(ulong.Parse(stringId)));
		multiToggle.isOn = true;
	}

	private void acceptP2PSession(P2PSessionRequest_t result)
	{
		foreach (CSteamID connectId in connectIds)
		{
			if (connectId == result.m_steamIDRemote)
			{
				SteamNetworking.AcceptP2PSessionWithUser(result.m_steamIDRemote);
				break;
			}
		}
	}

	private void OnP2PSessionRequest(P2PSessionRequest_t request)
	{
		SteamNetworking.AcceptP2PSessionWithUser(request.m_steamIDRemote);
		UnityEngine.Debug.Log("accept request");
	}

	public void sendRecieve(bool state)
	{
		_Sender.enabled = state;
		_Receiver.enabled = state;
	}

	public void MPsettingsOn()
	{
		sendRecieve(state: true);
		if (Stability.inst.raidMode)
		{
			stabilityTog.isOn = false;
		}
		stabilityTog.interactable = false;
		if (Symmetry.inst.isOn)
		{
			Symmetry.inst.symTog.isOn = false;
		}
		Symmetry.inst.symTog.interactable = false;
		BuilderSystem.multiplayer = true;
		Application.runInBackground = true;
		connectPanel.SetActive(value: true);
		P2Pbutton.SetActive(value: true);
	}

	public void MPsettingsOff()
	{
		BuilderSystem.disableInput = false;
		sendRecieve(state: false);
		BuilderSystem.multiplayer = false;
		host = false;
		Application.runInBackground = false;
		popUp.inst.message("disconnected");
		stabilityTog.interactable = true;
		Symmetry.inst.symTog.interactable = true;
		invitePanel.SetActive(value: false);
		connectPanel.SetActive(value: false);
		P2Pbutton.SetActive(value: false);
		syncObjList.Clear();
		foreach (Text playerName in playerNames)
		{
			playerName.enabled = false;
		}
		foreach (CSteamID connectId in connectIds)
		{
			SteamNetworking.CloseP2PSessionWithUser(connectId);
		}
		connectIds.Clear();
	}

	public void syncToggleState()
	{
		hideSync = syncToggle.GetComponent<Toggle>().isOn;
		if (hideSync)
		{
			syncHideState();
		}
	}

	public void syncHideState()
	{
		if (!BuilderSystem.multiplayer)
		{
			popUp.inst.message("used in multiplayer");
			return;
		}
		Sender.sendFloat = float.Parse(hf.floorLvl.GetComponent<Text>().text);
		Sender.send(8);
	}

	public void GetFriendList()
	{
		int num = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);
		if (!online)
		{
			num = 0;
		}
		FriendGameInfo_t pFriendGameInfo = default(FriendGameInfo_t);
		friendIds.Clear();
		dropdown.ClearOptions();
		for (int i = 0; i < num; i++)
		{
			CSteamID friendByIndex = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
			bool friendGamePlayed = SteamFriends.GetFriendGamePlayed(friendByIndex, out pFriendGameInfo);
			if (!(SteamFriends.GetFriendPersonaState(friendByIndex).ToString() == "k_EPersonaStateOffline"))
			{
				string friendPersonaName = SteamFriends.GetFriendPersonaName(friendByIndex);
				if (friendGamePlayed && pFriendGameInfo.m_gameID.ToString() == "505040")
				{
					dropdown.options.Insert(0, new Dropdown.OptionData
					{
						text = friendPersonaName
					});
					friendIds.Insert(0, friendByIndex);
				}
				else
				{
					dropdown.options.Add(new Dropdown.OptionData
					{
						text = friendPersonaName
					});
					friendIds.Add(friendByIndex);
				}
			}
		}
		dropdown.RefreshShownValue();
	}

	private int getPlayerIndex(CSteamID id)
	{
		for (int i = 0; i < connectIds.Count; i++)
		{
			if (connectIds[i] == id)
			{
				return i;
			}
		}
		return -1;
	}

	public void sendNewColor(CSteamID id)
	{
		Packet.BuildPacket(6);
		Packet.AddInt(adjustColors[1].place);
		Packet.SendPacket(id);
	}

	public void sendAvatar()
	{
		SteamMatchmaking.SetLobbyMemberData((CSteamID)lobbyID, "avatar", Options.avatarIndex.ToString());
	}

	public void lobbyMemberDataChange(string key, string value)
	{
		SteamMatchmaking.SetLobbyMemberData((CSteamID)lobbyID, key, value);
	}

	public static void sendResetCmd(int sceneIndex, ulong id)
	{
		Packet.BuildPacket(1);
		Packet.AddInt(sceneIndex);
		if (id != 0L)
		{
			Packet.SendPacket((CSteamID)id);
		}
		else
		{
			foreach (CSteamID connectId in connectIds)
			{
				Packet.SendPacket(connectId);
			}
		}
	}

	public void sendScene(bool sendAll)
	{
		_Receiver.enabled = false;
		for (int i = 0; i < syncObjList.Count; i++)
		{
			Sender.sendPart(syncObjList[i], sendAll, placeSound: false);
		}
		foreach (wire wire in BuilderSystem.wireList)
		{
			int outSyncId = syncObjList.IndexOf(wire.output.dev.owner);
			int inSyncId = syncObjList.IndexOf(wire.output.connectedTo.dev.owner);
			Vector3[] array = new Vector3[wire.lr.positionCount];
			wire.lr.GetPositions(array);
			Sender.sendWire(outSyncId, wire.output.index, inSyncId, wire.input.index, array, powerThru: false, sendAll);
		}
		if (BuilderSystem.wireList.Count > 0)
		{
			Packet.BuildPacket(17);
			if (sendAll)
			{
				foreach (CSteamID connectId in connectIds)
				{
					Packet.SendPacket(connectId);
				}
			}
			else
			{
				Packet.SendPacket(Sender.sendId);
			}
		}
		_Receiver.enabled = true;
	}

	public static void destroySend(BuilderPart bp)
	{
		Sender.sendObj = bp;
		Sender.send(4);
	}

	public static void tierSend(BuilderPart bp)
	{
		Sender.sendObj = bp;
		Sender.send(5);
	}

	public static void stageSend(BuilderPart bp)
	{
		Sender.sendObj = bp;
		Sender.send(9);
	}

	private void OnApplicationQuit()
	{
		if (BuilderSystem.multiplayer)
		{
			leaveLobby();
		}
	}
}
