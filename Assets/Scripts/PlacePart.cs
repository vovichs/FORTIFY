using System.Collections;
using UnityEngine;

public class PlacePart : MonoBehaviour
{
	public static BuilderSystem cs;

	public static RaycastHit hit;

	private bool placed;

	private block pblock;

	private Collider col;

	private Transform t;

	public bool twoSided;

	public bool levelUpPart;

	public bool midWall;

	public bool frameDeploy;

	public bool conditionBlocked;

	public static bool rampHigh;

	public static bool walldir;

	public static bool contPlace;

	public static bool clickWait;

	private bool proximityFail;

	private void OnMouseEnter()
	{
		if (pblock == null)
		{
			t = base.transform;
			pblock = t.parent.GetComponent<block>();
			col = t.GetComponent<Collider>();
		}
		if (contPlace || BuilderSystem.disableInput || BuilderSystem.editMode || BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		if (pblock.found && cs.BPinfo.found)
		{
			if (!RayPlaceGround.heightAdjust)
			{
				extendArrow.inst.alignWith(t.position, -t.right);
				PlaceOn();
			}
			return;
		}
		if (cs.BPinfo.wall && cs.BPinfo.block.edgeCols.Length != 0)
		{
			if (extendArrow.inst.tog.isOn)
			{
				extendArrow.inst.arrow.SetPositionAndRotation(t.position, t.rotation * Quaternion.Euler(0f, 0f, 90f));
			}
		}
		else if (cs.BPinfo.floor && !pblock.found)
		{
			extendArrow.inst.alignWith(t.position, -t.right);
		}
		if (Input.GetMouseButton(0) && placeOptions.continuous && !clickWait)
		{
			if (!placeOptions.allowOverlap && (bool)cs.BPinfo.block && cs.BPinfo.block.continuous && !RayPlaceGround.heightAdjust && (!pblock.wall || cs.BPinfo.floor))
			{
				PlaceOn();
				if (!cs.redblock)
				{
					StartCoroutine(placeObjWait(cs.BPinfo.gameObject));
				}
			}
		}
		else
		{
			PlaceOn();
		}
	}

	private void OnMouseOver()
	{
		if (cs.swapCheck)
		{
			contPlace = false;
			PlaceOn();
			proximityFail = false;
		}
		if (proximityFail)
		{
			cs.redblock = true;
		}
		if (Input.anyKey && (Input.GetButtonDown("rot-") || Input.GetButtonDown("rot+")) && cs.BPinfo.wall)
		{
			walldir = !walldir;
			cs.BPinfo._transform.Rotate(0f, 180f, 0f);
		}
		if (!twoSided || !cs.BPinfo.floor || contPlace)
		{
			return;
		}
		Ray ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (!col.Raycast(ray, out hit, 150f))
		{
			return;
		}
		cs.redblock = false;
		Transform transform = cs.BPinfo._transform;
		Quaternion rotation = transform.rotation;
		if (Vector3.Dot(hit.point - t.position, t.right) > 0f)
		{
			transform.rotation = t.rotation * Quaternion.AngleAxis(180f, Vector3.up);
			if (pblock.roof && cs.BPinfo.block.tri)
			{
				cs.redblock = true;
			}
		}
		else
		{
			transform.rotation = t.rotation;
			if (pblock.roof && cs.BPinfo.block.tri)
			{
				cs.redblock = false;
			}
		}
		extendArrow.inst.alignWith(t.position, -transform.right);
		if (placeOptions.proximityCheck && floorProximityCheck())
		{
			cs.redblock = true;
		}
		else if (cs.BPinfo.block.checkForPipes())
		{
			cs.redblock = true;
		}
	}

	private void OnMouseExit()
	{
		cs.redblock = false;
		proximityFail = false;
		if (!contPlace)
		{
			BuilderSystem.inst.hidePlacePart();
		}
		else
		{
			extendArrow.inst.hideArrow();
		}
		if (BuilderSystem.multiplayer && (bool)cs.BPinfo.block)
		{
			Sender.sendObj = null;
			Sender.send(12);
		}
		base.enabled = false;
	}

	private void OnMouseDown()
	{
		if (!clickWait && placed && !BuilderUI.inst.mouseOverUI && !BuilderSystem.disableInput)
		{
			StartCoroutine(placeObjWait(cs.BPinfo.gameObject));
		}
	}

	private bool compatibleCheck()
	{
		if (placeOptions.ignoreRules && (bool)cs.BPinfo.block && !frameDeploy && !cs.BPinfo.block.pillar)
		{
			return true;
		}
		if ((bool)cs.BPinfo.block)
		{
			block block = cs.BPinfo.block;
			if (frameDeploy)
			{
				return false;
			}
			if (conditionBlocked && block.roof)
			{
				return false;
			}
			if (block.found && !pblock.found)
			{
				return false;
			}
			if (block.floor && pblock.found)
			{
				return false;
			}
			if (midWall && !block.floor)
			{
				return false;
			}
			if (block.stairs)
			{
				if (block.steps || block.ramp)
				{
					if (pblock.floor && pblock.tri)
					{
						return false;
					}
				}
				else if (block.tri != pblock.tri)
				{
					return false;
				}
				if (pblock.wall || pblock.frame || pblock.roof)
				{
					return false;
				}
			}
			if (pblock.wall && block.roof)
			{
				return true;
			}
			if (pblock.ramp && !block.ramp)
			{
				return false;
			}
			if (cs.BPinfo.block.pillar)
			{
				return false;
			}
			return true;
		}
		if (cs.BPinfo.deploy.frameWall && frameDeploy && pblock.wall)
		{
			return true;
		}
		if (cs.BPinfo.deploy.frameFloor && frameDeploy && pblock.floor && pblock.tri == cs.BPinfo.tri)
		{
			return true;
		}
		return false;
	}

	private void PlaceOn()
	{
		placed = false;
		base.enabled = true;
		if (BuilderSystem.editMode || BuilderSystem.placeMode || !compatibleCheck())
		{
			return;
		}
		Transform transform = cs.BPinfo._transform;
		placed = true;
		transform.SetPositionAndRotation(t.position, t.rotation);
		BuilderPart bPinfo = cs.BPinfo;
		block block = bPinfo.block;
		if (!block)
		{
			return;
		}
		if (bPinfo.found)
		{
			transform.Translate(Vector3.down * 0.5f);
			if (!Proximity.inst.Check(bPinfo) || block.heightCheck() || block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else if (bPinfo.wall)
		{
			Vector3 to = CameraCtrl.inst.cam.transform.position - transform.position;
			float num = Vector3.Angle(transform.right, to);
			if (walldir)
			{
				if (num <= 89f)
				{
					transform.Rotate(0f, 180f, 0f);
				}
			}
			else if (num >= 90f)
			{
				transform.Rotate(0f, 180f, 0f);
			}
			if (!Proximity.inst.Check(bPinfo) || wallSocketCheck() || block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else if (bPinfo.floor)
		{
			if (placeOptions.proximityCheck && floorProximityCheck())
			{
				cs.redblock = true;
			}
			else if (block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else if (block.roof)
		{
			if (!pblock.roof && !pblock.wall)
			{
				transform.Rotate(0f, 180f, 0f);
			}
			if (wallSocketCheck() || block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else if (block.steps || block.ramp)
		{
			if (!pblock.ramp)
			{
				if (block.ramp)
				{
					if (pblock.tri)
					{
						transform.Translate(Vector3.down * 0.25f);
					}
					else
					{
						transform.Translate(Vector3.right * 1f);
						if (rampHigh)
						{
							transform.Translate(Vector3.up * 0.25f);
						}
					}
				}
				else if (!pblock.tri || pblock.floor)
				{
					transform.Translate(Vector3.right * 1f);
				}
				else
				{
					transform.Translate(Vector3.down * 0.5f);
				}
			}
			if (block.heightCheck() || block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = bPinfo;
			Sender.send(12);
		}
	}

	private IEnumerator placeObjWait(GameObject obj)
	{
		if (pblock.wall || pblock.floor)
		{
			StartCoroutine(ClickWait());
		}
		cs.StartCoroutine(cs.continuousWait());
		yield return new WaitForFixedUpdate();
		if (!obj || (overlapCheck.overlapCount > 0 && !placeOptions.allowOverlap))
		{
			yield break;
		}
		float num = pblock.owner.level;
		if (pblock.halfHeight)
		{
			if (pblock.found)
			{
				num = 0.5f;
			}
			if (levelUpPart)
			{
				num += 0.5f;
			}
			if (pblock.floor)
			{
				num -= 0.5f;
			}
		}
		else if (levelUpPart)
		{
			num = ((!midWall) ? (num + 1f) : (num + 0.5f));
		}
		cs.placeObj(0, num);
	}

	private IEnumerator ClickWait()
	{
		clickWait = true;
		yield return new WaitForSeconds(0.1f);
		clickWait = false;
	}

	public bool FullOverlapCheck(BuilderPart bp)
	{
		if ((bool)bp.deploy)
		{
			return true;
		}
		Vector3 vector = bp.origin ? bp.origin.position : bp._transform.position;
		Collider[] array = Physics.OverlapSphere(vector, 0.25f, 14, QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			BuilderPart component = array[i].transform.root.GetComponent<BuilderPart>();
			if (!(component == bp))
			{
				Vector3 b = component.origin ? component.origin.position : component._transform.position;
				MonoBehaviour.print((vector - b).sqrMagnitude);
				if ((vector - b).sqrMagnitude < 0.01f)
				{
					return false;
				}
			}
		}
		return true;
	}

	private bool wallSocketCheck()
	{
		if (placeOptions.ignoreRules)
		{
			return false;
		}
		block block = cs.BPinfo.block;
		int num = tools.blockNeighborCheck(radius: block.bounds.w, pos: block.getBoundsCenter());
		for (int i = 0; i < num; i++)
		{
			block component = tools.hit[i].transform.root.GetComponent<block>();
			if (!(component == this) && !component.placing && (component.wall || component.roof) && Vector3.Distance(component.transform.position, cs.BPinfo._transform.position) <= 0.01f)
			{
				return true;
			}
		}
		return false;
	}

	private bool floorProximityCheck()
	{
		if (placeOptions.ignoreRules)
		{
			return false;
		}
		block block = cs.BPinfo.block;
		float w = block.bounds.w;
		int num = tools.blockNeighborCheck(cs.BPinfo.block.getBoundsCenter(), w);
		for (int i = 0; i < num; i++)
		{
			block component = tools.hit[i].transform.root.GetComponent<block>();
			if (component == this || component.placing || !component.roof)
			{
				continue;
			}
			for (int j = 0; j < block.sockets.Length; j++)
			{
				socket socket = block.sockets[j];
				Vector3 position = socket.transform.position;
				for (int k = 0; k < component.sockets.Length; k++)
				{
					socket socket2 = component.sockets[k];
					if (socket.distanceCheck(socket2, position) && (socket2.name == "neighbor 1" || socket2.name == "neighbor 2"))
					{
						return true;
					}
				}
			}
		}
		return false;
	}
}
