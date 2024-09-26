using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class wiring : MonoBehaviour
{
	public enum connected
	{
		input,
		output,
		none
	}

	public static wiring inst;

	public bool on;

	public int ioMode;

	private bool tooClose;

	public RectTransform[] modeButtons;

	public Text IoText;

	public PlacingLine placingLine;

	public GameObject warningPanel;

	public bool wireFlow;

	public bool labelsOn;

	public LayerMask rayMask;

	private BuilderSystem sys;

	public Toggle wireTog;

	public RectTransform panelRT;

	public devicePanel devPanel;

	public Text lengthText;

	public Text jointText;

	public wire wire;

	private io io1;

	private int ioLayer;

	public wire wirePrefab;

	public wire pipePrefab;

	public AudioClip[] sounds;

	private Renderer wireRend;

	public Material wireMat;

	public Material wireMatGlow;

	public Material wireMatGlowFlow;

	private int[] placingColorIndex = new int[2];

	public GameObject[] placingColorPanels;

	public Material[] wireMatColors;

	public Material[] wireMatColorsFlow;

	public Material[] pipeMatColors;

	public Material[] pipeMatColorsFlow;

	private LineRenderer lr;

	public Material lightGreen;

	public Material lightRed;

	public Material lightOff;

	public Material ioMat;

	public Material ioOverMat;

	public Material ioConnectMat;

	public int newPID;

	public int newUID;

	private int endIndex;

	public float wireLength;

	public connected io1Type = connected.none;

	private connected ioType;

	private Ray ray;

	private RaycastHit hit;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		ioLayer = LayerMask.NameToLayer("io");
	}

	private void Update()
	{
		if (BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		float num = 0f;
		if (Input.anyKey)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad1))
			{
				changeIoMode(0);
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad2))
			{
				changeIoMode(4);
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad3))
			{
				changeIoMode(1);
				return;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Backspace) && wire != null)
			{
				if (endIndex == 1)
				{
					resetWireData(triggerReset: false);
				}
				else
				{
					changeLastPoint(null, undo: true);
				}
				return;
			}
		}
		ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (!Physics.Raycast(ray, out hit, 100f, rayMask))
		{
			return;
		}
		Transform transform = hit.transform;
		Collider collider = hit.collider;
		ioTrigger.inst.move(hit.point);
		if (endIndex > 0)
		{
			num = Vector3.Distance(lr.GetPosition(endIndex - 1), hit.point) * 3f;
			if (wireLength + num >= 30f)
			{
				lengthText.text = "30.00";
				return;
			}
		}
		if (collider.gameObject.layer == ioLayer)
		{
			if (collider.tag == "output")
			{
				ioType = connected.output;
			}
			else
			{
				ioType = connected.input;
			}
			if (endIndex > 0)
			{
				snapWireEnd(transform.position);
			}
		}
		else
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Delete) || (Input.GetButtonDown("Del") && wire != null))
			{
				resetWireData(triggerReset: false);
				return;
			}
			ioType = connected.none;
			if (endIndex > 0)
			{
				moveWireEnd(hit);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			if ((ioMode != 4 && collider.tag == "deploy") || (ioMode == 4 && endIndex > 0 && (placingLine.blocked || tooClose)))
			{
				return;
			}
			if (ioType != connected.none)
			{
				if (io1Type == connected.none)
				{
					io1 = collider.GetComponent<io>();
					if (io1.connectedTo != null)
					{
						io1 = null;
						return;
					}
					startWire(transform);
				}
				else
				{
					io component = collider.GetComponent<io>();
					if (component.connectedTo != null || !combinerCheck(component))
					{
						return;
					}
					Transform transform2 = component.transform;
					Transform transform3 = io1.transform;
					if (!io1.dev.selfLoop && transform3.parent == transform2.parent)
					{
						return;
					}
					int color = 0;
					if (io1.type == 1)
					{
						color = 6;
						if (io1.tag == "output")
						{
							if (transform2.position.y > transform3.position.y)
							{
								popUp.inst.message("connection requires pump");
							}
						}
						else if (transform3.position.y > transform2.position.y)
						{
							popUp.inst.message("connection requires pump");
						}
					}
					if (io1Type != ioType)
					{
						snapWireEnd(transform.position);
						if (io1Type == connected.input)
						{
							ioConnect(component, io1, color);
						}
						else
						{
							ioConnect(io1, component, color);
						}
						StartCoroutine(ioInfoBlock(component));
						if (ioMode == 0)
						{
							AudioPlayer.inst.playAtPoint(transform.position, sounds[0], 1f);
						}
						else
						{
							AudioPlayer.inst.playAtPoint(transform.position, sounds[1], 0.6f);
						}
						wire = null;
						resetWireData(triggerReset: false);
					}
				}
			}
			else if (endIndex > 0 && endIndex <= 16)
			{
				Vector3 vector = moveWireEnd(hit);
				wireLength += num;
				jointText.text = (16 - endIndex).ToString();
				endIndex++;
				lr.positionCount++;
				lr.SetPosition(endIndex, vector);
				if (wire is pipe)
				{
					AudioPlayer.inst.playAtPoint(transform.position, sounds[1], 0.6f);
					placingLine.updateLine(vector, vector);
					if (endIndex > 1)
					{
						(wire as pipe).buildPipe(_placing: true, undo: false);
					}
				}
			}
		}
		lengthText.text = (wireLength + num).ToString("#.00");
	}

	private void snapWireEnd(Vector3 pos)
	{
		if (ioMode == 4)
		{
			tooClose = placingLine.updateLine(Vector3.zero, pos);
		}
		lr.SetPosition(endIndex, pos);
	}

	private Vector3 moveWireEnd(RaycastHit hit)
	{
		if (ioMode == 4)
		{
			Vector3 vector = hit.point + hit.normal * 0.02f;
			lr.SetPosition(endIndex, vector);
			tooClose = placingLine.updateLine(Vector3.zero, vector);
			return vector;
		}
		Vector3 vector2 = hit.point + hit.normal * 0.01f;
		lr.SetPosition(endIndex, vector2);
		return vector2;
	}

	private void startWire(Transform hitT)
	{
		if (ioMode == 0)
		{
			AudioPlayer.inst.playAtPoint(hitT.position, sounds[0], 1f);
		}
		else
		{
			AudioPlayer.inst.playAtPoint(hitT.position, sounds[1], 0.6f);
		}
		if (ioMode == 4)
		{
			wire = UnityEngine.Object.Instantiate(pipePrefab, hit.point, Quaternion.identity);
			placingLine.transform.parent.gameObject.SetActive(value: true);
			tooClose = placingLine.updateLine(hitT.position, hit.point);
			if (placingColorIndex[1] > 0)
			{
				wire.setMat(placingColorIndex[1], circuit: false);
			}
		}
		else
		{
			wire = UnityEngine.Object.Instantiate(wirePrefab, hit.point, Quaternion.identity);
			if (placingColorIndex[0] > 0)
			{
				wire.setMat(placingColorIndex[0], circuit: false);
			}
		}
		lr = wire.lr;
		if (io1.type == 1)
		{
			lr.material = wireMatColors[6];
		}
		lr.SetPosition(endIndex, hitT.position);
		lr.SetPosition(1, hit.point);
		endIndex = 1;
		jointText.text = "16";
		lengthText.enabled = true;
		jointText.enabled = true;
		if (hit.collider.tag == "output")
		{
			io1Type = connected.output;
		}
		else
		{
			io1Type = connected.input;
		}
		ioTrigger.inst.displayByType(io1Type);
	}

	private void ioConnect(io outputIO, io inputIO, int color)
	{
		outputIO.wire = wire;
		MGMT.inst.undoListInsert();
		MGMT.inst.undoListList[0].Add(new AddedWire(wiredConnect(outputIO, inputIO, null, color, powerThru: true, send: true)));
	}

	public wire wiredConnect(io outputIO, io inputIO, Vector3[] points, int color, bool powerThru, bool send)
	{
		outputIO.connectedTo = inputIO;
		outputIO.rend.sharedMaterial = ioConnectMat;
		inputIO.connectedTo = outputIO;
		inputIO.rend.sharedMaterial = ioConnectMat;
		if (outputIO.type == 0 && powerThru)
		{
			outputIO.dev.newPowerThru();
		}
		if (outputIO.wire == null)
		{
			wire wire = null;
			wire = ((outputIO.type != 4) ? UnityEngine.Object.Instantiate(wirePrefab, points[0], Quaternion.identity) : UnityEngine.Object.Instantiate(pipePrefab, points[0], Quaternion.identity));
			wire.lr.positionCount = points.Length;
			wire.lr.SetPositions(points);
			outputIO.wire = wire;
		}
		else
		{
			if (points == null)
			{
				points = new Vector3[lr.positionCount];
				lr.GetPositions(points);
			}
			if (Vector3.Distance(inputIO.transform.position, points[0]) > 0.05f)
			{
				Array.Reverse((Array)points);
				lr.SetPositions(points);
			}
		}
		wire wire2 = outputIO.wire;
		if (outputIO.type == 0)
		{
			wire2.getMidpoint(points);
		}
		if (labelsOn)
		{
			wire2.powerDisplay.SetActive(value: true);
		}
		inputIO.wire = wire2;
		wire2.output = outputIO;
		wire2.input = inputIO;
		if (wire2.colorIndex > 0)
		{
			wire2.setMat(wire2.colorIndex, circuit: false);
		}
		else
		{
			wire2.setMat(color, circuit: false);
		}
		if (wire2 is pipe)
		{
			(wire2 as pipe).buildPipe(_placing: false, undo: false);
		}
		BuilderSystem.wireList.Add(wire2);
		if (!outputIO.dev.owner)
		{
			outputIO.dev.owner = outputIO.dev.GetComponent<BuilderPart>();
		}
		BuilderPart owner = outputIO.dev.owner;
		if (!inputIO.dev.owner)
		{
			inputIO.dev.owner = inputIO.dev.GetComponent<BuilderPart>();
		}
		BuilderPart owner2 = inputIO.dev.owner;
		float level = owner.level;
		float level2 = owner2.level;
		if (level < level2)
		{
			wire2.lvl = level;
		}
		else
		{
			wire2.lvl = level2;
		}
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendWire(Multiplayer.syncObjList.IndexOf(owner), inSyncId: Multiplayer.syncObjList.IndexOf(owner2), outIndex: outputIO.index, inIndex: inputIO.index, points: points, powerThru: true, sendAll: true);
		}
		return wire2;
	}

	private void changeLastPoint(Vector3[] points, bool undo)
	{
		if (points == null)
		{
			points = new Vector3[lr.positionCount];
			lr.GetPositions(points);
		}
		int num = points.Length;
		if (undo)
		{
			num--;
		}
		Vector3[] array = new Vector3[num];
		Array.Copy(points, array, array.Length);
		array[array.Length - 1] = points[points.Length - 1];
		lr.positionCount = array.Length;
		lr.SetPositions(array);
		endIndex = array.Length - 1;
		jointText.text = (16 - (endIndex - 1)).ToString();
		if (ioMode == 4)
		{
			(wire as pipe).buildPipe(_placing: true, undo: true);
			ref Vector3 reference = ref array[endIndex];
			placingLine.transform.parent.gameObject.SetActive(value: true);
			placingLine.updateLine(array[endIndex - 1], array[endIndex]);
		}
		calcWireLength(array);
	}

	public void disconnectEnd(io io)
	{
		wire = io.wire;
		BuilderSystem.wireList.Remove(io.wire);
		lr = io.wire.lr;
		io1 = io.connectedTo;
		io.wire = null;
		io1.wire = null;
		io.sendDisconnect(destroyed: false);
		if (io1.tag == "output")
		{
			io1Type = connected.output;
		}
		else
		{
			io1Type = connected.input;
		}
		Vector3[] array = new Vector3[lr.positionCount];
		lr.GetPositions(array);
		if (Vector3.Distance(io1.transform.position, lr.GetPosition(0)) > 0.1f)
		{
			Array.Reverse((Array)array);
			lr.SetPositions(array);
		}
		changeLastPoint(array, undo: false);
		if (BuilderSystem.multiplayer)
		{
			Sender.sendWireRemove(io);
		}
		lengthText.enabled = true;
		jointText.enabled = true;
	}

	public void wiringMode()
	{
		if (wireTog.isOn)
		{
			on = true;
			sys.hidePlacePart();
			ioTrigger.inst.ioTriggerCol.gameObject.SetActive(value: true);
			if (BuilderSystem.editMode)
			{
				BuilderUI.inst.editToggle.isOn = false;
			}
		}
		else
		{
			on = false;
			if (wirePanel.inst.panel.activeSelf)
			{
				wirePanel.inst.panel.SetActive(value: false);
			}
			ioTrigger.inst.stopDisplaying();
			ioTrigger.inst.ioTriggerCol.gameObject.SetActive(value: false);
			if (devPanel.panel != null)
			{
				devPanel.closePanel();
			}
			devPanel.text.enabled = false;
			resetWireData(triggerReset: true);
		}
		RayPlace.noPlace = on;
		if (!on)
		{
			sys.rpOverCheck();
		}
		sys.SetPlaceColMode(on);
		base.enabled = on;
		RectTransform[] array = modeButtons;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].GetChild(1).gameObject.SetActive(on);
		}
	}

	public void changeIoMode(int newMode)
	{
		if (!on)
		{
			wireTog.isOn = true;
		}
		if (newMode == ioMode)
		{
			return;
		}
		ioMode = newMode;
		ioTrigger.inst.stopDisplaying();
		ioTrigger.inst.ioTriggerCol.position = Vector3.zero;
		resetWireData(triggerReset: true);
		RectTransform rectTransform = null;
		if (ioMode == 0)
		{
			IoText.text = "wire";
			rectTransform = modeButtons[0];
			warningPanel.SetActive(value: false);
		}
		placingColorPanels[0].SetActive(ioMode == 0);
		if (ioMode == 4)
		{
			IoText.text = "pipe";
			rectTransform = modeButtons[1];
			warningPanel.SetActive(value: true);
		}
		placingColorPanels[1].SetActive(ioMode == 4);
		if (ioMode == 1)
		{
			IoText.text = "hose";
			rectTransform = modeButtons[2];
			warningPanel.SetActive(value: true);
		}
		rectTransform.sizeDelta = new Vector2(30f, 30f);
		RectTransform[] array = modeButtons;
		foreach (RectTransform rectTransform2 in array)
		{
			if (rectTransform2 != rectTransform)
			{
				rectTransform2.sizeDelta = new Vector2(25f, 25f);
			}
		}
	}

	public void setPlacingWireColor(int index)
	{
		int num = 0;
		if (ioMode == 4)
		{
			num = 1;
		}
		placingColorIndex[num] = index;
		if ((bool)wire)
		{
			wire.setMat(placingColorIndex[num], circuit: false);
		}
	}

	private void resetWireData(bool triggerReset)
	{
		if ((bool)wire)
		{
			UnityEngine.Object.Destroy(wire.gameObject);
		}
		wire = null;
		lr = null;
		wireLength = 0f;
		endIndex = 0;
		placingLine.transform.parent.gameObject.SetActive(value: false);
		io1Type = connected.none;
		ioType = connected.none;
		io1 = null;
		lengthText.enabled = false;
		jointText.enabled = false;
		if (triggerReset)
		{
			ioTrigger.inst.stopDisplaying();
			StartCoroutine(ioTrigger.inst.resetPos());
		}
		else
		{
			ioTrigger.inst.displayByType(connected.none);
		}
	}

	private IEnumerator ioInfoBlock(io io)
	{
		io.blockPanel = true;
		yield return null;
		io.blockPanel = false;
	}

	private IEnumerator warning()
	{
		warningPanel.SetActive(value: true);
		yield return new WaitForSeconds(1f);
		warningPanel.SetActive(value: false);
	}

	private void calcWireLength(Vector3[] points)
	{
		Vector3 a = Vector3.zero;
		wireLength = 0f;
		for (int i = 0; i < points.Length - 1; i++)
		{
			if (i > 0)
			{
				wireLength += Vector3.Distance(a, points[i]) * 3f;
			}
			a = points[i];
		}
	}

	public void wireFlowMode()
	{
		wireFlow = !wireFlow;
		foreach (wire wire2 in BuilderSystem.wireList)
		{
			if ((bool)wire2)
			{
				if (wire2.lockGlow)
				{
					wire2.highlightMat(state: true);
				}
				else
				{
					wire2.highlightMat(state: false);
				}
			}
		}
	}

	public void showLabels(bool state)
	{
		labelsOn = state;
		camAlignStatic.inst.enabled = state;
		foreach (wire wire2 in BuilderSystem.wireList)
		{
			if (!(wire2 == null) && wire2.output.type == 0)
			{
				if (!state)
				{
					wire2.powerText.text = "0";
				}
				else
				{
					wire2.powerText.text = wire2.output.power.ToString();
				}
				wire2.powerDisplay.SetActive(state);
			}
		}
	}

	private bool combinerCheck(io io2)
	{
		if (ioMode != 0)
		{
			return true;
		}
		if (io1.dev is combiner && io1.tag == "input")
		{
			return io2.dev.combine;
		}
		if (io2.dev is combiner && io2.tag == "input")
		{
			return io1.dev.combine;
		}
		return true;
	}
}
