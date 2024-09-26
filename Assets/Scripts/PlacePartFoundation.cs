using System.Collections;
using UnityEngine;

public class PlacePartFoundation : MonoBehaviour
{
	public static BuilderSystem cs;

	public bool tri;

	private bool place;

	private bool steps;

	private Collider col;

	private Transform t;

	public static bool snapMode;

	public socket[] sockets;

	private void OnMouseEnter()
	{
		if (col == null)
		{
			col = GetComponent<Collider>();
			t = base.transform;
		}
		if (!cs.BPinfo.block || PlacePart.contPlace || BuilderSystem.disableInput || (!cs.BPinfo.found && !cs.BPinfo.block.steps && !cs.BPinfo.block.ramp) || BuilderSystem.editMode || BuilderUI.inst.mouseOverUI || RayPlaceGround.heightAdjust)
		{
			return;
		}
		partCheck();
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = cs.BPinfo;
			Sender.send(12);
		}
		if (Input.GetMouseButton(0) && placeOptions.continuous && place && cs.BPinfo.block.continuous && !RayPlaceGround.heightAdjust && !placeOptions.allowOverlap)
		{
			if (!placeOptions.allowOverlap)
			{
				StartCoroutine(WaitToPlace(cs.BPinfo));
				return;
			}
			cs.placeObj(0, 0f);
			place = false;
		}
	}

	private IEnumerator WaitToPlace(BuilderPart bp)
	{
		cs.StartCoroutine(cs.continuousWait());
		yield return new WaitForFixedUpdate();
		if ((bool)bp && overlapCheck.overlapCount == 0)
		{
			cs.placeObj(0, 0f);
			place = false;
		}
	}

	private void OnMouseOver()
	{
		BuilderPart bPinfo = cs.BPinfo;
		if (cs.swapCheck && (bPinfo.found || bPinfo.block.steps || bPinfo.block.ramp))
		{
			PlacePart.contPlace = false;
			partCheck();
		}
		if ((!steps && !snapMode) || AlignStructure.inst.on)
		{
			return;
		}
		Ray ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		bool flag = false;
		if (col.Raycast(ray, out RaycastHit hitInfo, 150f))
		{
			flag = (t.InverseTransformPoint(hitInfo.point).x > -0.1f);
		}
		Transform transform = bPinfo._transform;
		transform.SetPositionAndRotation(t.position, t.rotation);
		if (steps)
		{
			if (bPinfo.block.ramp)
			{
				if (flag && !tri)
				{
					transform.Translate(new Vector3(1f, 0.5f, 0f));
				}
				else
				{
					transform.Translate(Vector3.up * 0.25f);
				}
			}
			else if (flag && !tri)
			{
				transform.Translate(new Vector3(1f, 0.5f, 0f));
			}
			if (bPinfo.block.heightCheck() || bPinfo.block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else
		{
			if (flag)
			{
				transform.Translate(Vector3.up * 0.5f);
			}
			else
			{
				transform.Translate(Vector3.up * -0.5f);
			}
			if (!Proximity.inst.Check(bPinfo) || bPinfo.block.heightCheck() || bPinfo.block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		place = !cs.redblock;
	}

	private void partCheck()
	{
		cs.BPinfo._transform.SetPositionAndRotation(t.position, t.rotation);
		if (cs.BPinfo.block.steps || cs.BPinfo.block.ramp)
		{
			steps = true;
			if (cs.BPinfo.block.ramp)
			{
				cs.BPinfo._transform.Translate(Vector3.up * 0.25f);
			}
			if (cs.BPinfo.block.heightCheck() || cs.BPinfo.block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		else
		{
			steps = false;
			extendArrow.inst.alignWith(t.TransformPoint(new Vector3(0f, 0.5f, 0f)), -t.right);
			if (!Proximity.inst.Check(cs.BPinfo) || cs.BPinfo.block.heightCheck() || cs.BPinfo.block.checkForPipes())
			{
				cs.redblock = true;
			}
		}
		place = !cs.redblock;
	}

	private void OnMouseDown()
	{
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = null;
			Sender.send(12);
		}
		if (place && !BuilderUI.inst.mouseOverUI && !BuilderSystem.disableInput)
		{
			BuilderPart bPinfo = cs.BPinfo;
			cs.placeObj(0, 0f);
			place = false;
			if (!steps && extendArrow.inst.tog.isOn)
			{
				cs.Extend(bPinfo);
			}
		}
	}

	private void OnMouseExit()
	{
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = null;
			Sender.send(12);
		}
		if (!PlacePart.contPlace)
		{
			place = false;
			steps = false;
			cs.redblock = false;
			cs.hidePlacePart();
		}
		else
		{
			extendArrow.inst.hideArrow();
		}
	}

	private IEnumerator waitReset()
	{
		yield return new WaitForSeconds(1.5f);
		if (!BuilderSystem.editMode)
		{
			col.gameObject.layer = 16;
		}
	}
}
