using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MGMT : MonoBehaviour
{
	public static MGMT inst;

	public static BuilderSystem sys;

	public static bool destroyInProgress;

	public List<GameObject> destroyObjs = new List<GameObject>();

	public List<UndoPart> undoList = new List<UndoPart>();

	public List<block> blockList = new List<block>();

	public List<List<UndoPart>> undoListList = new List<List<UndoPart>>();

	private List<List<UndoPart>> redoListList = new List<List<UndoPart>>();

	public Dictionary<int, Device> devices = new Dictionary<int, Device>();

	public Mesh[] wallMesh;

	public Mesh found;

	public Mesh foundWood;

	public Mesh foundTri;

	public Mesh foundTriWood;

	public Mesh[] foundTriCols;

	public Mesh[] rampLow;

	public Mesh[] rampHigh;

	public Mesh[] spiral;

	public Mesh[] spiralTri;

	public GameObject[] avatars;

	public GameObject[] avatars3rd;

	public GameObject[] prefabList;

	public GameObject[] testList;

	private int max = 15;

	private void Awake()
	{
		sys = BuilderSystem.inst;
		if (inst != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		inst = this;
		Object.DontDestroyOnLoad(this);
		for (int i = 0; i < max; i++)
		{
			undoListList.Add(new List<UndoPart>());
			redoListList.Add(new List<UndoPart>());
		}
	}

	private void Start()
	{
	}

	public void Undo()
	{
		if (undoListList[0].Count == 0)
		{
			undoListList.RemoveAt(0);
			undoListList.Add(new List<UndoPart>());
			return;
		}
		undoList = new List<UndoPart>(undoListList[0]);
		undoListList.RemoveAt(0);
		undoListList.Add(new List<UndoPart>());
		redoListInsert();
		for (int i = 0; i < undoList.Count; i++)
		{
			if (undoList[i] is DeletedPart)
			{
				BuilderPart bp = returnDeletedPart(i, undo: true);
				if (Symmetry.inst.isOn)
				{
					StartCoroutine(waitForSymmetry(bp));
				}
			}
			else if (undoList[i] is AddedPart)
			{
				removeAddedPart(i, undo: true);
			}
			else if (undoList[i] is MovedPart || undoList[i] is MovedWire)
			{
				restoreMovedPart(i, undo: true);
			}
			else if (undoList[i] is AddedWire)
			{
				removeAddedWire(i, undo: true);
			}
		}
		for (int j = 0; j < undoList.Count; j++)
		{
			if (undoList[j] is DeletedWire)
			{
				returnDeletedWire(j, undo: true);
			}
		}
		ConditionalChecks(updateNeighbors: true);
		undoList.Clear();
	}

	public void Redo()
	{
		if (redoListList[0].Count == 0)
		{
			redoListList.RemoveAt(0);
			redoListList.Add(new List<UndoPart>());
			return;
		}
		undoList = new List<UndoPart>(redoListList[0]);
		redoListList.RemoveAt(0);
		redoListList.Add(new List<UndoPart>());
		undoListInsert();
		for (int i = 0; i < undoList.Count; i++)
		{
			if (undoList[i] is DeletedPart)
			{
				BuilderPart bp = returnDeletedPart(i, undo: false);
				if (Symmetry.inst.isOn)
				{
					StartCoroutine(waitForSymmetry(bp));
				}
			}
			else if (undoList[i] is AddedPart)
			{
				removeAddedPart(i, undo: false);
			}
			else if (undoList[i] is MovedPart || undoList[i] is MovedWire)
			{
				restoreMovedPart(i, undo: false);
			}
			else if (undoList[i] is AddedWire)
			{
				removeAddedWire(i, undo: false);
			}
		}
		for (int j = 0; j < undoList.Count; j++)
		{
			if (undoList[j] is DeletedWire)
			{
				returnDeletedWire(j, undo: false);
			}
		}
		ConditionalChecks(updateNeighbors: true);
		undoList.Clear();
	}

	public void undoListInsert()
	{
		undoListList.Insert(0, new List<UndoPart>());
		undoListList.RemoveAt(max);
	}

	private void redoListInsert()
	{
		redoListList.Insert(0, new List<UndoPart>());
		redoListList.RemoveAt(max);
	}

	public void clearLists()
	{
		undoList.Clear();
		devices.Clear();
		for (int i = 0; i < undoListList.Count; i++)
		{
			undoListList[i].Clear();
		}
		for (int j = 0; j < redoListList.Count; j++)
		{
			redoListList[j].Clear();
		}
	}

	public void undoListAddedPart(BuilderPart bp)
	{
		undoListList[0].Add(new AddedPart(bp));
	}

	public void addDeletedPart(BuilderPart bp, bool undo)
	{
		io[] outputTo;
		if (undo)
		{
			undoListList[0].Add(newDeletedPart(bp));
			if (!bp.device)
			{
				return;
			}
			outputTo = bp.device.outputTo;
			foreach (io io in outputTo)
			{
				if (io.wire != null && !io.wire.skip)
				{
					undoListList[0].Add(newDeletedWire(io.wire));
				}
			}
			outputTo = bp.device.inputFrom;
			foreach (io io2 in outputTo)
			{
				if (io2.wire != null && !io2.wire.skip)
				{
					undoListList[0].Add(newDeletedWire(io2.wire));
				}
			}
			return;
		}
		redoListList[0].Add(newDeletedPart(bp));
		if (!bp.device)
		{
			return;
		}
		outputTo = bp.device.outputTo;
		foreach (io io3 in outputTo)
		{
			if (io3.wire != null && !io3.wire.skip)
			{
				redoListList[0].Add(newDeletedWire(io3.wire));
			}
		}
		outputTo = bp.device.inputFrom;
		foreach (io io4 in outputTo)
		{
			if (io4.wire != null && !io4.wire.skip)
			{
				redoListList[0].Add(newDeletedWire(io4.wire));
			}
		}
	}

	public DeletedPart newDeletedPart(BuilderPart bp)
	{
		if (bp == null)
		{
			return null;
		}
		Transform transform = bp._transform;
		if ((bool)bp.block)
		{
			return new DeletedPart(bp.id, bp.instId, transform.rotation, transform.position, bp.level, bp.tier, bp.stage);
		}
		int val = 0;
		if ((bool)bp.device)
		{
			val = ((bp.device is timerDevice) ? Mathf.RoundToInt((bp.device as timerDevice).time) : ((!(bp.device is battery)) ? bp.device.value : (bp.device as battery).chargePower));
		}
		return new DeletedPart(bp.id, bp.instId, transform.rotation, transform.position, bp.level, bp.stage, bp.GetCodeString(), bp.GetDoorState(), val);
	}

	public DeletedWire newDeletedWire(wire w)
	{
		w.skip = true;
		Vector3[] array = new Vector3[w.lr.positionCount];
		w.lr.GetPositions(array);
		return new DeletedWire(w.output.dev.owner.instId, InId: w.input.dev.owner.instId, OutIndex: w.output.index, InIndex: w.input.index, Verts: array, ColorId: w.colorIndex);
	}

	private BuilderPart returnDeletedPart(int i, bool undo)
	{
		DeletedPart deletedPart = undoList[i] as DeletedPart;
		GameObject gameObject = UnityEngine.Object.Instantiate(prefabList[deletedPart.id], deletedPart.pos, deletedPart.rot);
		gameObject.name = prefabList[deletedPart.id].name;
		BuilderPart component = gameObject.GetComponent<BuilderPart>();
		component.level = deletedPart.floor;
		component.stage = deletedPart.stage;
		component.instId = deletedPart.instId;
		if ((bool)component.block)
		{
			component.tier = deletedPart.tier;
			blockList.Add(component.block);
		}
		if (Symmetry.inst.isOn)
		{
			component.sg = component.gameObject.AddComponent<symmetryGroup>();
		}
		sys.PlacedSetup(component, send: true, notLoaded: false, sound: false, useCodeTog: false);
		if ((bool)component.deploy)
		{
			if ((bool)component.door)
			{
				if (!deletedPart.check || Stability.inst.raidMode)
				{
					StartCoroutine(component.changeDoorMeshState(state: false, audio: false, send: false));
				}
				component.door.controlCheck();
			}
			if ((bool)component.deploy.lockEnt && deletedPart.code.Length > 0)
			{
				locks.inst.addCodelock(component, deletedPart.code);
			}
			if ((bool)component.device)
			{
				if (component.device is timerDevice)
				{
					(component.device as timerDevice).time = deletedPart.val;
				}
				else
				{
					component.device.setValue(deletedPart.val, send: false);
				}
			}
		}
		if (undo)
		{
			redoListList[0].Add(new AddedPart(component));
		}
		else
		{
			undoListList[0].Add(new AddedPart(component));
		}
		return component;
	}

	private bool removeAddedPart(int ind, bool undo)
	{
		AddedPart addedPart = undoList[ind] as AddedPart;
		BuilderPart builderPart = addedPart.bp;
		if (builderPart == null)
		{
			builderPart = findInstId(addedPart.instId);
			if (builderPart == null)
			{
				return false;
			}
		}
		if (builderPart.gameObject.tag == "destroy")
		{
			return false;
		}
		if (builderPart.selected)
		{
			sys.objList.Remove(builderPart.gameObject);
		}
		addDeletedPart(builderPart, !undo);
		sys.destroyPart(builderPart, audio: false, receive: false);
		return true;
	}

	private void restoreMovedPart(int i, bool undo)
	{
		if (undoList[i] is MovedPart)
		{
			MovedPart movedPart = undoList[i] as MovedPart;
			BuilderPart builderPart = movedPart.bp;
			if (builderPart == null)
			{
				builderPart = findInstId(movedPart.instId);
				if (builderPart == null)
				{
					return;
				}
			}
			if (undo)
			{
				redoListList[0].Add(new MovedPart(builderPart));
			}
			else
			{
				undoListList[0].Add(new MovedPart(builderPart));
			}
			if ((bool)builderPart.block)
			{
				builderPart.block.UpdateNeighborLinks(removed: true);
				blockList.Add(builderPart.block);
			}
			builderPart._transform.SetPositionAndRotation(movedPart.pos, movedPart.rot);
			builderPart.moved();
			if ((bool)builderPart.sg)
			{
				builderPart.sg.groupMove(builderPart);
			}
			return;
		}
		MovedWire movedWire = undoList[i] as MovedWire;
		if (!(movedWire.w == null))
		{
			Vector3[] array = new Vector3[movedWire.w.lr.positionCount];
			movedWire.w.lr.GetPositions(array);
			if (undo)
			{
				redoListList[0].Add(new MovedWire(movedWire.w, array));
			}
			else
			{
				undoListList[0].Add(new MovedWire(movedWire.w, array));
			}
			movedWire.w.lr.SetPositions(movedWire.verts);
			if (movedWire.w is pipe)
			{
				(movedWire.w as pipe).buildPipe(_placing: false, undo: false);
			}
		}
	}

	private void returnDeletedWire(int i, bool undo)
	{
		DeletedWire deletedWire = undoList[i] as DeletedWire;
		Device value2;
		if (devices.TryGetValue(deletedWire.outId, out Device value) && (bool)value && devices.TryGetValue(deletedWire.inId, out value2) && (bool)value2)
		{
			io io = value.outputTo[deletedWire.outIndex];
			io io2 = value2.inputFrom[deletedWire.inIndex];
			if ((bool)io && io.connectedTo == null && (bool)io2 && io2.connectedTo == null)
			{
				wire w = wiring.inst.wiredConnect(io, io2, deletedWire.verts, deletedWire.colorId, powerThru: true, send: true);
				if (undo)
				{
					redoListList[0].Add(new AddedWire(w));
				}
				else
				{
					undoListList[0].Add(new AddedWire(w));
				}
			}
			else
			{
				MonoBehaviour.print("io error");
			}
		}
		else
		{
			MonoBehaviour.print("id error");
		}
	}

	private void removeAddedWire(int i, bool undo)
	{
		wire w = (undoList[i] as AddedWire).w;
		if (!(w == null) && !w.skip)
		{
			if (undo)
			{
				redoListList[0].Add(newDeletedWire(w));
			}
			else
			{
				undoListList[0].Add(newDeletedWire(w));
			}
			w.output.sendDisconnect(destroyed: false);
		}
	}

	private IEnumerator waitForSymmetry(BuilderPart bp)
	{
		yield return new WaitForFixedUpdate();
		if ((bool)bp)
		{
			sys.AddSymmetry(bp, addSelection: false);
		}
	}

	public void ConditionalChecks(bool updateNeighbors)
	{
		for (int i = 0; i < blockList.Count; i++)
		{
			if (!(blockList[i] == null))
			{
				blockList[i].GetNeighborLinks(updateNeighbors, clear: true);
			}
		}
		for (int j = 0; j < blockList.Count; j++)
		{
			if (!(blockList[j] == null))
			{
				if ((bool)blockList[j].conditional)
				{
					blockList[j].conditional.RunCheck();
				}
				if ((bool)blockList[j].stabilityEntity)
				{
					blockList[j].stabilityEntity.UpdateStability();
				}
			}
		}
		blockList.Clear();
	}

	private BuilderPart findInstId(int InstId)
	{
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			if (sys.bpList[i].instId == InstId)
			{
				return sys.bpList[i];
			}
		}
		return null;
	}

	public GameObject getPrefabFromName(string named)
	{
		for (int i = 0; i < prefabList.Length; i++)
		{
			if (prefabList[i].name == named)
			{
				return prefabList[i];
			}
		}
		return null;
	}

	public void getDestroyObjs(bool clearScene)
	{
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			if (!(sys.bpList[i] == null))
			{
				GameObject gameObject = sys.bpList[i].gameObject;
				destroyObjs.Add(gameObject);
				if (clearScene)
				{
					Object.DontDestroyOnLoad(gameObject);
				}
				gameObject.SetActive(value: false);
				gameObject.tag = "destroy";
			}
		}
		sys.bpList.Clear();
	}

	public IEnumerator destroySlowly()
	{
		if (destroyObjs.Count == 0)
		{
			yield break;
		}
		GameObject[] objs = destroyObjs.ToArray();
		destroyObjs.Clear();
		destroyInProgress = true;
		int num = 0;
		for (int i = 0; i < objs.Length; i++)
		{
			if (!(objs[i] == null))
			{
				UnityEngine.Object.Destroy(objs[i]);
				num++;
				if (num == 50)
				{
					yield return null;
					num = 0;
				}
			}
		}
		destroyInProgress = false;
	}

	public IEnumerator elevatorStackChange(elevator elev)
	{
		if (!elev.stackHolder || elev.stackHolder.isRemoving)
		{
			yield break;
		}
		elevator stackHolder = elev.stackHolder;
		stackHolder.isRemoving = true;
		List<elevator> stack = new List<elevator>(stackHolder.stack);
		elevator newStackHolder = null;
		yield return null;
		for (int i = 0; i < stack.Count; i++)
		{
			if (stack[i] == null)
			{
				if (i > 0 && stack[i - 1] != null)
				{
					stack[i - 1].setTopConditional(set: true);
				}
				continue;
			}
			if (i > 0 && !stack[i - 1])
			{
				newStackHolder = stack[i];
				newStackHolder.stack.Clear();
			}
			if ((bool)newStackHolder)
			{
				newStackHolder.stack.Add(stack[i]);
				stack[i].stackHolder = newStackHolder;
				if ((bool)stackHolder)
				{
					stackHolder.stack[i] = null;
				}
			}
		}
		stackHolder.isRemoving = false;
		foreach (elevator item in stack)
		{
			if ((bool)item && item.top && item.lift.activeSelf)
			{
				item.moveStop();
			}
		}
		if ((bool)stackHolder)
		{
			stackHolder.stack = (from item in stackHolder.stack
				where item != null
				select item).ToList();
		}
	}
}
