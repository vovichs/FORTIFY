using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BuilderPart : MonoBehaviour
{
	public static BuilderSystem cs;

	public static BuilderPart over;

	private static Material matref;

	public partInfo info;

	public Transform _transform;

	public int id;

	public int instId;

	public float level = 1f;

	public Renderer[] rend;

	public Collider col;

	public Transform center;

	[HideInInspector]
	public bool mouseEnter;

	[HideInInspector]
	public static bool placedOn;

	[HideInInspector]
	public bool selected;

	[HideInInspector]
	public bool hidden;

	public float stability;

	[HideInInspector]
	public float health;

	[HideInInspector]
	public int edgeDist = 1;

	[HideInInspector]
	public int stage = 1;

	public int strucId;

	[HideInInspector]
	public bool check;

	public bool tri;

	public bool found;

	public bool floor;

	public bool wall;

	public int placedRotate;

	public GameObject sObj;

	public SupportList sList;

	public block block;

	public Device device;

	public door door;

	public Deploy deploy;

	public symmetryGroup sg;

	public Transform origin;

	public overlapCheck oc;

	private Coroutine doubleClickWait;

	public int tier
	{
		get
		{
			if ((bool)block)
			{
				return block.tier;
			}
			return 5;
		}
		set
		{
			if ((bool)block)
			{
				block.tier = value;
			}
		}
	}

	private void OnMouseEnter()
	{
		if (!BuilderUI.inst.mouseOverUI && !RayPlaceGround.heightAdjust)
		{
			over = this;
			enterCheck();
			if ((bool)deploy && (bool)deploy.mouseOverObj && !BuilderSystem.editMode && !wiring.inst.on)
			{
				deploy.mouseOverObj.SetActive(value: true);
			}
		}
	}

	public void enterCheck()
	{
		if ((!wiring.inst.on || (bool)device) && !cupboardRange.inst.enabled && !cs.inProgress && !BuilderSystem.disableInput)
		{
			mouseEnter = true;
			selectPart();
			UpdateInfo();
		}
	}

	public void UpdateInfo()
	{
		TextMeshProUGUI text = cursorOver.inst.text;
		if (stability == -2f)
		{
			text.text = "";
		}
		else if (Stability.inst.raidMode)
		{
			string text2 = Mathf.Round(health).ToString();
			if ((bool)block)
			{
				string text3 = Mathf.Round(stability * 100f).ToString();
				text.text = text2 + "/" + cs.tierHealth[block.tier].ToString() + " HP\n" + text3;
			}
			else
			{
				text.text = text2 + "/" + info.HP.ToString() + " HP\n";
			}
			cursorOver.inst.setActive(0, on: true);
		}
		else if ((bool)block)
		{
			if (!BuilderSystem.placeMode && stability < 1f)
			{
				text.text = Mathf.Round(stability * 100f).ToString();
				cursorOver.inst.setActive(0, on: true);
			}
		}
		else if ((bool)deploy.lockEnt && deploy.lockEnt.code.Length > 1)
		{
			locks.inst.codePanelInput.text = deploy.lockEnt.code;
		}
	}

	private void OnMouseOver()
	{
		if (BuilderSystem.disableInput || RayPlaceGround.heightAdjust)
		{
			return;
		}
		if (BuilderUI.inst.mouseOverUI)
		{
			if (mouseEnter)
			{
				mouseExit();
			}
			return;
		}
		if (!mouseEnter)
		{
			enterCheck();
			return;
		}
		if (cs.swapCheck)
		{
			cursorOver.inst.setActive(1, on: false);
			placeOnCheck();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Delete) || UnityEngine.Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Del"))
		{
			cursorOver.inst.allOff();
			deletePart();
			return;
		}
		bool flag = false;
		if ((bool)block)
		{
			if (BuilderSystem.editMode)
			{
				if (block.wall)
				{
					if (!selected || !BuilderSystem.placeMode)
					{
						flag = true;
					}
				}
				else if (placedRotate > 0 && !block.roof)
				{
					flag = true;
				}
			}
		}
		else
		{
			if (placedRotate > 0)
			{
				flag = true;
			}
			if (deploy.snapPoints.Length != 0 && (cs.BPinfo.name == "door_controller" || cs.BPinfo.name == "storage_adaptor" || cs.BPinfo.name == "industrial_crafter"))
			{
				PlaceOn();
			}
		}
		if (!flag || (placedOn && !cs.BPinfo.block))
		{
			return;
		}
		float rotationInput = getRotationInput(placedRotate);
		if (rotationInput != 0f && !BuilderUI.inst.mouseOverUI)
		{
			if ((bool)sg && Symmetry.inst.isOn)
			{
				sg.groupRotate(rotationInput);
			}
			rotatePart(rotationInput);
		}
	}

	private void OnMouseExit()
	{
		if (!RayPlaceGround.heightAdjust && mouseEnter)
		{
			mouseExit();
		}
	}

	private void mouseExit()
	{
		mouseEnter = false;
		if ((bool)deploy)
		{
			if ((bool)deploy.mouseOverObj && !deploy.mouseOverOn)
			{
				deploy.mouseOverObj.SetActive(value: false);
			}
			if ((bool)deploy.lockEnt)
			{
				locks.inst.codePanelInput.text = "";
			}
			if (!selected)
			{
				selectMaterial(select: false);
			}
		}
		else if (!selected)
		{
			selectMaterial(select: false);
		}
		if (placedOn)
		{
			cs.BPinfo._transform.position = new Vector3(0f, -1000f, 0f);
		}
		placedOn = false;
		cursorOver.inst.allOff();
	}

	private void OnMouseDown()
	{
		if (!mouseEnter || BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		if (doubleClickWait == null)
		{
			doubleClickWait = StartCoroutine(DoubleClickWait());
		}
		else if ((bool)deploy && (bool)deploy.mouseOverObj)
		{
			deploy.mouseOverObj.SetActive(!deploy.mouseOverOn);
			deploy.mouseOverOn = deploy.mouseOverObj.activeSelf;
			doubleClickWait = null;
			return;
		}
		if (placePanelSwap.inst.pickTog.isOn)
		{
			placePanelSwap.inst.partPicker(this);
			return;
		}
		if (RustCopyPaste.getCenter)
		{
			RustCopyPaste.inst.SetCenter(base.transform.position);
			return;
		}
		dragSelection();
		if (BuilderSystem.editMode)
		{
			return;
		}
		if (placedOn)
		{
			float num = level;
			int mode = 1;
			if ((bool)cs.BPinfo.block)
			{
				if (block.wall)
				{
					if (block.halfHeight)
					{
						if (!cs.BPinfo.block.halfHeight)
						{
							num += 0.5f;
						}
					}
					else if (cs.BPinfo.block.halfHeight)
					{
						num -= 0.5f;
					}
				}
				else if (cs.BPinfo.block.halfHeight)
				{
					num = (block.found ? 0.5f : ((!floor) ? (num + 0.5f) : (num - 0.5f)));
				}
			}
			else if (cs.BPinfo.name == "elevator")
			{
				if (overlapCheck.overlap)
				{
					return;
				}
				if ((bool)deploy)
				{
					num += 1f;
				}
			}
			else if (cs.BPinfo.name == "storage_adaptor" || cs.BPinfo.name == "industrial_crafter" || cs.BPinfo.name == "door_controller")
			{
				mode = ((base.name == "vending_machine") ? 2 : 0);
			}
			placedOn = false;
			cs.placeObj(mode, num);
			cursorOver.inst.setActive(1, on: false);
		}
		if ((bool)deploy)
		{
			if ((bool)door && (wiring.inst.on || !BuilderSystem.placeMode || !BuilderSystem.inst.BPinfo.deviceIs(typeof(doorController))))
			{
				StartCoroutine(changeDoorMeshState(!door.open, audio: true, send: true));
			}
			if ((bool)deploy.ignitable && (bool)deploy.ignitable)
			{
				deploy.ignitable.ignite();
			}
		}
	}

	private void placeOnCheck()
	{
		Transform transform = cs.BPinfo._transform;
		BuilderPart bPinfo = cs.BPinfo;
		if ((bool)deploy)
		{
			if ((deploy.snapPoints.Length == 0 || (!(cs.BPinfo.name == "door_controller") && !(cs.BPinfo.name == "storage_adaptor") && !(cs.BPinfo.name == "industrial_crafter"))) && (bool)bPinfo.deploy && bPinfo.deploy.place == Deploy.Place.snap && (bool)_transform.Find(cs.BPinfo.name + "_snap"))
			{
				PlaceOn();
			}
			return;
		}
		if ((bool)bPinfo.deploy)
		{
			if (block.wall)
			{
				if (block.frame && bPinfo.deploy.frameWall)
				{
					PlaceOn();
				}
				else if (bPinfo.deploy.windowPart && base.name == "wall_window")
				{
					PlaceOn();
				}
				else if (base.name == "wall" && cs.BPinfo.name == "fireplace")
				{
					PlaceOn();
				}
				else if (block.doorway && (cs.BPinfo.name == "vending_machine" || ((bool)bPinfo.door && bPinfo.door.type == door.Type.door && !bPinfo.deploy.frameWall)))
				{
					PlaceOn();
				}
			}
			else if (cs.BPinfo.name == "elevator")
			{
				if ((bool)_transform.Find("snapPoint"))
				{
					PlaceOn();
				}
			}
			else if (bPinfo.deploy.frameFloor && block.frame && floor && tri == bPinfo.tri)
			{
				PlaceOn();
			}
			return;
		}
		if (block.wall)
		{
			if (bPinfo.block.wall)
			{
				PlaceOn();
				return;
			}
			if (bPinfo.block.found && block.GetSocketNamed("wall male").connections.Count == 0)
			{
				PlaceOn();
				return;
			}
		}
		else if (bPinfo.block.frame && bPinfo.floor)
		{
			if (floor && tri == bPinfo.tri)
			{
				PlaceOn();
				return;
			}
		}
		else if (block.spiral)
		{
			if (bPinfo.block.spiral && tri == bPinfo.tri)
			{
				PlaceOn();
				return;
			}
		}
		else if (block.frame)
		{
			if (block.wall && bPinfo.block.wall)
			{
				PlaceOn();
				return;
			}
			if (bPinfo.floor && bPinfo.tri == tri)
			{
				PlaceOn();
				return;
			}
		}
		else
		{
			if (bPinfo.block.spiral && block.spiral)
			{
				PlaceOn();
				return;
			}
			if (bPinfo.block.stairs && (block.floor || block.found) && tri == bPinfo.tri)
			{
				PlaceOn();
				return;
			}
		}
		if (placeOptions.ignoreRules && block.found && bPinfo.found)
		{
			if (tri == bPinfo.tri)
			{
				transform.rotation = _transform.rotation;
				transform.position = _transform.position + new Vector3(0f, 1f, 0f);
				placedOn = true;
			}
		}
		else if (bPinfo.block.pillar && !tri)
		{
			if (block.found)
			{
				transform.position = _transform.position + new Vector3(0f, 0.5f, 0f);
				transform.rotation = _transform.rotation * Quaternion.Euler(0f, -90f, 0f);
			}
			else if (floor && !block.frame)
			{
				transform.position = _transform.position;
				transform.rotation = _transform.rotation * Quaternion.Euler(0f, -90f, 0f);
			}
			else
			{
				if (!block.pillar)
				{
					return;
				}
				transform.position = _transform.position + new Vector3(0f, 1f, 0f);
				transform.rotation = _transform.rotation;
				placedOn = true;
			}
			placedOn = true;
			rend[0].sharedMaterial = cs.tierMats[block.tier];
		}
		else if (block.pillar && bPinfo.floor && !bPinfo.tri)
		{
			transform.position = _transform.position + new Vector3(0f, 1f, 0f);
			transform.rotation = _transform.rotation * Quaternion.Euler(0f, 90f, 0f);
			placedOn = true;
		}
	}

	private void PlaceOn()
	{
		if (BuilderSystem.editMode)
		{
			return;
		}
		Transform transform = cs.BPinfo._transform;
		if ((bool)cs.BPinfo.deploy)
		{
			if (cs.BPinfo.deploy.place == Deploy.Place.snap)
			{
				if (cs.BPinfo.name == "storage_adaptor" || cs.BPinfo.name == "industrial_crafter" || cs.BPinfo.name == "door_controller")
				{
					if (deploy.snapPoints[0].name != cs.BPinfo.name + "_snap")
					{
						return;
					}
					if (deploy.snapPoints.Length == 1)
					{
						transform.SetPositionAndRotation(deploy.snapPoints[0].position, deploy.snapPoints[0].rotation);
					}
					else
					{
						Ray ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
						if (col.Raycast(ray, out RaycastHit hitInfo, 150f))
						{
							Transform closestSnap = deploy.getClosestSnap(hitInfo.point);
							transform.SetPositionAndRotation(closestSnap.position, closestSnap.rotation);
						}
					}
					if (base.name == "vending_machine")
					{
						overlapCheck.overlap = false;
						cs.redblock = false;
					}
					placedOn = true;
				}
				else
				{
					Transform transform2 = _transform.Find(cs.BPinfo.name + "_snap");
					if ((bool)transform2)
					{
						transform.SetPositionAndRotation(transform2.position, transform2.rotation);
						cs.redblock = (!placeOptions.ignoreRules && deviceIs(typeof(elevator)) && !(device as elevator).stackLimitCheck());
						placedOn = true;
					}
				}
				return;
			}
			transform.SetPositionAndRotation(_transform.position, _transform.rotation);
			if (cs.BPinfo.name == "vending_machine")
			{
				transform.RotateAround(_transform.position, Vector3.up, -90f);
				cs.redblock = cs.BPinfo.deploy.cornerCheck();
			}
			else if (cs.BPinfo.name == "fireplace")
			{
				Vector3 to = BuilderSystem.inst._transform.position - transform.position;
				if (Vector3.Angle(transform.right, to) <= 89f)
				{
					transform.Rotate(0f, 90f, 0f);
				}
				else
				{
					transform.Rotate(0f, -90f, 0f);
				}
				transform.Translate(Vector3.forward * 0.275f);
				cs.redblock = cs.BPinfo.deploy.cornerCheck();
			}
			else
			{
				if (cs.BPinfo.deploy.windowPart)
				{
					transform.Translate(Vector3.up * 0.333f);
				}
				cs.redblock = cs.BPinfo.deploy.cornerCheck();
				if (!placeOptions.allowOverlap)
				{
					StartCoroutine(waitForReplaceIcon());
				}
			}
		}
		else
		{
			block block = cs.BPinfo.block;
			if (block.stairs)
			{
				if (block.spiral && this.block.spiral)
				{
					Transform transform3 = _transform.Find("snapPoint");
					transform.SetPositionAndRotation(transform3.position, transform3.rotation);
				}
				else
				{
					Ray ray2 = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
					if (col.Raycast(ray2, out RaycastHit hitInfo2, 150f))
					{
						Transform closestEdgeCol = this.block.getClosestEdgeCol(hitInfo2.point);
						transform.SetPositionAndRotation(closestEdgeCol.position, closestEdgeCol.rotation);
					}
					if (block.steps)
					{
						transform.Translate(Vector3.right * 1f, Space.Self);
					}
					if (block.ramp)
					{
						transform.position += new Vector3(0f, 0.25f, 0f);
						transform.Translate(Vector3.right * 1f, Space.Self);
					}
				}
			}
			else if (block.found)
			{
				transform.position = _transform.position - new Vector3(0f, 0.5f, 0f);
				transform.rotation = _transform.rotation;
				Vector3 to2 = BuilderSystem.inst._transform.position - transform.position;
				if (Vector3.Angle(transform.right, to2) <= 89f)
				{
					transform.Rotate(0f, 180f, 0f);
				}
				if (!Proximity.inst.Check(cs.BPinfo) || block.heightCheck() || block.checkForPipes())
				{
					cs.redblock = true;
				}
			}
			else
			{
				transform.SetPositionAndRotation(_transform.position, _transform.rotation);
			}
			if (!placeOptions.allowOverlap)
			{
				StartCoroutine(waitForReplaceIcon());
			}
		}
		placedOn = true;
	}

	public void dragSelection()
	{
		if (BuilderSystem.editMode && !Stability.inst.raidMode && !RectSelection.mouse3rectSelect)
		{
			if (!selected)
			{
				cs.objList.Add(base.gameObject);
				selected = true;
				selectMaterial(select: true);
			}
			else
			{
				cs.objList.Remove(base.gameObject);
				selected = false;
				selectMaterial(select: false);
			}
		}
	}

	public void selectPart()
	{
		if (!BuilderSystem.editMode)
		{
			selectMaterial(select: true);
			if (!Stability.inst.raidMode && !wiring.inst.on)
			{
				placeOnCheck();
			}
		}
		else
		{
			if (RectSelection.mouse3rectSelect || cs.inProgress || AlignStructure.inst.on || RayPlaceGround.heightAdjust)
			{
				return;
			}
			selectMaterial(select: true);
			if (Input.GetMouseButton(0) && !BuilderUI.inst.mouseOverUI)
			{
				if (selected)
				{
					selected = false;
					cs.objList.Remove(base.gameObject);
				}
				else
				{
					selected = true;
					cs.objList.Add(base.gameObject);
				}
			}
		}
	}

	public void deletePart()
	{
		if (Stability.inst.raidMode)
		{
			if (!hidden && stability != -2f)
			{
				AudioPlayer.inst.playAtPoint(_transform.position, 1);
				cursorOver.inst.setActive(0, on: false);
				if ((bool)block)
				{
					block.destroyed = true;
					block.UpdateNeighborLinks(removed: true);
				}
				raidDestroy(Vector3.zero);
			}
		}
		else if (BuilderSystem.editMode)
		{
			if (!selected)
			{
				selected = true;
				cs.objList.Add(base.gameObject);
			}
		}
		else
		{
			cs.BPinfo._transform.position = new Vector3(0f, -1000f, 0f);
			cs.BPinfo.oc.reset();
			MGMT.inst.undoListInsert();
			MGMT.inst.addDeletedPart(this, undo: true);
			cs.destroyPart(this, audio: true, receive: false);
		}
	}

	public void SetMaterial()
	{
		if (selected)
		{
			return;
		}
		if ((bool)deploy)
		{
			if (Staging.inst.stageMode && (!device || !(device is electricLight)))
			{
				matref = Staging.inst.stageMats[stage];
			}
			else
			{
				matref = deploy.mat;
			}
			Renderer[] array = rend;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].sharedMaterial = matref;
			}
			return;
		}
		if (Staging.inst.stageMode)
		{
			matref = Staging.inst.stageMats[stage];
		}
		else
		{
			if (Stability.inst.colorView)
			{
				SetStabilityMat();
				return;
			}
			matref = BuilderSystem.inst.tierMats[block.tier];
		}
		rend[0].sharedMaterial = matref;
	}

	public void SetStabilityMat()
	{
		if (!selected)
		{
			int num = 0;
			if (!((double)stability < 0.05))
			{
				num = (((double)stability < 0.1) ? 1 : (((double)stability < 0.2) ? 2 : (((double)stability < 0.4) ? 3 : (((double)stability < 0.6) ? 4 : ((!((double)stability < 0.8)) ? 6 : 5)))));
			}
			if (num > 1 && !Stability.inst.fullColorView)
			{
				matref = BuilderSystem.inst.tierMats[block.tier];
			}
			else
			{
				matref = BuilderSystem.inst.stabMats[num];
			}
			if (rend[0].sharedMaterial != matref)
			{
				rend[0].sharedMaterial = matref;
			}
		}
	}

	public void selectMaterial(bool select)
	{
		if (select)
		{
			if ((bool)deploy)
			{
				if ((bool)deploy.selectMat)
				{
					Renderer[] array = rend;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].sharedMaterial = deploy.selectMat;
					}
				}
				else
				{
					Renderer[] array = rend;
					for (int i = 0; i < array.Length; i++)
					{
						array[i].sharedMaterial = cs.highLight;
					}
				}
			}
			else
			{
				rend[0].sharedMaterial = cs.highLight;
			}
		}
		else
		{
			SetMaterial();
		}
	}

	public bool deviceIs(Type type)
	{
		if (!device || device.GetType() != type)
		{
			return false;
		}
		return true;
	}

	public string GetCodeString()
	{
		if (!deploy || !deploy.lockEnt)
		{
			return "";
		}
		return deploy.lockEnt.code;
	}

	public bool GetDoorState()
	{
		if (!door)
		{
			return false;
		}
		return door.open;
	}

	public void setRaycastLayer()
	{
		if ((bool)device && device is doorController)
		{
			base.gameObject.layer = 24;
		}
		else
		{
			base.gameObject.layer = 0;
		}
	}

	public IEnumerator changeDoorMeshState(bool state, bool audio, bool send)
	{
		if (door == null)
		{
			yield break;
		}
		if (state && !door.open)
		{
			door.open = true;
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDoorChange(this, state);
			}
			door.mf.sharedMesh = door.openMesh;
			if (audio)
			{
				StartCoroutine(door.playSound(open: true));
			}
			if (send)
			{
				yield return new WaitForSeconds(0.5f);
			}
			door.openCol.enabled = true;
			door.closeCol.enabled = false;
			col = door.openCol;
			if (door.type == door.Type.hatch)
			{
				door.ladderArea.SetActive(value: true);
			}
		}
		if (!state && door.open)
		{
			door.open = false;
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDoorChange(this, state);
			}
			if (audio)
			{
				StartCoroutine(door.playSound(open: false));
			}
			door.mf.sharedMesh = door.closeMesh;
			door.openCol.enabled = false;
			door.closeCol.enabled = true;
			col = door.closeCol;
			if (door.type == door.Type.hatch)
			{
				door.ladderArea.SetActive(value: false);
			}
		}
	}

	private IEnumerator DoubleClickWait()
	{
		yield return new WaitForSeconds(0.6f);
		doubleClickWait = null;
	}

	private IEnumerator waitForReplaceIcon()
	{
		yield return new WaitForFixedUpdate();
		if (overlapCheck.overlapCount > 0 && placedOn)
		{
			cursorOver.inst.setActive(1, on: true);
		}
	}

	public void applyDamage(Damage.DamageTypeList damageList, Vector3 hitPos)
	{
		if (!Stability.inst.raidMode || stability == -2f)
		{
			return;
		}
		if ((bool)deploy)
		{
			float num = 1f;
			for (int i = 0; i < info.protection.Length; i++)
			{
				damageList.Scale((Damage.DamageType)i, 1f - Mathf.Clamp(info.protection[i] * num, -1f, 1f));
			}
		}
		else
		{
			for (int j = 0; j < Damage.damageTypeCount; j++)
			{
				damageList.Scale((Damage.DamageType)j, 1f - Mathf.Clamp(Damage.tierProtection[block.tier, j] * 1f, -1f, 1f));
			}
		}
		health -= damageList.Total();
		if (health <= 0f)
		{
			health = 0f;
			raidDestroy(hitPos);
		}
		if (mouseEnter)
		{
			UpdateInfo();
		}
	}

	public void rotatePart(float rot)
	{
		if (center == null)
		{
			_transform.Rotate(Vector3.up, rot);
		}
		else
		{
			_transform.RotateAround(center.position, Vector3.up, rot);
		}
		if ((bool)block)
		{
			block.UpdateNeighborLinks(removed: false);
		}
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = this;
			Sender.sendFloat = rot;
			Sender.send(7);
		}
	}

	public void raidDestroy(Vector3 expPos)
	{
		if (stability == -2f)
		{
			return;
		}
		stability = -2f;
		if ((bool)block && Stability.inst.colorView)
		{
			rend[0].sharedMaterial = cs.stabMats[0];
		}
		GameObject gameObject = base.gameObject;
		if (gameObject.layer == 15 || !gameObject.activeSelf)
		{
			return;
		}
		gameObject.layer = 15;
		gameObject.tag = "destroy";
		if (col is MeshCollider)
		{
			(col as MeshCollider).convex = true;
		}
		Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
		if (rigidbody != null)
		{
			rigidbody.useGravity = true;
			if ((bool)block)
			{
				block.destroyed = true;
				block.UpdateNeighborLinks(removed: true);
				if (col is BoxCollider)
				{
					col.enabled = true;
				}
				StartCoroutine(raidDestroyWait(quick: false));
				rigidbody.mass = 15f;
			}
			else
			{
				StartCoroutine(raidDestroyWait(quick: true));
				if (deploy.ground == Deploy.Ground.noAlign || deploy.ground == Deploy.Ground.heightAdjust)
				{
					rigidbody.mass = 30f;
				}
				else
				{
					rigidbody.mass = 10f;
				}
			}
			if (expPos != Vector3.zero)
			{
				rigidbody.AddExplosionForce(50f, expPos, 3.5f, 0.5f, ForceMode.Impulse);
			}
			else
			{
				rigidbody.AddExplosionForce(50f, _transform.position, 1f, 0.5f, ForceMode.Impulse);
			}
		}
		if (CrashSound.count < 40)
		{
			gameObject.AddComponent<CrashSound>();
		}
		if (!(sList != null))
		{
			return;
		}
		for (int i = 0; i < sList.sParts.Count; i++)
		{
			BuilderPart builderPart = sList.sParts[i];
			if (builderPart != null && builderPart.gameObject.tag != "destroy" && builderPart.gameObject.activeSelf)
			{
				builderPart.raidDestroy(expPos);
			}
		}
	}

	private IEnumerator raidDestroyWait(bool quick)
	{
		if (quick)
		{
			yield return new WaitForSeconds(2f);
		}
		else
		{
			yield return new WaitForSeconds(UnityEngine.Random.Range(3f, 4f));
		}
		if (Stability.inst.raidMode)
		{
			cs.bpList.Remove(this);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void removeSymmetry()
	{
		if ((bool)sg)
		{
			UnityEngine.Object.Destroy(sg);
			sg = null;
		}
	}

	public void moved()
	{
		if (BuilderSystem.multiplayer)
		{
			Sender.sendObj = this;
			Sender.send(10);
		}
		if (isDevice(typeof(autoTurret)))
		{
			(device as autoTurret).UpdateInterferenceOnOthers(remove: true);
		}
		if (found && (bool)sObj)
		{
			sObj.GetComponent<rangeCircle>().movedPos(origin.position);
		}
	}

	public bool compareDirection(Transform otherTransform)
	{
		float num = Vector3.Angle(_transform.forward, otherTransform.forward);
		MonoBehaviour.print(num);
		return Mathf.Abs(num) < 0.02f;
	}

	public bool isDevice(Type type)
	{
		if ((bool)device)
		{
			return device.GetType() == type;
		}
		return false;
	}

	private float getRotationInput(float amount)
	{
		bool buttonDown = Input.GetButtonDown("rot-");
		bool buttonDown2 = Input.GetButtonDown("rot+");
		float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		if ((buttonDown | buttonDown2) || axis != 0f)
		{
			if (buttonDown || axis > 0f)
			{
				amount = 0f - amount;
			}
		}
		else
		{
			amount = 0f;
		}
		return amount;
	}
}
