using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayPlaceGround : MonoBehaviour
{
	private Collider col;

	private BuilderSystem sys;

	public static bool heightAdjust = false;

	public static bool blocked = false;

	public bool cave;

	public bool caveblock;

	private float foundMax = 0.5f;

	private float foundMin = -0.49f;

	private float groundOffset;

	public static RayPlaceGround overThis;

	public static List<RayPlaceGround> terrains = new List<RayPlaceGround>();

	private RaycastHit hit;

	private void Awake()
	{
		blocked = false;
		heightAdjust = false;
		sys = BuilderSystem.inst;
		col = GetComponent<Collider>();
		base.enabled = false;
		terrains.Add(this);
		overThis = null;
	}

	private void OnMouseEnter()
	{
		enterCheck();
	}

	public void enterCheck()
	{
		if (!RayPlace.noPlace && !blocked)
		{
			overThis = this;
			sys.redblock = false;
			base.enabled = true;
			BuilderPart bPinfo = sys.BPinfo;
			if ((bool)bPinfo.deploy && (bPinfo.deploy.ground == Deploy.Ground.noAlign || bPinfo.deploy.ground == Deploy.Ground.heightAdjust))
			{
				bPinfo._transform.rotation = Quaternion.Euler(0f, bPinfo._transform.eulerAngles.y, 0f);
			}
			if (!placeOptions.ignoreRules)
			{
				foundMax = 0.5f;
			}
			else
			{
				foundMax = 1000f;
			}
		}
	}

	private void OnMouseExit()
	{
		if (base.enabled)
		{
			if (!heightAdjust)
			{
				sys.hidePlacePart();
			}
			else
			{
				extendArrow.inst.hideArrow();
			}
			base.enabled = false;
		}
		overThis = null;
	}

	public static void ChangedPart()
	{
		BuilderPart bPinfo = BuilderSystem.inst.BPinfo;
		heightAdjust = false;
		extendArrow.inst.hideArrow();
		if ((bool)bPinfo.deploy && bPinfo.deploy.ground == Deploy.Ground.none)
		{
			blocked = true;
			return;
		}
		blocked = false;
		if (!bPinfo.block)
		{
			caveCheckAll();
		}
	}

	private void Update()
	{
		if (RayPlace.noPlace || blocked)
		{
			return;
		}
		BuilderPart bPinfo = BuilderSystem.inst.BPinfo;
		Transform transform = sys.BPinfo._transform;
		if (BuilderUI.inst.mouseOverUI)
		{
			if (!sys.inProgress)
			{
				sys.hidePlacePart();
			}
			heightAdjust = false;
			return;
		}
		Ray ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		if (BuilderSystem.placeMode)
		{
			if (!placeOptions.allowOverlap && caveblock)
			{
				overlapCheck.overlap = true;
			}
			if (!col.Raycast(ray, out hit, 150f))
			{
				return;
			}
			Vector3 point = hit.point;
			if (!heightAdjust)
			{
				transform.position = point;
			}
			if ((double)point.y < -0.1)
			{
				if (bPinfo.deploy.water == Deploy.Water.on)
				{
					if ((bool)bPinfo.deploy.offset)
					{
						transform.position = new Vector3(point.x, -0.2f, point.z);
					}
					else
					{
						transform.position = new Vector3(point.x, -0.1f, point.z);
					}
					transform.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
					if (Input.GetMouseButtonDown(0) && !overlapCheck.overlap)
					{
						sys.placeObj(0, 1f);
					}
					return;
				}
				if (bPinfo.deploy.water == Deploy.Water.underOnly)
				{
					if ((bool)bPinfo.device)
					{
						transform.position = new Vector3(point.x, -0.1f, point.z);
					}
					if (!placeOptions.ignoreRules && (double)point.y < -0.2)
					{
						overlapCheck.overlap = true;
						return;
					}
				}
				else if (!placeOptions.ignoreRules && bPinfo.deploy.water != Deploy.Water.under)
				{
					overlapCheck.overlap = true;
					return;
				}
			}
			else if (bPinfo.deploy.water == Deploy.Water.underOnly && !placeOptions.ignoreRules && sceneSettings.inst.scene != sceneSettings.eScene.flat)
			{
				overlapCheck.overlap = true;
				return;
			}
			if (bPinfo.deploy.ground == Deploy.Ground.norm)
			{
				if ((bool)bPinfo.deploy.offset)
				{
					transform.Translate(transform.position - bPinfo.deploy.offset.position, Space.World);
					transform.rotation = Quaternion.FromToRotation(bPinfo.deploy.offset.up, hit.normal) * transform.rotation;
				}
				else
				{
					Vector3 fromDirection = transform.up;
					if (bPinfo.deploy.place == Deploy.Place.forward)
					{
						fromDirection = transform.forward;
					}
					else if (bPinfo.deploy.place == Deploy.Place.backward)
					{
						fromDirection = -transform.forward;
					}
					transform.rotation = Quaternion.FromToRotation(fromDirection, hit.normal) * transform.rotation;
				}
			}
			else if (bPinfo.deploy.ground == Deploy.Ground.noAlign)
			{
				deployHeightCheck(bPinfo.deploy);
			}
			else if (bPinfo.deploy.ground == Deploy.Ground.heightAdjust)
			{
				if (!heightAdjust)
				{
					Vector3 vector2 = transform.position + new Vector3(0f, groundOffset, 0f);
					deployHeightCheckNew(bPinfo.deploy);
					if (Input.GetMouseButtonDown(0) && !overlapCheck.overlap)
					{
						StartCoroutine(heightAdjustDeploy(bPinfo));
					}
				}
				return;
			}
			if (bPinfo.info.angleLimit > 0f && bPinfo.info.angleLimit < Vector3.Angle(hit.normal, Vector3.up))
			{
				overlapCheck.overlap = true;
			}
			else if (Input.GetMouseButtonDown(0) && !overlapCheck.overlap)
			{
				sys.placeObj(0, 1f);
			}
		}
		else
		{
			if (!bPinfo.found || PlacePart.contPlace || heightAdjust)
			{
				return;
			}
			if (col.Raycast(ray, out hit, 150f))
			{
				transform.position = hit.point;
				float d = 0.5f;
				if (bPinfo.tri)
				{
					d = 0.288f;
				}
				Vector3 vector = transform.TransformPoint(Vector3.right * d);
				transform.position = vector + new Vector3(0f, groundOffset, 0f);
				extendArrow.inst.alignOnGround(transform.position, -transform.right);
				if (!Proximity.inst.Check(bPinfo) || bPinfo.block.heightCheck())
				{
					sys.redblock = true;
				}
				if (Input.GetMouseButtonDown(0) && !overlapCheck.overlap)
				{
					StartCoroutine(heightAdjustBlock(vector));
				}
			}
			else if (sys.BPinfo != null)
			{
				sys.hidePlacePart();
			}
		}
	}

	private IEnumerator heightAdjustBlock(Vector3 pos)
	{
		BuilderPart bp = sys.BPinfo;
		heightAdjust = true;
		bool place = false;
		while (!Input.GetMouseButtonUp(0) && heightAdjust && (bool)bp)
		{
			groundOffset += UnityEngine.Input.GetAxis("Mouse Y") * 0.5f;
			groundOffset = Mathf.Clamp(groundOffset, foundMin, foundMax);
			bp._transform.position = new Vector3(pos.x, pos.y + groundOffset, pos.z);
			if (!Proximity.inst.Check(bp) || bp.block.heightCheck())
			{
				sys.redblock = true;
			}
			place = !sys.redblock;
			yield return null;
		}
		if (heightAdjust && place)
		{
			sys.placeObj(0, 1f);
		}
		else
		{
			sys.hidePlacePart();
		}
		heightAdjust = false;
		sys.redblock = false;
	}

	private IEnumerator heightAdjustDeploy(BuilderPart bp)
	{
		heightAdjust = true;
		bool place = false;
		float groundOffsetY2 = 0f;
		Vector3 pos = bp._transform.position;
		float max = 0f - bp.deploy.corners[0].localPosition.y;
		while (!Input.GetMouseButtonUp(0) && heightAdjust && (bool)bp)
		{
			groundOffsetY2 += UnityEngine.Input.GetAxis("Mouse Y") * 0.5f;
			groundOffsetY2 = Mathf.Clamp(groundOffsetY2, 0f, max);
			bp._transform.position = new Vector3(pos.x, pos.y + groundOffsetY2, pos.z);
			deployHeightCheckNew(bp.deploy);
			place = !sys.redblock;
			yield return null;
		}
		if (heightAdjust && place)
		{
			sys.placeObj(0, 1f);
		}
		else
		{
			sys.hidePlacePart();
		}
		heightAdjust = false;
		sys.redblock = false;
	}

	public void deployHeightCheck(Deploy deploy)
	{
		sys.redblock = false;
		if (placeOptions.ignoreRules || BuilderSystem.editMode)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < deploy.corners.Length)
			{
				Transform transform = deploy.corners[num];
				if ((bool)transform && Physics.Raycast(transform.position, Vector3.down, out hit, 50f, 256) && hit.distance > 0.7f)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		sys.redblock = true;
	}

	public void deployHeightCheckNew(Deploy deploy)
	{
		sys.redblock = false;
		if (placeOptions.ignoreRules || BuilderSystem.editMode)
		{
			return;
		}
		int num = 0;
		while (true)
		{
			if (num < deploy.corners.Length)
			{
				Transform transform = deploy.corners[num];
				if ((bool)transform && Physics.Raycast(transform.position, Vector3.down, out hit, 50f, 256) && hit.distance > 0.025f)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		sys.redblock = true;
	}

	public static void setLayer(int layer)
	{
		for (int num = terrains.Count - 1; num >= 0; num--)
		{
			if (terrains[num] == null)
			{
				terrains.Remove(terrains[num]);
			}
			else
			{
				terrains[num].gameObject.layer = layer;
			}
		}
	}

	public static void caveCheckAll()
	{
		for (int num = terrains.Count - 1; num >= 0; num--)
		{
			if (terrains[num] == null)
			{
				terrains.Remove(terrains[num]);
			}
			else
			{
				terrains[num].caveCheck();
			}
		}
	}

	public void caveCheck()
	{
		if (cave)
		{
			if (sys.BPinfo.name == "furnace_large")
			{
				caveblock = true;
			}
			else if (sys.BPinfo.name == "water_catcher")
			{
				caveblock = true;
			}
			else if (sys.BPinfo.name == "water_catcher_large")
			{
				caveblock = true;
			}
			else
			{
				caveblock = false;
			}
		}
	}

	private void OnDestory()
	{
		terrains.Remove(this);
	}
}
