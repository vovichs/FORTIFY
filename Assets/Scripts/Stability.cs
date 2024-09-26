using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stability : MonoBehaviour
{
	public static Stability inst;

	public bool raidMode;

	[HideInInspector]
	public bool colorView;

	[HideInInspector]
	public bool fullColorView = true;

	public GameObject colorGreyout;

	private bool fireWait;

	private List<BuilderPart> hidden = new List<BuilderPart>();

	public LayerMask usedMask;

	public LayerMask deploysMask;

	public LayerMask rocketMask;

	private BuilderSystem sys;

	private AudioSource AS;

	public Toggle stabViewToggle;

	public Toggle hideToggle;

	public GameObject textOver;

	public GameObject raidPanel;

	public GameObject crosshair;

	public GameObject colors;

	public Toggle[] weaponTogs;

	public GameObject MLRSrocket;

	public Transform MLRStarget;

	public InputField MLRSinput;

	public GameObject shell_stblt;

	public GameObject sedan;

	public GameObject lastCar;

	public int mode = -2;

	public GameObject[] weaponPrefabs;

	public Text[] weaponTexts;

	public int usedCount;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		AS = sys.GetComponent<AudioSource>();
		colorView = false;
	}

	private void Update()
	{
		if (!raidMode)
		{
			return;
		}
		if (Input.anyKey)
		{
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad1))
			{
				weaponTogs[0].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad2))
			{
				weaponTogs[1].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad3))
			{
				weaponTogs[2].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad4))
			{
				weaponTogs[3].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad5))
			{
				weaponTogs[4].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad6))
			{
				weaponTogs[5].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad7))
			{
				weaponTogs[6].isOn = true;
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8) || UnityEngine.Input.GetKeyDown(KeyCode.Keypad8))
			{
				weaponTogs[7].isOn = true;
			}
		}
		if (mode == -3)
		{
			if (fireWait)
			{
				return;
			}
			if (Physics.Raycast(CameraCtrl.inst.fireFrom.position, CameraCtrl.inst.fireFrom.forward, out RaycastHit hitInfo, 250f, 256))
			{
				MLRStarget.position = Vector3.Lerp(MLRStarget.position, hitInfo.point, 0.5f);
				if (Input.GetButtonDown("fire"))
				{
					MLRStarget.position = new Vector3(0f, -1000f, 0f);
					StartCoroutine(MLRS(hitInfo.point));
				}
			}
			else
			{
				MLRStarget.position = new Vector3(0f, -1000f, 0f);
			}
		}
		else
		{
			if (Input.GetButtonDown("fire") && !fireWait)
			{
				StartCoroutine(fire());
			}
			if (UnityEngine.Input.GetKeyDown(KeyCode.B) && lastCar != null)
			{
				lastCar.GetComponent<Car>().rocket();
			}
		}
	}

	public void RaidMode()
	{
		if (!raidMode)
		{
			raidMode = true;
			Symmetry.inst.symTog.isOn = false;
			sys.hidePlacePart();
			overlapCheck.inst.reset();
			if (sys.showRanges)
			{
				sys.showRangesTog.isOn = false;
			}
			crosshair.SetActive(value: true);
			raidPanel.SetActive(value: true);
			resetCounts();
			base.enabled = true;
			if (wiring.inst.on)
			{
				wiring.inst.wireTog.isOn = false;
			}
			if (hideToggle.isOn)
			{
				hideToggle.isOn = false;
			}
			RayPlace.noPlace = true;
			Device.disabled = true;
			if (BuilderSystem.editMode)
			{
				BuilderUI.inst.editToggle.isOn = false;
			}
			if (CameraCtrl.inst.mode == CameraCtrl.Mode.third)
			{
				CameraCtrl.avatar.swapPlayerItems(fireMode: true);
			}
			if (cupboardRange.inst.tog.isOn)
			{
				cupboardRange.inst.tog.isOn = false;
			}
			infoSpotPanel.inst.toggleSpots(hide: true);
			foreach (wire wire in BuilderSystem.wireList)
			{
				if ((bool)wire)
				{
					wire.showWire(state: false);
				}
			}
			sys.objList.Clear();
			MGMT.inst.undoList.Clear();
			PrepParts(reset: false);
			ShowStability();
		}
		else
		{
			raidMode = false;
			crosshair.SetActive(value: false);
			raidPanel.SetActive(value: false);
			RayPlace.noPlace = false;
			sys.rpOverCheck();
			Device.disabled = false;
			base.enabled = false;
			if (CameraCtrl.inst.mode == CameraCtrl.Mode.third)
			{
				CameraCtrl.avatar.swapPlayerItems(fireMode: false);
			}
			foreach (wire wire2 in BuilderSystem.wireList)
			{
				if ((bool)wire2)
				{
					wire2.showWire(state: true);
				}
			}
			MLRStarget.gameObject.SetActive(value: false);
			MGMT.inst.getDestroyObjs(clearScene: false);
			StartCoroutine(MGMT.inst.destroySlowly());
			if (hideToggle.isOn)
			{
				hideToggle.isOn = false;
			}
			foreach (BuilderPart item in hidden)
			{
				if ((bool)item)
				{
					item.SetMaterial();
					item.gameObject.SetActive(value: true);
					sys.bpList.Add(item);
					if (item.deviceIs(typeof(autoTurret)))
					{
						(item.device as autoTurret).stabilityExitCheck();
					}
				}
			}
			hidden.Clear();
			sys.SetPlaceColMode(off: false);
		}
	}

	public void resetStabilityMode()
	{
		MGMT.inst.getDestroyObjs(clearScene: false);
		StartCoroutine(MGMT.inst.destroySlowly());
		resetCounts();
		StartCoroutine(waitToDestruct());
		sys.bpList = new List<BuilderPart>(hidden);
		PrepParts(reset: true);
	}

	private void PrepParts(bool reset)
	{
		for (int num = sys.bpList.Count - 1; num >= 0; num--)
		{
			if (!(sys.bpList[num] == null))
			{
				BuilderPart builderPart = sys.bpList[num];
				GameObject gameObject = builderPart.gameObject;
				if (!reset)
				{
					hidden.Add(builderPart);
					gameObject.SetActive(value: false);
				}
				if (builderPart.stability == -1f)
				{
					sys.bpList.RemoveAt(num);
				}
				else
				{
					builderPart = cloneObj(gameObject);
					sys.bpList[num] = builderPart;
					builderPart.gameObject.SetActive(value: true);
					if ((bool)builderPart.block)
					{
						builderPart.health = sys.tierHealth[builderPart.tier];
						Transform[] edgeCols = builderPart.block.edgeCols;
						for (int i = 0; i < edgeCols.Length; i++)
						{
							edgeCols[i].gameObject.SetActive(value: false);
						}
						GameObject[] baseCols = builderPart.block.baseCols;
						for (int i = 0; i < baseCols.Length; i++)
						{
							baseCols[i].SetActive(value: false);
						}
						if ((bool)builderPart.block.frameCol)
						{
							builderPart.block.frameCol.gameObject.SetActive(value: false);
						}
						if (builderPart.wall)
						{
							if ((bool)builderPart.block.frameCol)
							{
								builderPart.block.frameCol.gameObject.SetActive(value: false);
							}
							if (builderPart.col is MeshCollider)
							{
								(builderPart.col as MeshCollider).convex = false;
							}
						}
						else if (builderPart.block.ramp)
						{
							(builderPart.col as MeshCollider).convex = true;
						}
					}
					else
					{
						builderPart.health = builderPart.info.HP;
						if ((bool)builderPart.door && builderPart.door.open)
						{
							StartCoroutine(builderPart.changeDoorMeshState(state: false, audio: false, send: false));
						}
						if (!builderPart.deploy.shelf && builderPart.col is MeshCollider)
						{
							(builderPart.col as MeshCollider).convex = true;
						}
						LODGroup component = builderPart.gameObject.GetComponent<LODGroup>();
						if (component != null)
						{
							component.ForceLOD(0);
						}
					}
				}
			}
		}
		for (int num2 = sys.bpList.Count - 1; num2 >= 0; num2--)
		{
			if ((bool)sys.bpList[num2].deploy)
			{
				supportCheck(sys.bpList[num2]);
			}
		}
		BuilderSystem.inst.checkAllConditionals(clearNeighbors: true);
	}

	private void supportCheck(BuilderPart bp)
	{
		if ((bp.deploy.ground == Deploy.Ground.noAlign || bp.deploy.ground == Deploy.Ground.heightAdjust) && !bp.device)
		{
			bp.stability = 100f;
			return;
		}
		bp.gameObject.layer = 2;
		if (bp.deploy.corners.Length != 0)
		{
			bp.stability = 0f;
			bool flag = false;
			for (int i = 0; i < bp.deploy.corners.Length; i++)
			{
				if (!Physics.Raycast(bp.deploy.corners[i].position, -bp.deploy.corners[i].up, out RaycastHit hitInfo, 0.1f, deploysMask))
				{
					continue;
				}
				bp.stability = 100f;
				GameObject gameObject = hitInfo.collider.gameObject;
				if (gameObject.tag == "terrain")
				{
					flag = true;
					break;
				}
				BuilderPart component = gameObject.GetComponent<BuilderPart>();
				if ((bool)component)
				{
					if (component.sList == null)
					{
						component.sList = gameObject.AddComponent<SupportList>();
						component.sList.sParts.Add(bp);
					}
					else if (!component.sList.sParts.Contains(bp))
					{
						component.sList.sParts.Add(bp);
					}
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				bp.raidDestroy(Vector3.zero);
			}
		}
		else
		{
			GameObject gameObject2 = null;
			if (bp.deploy.frameFloor)
			{
				gameObject2 = bp.deploy.getParentFloorFrame(bp);
			}
			else
			{
				Vector3 position = bp._transform.position;
				Vector3 vector = Vector3.down;
				if ((bool)bp.door || bp.deploy.frameWall)
				{
					vector = Vector3.up;
					position += new Vector3(0f, 0.7f, 0f);
				}
				else
				{
					position += new Vector3(0f, 0.2f, 0f);
					if (bp.deploy.windowPart)
					{
						vector = -vector;
					}
				}
				if (Physics.Raycast(position, vector, out RaycastHit hitInfo2, 0.3f, deploysMask))
				{
					GameObject gameObject3 = hitInfo2.collider.gameObject;
					if (gameObject3.tag == "block" || gameObject3.tag == "deploy")
					{
						gameObject2 = gameObject3;
					}
				}
				else
				{
					bp.stability = 0f;
				}
			}
			if (gameObject2 != null)
			{
				BuilderPart component2 = gameObject2.GetComponent<BuilderPart>();
				if (component2.sList == null)
				{
					component2.sList = gameObject2.AddComponent<SupportList>();
				}
				component2.sList.sParts.Add(bp);
			}
		}
		if (!bp.hidden)
		{
			bp.setRaycastLayer();
		}
	}

	private IEnumerator waitToDestruct()
	{
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator waitShowStability()
	{
		PrepParts(reset: false);
		yield return new WaitForSeconds(0.5f);
		ShowStability();
	}

	public void setColorView()
	{
		colorView = !colorView;
		colors.SetActive(colorView);
		if (colorView && Staging.inst.stageMode)
		{
			Staging.inst.stageMode = false;
			Staging.inst.stageTog.SetIsOnWithoutNotify(value: false);
			Staging.inst.stageTog.GetComponent<toggleColor>().OnToggleValueChanged(isOn: false);
		}
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			BuilderPart builderPart = sys.bpList[i];
			if ((bool)builderPart)
			{
				builderPart.SetMaterial();
			}
		}
	}

	public void setFullColorView()
	{
		fullColorView = !fullColorView;
		colorGreyout.SetActive(!fullColorView);
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			BuilderPart builderPart = sys.bpList[i];
			if ((bool)builderPart)
			{
				builderPart.SetMaterial();
			}
		}
	}

	private void ShowStability()
	{
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			BuilderPart builderPart = sys.bpList[i];
			if (!(builderPart == null) && (bool)builderPart.block && builderPart.stability < 0.05f)
			{
				builderPart.raidDestroy(Vector3.zero);
			}
		}
	}

	public bool SetMaterialCheck(BuilderPart bp)
	{
		if (colorView)
		{
			bp.SetStabilityMat();
		}
		if (!raidMode)
		{
			return false;
		}
		if ((double)bp.stability < 0.05)
		{
			return true;
		}
		return false;
	}

	public void ChangeWeaponType(int type)
	{
		if (mode != type)
		{
			fireWait = false;
			mode = type;
			MLRStarget.gameObject.SetActive(mode == -3);
		}
	}

	private IEnumerator fire()
	{
		if (CameraCtrl.inst.mode == CameraCtrl.Mode.third)
		{
			CameraCtrl.avatar.cannonSmoke.Play();
		}
		fireWait = true;
		if (mode == -2)
		{
			AS.Play();
			Object.Instantiate(shell_stblt, CameraCtrl.inst.fireFrom.position, CameraCtrl.inst.fireFrom.rotation);
		}
		else if (mode == -1)
		{
			AS.Play();
			lastCar = UnityEngine.Object.Instantiate(sedan, CameraCtrl.inst.fireFrom.position, CameraCtrl.inst.fireFrom.rotation);
		}
		else
		{
			AS.Play();
			Object.Instantiate(weaponPrefabs[mode], CameraCtrl.inst.fireFrom.position, CameraCtrl.inst.fireFrom.rotation);
			weaponTexts[mode].text = (1 + int.Parse(weaponTexts[mode].text)).ToString();
		}
		yield return new WaitForSeconds(0.5f);
		fireWait = false;
	}

	[ContextMenu("MLRS")]
	public void testMLRS()
	{
		StartCoroutine(MLRS(Vector3.zero));
	}

	private IEnumerator MLRS(Vector3 targetPos)
	{
		float targetAreaRadius = 10f;
		float RocketDamageRadius = 5f;
		int radiusModIndex = 0;
		float[] radiusMods = new float[4]
		{
			0.1f,
			0.2f,
			0.333333343f,
			2f / 3f
		};
		int count2 = int.Parse(MLRSinput.text);
		count2 = Mathf.Clamp(count2, 1, 12);
		fireWait = true;
		for (int i = 0; i < count2; i++)
		{
			yield return new WaitForSeconds(0.5f);
			float d = 1f;
			if (radiusModIndex < radiusMods.Length)
			{
				d = radiusMods[radiusModIndex];
			}
			radiusModIndex++;
			Vector2 vector = UnityEngine.Random.insideUnitCircle * (targetAreaRadius - RocketDamageRadius) * d;
			Vector3 position = targetPos + new Vector3(vector.x, 50f, vector.y);
			Object.Instantiate(MLRSrocket, position, Quaternion.Euler(90f, 0f, 0f));
		}
		yield return new WaitForSeconds(5f);
		fireWait = false;
	}

	public void resetCounts()
	{
		Text[] array = weaponTexts;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].text = "0";
		}
	}

	private BuilderPart cloneObj(GameObject obj)
	{
		BuilderPart component = UnityEngine.Object.Instantiate(obj).GetComponent<BuilderPart>();
		component.sObj = null;
		return component;
	}
}
