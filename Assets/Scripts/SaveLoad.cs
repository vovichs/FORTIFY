using Pastebin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class SaveLoad : MonoBehaviour
{
	public class ioPair
	{
		public io output;

		public io input;

		public int color;

		public Vector3[] points;
	}

	private BuilderUI ui;

	private BuilderSystem sys;

	[HideInInspector]
	public string folderPath;

	[HideInInspector]
	private string loadPath;

	private string curPart;

	public InputField inputField;

	public InputField[] pastebinFields;

	public Toggle pastebinImportTog;

	public Text pastebinLabel;

	public GameObject codeToggle;

	public GameObject stabilityToggle;

	public GameObject loadWarnPanel;

	private Toggle codes;

	private bool addCodes = true;

	private bool pastebinWait;

	public Transform infoCanvas;

	private string[] partData;

	private void Awake()
	{
		CultureInfo cultureInfo = (CultureInfo)Thread.CurrentThread.CurrentCulture.Clone();
		cultureInfo.NumberFormat.NumberDecimalSeparator = ".";
		Thread.CurrentThread.CurrentCulture = cultureInfo;
		sys = BuilderSystem.inst;
		ui = BuilderUI.inst;
		if (codeToggle != null)
		{
			codes = codeToggle.GetComponent<Toggle>();
		}
		folderPath = Application.dataPath + "/Saves/";
		if (!Directory.Exists(folderPath))
		{
			Directory.CreateDirectory(folderPath);
			BuilderUI.inst.menuToggle.isOn = true;
			BuilderUI.inst.guidePopUp.SetActive(value: true);
		}
		else
		{
			BuilderUI.inst.guidePopUp.SetActive(value: false);
		}
	}

	public void SavePressed()
	{
		if (fileList.inst.storedPath != "")
		{
			if (fileList.inst.jsonToggle.isOn)
			{
				RustCopyPaste.inst.CopyPasteExport(fileList.inst.storedPath + ".json", returns: false);
			}
			else
			{
				SaveFile(fileList.inst.storedPath + ".txt");
			}
		}
	}

	public void SaveAsPressed()
	{
		string text = inputField.text;
		if (string.IsNullOrEmpty(text))
		{
			popUp.inst.message("no name?");
			return;
		}
		if (text.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
		{
			popUp.inst.message("invalid character in name");
			return;
		}
		string str = Path.Combine(folderPath, text);
		if (fileList.inst.jsonToggle.isOn)
		{
			RustCopyPaste.inst.CopyPasteExport(str += ".json", returns: false);
		}
		else
		{
			SaveFile(str += ".txt");
		}
		if (File.Exists(str))
		{
			fileList.inst.changeSaveInfo(str);
			fileList.inst.loadList();
		}
		else
		{
			popUp.inst.message("save failed");
		}
	}

	public void SaveFile(string path)
	{
		if (!string.IsNullOrEmpty(path))
		{
			File.WriteAllText(path, Save(infoSpots: true));
			UnityEngine.Debug.Log(path + " saved");
			string name = new DirectoryInfo(folderPath).Name;
			if (name != "Saves")
			{
				popUp.inst.message("saved in " + name);
			}
			else
			{
				popUp.inst.message("saved");
			}
		}
	}

	private int SortById(BuilderPart bp1, BuilderPart bp2)
	{
		return bp1.id.CompareTo(bp2.id);
	}

	public string Save(bool infoSpots)
	{
		StringBuilder stringBuilder = new StringBuilder();
		Transform transform = BuilderSystem.inst._transform;
		int num = 0;
		string text = "|" + roundUP(transform.position.x) + "|" + roundUP(transform.position.y) + "|" + roundUP(transform.position.z);
		text = text + "|" + roundUP(transform.eulerAngles.x) + "|" + roundUP(transform.parent.eulerAngles.y);
		stringBuilder.Append(sys.currentScene.name + text + ";");
		if (sys.bpList.Count < 1)
		{
			return stringBuilder.ToString();
		}
		sys.bpList.Sort(SortById);
		int id = sys.bpList[0].id;
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			if (sys.bpList[i] == null)
			{
				continue;
			}
			BuilderPart builderPart = sys.bpList[i];
			if (i == 0 || id != builderPart.id)
			{
				if (i != 0)
				{
					stringBuilder.Append(";");
				}
				stringBuilder.Append(sys.bpList[i].name);
				id = builderPart.id;
			}
			Transform transform2 = builderPart._transform;
			Vector3 position = transform2.position;
			string text2 = "," + roundUP(position.x) + "|" + roundUP(position.y) + "|" + roundUP(position.z);
			Vector3 eulerAngles = transform2.eulerAngles;
			text2 = ((!builderPart.block) ? (text2 + "|" + roundUP(eulerAngles.x) + "_" + roundUP(eulerAngles.y) + "_" + roundUP(eulerAngles.z)) : (text2 + "|" + roundUP(eulerAngles.y)));
			text2 = ((!builderPart.block) ? (text2 + "|") : (text2 + "|" + builderPart.tier));
			text2 = text2 + "_" + builderPart.level;
			if (builderPart.stage > 1)
			{
				text2 = text2 + "_" + builderPart.stage;
			}
			if (!builderPart.deploy)
			{
				stringBuilder.Append(text2);
				continue;
			}
			if ((bool)builderPart.deploy && (bool)builderPart.deploy.lockEnt)
			{
				text2 += "|";
				if (addCodes)
				{
					text2 += builderPart.deploy.lockEnt.code;
				}
			}
			if ((bool)builderPart.door && !builderPart.door.open)
			{
				text2 += "_";
			}
			Device device = builderPart.device;
			if (device != null)
			{
				string arg = "|";
				arg = ((!(device is timerDevice)) ? (arg + device.value + "\\") : (arg + (device as timerDevice).time + "\\"));
				int num2 = 0;
				for (int j = 0; j < device.outputTo.Length; j++)
				{
					io io = device.outputTo[j];
					if (!io.connectedTo)
					{
						continue;
					}
					num2++;
					arg += "\\";
					if (io.connectedTo.id != 0)
					{
						arg = arg + io.connectedTo.id + "_0_" + j;
						io.connectedTo.id = 0;
					}
					else
					{
						num++;
						arg = arg + num + "_0_" + j;
						io.connectedTo.id = num;
					}
					if (io.wire != null)
					{
						if (io.wire.colorIndex > 0)
						{
							arg = arg + "_" + io.wire.colorIndex;
						}
						LineRenderer lr = io.wire.lr;
						Vector3[] array = new Vector3[lr.positionCount];
						lr.GetPositions(array);
						Vector3[] array2 = array;
						foreach (Vector3 vector in array2)
						{
							arg = arg + "_" + roundUP(vector.x) + "/" + roundUP(vector.y) + "/" + roundUP(vector.z);
						}
					}
					if (num2 > 1)
					{
						arg = (arg ?? "");
					}
				}
				for (int l = 0; l < device.inputFrom.Length; l++)
				{
					io io2 = device.inputFrom[l];
					if ((bool)io2.connectedTo)
					{
						num2++;
						if (num2 > 1)
						{
							arg += "\\";
						}
						if (io2.id == 0)
						{
							num = (io2.id = num + 1);
							arg = arg + num + "_1_" + l;
						}
						else
						{
							arg = arg + io2.id + "_1_" + l;
							io2.id = 0;
						}
					}
				}
				text2 += arg;
			}
			stringBuilder.Append(text2);
		}
		if (infoSpots)
		{
			List<infoSpot> spots = infoSpotPanel.inst.spots;
			if (spots.Count > 0)
			{
				string text3 = ";infoSpot";
				for (int m = 0; m < spots.Count; m++)
				{
					Vector3 savedPos = spots[m].savedPos;
					text3 = text3 + "," + roundUP(savedPos.x) + "|" + roundUP(savedPos.y) + "|" + roundUP(savedPos.z);
					Quaternion savedRot = spots[m].savedRot;
					text3 = text3 + "|" + roundUP(savedRot.x) + "_" + roundUP(savedRot.y) + "_" + roundUP(savedRot.z) + "_" + roundUP(savedRot.w);
					string infoString = spots[m].infoString;
					infoString = infoString.Replace(",", "<");
					infoString = infoString.Replace(".", ">");
					text3 = text3 + "|" + infoString;
				}
				stringBuilder.Append(text3);
			}
		}
		if (RustCopyPaste.center != null)
		{
			Vector3 position2 = RustCopyPaste.center.transform.position;
			string value = ";center," + roundUP(position2.x) + "|" + roundUP(position2.y) + "|" + roundUP(position2.z);
			stringBuilder.Append(value);
		}
		if (Symmetry.inst.centerSet)
		{
			Vector3 position3 = Symmetry.inst.symTransform.position;
			string text4 = ";symmetry," + roundUP(position3.x) + "|" + roundUP(position3.y) + "|" + roundUP(position3.z);
			Vector3 eulerAngles2 = Symmetry.inst.symTransform.rotation.eulerAngles;
			text4 = text4 + "|" + roundUP(eulerAngles2.x) + "_" + roundUP(eulerAngles2.y) + "_" + roundUP(eulerAngles2.z);
			text4 = text4 + "|" + Symmetry.inst.getSymType();
			stringBuilder.Append(text4);
		}
		return stringBuilder.ToString();
	}

	public void LoadFile(string path)
	{
		FileInfo fileInfo = new FileInfo(path);
		if (!fileInfo.Exists)
		{
			popUp.inst.message("file not found");
			return;
		}
		if (fileInfo.Length == 0L)
		{
			popUp.inst.message("file is empty");
			return;
		}
		UnityEngine.Debug.Log(path);
		if (Load(File.ReadAllText(path), clear: true))
		{
			fileList.inst.changeSaveInfo(path);
			if (BuilderSystem.multiplayer)
			{
				Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
				Multiplayer.inst.sendScene(sendAll: true);
			}
		}
		else
		{
			fileList.inst.changeSaveInfo("");
		}
	}

	public void loadFileButton(string path)
	{
		if (sys.bpList.Count > 0)
		{
			loadPath = path;
			loadWarnPanel.SetActive(value: true);
		}
		else
		{
			LoadFile(path);
		}
	}

	private bool Load(string LoadString, bool clear)
	{
		Symmetry.inst.symTog.isOn = false;
		if (LoadString[0].ToString() == "{")
		{
			popUp.inst.message("change file mode to rust copy-paste");
			return false;
		}
		popUp.inst.message("loading");
		if (clear)
		{
			sys.ClearScene();
		}
		bool flag = false;
		if (BuilderUI.inst.mergeToggle.isOn)
		{
			flag = true;
			popUp.inst.message("click to place merged");
			BuilderUI.inst.mergeToggle.isOn = false;
		}
		else
		{
			Symmetry.inst.centerSet = false;
		}
		string[] array = LoadString.Split(';');
		bool flag2 = false;
		bool flag3 = true;
		bool flag4 = true;
		bool flag5 = false;
		Vector3 vector = Vector3.zero;
		float num = 0f;
		infoSpotPanel.inst.show = true;
		List<ioPair> list = new List<ioPair>();
		List<Device> list2 = new List<Device>();
		List<Vector3> list3 = new List<Vector3>();
		list.Add(new ioPair());
		string[] array2 = array;
		foreach (string text in array2)
		{
			if (flag3)
			{
				flag3 = false;
				string[] array3 = text.Split('|');
				if (array3[0] == "terrain2d" || array3[0] == "terrain3d" || array3[0] == "caves" || array3[0] == "icebergs" || array3[0] == "water")
				{
					flag4 = false;
				}
				if (!flag)
				{
					if (!flag4)
					{
						if (array3.Length == 6)
						{
							flag5 = true;
							Transform transform = BuilderSystem.inst._transform;
							Transform parent = transform.parent;
							parent.position = new Vector3(Convert.ToSingle(array3[1]), Convert.ToSingle(array3[2]), Convert.ToSingle(array3[3]));
							transform.localRotation = Quaternion.Euler(new Vector3(Convert.ToSingle(array3[4]), transform.localRotation.y, transform.localRotation.z));
							parent.rotation = Quaternion.Euler(new Vector3(parent.rotation.x, Convert.ToSingle(array3[5]), parent.rotation.z));
							CameraCtrl.setRotation();
						}
						sys.SceneLoader(-1, array3[0]);
						continue;
					}
					sys.SceneLoader(2, null);
				}
				else if (!flag4)
				{
					continue;
				}
			}
			string[] array4 = text.Split(',');
			string text2 = array4[0].ToString();
			if (text2 == "infoSpot")
			{
				if (!flag)
				{
					infoSpotPanel.inst.show = false;
					Transform parent2 = infoSpotPanel.inst.infoPanel.transform.parent;
					for (int j = 1; j < array4.Length; j++)
					{
						GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(text2), Vector3.zero, Quaternion.identity) as GameObject;
						infoSpot component = gameObject.GetComponent<infoSpot>();
						infoSpotPanel.inst.spots.Add(component);
						partData = array4[j].Split('|');
						component.savedPos = partDataPosition();
						string[] array5 = partData[3].Split('_');
						component.savedRot.x = Convert.ToSingle(array5[0]);
						component.savedRot.y = Convert.ToSingle(array5[1]);
						component.savedRot.z = Convert.ToSingle(array5[2]);
						component.savedRot.w = Convert.ToSingle(array5[3]);
						string text3 = partData[4];
						text3 = text3.Replace("<", ",");
						text3 = (component.infoString = text3.Replace(">", "."));
						gameObject.transform.SetParent(parent2);
					}
				}
				continue;
			}
			if (text2 == "center")
			{
				if (!flag)
				{
					partData = array4[1].Split('|');
					Vector3 position = partDataPosition();
					(RustCopyPaste.center = (UnityEngine.Object.Instantiate(Resources.Load(text2), position, Quaternion.identity) as GameObject)).SetActive(value: false);
				}
				continue;
			}
			if (text2 == "symmetry")
			{
				if (!flag)
				{
					partData = array4[1].Split('|');
					Symmetry.inst.symTransform.position = partDataPosition();
					string[] stringArr = partData[3].Split('_');
					Symmetry.inst.symTransform.rotation = Quaternion.Euler(stringToV3(stringArr));
					Symmetry.inst.changeSymType(int.Parse(partData[4]));
					Symmetry.inst.centerSet = true;
				}
				continue;
			}
			GameObject prefabFromName = MGMT.inst.getPrefabFromName(text2);
			if (!prefabFromName)
			{
				flag2 = true;
				MonoBehaviour.print(text2);
				continue;
			}
			for (int k = 1; k < array4.Length; k++)
			{
				partData = array4[k].Split('|');
				if (partData.Length < 4)
				{
					continue;
				}
				Vector3 vector2 = partDataPosition();
				if (vector2.y < -100f)
				{
					continue;
				}
				if (!flag5)
				{
					if (vector2.y > num)
					{
						num = vector2.y;
					}
					if (k == 1)
					{
						vector = vector2;
					}
				}
				string[] stringArr2 = partData[3].Split('_');
				Vector3 euler = stringToV3(stringArr2);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(prefabFromName, vector2, Quaternion.Euler(euler));
				gameObject2.name = prefabFromName.name;
				BuilderPart component2 = gameObject2.GetComponent<BuilderPart>();
				if (partData.Length >= 5 && partData[4].Length != 0)
				{
					string[] array6 = partData[4].Split('_');
					if (array6[0].Length != 0)
					{
						int num3 = component2.tier = IntParseFast(array6[0]);
						component2.rend[0].sharedMaterial = sys.tierMats[num3];
					}
					if (!flag4)
					{
						component2.level = float.Parse(array6[1]);
					}
					else
					{
						component2.level = 0f;
					}
					if (array6.Length == 3 && array6[2] != "")
					{
						component2.stage = IntParseFast(array6[2]);
					}
				}
				if (partData.Length >= 6 && partData[5] != "")
				{
					Device device = component2.device;
					if (device != null)
					{
						if (device.powerSource)
						{
							list2.Add(device);
						}
						string[] array7 = partData[5].Split('\\');
						if (device.usesValue)
						{
							if (device is timerDevice)
							{
								device.setValue(float.Parse(array7[0]), send: false);
							}
							else
							{
								int num4 = int.Parse(array7[0]);
								if (device is power && num4 == 0)
								{
									num4 = 100;
								}
								device.setValue(num4, send: false);
							}
						}
						for (int l = 1; l < array7.Length; l++)
						{
							string[] array8 = array7[l].Split('_');
							if (array8.Length == 1)
							{
								continue;
							}
							int num5 = int.Parse(array8[0]);
							if (num5 >= list.Count)
							{
								list.Add(new ioPair());
							}
							int num6 = int.Parse(array8[2]);
							if (int.Parse(array8[1]) == 0)
							{
								if (num5 > list.Count - 1)
								{
									list.RemoveAt(list.Count - 1);
									UnityEngine.Debug.Log("connection ID error");
									continue;
								}
								if (num6 < device.outputTo.Length)
								{
									list[num5].output = device.outputTo[num6];
								}
								list3.Clear();
								for (int m = 3; m < array8.Length; m++)
								{
									if (array8[m].Length < 3)
									{
										list[num5].color = int.Parse(array8[m]);
										continue;
									}
									string[] stringArr3 = array8[m].Split('/');
									list3.Add(stringToV3(stringArr3));
								}
								if (list3.Count > 1)
								{
									list[num5].points = list3.ToArray();
								}
							}
							else if (num5 > list.Count - 1)
							{
								list.RemoveAt(list.Count - 1);
								UnityEngine.Debug.Log("connection ID error");
							}
							else if (num6 < device.inputFrom.Length)
							{
								list[num5].input = device.inputFrom[num6];
							}
						}
					}
					else
					{
						string[] array9 = partData[5].Split('_');
						if (array9.Length == 1)
						{
							if (array9[0] == "_")
							{
								if ((bool)component2.door)
								{
									StartCoroutine(component2.changeDoorMeshState(state: false, audio: false, send: false));
								}
							}
							else if ((bool)component2.deploy && (bool)component2.deploy.lockEnt)
							{
								locks.inst.addCodelock(component2, array9[0]);
							}
						}
						else
						{
							if ((bool)component2.deploy && (bool)component2.deploy.lockEnt)
							{
								locks.inst.addCodelock(component2, array9[0]);
							}
							if ((bool)component2.door)
							{
								StartCoroutine(component2.changeDoorMeshState(state: false, audio: false, send: false));
							}
						}
					}
				}
				if (flag)
				{
					component2.selected = true;
					if ((bool)component2.device)
					{
						component2.device.owner = component2;
					}
					UnityEngine.Object.Destroy(component2.oc);
					sys.objList.Add(gameObject2);
					if (flag4)
					{
						gameObject2.layer = 0;
					}
				}
				else
				{
					sys.PlacedSetup(component2, send: false, notLoaded: false, sound: false, useCodeTog: false);
				}
			}
		}
		int count = sys.bpList.Count;
		for (int n = 0; n < count; n++)
		{
			if ((bool)sys.bpList[n].block)
			{
				sys.bpList[n].block.GetNeighborLinks(updateNeighbors: false, clear: false);
			}
		}
		for (int num7 = 0; num7 < count; num7++)
		{
			block block = sys.bpList[num7].block;
			if ((bool)block)
			{
				if ((bool)block.conditional)
				{
					block.conditional.RunCheck();
				}
				if ((bool)block.stabilityEntity)
				{
					block.stabilityEntity.UpdateStability();
				}
			}
		}
		infoSpotPanel.inst.toggleSpots(hide: false);
		if (flag2)
		{
			popUp.inst.message("invalid part data, incomplete save string?");
		}
		else if (!flag)
		{
			ui.saveButton.interactable = true;
		}
		for (int num8 = 1; num8 < list.Count; num8++)
		{
			ioPair ioPair = list[num8];
			if (ioPair.output == null || ioPair.input == null)
			{
				UnityEngine.Debug.Log("IO pair missing");
			}
			else if (ioPair.output.type != ioPair.input.type)
			{
				UnityEngine.Debug.Log("IO type mismatch");
			}
			else
			{
				wiring.inst.wiredConnect(ioPair.output, ioPair.input, ioPair.points, ioPair.color, powerThru: false, send: false);
			}
		}
		list.Clear();
		foreach (Device item in list2)
		{
			item.newPowerThru();
		}
		if (!flag)
		{
			if (!flag5)
			{
				CameraCtrl.parent.position = new Vector3(vector.x + 10f, num + 5f, vector.z + 10f);
				CameraCtrl.lookAt(vector);
				CameraCtrl.setRotation();
			}
		}
		else
		{
			ui.menuToggle.isOn = false;
		}
		if (flag4 && !flag)
		{
			GetComponent<SetLevels>().setLevels();
		}
		return true;
	}

	public void continueLoading(bool load)
	{
		if (load && loadPath != null)
		{
			LoadFile(loadPath);
		}
		loadPath = null;
		loadWarnPanel.SetActive(value: false);
	}

	public void PastebinModeState()
	{
		if (pastebinImportTog.isOn)
		{
			pastebinLabel.text = "Import Pastebin";
			pastebinFields[0].gameObject.SetActive(value: false);
			pastebinFields[1].gameObject.SetActive(value: true);
		}
		else
		{
			pastebinLabel.text = "Get Pastebin Link";
			pastebinFields[0].gameObject.SetActive(value: true);
			pastebinFields[1].gameObject.SetActive(value: false);
		}
	}

	public void PastebinButtPressed()
	{
		if (pastebinImportTog.isOn)
		{
			PastebinImport();
		}
		else
		{
			PastebinGetLink();
		}
	}

	public void CopyPastebinLink()
	{
		GUIUtility.systemCopyBuffer = PastebinCreate.response;
		popUp.inst.message("copied to clipboard");
	}

	private void PastebinGetLink()
	{
		if (!pastebinWait)
		{
			string text = "";
			text = ((!fileList.inst.jsonToggle.isOn) ? PastebinCreate.Send(Save(infoSpots: true), fileList.inst.storedText.text) : PastebinCreate.Send(RustCopyPaste.inst.CopyPasteExport(null, returns: true), fileList.inst.storedText.text));
			pastebinFields[0].text = text;
			GUIUtility.systemCopyBuffer = text;
			StartCoroutine(PastebinWait());
		}
		else
		{
			popUp.inst.message("10 sec. wait between pastebins");
		}
	}

	private void PastebinImport()
	{
		if (pastebinFields[1].text == "")
		{
			popUp.inst.message("code/link missing from field");
			return;
		}
		string text = pastebinFields[1].text.Split('/').Last();
		if (text == "")
		{
			popUp.inst.message("incorrect code/link");
			return;
		}
		if (fileList.inst.jsonToggle.isOn)
		{
			RustCopyPaste.inst.CopyPasteString(PastebinCreate.Get(text));
		}
		else
		{
			Load(PastebinCreate.Get(text), clear: true);
		}
		fileList.inst.changeSaveInfo("");
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
			Multiplayer.inst.sendScene(sendAll: true);
		}
	}

	private IEnumerator PastebinWait()
	{
		pastebinWait = true;
		yield return new WaitForSeconds(10f);
		pastebinWait = false;
	}

	public void ClipboardExport()
	{
		if (!codes.isOn)
		{
			addCodes = false;
		}
		if (fileList.inst.jsonToggle.isOn)
		{
			GUIUtility.systemCopyBuffer = RustCopyPaste.inst.CopyPasteExport(null, returns: true);
		}
		else
		{
			GUIUtility.systemCopyBuffer = Save(infoSpots: true);
		}
		addCodes = true;
	}

	public void ClipboardImport()
	{
		if (fileList.inst.jsonToggle.isOn)
		{
			RustCopyPaste.inst.CopyPasteString(GUIUtility.systemCopyBuffer);
		}
		else
		{
			Load(GUIUtility.systemCopyBuffer, clear: true);
		}
		fileList.inst.changeSaveInfo("");
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
			Multiplayer.inst.sendScene(sendAll: true);
		}
	}

	public void WorkshopLoad(string content)
	{
		Load(content, clear: true);
		fileList.inst.changeSaveInfo("");
		if (BuilderSystem.multiplayer)
		{
			Multiplayer.sendResetCmd(sys.currentScene.buildIndex, 0uL);
			Multiplayer.inst.sendScene(sendAll: true);
		}
	}

	public void MergeFile(string path)
	{
		if (!BuilderSystem.editMode)
		{
			Toggle component = ui.editToggle.GetComponent<Toggle>();
			component.isOn = !component.isOn;
		}
		foreach (GameObject obj in sys.objList)
		{
			sys.changeSelection(obj, add: false);
		}
		Load(File.ReadAllText(path), clear: false);
		copyMove.inst.CopyMove(0);
	}

	public static int IntParseFast(string value)
	{
		int num = 0;
		foreach (char c in value)
		{
			num = 10 * num + (c - 48);
		}
		return num;
	}

	private float roundUP(float val)
	{
		val = Mathf.Round(val * 1000f) / 1000f;
		return val;
	}

	private Vector3 partDataPosition()
	{
		Vector3 result = default(Vector3);
		result.x = Convert.ToSingle(partData[0]);
		result.y = Convert.ToSingle(partData[1]);
		result.z = Convert.ToSingle(partData[2]);
		return result;
	}

	private Vector3 stringToV3(string[] stringArr)
	{
		Vector3 result = Vector3.zero;
		if (stringArr.Length != 3)
		{
			result = new Vector3(0f, Convert.ToSingle(partData[3]), 0f);
		}
		else
		{
			result.x = Convert.ToSingle(stringArr[0]);
			result.y = Convert.ToSingle(stringArr[1]);
			result.z = Convert.ToSingle(stringArr[2]);
		}
		return result;
	}
}
