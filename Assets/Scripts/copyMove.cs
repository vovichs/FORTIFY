using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class copyMove : MonoBehaviour
{
	public enum Placing
	{
		structure,
		onFloor,
		onWall
	}

	public static copyMove inst;

	public static BuilderSystem sys;

	public GameObject cancelPanel;

	public bool inProcess;

	public bool move;

	public List<UndoPart> moveList = new List<UndoPart>();

	public bool merge;

	public static bool copyDeploys;

	public Placing placing;

	public Text text1;

	public Text text2;

	public Image image1;

	public Image image2;

	private float offset;

	private float placeParentOffset;

	private RaycastHit hit;

	private Ray ray;

	public Transform placeParent;

	public MovedPart placeParentOrigin;

	private float scrollAmount;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		copyDeploys = true;
		move = false;
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && !BuilderUI.inst.mouseOverUI)
		{
			if (placing == Placing.structure)
			{
				offset += UnityEngine.Input.GetAxis("Mouse Y");
				offset = Mathf.Clamp(offset, -0.49f, 20f);
				placeParent.position = hit.point;
				placeParent.Translate(Vector3.up * (offset + placeParentOffset));
			}
			else
			{
				place();
			}
			return;
		}
		if (Input.GetMouseButtonUp(0) && !BuilderUI.inst.mouseOverUI)
		{
			place();
			return;
		}
		ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (!Physics.Raycast(ray, out hit))
		{
			return;
		}
		if (hit.collider.tag == "deploy" || hit.collider.tag == "edgeCollider")
		{
			hide();
			return;
		}
		if (placing == Placing.onWall)
		{
			if (hit.collider.tag == "terrain")
			{
				hide();
				return;
			}
			if (hit.normal == Vector3.up || hit.normal == Vector3.down)
			{
				hide();
				return;
			}
		}
		else if (hit.collider.tag == "block" && (placing == Placing.structure || hit.normal != Vector3.up))
		{
			hide();
			return;
		}
		placeParent.position = hit.point;
		if (placing == Placing.onWall)
		{
			placeParent.rotation = Quaternion.LookRotation(hit.normal);
			return;
		}
		float num = 15f;
		if (Input.GetButton("rot-"))
		{
			placeParent.Rotate(Vector3.up, -15f);
		}
		else if (Input.GetButton("rot+"))
		{
			placeParent.Rotate(Vector3.up, 15f);
		}
		float axisRaw = UnityEngine.Input.GetAxisRaw("Mouse ScrollWheel");
		if (axisRaw != 0f)
		{
			if (UnityEngine.Input.GetKey(KeyCode.LeftAlt))
			{
				num = 2.5f;
			}
			if (num > 5f && scrollAmount < 10f)
			{
				num = 7.5f;
			}
			if (axisRaw > 0f)
			{
				placeParent.Rotate(Vector3.up, 0f - num);
			}
			else
			{
				placeParent.Rotate(Vector3.up, num);
			}
			if (scrollAmount < 40f)
			{
				scrollAmount += 10f;
			}
		}
		else if (scrollAmount >= 2f)
		{
			scrollAmount -= 2f;
		}
	}

	public void CopyMove(int mode)
	{
		merge = false;
		bool flag = false;
		sys.objList2.Clear();
		moveList.Clear();
		if (sys.objList.Count == 0)
		{
			popUp.inst.message("needs selection");
		}
		else
		{
			if (sys.inProgress)
			{
				return;
			}
			offset = 0f;
			placeParentOffset = 0f;
			switch (mode)
			{
			case 0:
				placing = Placing.structure;
				if (!move)
				{
					onlyMoveSwap();
				}
				merge = true;
				break;
			case 1:
				placing = Placing.structure;
				break;
			case 2:
				placing = Placing.onFloor;
				break;
			case 3:
				placing = Placing.onWall;
				break;
			}
			if (!move)
			{
				flag = true;
			}
			if (move && !Symmetry.inst.selectionCheck())
			{
				return;
			}
			inProcess = true;
			if (placing == Placing.onFloor)
			{
				bool flag2 = false;
				for (int num = sys.objList.Count - 1; num >= 0; num--)
				{
					BuilderPart component = sys.objList[num].GetComponent<BuilderPart>();
					if ((bool)component.deploy)
					{
						if (component.deploy.frameFloor || component.deploy.frameWall)
						{
							sys.objList.Remove(sys.objList[num]);
							sys.changeSelection(component, add: false);
						}
						if (!flag2 && component.deploy.shelf)
						{
							placeParent.position = component._transform.position;
							flag2 = true;
						}
					}
					else
					{
						sys.objList.Remove(sys.objList[num]);
						sys.changeSelection(component, add: false);
					}
					if (flag)
					{
						sys.objList2.Add(component);
					}
				}
				if (sys.objList.Count <= 0)
				{
					foreach (BuilderPart item in sys.objList2)
					{
						sys.changeSelection(item, add: true);
						sys.objList.Add(item.gameObject);
					}
					popUp.inst.message("select parts on floors");
					return;
				}
				if (!flag2)
				{
					placeParent.position = sys.objList[0].transform.position;
				}
				base.enabled = true;
			}
			else if (placing == Placing.onWall)
			{
				for (int num2 = sys.objList.Count - 1; num2 >= 0; num2--)
				{
					BuilderPart component2 = sys.objList[num2].GetComponent<BuilderPart>();
					if ((bool)component2.block)
					{
						sys.objList.Remove(sys.objList[num2]);
						sys.changeSelection(component2, add: false);
					}
					if (flag)
					{
						sys.objList2.Add(component2);
					}
				}
				if (sys.objList.Count <= 0)
				{
					foreach (BuilderPart item2 in sys.objList2)
					{
						sys.changeSelection(item2, add: true);
						sys.objList.Add(item2.gameObject);
					}
					popUp.inst.message("select parts on walls");
					return;
				}
				BuilderPart component3 = sys.objList[0].GetComponent<BuilderPart>();
				if ((bool)component3.deploy.offset)
				{
					placeParent.position = component3.deploy.offset.position;
				}
				else
				{
					placeParent.position = component3._transform.position;
				}
				base.enabled = true;
				Vector3 placeDirection = RayPlace.getPlaceDirection(component3.deploy, component3.transform);
				placeParent.rotation = Quaternion.LookRotation(placeDirection);
			}
			else
			{
				bool flag3 = false;
				foreach (GameObject obj in sys.objList)
				{
					if (!flag3 && obj.name.Length > 9 && obj.name.Substring(0, 3) == "fou")
					{
						float num3 = 0f;
						Transform transform = obj.transform;
						if (Physics.Raycast(transform.Find("snapPoint").position, Vector3.down, out RaycastHit hitInfo, 1f, 256))
						{
							float distance = hitInfo.distance;
							num3 = ((!(distance > 0.5f)) ? (0.5f - distance) : (0f - distance + 0.5f));
						}
						placeParent.position = transform.position + new Vector3(0f, num3, 0f);
						placeParentOffset = num3;
						offset = 0f - num3;
						flag3 = true;
					}
					if (flag)
					{
						sys.objList2.Add(obj.GetComponent<BuilderPart>());
					}
				}
				if (!flag3)
				{
					if (merge)
					{
						popUp.inst.message("no foundations in file");
						foreach (GameObject obj2 in sys.objList)
						{
							UnityEngine.Object.Destroy(obj2);
						}
						sys.objList.Clear();
					}
					else
					{
						popUp.inst.message("must include foundation");
					}
					return;
				}
				base.enabled = true;
			}
			if (move)
			{
				placeParentOrigin = new MovedPart(null, placeParent.rotation, placeParent.position, 0f, 0);
				parentWires(placeParent);
				foreach (GameObject obj3 in sys.objList)
				{
					obj3.layer = 2;
					BuilderPart component4 = obj3.GetComponent<BuilderPart>();
					component4._transform.SetParent(placeParent);
					moveList.Add(new MovedPart(component4));
					sys.changeSelection(obj3, add: false);
				}
			}
			else
			{
				foreach (GameObject obj4 in sys.objList)
				{
					copyPart(obj4);
				}
				copyWires();
				foreach (GameObject obj5 in sys.objList)
				{
					sys.changeSelection(obj5, add: false);
				}
			}
			sys.objList.Clear();
			sys.inProgress = true;
			cancelPanel.SetActive(value: true);
		}
	}

	private void moveListSymmetry(BuilderPart bp)
	{
		if (!bp.sg)
		{
			return;
		}
		for (int i = 0; i < bp.sg.group.Length; i++)
		{
			BuilderPart builderPart = bp.sg.group[i];
			if ((bool)builderPart && !builderPart.selected)
			{
				moveList.Add(new MovedPart(builderPart));
			}
		}
	}

	public void place()
	{
		unparentWires();
		MGMT.inst.undoListInsert();
		foreach (Transform item in placeParent)
		{
			BuilderPart component = item.GetComponent<BuilderPart>();
			if ((bool)component)
			{
				if (placing != 0)
				{
					deployLevelCheck(component);
				}
				if (!move || merge)
				{
					sys.PlacedSetup(component, send: true, notLoaded: false, sound: false, useCodeTog: false);
					MGMT.inst.undoListAddedPart(component);
				}
				else
				{
					component.moved();
					if ((bool)component.sg)
					{
						component.sg.groupMove(component);
					}
					if (component.device is autoTurret)
					{
						(component.device as autoTurret).Initialize(add: false);
					}
					component.setRaycastLayer();
				}
				GameObject gameObject = item.gameObject;
				sys.objList.Add(gameObject);
				sys.changeSelection(gameObject, add: true);
			}
		}
		if (move && !merge)
		{
			MGMT.inst.undoListList[0] = moveList;
		}
		placeParent.DetachChildren();
		AudioPlayer.inst.playAtPoint(placeParent.position, 0);
		if (placing == Placing.structure)
		{
			sys.checkAllConditionals(move);
		}
		inProcess = false;
		sys.inProgress = false;
		sys.objList2.Clear();
		cancelPanel.SetActive(value: false);
		base.enabled = false;
	}

	public void moveLevel(bool up)
	{
		MGMT.inst.undoListInsert();
		Vector3 vector = new Vector3(0f, 1f, 0f);
		if (!up)
		{
			vector = new Vector3(0f, -1f, 0f);
		}
		if (!up)
		{
			bool flag = false;
			foreach (GameObject obj in sys.objList)
			{
				BuilderPart component = obj.GetComponent<BuilderPart>();
				if (!component.found && component.level != 0f)
				{
					if (component.floor)
					{
						if (component.level <= 2f)
						{
							flag = true;
							break;
						}
					}
					else if (component.level <= 1f)
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				popUp.inst.message("parts too low");
				popUp.inst.message("preventing overlap");
				return;
			}
		}
		foreach (GameObject obj2 in sys.objList)
		{
			BuilderPart component2 = obj2.GetComponent<BuilderPart>();
			if (!component2.found && component2.level != 0f)
			{
				component2._transform.position += vector;
				if (up)
				{
					component2.level += 1f;
				}
				else
				{
					component2.level -= 1f;
				}
				MGMT.inst.undoListList[0].Add(new MovedPart(component2));
				if ((bool)component2.sg && Symmetry.inst.isOn)
				{
					component2.sg.groupVerticalMove(vector, component2.level);
				}
				component2.moved();
			}
		}
		sys.checkAllConditionals(clearNeighbors: true);
		foreach (wire wire in BuilderSystem.wireList)
		{
			bool flag2 = false;
			BuilderPart owner = wire.output.dev.owner;
			BuilderPart owner2 = wire.input.dev.owner;
			if (owner.selected || owner2.selected)
			{
				if (!owner2.selected || !owner.selected)
				{
					flag2 = true;
				}
				if (!flag2 && !BuilderSystem.multiplayer)
				{
					Vector3[] array = new Vector3[wire.lr.positionCount];
					wire.lr.GetPositions(array);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] += vector;
					}
					wire.lr.SetPositions(array);
					if (wire is pipe)
					{
						(wire as pipe).buildPipe(_placing: false, undo: false);
					}
					if (up)
					{
						wire.lvl += 1f;
					}
					else
					{
						wire.lvl -= 1f;
					}
				}
				else
				{
					if (BuilderSystem.multiplayer)
					{
						Sender.sendWireRemove(wire.output);
					}
					wire.output.sendDisconnect(destroyed: false);
				}
			}
		}
	}

	public GameObject copyPart(GameObject obj)
	{
		BuilderPart component = obj.GetComponent<BuilderPart>();
		if (!copyDeploys && (bool)component.deploy && !component.door && !component.deploy.frameFloor && !component.deploy.frameWall && !component.deploy.windowPart)
		{
			sys.changeSelection(component, add: false);
			return null;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(obj, component._transform.position, component._transform.rotation, placeParent);
		BuilderPart component2 = gameObject.GetComponent<BuilderPart>();
		component2.removeSymmetry();
		component2.instId = 0;
		component2.sObj = null;
		if ((bool)component2.block)
		{
			component2.block.ClearNeighborLinks();
		}
		else
		{
			if ((bool)component2.device)
			{
				if (component2.device is elevator)
				{
					(component2.device as elevator).stackHolder = null;
				}
				component2.device.clearConnectedTo();
			}
			if ((bool)component2.door)
			{
				component2.door.doorCtrl = null;
			}
		}
		gameObject.name = obj.name;
		gameObject.layer = 2;
		return gameObject;
	}

	public void copyWires()
	{
		List<Device> list = new List<Device>();
		for (int i = 0; i < BuilderSystem.wireList.Count; i++)
		{
			wire wire = BuilderSystem.wireList[i];
			BuilderPart owner = wire.output.dev.owner;
			BuilderPart owner2 = wire.input.dev.owner;
			if (!owner.selected || !owner2.selected)
			{
				continue;
			}
			int num = sys.objList.IndexOf(owner.gameObject);
			if (num < 0 || wire.output.index < 0)
			{
				continue;
			}
			int num2 = sys.objList.IndexOf(owner2.gameObject);
			if (num2 >= 0 && wire.input.index >= 0)
			{
				io io = placeParent.GetChild(num).GetComponent<Device>().outputTo[wire.output.index];
				io inputIO = placeParent.GetChild(num2).GetComponent<Device>().inputFrom[wire.input.index];
				io.wire = null;
				if (io.dev.powerSource)
				{
					list.Add(io.dev);
				}
				Vector3[] array = new Vector3[wire.lr.positionCount];
				wire.lr.GetPositions(array);
				wire wire2 = wiring.inst.wiredConnect(io, inputIO, array, wire.colorIndex, powerThru: false, send: true);
				wire2.lvl = wire.lvl;
				Transform transform = wire2.transform;
				for (int j = 0; j < array.Length; j++)
				{
					array[j] = transform.InverseTransformPoint(array[j]);
				}
				wire2.lr.useWorldSpace = false;
				wire2.lr.SetPositions(array);
				transform.SetParent(placeParent);
			}
		}
		foreach (Device item in list)
		{
			item.newPowerThru();
		}
	}

	public void parentWires(Transform parent)
	{
		foreach (wire wire in BuilderSystem.wireList)
		{
			bool flag = false;
			BuilderPart owner = wire.output.dev.owner;
			BuilderPart owner2 = wire.input.dev.owner;
			if (owner.selected || owner2.selected)
			{
				if (!owner2.selected || !owner.selected)
				{
					flag = true;
				}
				if (!flag && !BuilderSystem.multiplayer)
				{
					Vector3[] array = new Vector3[wire.lr.positionCount];
					wire.lr.GetPositions(array);
					moveList.Add(new MovedWire(wire, (Vector3[])array.Clone()));
					Transform transform = wire.transform;
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = transform.InverseTransformPoint(array[i]);
					}
					wire.lr.useWorldSpace = false;
					wire.lr.SetPositions(array);
					transform.SetParent(parent);
				}
				else
				{
					if (BuilderSystem.multiplayer)
					{
						Sender.sendWireRemove(wire.output);
					}
					wire.output.sendDisconnect(destroyed: false);
				}
			}
		}
	}

	public void unparentWires()
	{
		foreach (wire wire in BuilderSystem.wireList)
		{
			if (!(wire == null))
			{
				Transform transform = wire.transform;
				if (!(transform.parent == null))
				{
					transform.parent = null;
					Vector3[] array = new Vector3[wire.lr.positionCount];
					wire.lr.GetPositions(array);
					for (int i = 0; i < array.Length; i++)
					{
						array[i] = transform.TransformPoint(array[i]);
					}
					wire.lr.SetPositions(array);
					wire.lr.useWorldSpace = true;
				}
			}
		}
	}

	private void hide()
	{
		placeParent.position = new Vector3(0f, -1000f, 0f);
	}

	public void copyDeploysSwap()
	{
		copyDeploys = !copyDeploys;
	}

	public void onlyMoveSwap()
	{
		if (!sys.inProgress)
		{
			string text = text1.text;
			text1.text = text2.text;
			text2.text = text;
			move = !move;
			image1.enabled = !move;
			image2.enabled = move;
		}
	}

	private void deployLevelCheck(BuilderPart bp)
	{
		Collider[] array = Physics.OverlapSphere(bp._transform.position, 0.03f, 1);
		for (int i = 0; i < array.Length; i++)
		{
			BuilderPart component = array[i].GetComponent<BuilderPart>();
			if (!(component == null))
			{
				if ((bool)component.deploy)
				{
					StartCoroutine(waitForLvl(bp, component));
				}
				else
				{
					bp.level = component.level;
				}
				return;
			}
		}
		bp.level = 1f;
	}

	private IEnumerator waitForLvl(BuilderPart bp, BuilderPart hitbp)
	{
		yield return null;
		bp.level = hitbp.level;
	}

	public void Cancel()
	{
		if (move)
		{
			placeParent.SetPositionAndRotation(placeParentOrigin.pos, placeParentOrigin.rot);
			place();
		}
		else
		{
			if (sys.inProgress && placeParent != null && placeParent.childCount > 0)
			{
				foreach (Transform item in placeParent)
				{
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
			foreach (BuilderPart item2 in sys.objList2)
			{
				sys.changeSelection(item2, add: true);
				sys.objList.Add(item2.gameObject);
			}
		}
		sys.objList2.Clear();
		cancelPanel.SetActive(value: false);
		base.enabled = false;
		inProcess = false;
		sys.inProgress = false;
	}

	public void OnClickMessage()
	{
		popUp.inst.message("select an option above");
	}
}
