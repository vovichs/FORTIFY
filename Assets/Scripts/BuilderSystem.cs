using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BuilderSystem : MonoBehaviour
{
	public enum PlaceColMode
	{
		edge,
		off,
		found,
		frame,
		all
	}

	public static BuilderSystem inst;

	public Transform _transform;

	public LayerMask usedMask;

	public LayerMask groundMask;

	public static int tier;

	public float[] tierHealth;

	public Material[] tierMats;

	public Material[] stabMats;

	public Material checkRed;

	public Material checkBlue;

	public Material highLight;

	public static bool multiplayer = false;

	public static bool rockPlacement;

	public static bool placeMode = false;

	public static bool editMode = false;

	public static bool saveSelection = false;

	public static bool disableInput = false;

	[HideInInspector]
	public bool showRanges;

	public Toggle showRangesTog;

	[HideInInspector]
	public bool hideMode;

	[HideInInspector]
	public bool rectSelectMode;

	[HideInInspector]
	public bool redblock;

	[HideInInspector]
	public bool swapCheck;

	[HideInInspector]
	public bool inProgress;

	[HideInInspector]
	public bool blockRotate;

	[HideInInspector]
	public bool blueMat = true;

	[HideInInspector]
	[SerializeField]
	public BuilderPart BPinfo;

	public List<BuilderPart> bpList = new List<BuilderPart>();

	[HideInInspector]
	public static List<wire> wireList = new List<wire>();

	[HideInInspector]
	public List<GameObject> objList;

	[HideInInspector]
	public List<BuilderPart> objList2;

	[HideInInspector]
	public List<BuilderPart> waitList;

	[HideInInspector]
	public Scene currentScene;

	public GameObject floorLvl;

	public GameObject copyParent;

	public Dropdown sceneDropDn;

	private float rotDeploy;

	private float rotBlock;

	private float scrollAmount;

	private int scrollMulti = 10;

	[HideInInspector]
	public Vector3 v3 = Vector3.up;

	public bool forward;

	public PlaceColMode placeColMode;

	public bool ShowRanges
	{
		get
		{
			return showRanges;
		}
		set
		{
			showRanges = !showRanges;
			for (int i = 0; i < bpList.Count; i++)
			{
				BuilderPart builderPart = bpList[i];
				if (!builderPart.hidden && (bool)builderPart.deploy && (bool)builderPart.deploy.mouseOverObj)
				{
					builderPart.deploy.mouseOverObj.SetActive(showRanges);
				}
			}
		}
	}

	private void Awake()
	{
		inst = this;
		_transform = base.transform;
		placeColMode = PlaceColMode.found;
		RayPlace.cs = this;
		BuilderPart.cs = this;
		PlacePartFoundation.cs = this;
		PlacePartFoundation.snapMode = false;
		RayPlace.noPlace = false;
		PlacePart.cs = this;
		if (PlayerPrefs.HasKey("Scene"))
		{
			sceneDropDn.value = PlayerPrefs.GetInt("Scene");
			sceneDropDn.RefreshShownValue();
		}
		SceneLoader(sceneDropDn.value + 1, null);
		disableInput = false;
		placeMode = false;
		editMode = false;
		tier = 2;
		if (MGMT.inst != null)
		{
			StartCoroutine(MGMT.inst.destroySlowly());
		}
	}

	public void Start()
	{
		InstPart(35);
		if (dayNightCtrl.inst != null)
		{
			dayNightCtrl.inst.updateWaterBrightness();
		}
	}

	public void SceneLoader(int index, string named)
	{
		if (index > 0)
		{
			SceneManager.LoadScene(index, LoadSceneMode.Additive);
			currentScene = SceneManager.GetSceneByBuildIndex(index);
		}
		else
		{
			SceneManager.LoadScene(named, LoadSceneMode.Additive);
			currentScene = SceneManager.GetSceneByName(named);
		}
		MonoBehaviour.print(currentScene.name + " loaded");
	}

	public void ClearScene()
	{
		MGMT.inst.clearLists();
		if (multiplayer)
		{
			Multiplayer.syncObjList.Clear();
		}
		if (placeMode)
		{
			BuilderUI.inst.modeToggle.isOn = true;
		}
		if (editMode)
		{
			BuilderUI.inst.editToggle.isOn = false;
		}
		if (wiring.inst.wireTog.isOn)
		{
			wiring.inst.wireTog.isOn = false;
		}
		if (currentScene.IsValid())
		{
			SceneManager.UnloadScene(currentScene);
		}
		if (CameraCtrl.inst.orthoToggle.isOn)
		{
			CameraCtrl.inst.orthoToggle.isOn = false;
		}
		if (CameraCtrl.inst.hideTerrainTog.isOn)
		{
			CameraCtrl.inst.hideTerrainTog.SetIsOnWithoutNotify(value: false);
		}
		ResourceCount.inst.reset();
		infoSpotPanel.inst.infoPanel.SetActive(value: false);
		infoSpotPanel.inst.destroySpots();
		foreach (wire wire in wireList)
		{
			UnityEngine.Object.Destroy(wire.gameObject);
		}
		cupboardRange.inst.removeRanges();
		objList.Clear();
		wiring.inst.newPID = 0;
		MGMT.inst.getDestroyObjs(clearScene: true);
		cupboardRange.inst.rangeList.Clear();
		StartCoroutine(MGMT.inst.destroySlowly());
		if (RustCopyPaste.center != null)
		{
			UnityEngine.Object.Destroy(RustCopyPaste.center);
		}
	}

	private void Update()
	{
		if (Input.anyKey && !disableInput && editMode)
		{
			if (Input.GetButtonDown("rot-") || Input.GetButtonDown("rot+"))
			{
				foreach (GameObject obj in objList)
				{
					BuilderPart component = obj.GetComponent<BuilderPart>();
					if (component.wall)
					{
						if ((bool)component.sg && Symmetry.inst.isOn)
						{
							component.sg.groupRotate(180f);
						}
						component.rotatePart(180f);
					}
				}
			}
			if ((UnityEngine.Input.GetKeyDown(KeyCode.Delete) || UnityEngine.Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Del")) && !MGMT.destroyInProgress && objList.Count > 0)
			{
				Symmetry.inst.selectionFilter();
				destroySelection();
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.C) && !Input.GetKey(KeyCode.LeftControl))
			{
				SelectClear();
			}
		}
		placingRotation();
	}

	private void FixedUpdate()
	{
		if (!disableInput && !editMode)
		{
			if (overlapCheck.overlap || redblock)
			{
				if (blueMat)
				{
					BPinfo.rend[0].sharedMaterial = checkRed;
					blueMat = false;
				}
			}
			else if (!blueMat)
			{
				BPinfo.rend[0].sharedMaterial = checkBlue;
				blueMat = true;
			}
		}
		if (scrollAmount != 0f)
		{
			if (scrollMulti == 10)
			{
				StartCoroutine(scrollWheelDecel());
			}
			if ((float)scrollMulti < 50f)
			{
				scrollMulti += 4;
			}
		}
	}

	private void placingRotation()
	{
		if (BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		bool buttonDown = Input.GetButtonDown("rot-");
		bool buttonDown2 = Input.GetButtonDown("rot+");
		scrollAmount = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		float num = 0f;
		Transform transform = BPinfo._transform;
		if (placeMode || inProgress || BPinfo.found)
		{
			if (BPinfo.found && !RayPlaceGround.overThis)
			{
				return;
			}
			if (BuilderPart.placedOn && (bool)BPinfo.deploy && BPinfo.deploy.placedOnNoRot)
			{
				if (BPinfo.placedRotate > 0)
				{
					num = BPinfo.placedRotate;
					if (buttonDown || scrollAmount > 0f)
					{
						transform.Rotate(v3, 0f - num);
					}
					else if (buttonDown2 || scrollAmount < 0f)
					{
						transform.Rotate(v3, num);
					}
				}
				return;
			}
			num = (float)scrollMulti * 0.1f;
			if (!Input.GetKey(KeyCode.LeftAlt))
			{
				num *= 5f;
			}
			if (scrollAmount != 0f)
			{
				if (scrollAmount > 0f)
				{
					transform.Rotate(v3, 0f - num);
				}
				else
				{
					transform.Rotate(v3, num);
				}
			}
			else if (buttonDown)
			{
				transform.Rotate(v3, 0f - num);
			}
			else if (buttonDown2)
			{
				transform.Rotate(v3, num);
			}
			return;
		}
		if (buttonDown || scrollAmount > 0f)
		{
			num = -BPinfo.placedRotate;
		}
		else
		{
			if (!buttonDown2 && !(scrollAmount < 0f))
			{
				return;
			}
			num = BPinfo.placedRotate;
		}
		if (!editMode && !BPinfo.wall)
		{
			if ((bool)BPinfo.center)
			{
				transform.RotateAround(BPinfo.center.position, Vector3.up, num);
			}
			else
			{
				transform.Rotate(Vector3.up, num);
			}
		}
	}

	private IEnumerator scrollWheelDecel()
	{
		bool start = true;
		while ((scrollMulti > 10) | start)
		{
			start = false;
			yield return new WaitForSeconds(0.1f);
			scrollMulti -= 4;
		}
	}

	public void ChangePart(string partName)
	{
		StartCoroutine(swapCheckState());
		if (editMode)
		{
			BuilderUI.inst.editToggle.GetComponent<Toggle>().isOn = false;
		}
		if (wiring.inst.on)
		{
			wiring.inst.wireTog.isOn = false;
		}
		if (Symmetry.inst.setCenterTog.isOn)
		{
			Symmetry.inst.setCenterTog.isOn = false;
		}
		if (copyMove.inst.inProcess)
		{
			copyMove.inst.Cancel();
		}
		if ((bool)BPinfo.block)
		{
			rotBlock = BPinfo._transform.rotation.eulerAngles.y;
		}
		UnityEngine.Object.Destroy(BPinfo.gameObject);
		redblock = false;
		InstPart(findPrefabIndex(partName));
		SetPlaceColMode(off: false);
		RayPlaceGround.ChangedPart();
		rpOverCheck();
		ResourceCount.inst.GetResourceInfo(BPinfo);
		placeOptions.allowOverlapCheck(BPinfo);
		if ((bool)BPinfo.deploy)
		{
			if (BPinfo.deploy.place == Deploy.Place.forward)
			{
				v3 = Vector3.forward;
			}
			else if ((bool)BPinfo.deploy.offset)
			{
				v3 = BPinfo.deploy.offset.up;
			}
			else if (BPinfo.deploy.place == Deploy.Place.backward)
			{
				v3 = -Vector3.forward;
			}
			else
			{
				v3 = Vector3.up;
			}
		}
		else
		{
			if (multiplayer)
			{
				Sender.sendInt = BPinfo.id;
				Sender.send(11);
			}
			v3 = Vector3.up;
		}
	}

	public IEnumerator swapCheckState()
	{
		swapCheck = true;
		yield return null;
		swapCheck = false;
	}

	public void InstPart(int index)
	{
		GameObject gameObject = MGMT.inst.prefabList[index];
		redblock = false;
		GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, new Vector3(0f, -1000f, 0f), Quaternion.identity);
		gameObject2.name = gameObject.name;
		BPinfo = gameObject2.GetComponent<BuilderPart>();
		BPinfo.rend[0].sharedMaterial = checkRed;
		blueMat = false;
		BPinfo.oc.deployVolume();
		if ((bool)BPinfo.deploy)
		{
			if ((bool)BPinfo.deploy.mouseOverObj)
			{
				BPinfo.deploy.mouseOverObj.SetActive(value: true);
			}
			LODGroup component = gameObject2.GetComponent<LODGroup>();
			if (component != null)
			{
				component.ForceLOD(0);
			}
			BPinfo._transform.rotation = Quaternion.AngleAxis(rotDeploy, Vector3.up);
			if ((bool)BPinfo.device)
			{
				BPinfo.device.showIOs(placing: true);
			}
		}
		else
		{
			BPinfo.block.placing = true;
			BPinfo.block.tier = tier;
			if (BPinfo.block.found || BPinfo.block.ramp)
			{
				BPinfo.block.conditional.setMesh();
			}
			BPinfo._transform.rotation = Quaternion.AngleAxis(rotBlock, Vector3.up);
		}
	}

	public void placeObj(int mode, float lvl)
	{
		BuilderPart bPinfo = BPinfo;
		if (redblock || bPinfo._transform.position.y < -100f)
		{
			return;
		}
		if (!placeOptions.allowOverlap)
		{
			if (mode == 1)
			{
				MGMT.inst.undoListInsert();
				bPinfo.oc.DestroyOverlapObj();
			}
			if (mode != 2 && overlapCheck.overlap)
			{
				return;
			}
		}
		if (mode != 1)
		{
			MGMT.inst.undoListInsert();
		}
		if (!bPinfo.found)
		{
			bPinfo.level = lvl;
		}
		if ((bool)bPinfo.block)
		{
			bPinfo.block.tier = tier;
			rotBlock = bPinfo._transform.rotation.eulerAngles.y;
		}
		else
		{
			if ((bool)bPinfo.deploy.mouseOverObj)
			{
				bPinfo.deploy.mouseOverObj.SetActive(value: false);
			}
			rotDeploy = bPinfo._transform.rotation.eulerAngles.y;
			if ((bool)bPinfo.device)
			{
				bPinfo.device.hideIOs(listRemove: false);
			}
		}
		bPinfo.stage = Staging.stage;
		PlacedSetup(bPinfo, send: true, notLoaded: true, sound: true, useCodeTog: true);
		MGMT.inst.undoListAddedPart(bPinfo);
		if ((bool)bPinfo.block)
		{
			string name = BPinfo.name;
			if (bPinfo.wall)
			{
				if (BuilderUI.inst.includeToggles[0].isOn && name == "wall_window")
				{
					includePart(bPinfo, 0);
				}
				if (BuilderUI.inst.includeToggles[1].isOn && name == "wall_doorway")
				{
					includePart(bPinfo, 1);
				}
				if (BuilderUI.inst.includeToggles[2].isOn && name == "wall_frame")
				{
					includePart(bPinfo, 2);
				}
			}
			else if (bPinfo.block.frame)
			{
				if (BuilderUI.inst.includeToggles[3].isOn && name == "floor_frame")
				{
					includePart(bPinfo, 3);
				}
				if (BuilderUI.inst.includeToggles[4].isOn && name == "floor_frame_tri")
				{
					includePart(bPinfo, 4);
				}
			}
		}
		AudioPlayer.inst.playAtPoint(bPinfo._transform.position, 0);
		InstPart(bPinfo.id);
		if ((bool)bPinfo.block && extendArrow.inst.tog.isOn && !overlapCheck.overlap && !redblock && (bPinfo.block.found || bPinfo.block.pillar || bPinfo.block.floor || (bPinfo.block.wall && bPinfo.block.edgeCols.Length != 0)))
		{
			Extend(bPinfo);
		}
	}

	public void PlacedSetup(BuilderPart bp, bool send, bool notLoaded, bool sound, bool useCodeTog)
	{
		GameObject gameObject = bp.gameObject;
		bpList.Add(bp);
		bp.setRaycastLayer();
		if (multiplayer)
		{
			Multiplayer.syncObjList.Add(bp);
			if (send)
			{
				Sender.sendPart(bp, sendAll: true, sound);
			}
		}
		if (bp.instId == 0)
		{
			bp.instId = gameObject.GetInstanceID();
		}
		if ((bool)bp.oc)
		{
			bp.oc.placedVolume();
		}
		if (Symmetry.inst.isOn && !bp.sg)
		{
			AddSymmetry(bp, addSelection: true);
		}
		block block = bp.block;
		if ((bool)block)
		{
			block.placing = false;
			if (notLoaded)
			{
				block.GetNeighborLinks(updateNeighbors: true, clear: false);
				if ((bool)block.stabilityEntity)
				{
					bp.block.stabilityEntity.UpdateStability();
				}
				if ((bool)block.conditional)
				{
					block.conditional.RunCheck();
				}
			}
			else if (block.found || block.ramp)
			{
				block.conditional.setMesh();
			}
			if (!placeMode && !editMode)
			{
				block.getColliderLayer((int)placeColMode);
			}
		}
		else
		{
			if ((bool)bp.door)
			{
				StartCoroutine(bp.door.getDoorParent(bp));
				if (notLoaded)
				{
					bp.door.controlCheck();
				}
			}
			if (!bp.col)
			{
				bp.col = gameObject.GetComponent<Collider>();
			}
			if (notLoaded)
			{
				LODGroup component = gameObject.GetComponent<LODGroup>();
				if (component != null)
				{
					component.ForceLOD(-1);
				}
			}
			if ((bool)bp.device)
			{
				bp.device.owner = bp;
				MGMT.inst.devices.Add(bp.instId, bp.device);
				if (bp.device is elevator)
				{
					(bp.device as elevator).stackCheck();
				}
				else if (bp.device is powerWind)
				{
					(bp.device as powerWind).heightWindCheck();
				}
				else if (bp.device is doorController)
				{
					StartCoroutine((bp.device as doorController).connectToDoor(manual: false));
				}
				else if (bp.device is autoTurret)
				{
					if (notLoaded)
					{
						bp.device.setValue(ResourceCount.inst.GetWeaponId(), send: false);
					}
					(bp.device as autoTurret).Initialize(add: true);
				}
			}
			if ((bool)bp.deploy.lockEnt && useCodeTog && locks.inst.includeCodeLocks)
			{
				locks.inst.addCodelock(bp, "0");
			}
		}
		bp.SetMaterial();
	}

	private void includePart(BuilderPart bp, int mode)
	{
		string text = "";
		Vector3 vector = bp._transform.position;
		switch (mode)
		{
		case 0:
			text = IncludePartWith.inst.windowPart;
			vector += Vector3.up * 0.333f;
			break;
		case 1:
			text = IncludePartWith.inst.doorPart;
			break;
		case 2:
			text = IncludePartWith.inst.wallFramePart;
			break;
		case 3:
			text = IncludePartWith.inst.floorFramePart;
			break;
		case 4:
			text = IncludePartWith.inst.floorFrameTriPart;
			break;
		}
		GameObject prefabFromName = MGMT.inst.getPrefabFromName(text);
		if ((bool)prefabFromName)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(prefabFromName, vector, bp._transform.rotation);
			if ((bool)gameObject)
			{
				gameObject.name = text;
				BuilderPart component = gameObject.GetComponent<BuilderPart>();
				component.level = bp.level;
				component.stage = Staging.stage;
				PlacedSetup(component, send: true, notLoaded: false, sound: false, useCodeTog: true);
				MGMT.inst.undoListAddedPart(component);
			}
		}
	}

	public void changeSelection<T>(T obj_bp, bool add)
	{
		if (obj_bp == null)
		{
			return;
		}
		BuilderPart builderPart = null;
		builderPart = ((!(obj_bp is GameObject)) ? (obj_bp as BuilderPart) : (obj_bp as GameObject).GetComponent<BuilderPart>());
		if (add)
		{
			if (!builderPart.selected)
			{
				builderPart.selected = true;
				builderPart.selectMaterial(select: true);
			}
		}
		else if (builderPart.selected)
		{
			builderPart.selected = false;
			builderPart.SetMaterial();
		}
	}

	public void setTier(int newTier)
	{
		if (editMode)
		{
			StartCoroutine(changeSelectionTier(newTier));
			return;
		}
		tier = newTier;
		if ((bool)BPinfo.block)
		{
			BPinfo.block.tier = tier;
			if (BPinfo.block.found || BPinfo.block.ramp)
			{
				BPinfo.block.conditional.setMesh();
			}
		}
		ResourceCount.inst.GetResourceInfo(BPinfo);
	}

	public void hidePlacePart()
	{
		BPinfo._transform.position = new Vector3(0f, -1000f, 0f);
		extendArrow.inst.hideArrow();
	}

	public IEnumerator changeSelectionTier(int tier)
	{
		List<BuilderPart> rendList = new List<BuilderPart>();
		foreach (GameObject obj in objList)
		{
			BuilderPart component = obj.GetComponent<BuilderPart>();
			block block = component.block;
			if ((bool)block && block.tier != tier)
			{
				if ((bool)component.sg)
				{
					component.sg.groupTierSet(tier);
				}
				block.changeTier(tier, received: false);
				component.rend[0].sharedMaterial = tierMats[tier];
				rendList.Add(component);
			}
		}
		yield return new WaitForSeconds(0.3f);
		foreach (BuilderPart item in rendList)
		{
			if ((bool)item)
			{
				item.selectMaterial(select: true);
			}
		}
	}

	public void PlaceModeState()
	{
		if (placeMode)
		{
			rotDeploy = BPinfo._transform.localEulerAngles.y;
		}
		else
		{
			rotBlock = BPinfo._transform.rotation.eulerAngles.y;
		}
		placeMode = !placeMode;
		placePanelSwap.inst.modeSwap();
		Symmetry.inst.setPanelMode();
		if (placeMode)
		{
			rpOverCheck();
		}
		cursorOver.inst.allOff();
	}

	public void EditModeState()
	{
		editMode = !editMode;
		if (editMode)
		{
			BuilderUI.inst.editToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(89.67f, 78.5f);
			if (saveSelection)
			{
				for (int num = objList.Count - 1; num >= 0; num--)
				{
					if (objList[num] != null)
					{
						changeSelection(objList[num], add: true);
					}
					else
					{
						objList.RemoveAt(num);
					}
				}
			}
			if (wiring.inst.on)
			{
				wiring.inst.wireTog.isOn = false;
			}
			ResourceCount.inst.partInfoPanel.SetActive(value: false);
			RayPlaceGround.heightAdjust = false;
			RayPlace.noPlace = true;
			SetPlaceColMode(off: true);
			hidePlacePart();
			cursorOver.inst.allOff();
			return;
		}
		BuilderUI.inst.tierToggles[tier].isOn = true;
		BuilderUI.inst.editToggle.GetComponent<RectTransform>().sizeDelta = new Vector2(89.67f, 65f);
		ResourceCount.inst.partInfoPanel.SetActive(value: true);
		BuilderUI.inst.modeToggle.interactable = true;
		if (copyMove.inst.inProcess)
		{
			copyMove.inst.Cancel();
		}
		if (AlignStructure.inst.isActiveAndEnabled)
		{
			AlignStructure.inst.enabled = false;
		}
		if (Mirror.inst.mirrorTog.isOn)
		{
			Mirror.inst.mirrorTog.isOn = false;
		}
		if (!wiring.inst.on && !Stability.inst.raidMode)
		{
			SetPlaceColMode(off: false);
			RayPlace.noPlace = false;
			rpOverCheck();
		}
		foreach (GameObject obj in objList)
		{
			if (obj != null)
			{
				changeSelection(obj, add: false);
			}
		}
		copyParent = null;
		if (!saveSelection)
		{
			objList.Clear();
		}
	}

	public void SetPlaceColMode(bool off)
	{
		bool flag = false;
		if (off)
		{
			if (placeColMode != PlaceColMode.off)
			{
				flag = true;
				placeColMode = PlaceColMode.off;
			}
		}
		else
		{
			if (wiring.inst.on)
			{
				return;
			}
			if (Symmetry.inst.setCenterTog.isOn)
			{
				if (placeColMode != 0)
				{
					flag = true;
					placeColMode = PlaceColMode.edge;
				}
			}
			else if (editMode)
			{
				if (!inProgress)
				{
					return;
				}
				flag = true;
				placeColMode = PlaceColMode.found;
			}
			else if ((bool)BPinfo.deploy)
			{
				if (BPinfo.deploy.frameFloor || BPinfo.deploy.frameWall)
				{
					if (placeColMode != PlaceColMode.frame)
					{
						flag = true;
						placeColMode = PlaceColMode.frame;
					}
				}
				else if (placeColMode != PlaceColMode.off)
				{
					flag = true;
					placeColMode = PlaceColMode.off;
				}
			}
			else if (BPinfo.found)
			{
				if (placeColMode != PlaceColMode.found)
				{
					flag = true;
				}
				placeColMode = PlaceColMode.found;
			}
			else if (BPinfo.block.ramp || BPinfo.block.steps)
			{
				if (placeColMode != PlaceColMode.all)
				{
					flag = true;
					placeColMode = PlaceColMode.all;
				}
			}
			else if (placeColMode != 0)
			{
				flag = true;
				placeColMode = PlaceColMode.edge;
			}
		}
		if (flag)
		{
			PlaceColMode num = placeColMode;
			int edgeLayerId = 2;
			if (num == PlaceColMode.edge)
			{
				edgeLayerId = 16;
			}
			int baseLayerId = 2;
			if (num == PlaceColMode.found)
			{
				baseLayerId = 16;
			}
			int frameLayerId = 2;
			if (num == PlaceColMode.frame)
			{
				frameLayerId = 16;
			}
			if (num == PlaceColMode.all)
			{
				edgeLayerId = 16;
				baseLayerId = 16;
			}
			foreach (BuilderPart bp in bpList)
			{
				if (bp != null && bp.block != null && !bp.hidden)
				{
					bp.block.setColliderLayer(edgeLayerId, baseLayerId, frameLayerId);
				}
			}
		}
	}

	public void rpOverCheck()
	{
		if (!Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 1000f))
		{
			return;
		}
		if (hitInfo.transform.tag == "block")
		{
			RayPlace component = hitInfo.transform.GetComponent<RayPlace>();
			if ((bool)component)
			{
				component.enterCheck();
			}
		}
		else if (hitInfo.transform.tag == "terrain")
		{
			RayPlaceGround component2 = hitInfo.transform.GetComponent<RayPlaceGround>();
			if ((bool)component2)
			{
				component2.enterCheck();
			}
		}
	}

	public void SelectClear()
	{
		if (!(copyParent != null) || !inProgress)
		{
			foreach (GameObject obj in objList)
			{
				changeSelection(obj, add: false);
			}
			copyParent = null;
			objList.Clear();
		}
	}

	public void SelectAll()
	{
		if (!(copyParent != null) || !inProgress)
		{
			foreach (BuilderPart bp in bpList)
			{
				if (!(bp == null) && !bp.selected)
				{
					changeSelection(bp, add: true);
					objList.Add(bp.gameObject);
				}
			}
			copyParent = null;
		}
	}

	public void SelectInvert()
	{
		if (!(copyParent != null) || !inProgress)
		{
			objList.Clear();
			foreach (BuilderPart bp in bpList)
			{
				if (!(bp == null))
				{
					if (bp.selected)
					{
						changeSelection(bp, add: false);
					}
					else
					{
						changeSelection(bp, add: true);
						objList.Add(bp.gameObject);
					}
				}
			}
			copyParent = null;
		}
	}

	public void AddSymmetry(BuilderPart bp, bool addSelection)
	{
		if (!Symmetry.inst.deployTog.isOn && (bool)bp.deploy)
		{
			return;
		}
		List<BuilderPart> list = new List<BuilderPart>();
		float num = 0f;
		GameObject original = MGMT.inst.prefabList[bp.id];
		int addNum = Symmetry.inst.addNum;
		bp.check = true;
		list.Add(bp);
		Vector3 position = Symmetry.inst.symTransform.position;
		float radius = 0.5f;
		Vector3 point;
		if ((bool)bp.block)
		{
			point = ((!bp.block.roof) ? bp.block.getBoundsCenter() : bp._transform.position);
		}
		else
		{
			Bounds bounds = bp.rend[0].bounds;
			point = bounds.center;
			radius = Mathf.Min(0.7f, bounds.extents.magnitude);
		}
		for (int i = 0; i < addNum; i++)
		{
			num += Symmetry.inst.angle;
			bool flag = false;
			if (addSelection)
			{
				Vector3 vector = RotatePointAroundPivot(point, position, num);
				Collider[] array = (!bp.block) ? Physics.OverlapSphere(vector, radius, 4, QueryTriggerInteraction.Collide) : Physics.OverlapSphere(vector, radius, 16384, QueryTriggerInteraction.Collide);
				if (array.Length != 0)
				{
					Collider[] array2 = array;
					for (int j = 0; j < array2.Length; j++)
					{
						BuilderPart component = array2[j].transform.root.GetComponent<BuilderPart>();
						if (component.check || component.id != bp.id)
						{
							continue;
						}
						Vector3 a = (!component.block) ? component.rend[0].bounds.center : ((!component.block.roof) ? component.block.getBoundsCenter() : component._transform.position);
						if (Vector3.Distance(a, vector) <= 0.05f)
						{
							if (!list.Contains(component))
							{
								component.check = true;
								list.Add(component);
							}
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(original, bp._transform.position, bp._transform.rotation);
				BuilderPart component2 = gameObject.GetComponent<BuilderPart>();
				list.Add(component2);
				component2._transform.RotateAround(position, Vector3.up, num);
				if ((bool)component2.block)
				{
					component2.block.placing = true;
				}
				component2.sg = gameObject.AddComponent<symmetryGroup>();
				AddedSetup(gameObject, bp.name, bp.tier, bp.level);
			}
		}
		if (waitList.Count > 0)
		{
			StartCoroutine(WaitToCheck(new List<BuilderPart>(waitList), notExtend: false, addSymmetry: true));
		}
		waitList.Clear();
		StartCoroutine(Symmetry.inst.setupGroup(list));
	}

	public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
	{
		Vector3 point2 = point - pivot;
		point2 = Quaternion.AngleAxis(angle, Vector3.up) * point2;
		point = point2 + pivot;
		return point;
	}

	public void PartFill(int mode)
	{
		if (objList.Count == 0 || inProgress)
		{
			if (!inProgress)
			{
				popUp.inst.message("selection needed");
			}
			return;
		}
		Symmetry.inst.selectionFilter();
		MGMT.inst.undoListInsert();
		bool flag = !BuilderUI.inst.addCeilingTog.isOn;
		GameObject gameObject = MGMT.inst.prefabList[findPrefabIndex("floor")];
		GameObject gameObject2 = MGMT.inst.prefabList[findPrefabIndex("floor_frame")];
		GameObject gameObject3 = MGMT.inst.prefabList[findPrefabIndex("floor_tri")];
		string text = BuilderUI.inst.partFillOptions.options[BuilderUI.inst.partFillOptions.value].text;
		GameObject original = MGMT.inst.prefabList[findPrefabIndex(text)];
		foreach (GameObject obj3 in objList)
		{
			BuilderPart component = obj3.GetComponent<BuilderPart>();
			if (component.floor || component.found)
			{
				obj3.layer = 1;
				if (flag)
				{
					GameObject gameObject4 = null;
					float level = component.level;
					Vector3 b = new Vector3(0f, 1f, 0f);
					if (component.found)
					{
						b.y += 0.5f;
					}
					if (BuilderUI.inst.partFillOptions.value == 1)
					{
						b.y -= 0.5f;
						level += 0.5f;
					}
					else
					{
						level += 1f;
					}
					gameObject4 = (component.tri ? gameObject3 : ((BuilderUI.inst.partFillOptions.value != 2) ? gameObject : gameObject2));
					GameObject obj = UnityEngine.Object.Instantiate(gameObject4, component._transform.position + b, component._transform.rotation);
					AddedSetup(obj, gameObject4.name, tier, level);
				}
				float lvl = component.level;
				if (BuilderUI.inst.partFillOptions.value == 1)
				{
					lvl = component.level - 0.5f;
				}
				int num = 0;
				for (int i = 0; i < component.block.sockets.Length; i++)
				{
					socket socket = component.block.sockets[i];
					if (socket.female && (!component.found || socket.partType == socket.Part.foundation) && (!component.floor || socket.partType == socket.Part.floor))
					{
						num++;
						if ((component.tri && num > 3) || (!component.tri && num > 4))
						{
							break;
						}
						if (!socket.CheckSocketOccupied() || !socket.connections[0].owner.owner.selected)
						{
							Quaternion rotation = socket._transform.rotation * Quaternion.Euler(0f, 180f, 0f);
							GameObject obj2 = UnityEngine.Object.Instantiate(original, socket._transform.position, rotation);
							AddedSetup(obj2, text, tier, lvl);
						}
					}
				}
				if (mode == 0)
				{
					component.selected = false;
				}
				obj3.layer = 0;
			}
		}
		if (waitList.Count == 0)
		{
			popUp.inst.message("no floors/foundations in selection");
			return;
		}
		foreach (GameObject obj4 in objList)
		{
			obj4.GetComponent<BuilderPart>().selected = true;
			changeSelection(obj4, add: false);
		}
		objList.Clear();
		if (waitList.Count > 0)
		{
			StartCoroutine(WaitToCheck(new List<BuilderPart>(waitList), notExtend: true, addSymmetry: false));
		}
		waitList.Clear();
	}

	public void CopyUp()
	{
		if (objList.Count == 0 || inProgress)
		{
			if (!inProgress)
			{
				popUp.inst.message("selection needed");
			}
			return;
		}
		Symmetry.inst.selectionFilter();
		MGMT.inst.undoListInsert();
		foreach (GameObject obj3 in objList)
		{
			BuilderPart component = obj3.GetComponent<BuilderPart>();
			if (!copyMove.copyDeploys)
			{
				bool flag = false;
				if ((bool)component.deploy)
				{
					if ((bool)component.door || component.deploy.frameFloor || component.deploy.frameWall || component.deploy.windowPart)
					{
						flag = true;
					}
				}
				else
				{
					flag = true;
				}
				if (!flag)
				{
					changeSelection(component, add: false);
					continue;
				}
			}
			else if (component.isDevice(typeof(elevator)))
			{
				changeSelection(component, add: false);
				continue;
			}
			Vector3 b = new Vector3(0f, 1f, 0f);
			if (component.found)
			{
				string name = obj3.name;
				if (!component.tri)
				{
					name = "floor";
					b.y += 0.5f;
				}
				else
				{
					name = "floor_tri";
					b.y += 0.5f;
				}
				GameObject obj = UnityEngine.Object.Instantiate(Resources.Load(name), component._transform.position + b, component._transform.rotation) as GameObject;
				AddedSetup(obj, name, component.block.tier, component.level + 1f);
			}
			else
			{
				GameObject obj2 = UnityEngine.Object.Instantiate(MGMT.inst.prefabList[component.id], component._transform.position + b, component._transform.rotation);
				AddedSetup(obj2, obj3.name, component.tier, component.level + 1f);
			}
			changeSelection(component, add: false);
		}
		objList.Clear();
		if (waitList.Count > 0)
		{
			StartCoroutine(WaitToCheck(new List<BuilderPart>(waitList), notExtend: true, addSymmetry: false));
		}
		waitList.Clear();
	}

	public void Extend(BuilderPart objbp)
	{
		int amount = extendArrow.amount;
		if (objbp == null || amount == 0)
		{
			return;
		}
		GameObject gameObject = MGMT.inst.prefabList[BPinfo.id];
		GameObject gameObject2 = null;
		if (BPinfo.found)
		{
			Transform transform = objbp._transform.GetChild(5);
			if (BPinfo.tri)
			{
				int num = 0;
				bool flag = true;
				for (int i = 1; i < amount; i++)
				{
					Quaternion rotation = transform.rotation;
					num++;
					if (num == 2)
					{
						rotation = objbp._transform.rotation;
					}
					gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.position, rotation);
					BuilderPart component = gameObject2.GetComponent<BuilderPart>();
					component.block.placing = true;
					redblock = component.block.heightCheck();
					transform = ((!flag) ? component._transform.GetChild(5) : component._transform.GetChild(6));
					if (num == 2)
					{
						num = 0;
						flag = !flag;
					}
					if (redblock)
					{
						UnityEngine.Object.Destroy(gameObject2);
					}
					else
					{
						AddedSetup(gameObject2, gameObject.name, tier, 1f);
					}
				}
			}
			else
			{
				Quaternion rotation2 = objbp._transform.rotation;
				for (int j = 1; j < amount; j++)
				{
					gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.position, rotation2);
					BuilderPart component2 = gameObject2.GetComponent<BuilderPart>();
					component2.block.placing = true;
					redblock = component2.block.heightCheck();
					transform = component2._transform.GetChild(5);
					if (redblock)
					{
						UnityEngine.Object.Destroy(gameObject2);
					}
					else
					{
						AddedSetup(gameObject2, gameObject.name, tier, 1f);
					}
				}
			}
		}
		else if (BPinfo.block.wall || BPinfo.block.pillar)
		{
			if (objbp._transform.childCount == 1)
			{
				return;
			}
			float num2 = objbp.level;
			Vector3 vector = objbp._transform.position;
			for (int k = 1; k < amount; k++)
			{
				if (objbp.block.halfHeight)
				{
					num2 += 0.5f;
					vector += new Vector3(0f, 0.5f, 0f);
				}
				else
				{
					num2 += 1f;
					vector += new Vector3(0f, 1f, 0f);
				}
				gameObject2 = UnityEngine.Object.Instantiate(gameObject, vector, objbp._transform.rotation);
				AddedSetup(gameObject2, gameObject.name, tier, num2);
			}
		}
		else if (BPinfo.floor)
		{
			Transform transform2 = objbp._transform.GetChild(2);
			int num3 = 0;
			bool flag2 = false;
			for (int l = 1; l < amount; l++)
			{
				gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform2.position, transform2.rotation);
				if (BPinfo.tri)
				{
					if (num3 < 2 && num3 > 0)
					{
						num3++;
					}
					else
					{
						flag2 = !flag2;
						num3 = 1;
					}
					transform2 = ((!flag2) ? gameObject2.transform.GetChild(2) : gameObject2.transform.GetChild(3));
				}
				else
				{
					transform2 = gameObject2.transform.GetChild(2);
				}
				AddedSetup(gameObject2, gameObject.name, tier, objbp.level);
			}
		}
		if (waitList.Count > 0)
		{
			StartCoroutine(WaitToCheck(new List<BuilderPart>(waitList), notExtend: false, addSymmetry: false));
		}
		waitList.Clear();
	}

	private void AddedSetup(GameObject obj, string named, int tier, float lvl)
	{
		obj.layer = 1;
		obj.name = named;
		BuilderPart component = obj.GetComponent<BuilderPart>();
		component.tier = tier;
		component.stage = Staging.stage;
		component.level = lvl;
		if ((bool)component.device)
		{
			component.device.owner = component;
		}
		if ((bool)component.oc)
		{
			component.oc.AddRigidbody();
		}
		waitList.Add(component);
	}

	private IEnumerator WaitToCheck(List<BuilderPart> checkList, bool notExtend, bool addSymmetry)
	{
		inProgress = true;
		yield return new WaitForFixedUpdate();
		int num = -1;
		if (notExtend)
		{
			Vector3 position = checkList[0]._transform.position;
			AudioPlayer.inst.playAtPoint(position, 0);
			if (multiplayer)
			{
				Sender.sendSound(position, 0);
			}
		}
		else
		{
			overlapCheck.overlapCount = 0;
			if (!addSymmetry)
			{
				string name = checkList[0].name;
				if (BuilderUI.inst.includeToggles[0].isOn && name == "wall_window")
				{
					num = 0;
				}
				else if (BuilderUI.inst.includeToggles[1].isOn && name == "wall_doorway")
				{
					num = 1;
				}
				else if (BuilderUI.inst.includeToggles[2].isOn && name == "wall_frame")
				{
					num = 2;
				}
				else if (BuilderUI.inst.includeToggles[3].isOn && name == "floor_frame")
				{
					num = 3;
				}
				else if (BuilderUI.inst.includeToggles[4].isOn && name == "floor_frame_tri")
				{
					num = 4;
				}
			}
		}
		for (int i = 0; i < checkList.Count; i++)
		{
			BuilderPart builderPart = checkList[i];
			if (!builderPart)
			{
				continue;
			}
			GameObject gameObject = builderPart.gameObject;
			if ((bool)builderPart.oc && builderPart.oc.DestroySelfOverlap(builderPart, send: false))
			{
				UnityEngine.Object.Destroy(gameObject);
				continue;
			}
			if (notExtend)
			{
				PlacedSetup(builderPart, send: true, notLoaded: false, sound: false, useCodeTog: false);
				objList.Add(gameObject);
				if (!addSymmetry)
				{
					changeSelection(builderPart, add: true);
				}
			}
			else
			{
				PlacedSetup(builderPart, send: true, notLoaded: false, sound: false, useCodeTog: true);
			}
			if (num > -1)
			{
				includePart(builderPart, num);
			}
			if ((bool)builderPart.block)
			{
				builderPart.block.GetNeighborLinks(updateNeighbors: true, clear: false);
			}
		}
		for (int j = 0; j < checkList.Count; j++)
		{
			BuilderPart builderPart2 = checkList[j];
			if (!builderPart2)
			{
				continue;
			}
			if ((bool)builderPart2.block)
			{
				if ((bool)builderPart2.block.stabilityEntity)
				{
					builderPart2.block.stabilityEntity.UpdateStability();
				}
				if ((bool)builderPart2.block.conditional)
				{
					builderPart2.block.conditional.RunCheck();
				}
				if (builderPart2.block.found || builderPart2.block.ramp)
				{
					builderPart2.block.conditional.setMesh();
				}
			}
			if (!addSymmetry)
			{
				MGMT.inst.undoListAddedPart(builderPart2);
			}
		}
		inProgress = false;
	}

	public void destroyPart(BuilderPart bp, bool audio, bool receive)
	{
		if (!bp)
		{
			return;
		}
		GameObject gameObject = bp.gameObject;
		if (gameObject.tag == "destroy")
		{
			return;
		}
		gameObject.tag = "destroy";
		gameObject.layer = 2;
		bpList.Remove(bp);
		if ((bool)bp.sg)
		{
			bp.sg.groupDestroy(bp, addUndo: false);
		}
		if (!receive)
		{
			if (cursorOver.inst.onCount > 0)
			{
				cursorOver.inst.allOff();
			}
			if (Stability.inst.raidMode)
			{
				if (!bp.hidden)
				{
					AudioPlayer.inst.crashAtPoint(bp._transform.position);
				}
			}
			else if (multiplayer)
			{
				Multiplayer.destroySend(bp);
			}
		}
		else
		{
			if (bp == null)
			{
				return;
			}
			if (editMode && bp.selected)
			{
				objList.Remove(gameObject);
			}
		}
		if ((bool)bp.block)
		{
			bp.block.destroyed = true;
			bp.block.UpdateNeighborLinks(removed: true);
		}
		else
		{
			if ((bool)bp.device)
			{
				MGMT.inst.devices.Remove(bp.instId);
				StartCoroutine(bp.device.disconnectAll());
			}
			if ((bool)bp.door && (bool)bp.door.doorCtrl)
			{
				bp.door.doorCtrl.disconnect();
			}
		}
		if (audio)
		{
			AudioPlayer.inst.playAtPoint(bp._transform.position, 1);
		}
		if (BuilderPart.placedOn)
		{
			overlapCheck.overlapCount = 0;
		}
		gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(gameObject);
	}

	public void destroySelection()
	{
		AudioPlayer.inst.playAtPoint(objList[0].transform.position, 1);
		if (multiplayer)
		{
			Sender.sendSound(objList[0].transform.position, 1);
			Sender.placeSound = false;
		}
		List<block> list = new List<block>();
		MGMT.inst.undoListInsert();
		if (Symmetry.inst.isOn)
		{
			for (int i = 0; i < Symmetry.inst.groupList.Count; i++)
			{
				BuilderPart builderPart = Symmetry.inst.groupList[i];
				if ((bool)builderPart && builderPart.selected && (bool)builderPart.sg)
				{
					builderPart.sg.selectionGroupDestroy(builderPart);
				}
			}
		}
		for (int j = 0; j < objList.Count; j++)
		{
			BuilderPart component = objList[j].GetComponent<BuilderPart>();
			MGMT.inst.addDeletedPart(component, undo: true);
			if (multiplayer)
			{
				Multiplayer.destroySend(component);
			}
			if ((bool)component.block)
			{
				component.block.destroyed = true;
				list.Add(component.block);
			}
			else
			{
				if ((bool)component.device)
				{
					MGMT.inst.devices.Remove(component.instId);
					StartCoroutine(component.device.disconnectAll());
				}
				if ((bool)component.door && (bool)component.door.doorCtrl)
				{
					component.door.doorCtrl.disconnect();
				}
			}
			bpList.Remove(component);
			objList[j].layer = 2;
			objList[j].tag = "destroy";
			MGMT.inst.destroyObjs.Add(objList[j]);
		}
		Sender.placeSound = true;
		foreach (block item in list)
		{
			item.UpdateNeighborLinks(removed: true);
		}
		objList.Clear();
		StartCoroutine(MGMT.inst.destroySlowly());
	}

	public void checkAllConditionals(bool clearNeighbors)
	{
		for (int i = 0; i < bpList.Count; i++)
		{
			BuilderPart builderPart = bpList[i];
			if ((bool)builderPart.block)
			{
				builderPart.block.GetNeighborLinks(updateNeighbors: false, clearNeighbors);
			}
		}
		for (int j = 0; j < bpList.Count; j++)
		{
			block block = bpList[j].block;
			if ((bool)block)
			{
				if ((bool)block.stabilityEntity)
				{
					block.stabilityEntity.UpdateStability();
				}
				if ((bool)block.conditional)
				{
					block.conditional.RunCheck();
				}
			}
		}
	}

	private IEnumerator waitToDestroy(GameObject obj)
	{
		yield return new WaitForFixedUpdate();
		UnityEngine.Object.Destroy(obj);
	}

	public IEnumerator continuousWait()
	{
		PlacePart.contPlace = true;
		yield return new WaitForFixedUpdate();
		PlacePart.contPlace = false;
	}

	public int findPrefabIndex(string name)
	{
		for (int i = 0; i < MGMT.inst.prefabList.Length; i++)
		{
			if (!(MGMT.inst.prefabList[i] == null) && name == MGMT.inst.prefabList[i].name)
			{
				return i;
			}
		}
		return 0;
	}

	[ContextMenu("edge distance")]
	private void distancesFromEdge()
	{
		List<BuilderPart> list = new List<BuilderPart>();
		List<BuilderPart> list2 = new List<BuilderPart>();
		foreach (GameObject obj in objList)
		{
			BuilderPart component = obj.GetComponent<BuilderPart>();
			component.edgeDist = -1;
			if (component != null && (component.found || component.floor) && component.block.OpenEdgeCheck())
			{
				component.edgeDist = 0;
				list.Add(component);
			}
		}
		int num = 1;
		while (list != null && list.Count > 0)
		{
			for (int num2 = list.Count - 1; num2 >= 0; num2--)
			{
				BuilderPart builderPart = list[num2];
				builderPart.stage = builderPart.edgeDist + 1;
				for (int i = 0; i < builderPart.block.sockets.Length; i++)
				{
					socket socket = builderPart.block.sockets[i];
					if (!socket.female)
					{
						continue;
					}
					for (int j = 0; j < socket.connections.Count; j++)
					{
						block owner = socket.connections[j].owner;
						if (owner.owner.edgeDist <= -1 && ((builderPart.found && owner.found) || (builderPart.floor && owner.owner.floor)))
						{
							owner.owner.edgeDist = num;
							list2.Add(owner.owner);
						}
					}
				}
			}
			num++;
			list = new List<BuilderPart>(list2);
			list2.Clear();
		}
	}
}
