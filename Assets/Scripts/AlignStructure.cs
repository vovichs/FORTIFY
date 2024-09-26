using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlignStructure : MonoBehaviour
{
	private BuilderSystem sys;

	public static AlignStructure inst;

	public bool on;

	public GameObject alignToggle;

	public GameObject alignPrefab;

	private Transform temp;

	private Transform temp1;

	private bool firstStep = true;

	public List<BuilderPart> alignList;

	public List<UndoPart> moveList = new List<UndoPart>();

	private RaycastHit hit;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
	}

	private void Update()
	{
		if (BuilderUI.inst.mouseOverUI || !Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 150f) || hit.collider.tag != "edgeCollider" || hit.transform.name.Substring(0, 1) != "b")
		{
			return;
		}
		Transform transform = hit.transform;
		Transform transform2 = temp.transform;
		transform2.SetPositionAndRotation(transform.position, transform.rotation);
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		bool selected = transform.parent.GetComponent<BuilderPart>().selected;
		if (firstStep)
		{
			if (!selected)
			{
				firstStep = false;
				popUp.inst.message("pick align edge from selection");
				temp1 = UnityEngine.Object.Instantiate(alignPrefab, transform2.position, transform2.rotation).transform;
			}
		}
		else if (selected)
		{
			alignPlace(transform2);
		}
	}

	private void alignPlace(Transform tempT)
	{
		bool isOn = Symmetry.inst.isOn;
		foreach (BuilderPart align in alignList)
		{
			if ((bool)align)
			{
				align._transform.parent = tempT;
			}
		}
		copyMove.inst.parentWires(tempT);
		tempT.SetPositionAndRotation(temp1.position, temp1.rotation * Quaternion.Euler(0f, 180f, 0f));
		foreach (BuilderPart align2 in alignList)
		{
			if ((bool)align2)
			{
				align2.gameObject.layer = 1;
				align2._transform.parent = null;
				if (!align2.deploy)
				{
					if ((bool)align2.block)
					{
						align2.oc = align2._transform.GetChild(0).gameObject.AddComponent<overlapCheck>();
						align2.oc.AddRigidbody();
					}
					if (align2.found && align2.sObj != null)
					{
						cupboardRange.inst.rangeList.Remove(align2.sObj.GetComponent<rangeCircle>());
						UnityEngine.Object.Destroy(align2.sObj);
					}
				}
			}
		}
		copyMove.inst.unparentWires();
		StartCoroutine(WaitToCheck());
		AudioPlayer.inst.playAtPoint(alignList[0]._transform.position, 0);
		if (BuilderSystem.multiplayer)
		{
			Sender.sendSound(alignList[0]._transform.position, 0);
		}
	}

	private IEnumerator WaitToCheck()
	{
		yield return new WaitForFixedUpdate();
		foreach (BuilderPart align in alignList)
		{
			GameObject gameObject = align.gameObject;
			if ((bool)align.block && (bool)align.oc)
			{
				if (align.oc.DestroySelfOverlap(align, send: true))
				{
					sys.objList.Remove(gameObject);
					UnityEngine.Object.Destroy(gameObject);
					continue;
				}
				UnityEngine.Object.Destroy(align._transform.GetChild(0).GetComponent<Rigidbody>());
				UnityEngine.Object.Destroy(align.oc);
			}
			if ((bool)align.sg)
			{
				align.sg.groupMove(align);
			}
			align.moved();
		}
		MGMT.inst.undoListInsert();
		MGMT.inst.undoListList[0] = moveList;
		sys.checkAllConditionals(clearNeighbors: true);
		overlapCheck.overlapCount = 0;
		foreach (BuilderPart align2 in alignList)
		{
			if ((bool)align2)
			{
				align2.setRaycastLayer();
			}
		}
		alignList.Clear();
		base.enabled = false;
	}

	private void OnEnable()
	{
		if (sys.inProgress)
		{
			base.enabled = false;
			return;
		}
		if (sys.objList.Count < 1)
		{
			popUp.inst.message("selection needed");
			base.enabled = false;
			return;
		}
		if (!Symmetry.inst.selectionCheck())
		{
			base.enabled = false;
			return;
		}
		on = true;
		alignList.Clear();
		moveList.Clear();
		foreach (GameObject obj in sys.objList)
		{
			BuilderPart component = obj.GetComponent<BuilderPart>();
			alignList.Add(component);
			moveList.Add(new MovedPart(component));
			if ((bool)component.sg)
			{
				for (int i = 0; i < component.sg.group.Length; i++)
				{
					BuilderPart builderPart = component.sg.group[i];
					if ((bool)builderPart && !builderPart.selected)
					{
						moveList.Add(new MovedPart(builderPart));
					}
				}
			}
		}
		popUp.inst.message("pick edge to align with");
		popUp.inst.message("(not on selection)");
		firstStep = true;
		sys.inProgress = true;
		temp = UnityEngine.Object.Instantiate(alignPrefab).transform;
		sys.SetPlaceColMode(off: false);
	}

	private void OnDisable()
	{
		on = false;
		alignList.Clear();
		sys.inProgress = false;
		if (temp != null)
		{
			UnityEngine.Object.Destroy(temp.gameObject);
		}
		if (temp1 != null)
		{
			UnityEngine.Object.Destroy(temp1.gameObject);
		}
		sys.SetPlaceColMode(off: true);
		alignToggle.GetComponent<Toggle>().isOn = false;
	}
}
