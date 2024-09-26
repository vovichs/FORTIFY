using UnityEngine;

public class RayPlace : MonoBehaviour
{
	public static bool noPlace;

	public static BuilderSystem cs;

	public bool shelf;

	public bool child;

	private Transform placeTF;

	private BuilderPart bp;

	private Deploy deploy;

	public static RaycastHit hit;

	public static Ray ray;

	private void OnMouseEnter()
	{
		enterCheck();
	}

	public void enterCheck()
	{
		if (bp == null)
		{
			bp = GetComponent<BuilderPart>();
		}
		if (noPlace || (bool)cs.BPinfo.block)
		{
			return;
		}
		deploy = cs.BPinfo.deploy;
		if (deploy.place == Deploy.Place.none || (deploy.place == Deploy.Place.snap && !placeOptions.ignoreRules))
		{
			return;
		}
		bool flag = true;
		placeTF = cs.BPinfo._transform;
		if (deploy.ground == Deploy.Ground.noAlign || deploy.ground == Deploy.Ground.heightAdjust)
		{
			placeTF.rotation = Quaternion.Euler(0f, placeTF.eulerAngles.y, 0f);
		}
		if ((bool)bp.block && bp.block.wall)
		{
			if (base.gameObject.name == "wall_doorway")
			{
				if (deploy.place == Deploy.Place.sign)
				{
					(bp.col as MeshCollider).convex = true;
				}
				if (cs.BPinfo.name == "vending_machine")
				{
					flag = false;
				}
			}
			if (cs.BPinfo.name == "fireplace")
			{
				flag = false;
			}
		}
		base.enabled = (flag || placeOptions.ignoreRules);
	}

	private void OnMouseExit()
	{
		if (!noPlace)
		{
			base.enabled = false;
			cs.hidePlacePart();
			if (base.gameObject.name == "wall_doorway" && !Stability.inst.raidMode && (bp.col as MeshCollider).convex)
			{
				(bp.col as MeshCollider).convex = false;
			}
		}
	}

	private void Update()
	{
		if (noPlace || !placeTF || !deploy)
		{
			base.enabled = false;
		}
		else
		{
			if (!BuilderSystem.placeMode || BuilderSystem.editMode || Stability.inst.raidMode || BuilderUI.inst.mouseOverUI)
			{
				return;
			}
			bool flag = false;
			ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
			if (!bp.col.Raycast(ray, out hit, 150f))
			{
				cs.hidePlacePart();
			}
			else
			{
				if (shelf && hit.point.y > bp._transform.position.y + 0.5f)
				{
					return;
				}
				placeTF.position = hit.point;
				if ((bp.found || bp.floor) && !bp.tri && !bp.block.frame && !bp.tri && cs.BPinfo.name == "planter")
				{
					Transform transform = bp._transform.Find("snapPoint");
					if (Vector3.Distance(hit.point, transform.position) < 0.05f)
					{
						cs.BPinfo._transform.SetPositionAndRotation(transform.position, transform.rotation);
					}
				}
				if (deploy.water == Deploy.Water.on && hit.point.y < 0f)
				{
					if ((bool)deploy.offset)
					{
						placeTF.Translate(placeTF.position - deploy.offset.position, Space.World);
					}
					else
					{
						placeTF.position = new Vector3(hit.point.x, -0.1f, hit.point.z);
					}
					placeTF.rotation = Quaternion.Euler(0f, placeTF.eulerAngles.y, 0f);
					flag = true;
				}
				else if (deploy.place == Deploy.Place.down)
				{
					Quaternion rot = Quaternion.FromToRotation(-placeTF.up, hit.normal) * placeTF.rotation;
					flag = angleCheck(rot);
					if (!flag)
					{
						cs.redblock = true;
						return;
					}
				}
				else
				{
					Vector3 placeDirection = getPlaceDirection(deploy, placeTF);
					if ((bool)deploy.offset)
					{
						placeTF.Translate(placeTF.position - deploy.offset.position, Space.World);
					}
					Quaternion quaternion = Quaternion.FromToRotation(placeDirection, hit.normal) * placeTF.rotation;
					if (deploy.vertical)
					{
						Vector3 eulerAngles = quaternion.eulerAngles;
						if (deploy.place == Deploy.Place.up && !deploy.offset)
						{
							if (Vector3.Dot(Vector3.up, hit.normal) == 0f)
							{
								quaternion.eulerAngles = new Vector3(90f, eulerAngles.y, eulerAngles.z);
							}
						}
						else
						{
							quaternion.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, 0f);
						}
					}
					placeTF.rotation = quaternion;
					flag = angleCheck(quaternion);
					if (!flag)
					{
						cs.redblock = true;
						return;
					}
				}
				if (!placeOptions.ignoreRules && (double)hit.point.y < -0.1 && deploy.water != Deploy.Water.on && deploy.water != Deploy.Water.under)
				{
					cs.redblock = true;
				}
				else if (flag)
				{
					cs.redblock = deploy.cornerCheck();
					if (Input.GetMouseButtonDown(0) && !BuilderUI.inst.mouseOverUI && !overlapCheck.overlap)
					{
						float lvl = (!child) ? bp.level : base.transform.parent.GetComponent<BuilderPart>().level;
						cs.placeObj(0, lvl);
						placeTF = cs.BPinfo._transform;
					}
				}
			}
		}
	}

	public static Vector3 getPlaceDirection(Deploy deploy, Transform t)
	{
		Vector3 result = t.up;
		if ((deploy.vertical && deploy.place != Deploy.Place.up) || deploy.place == Deploy.Place.backward)
		{
			result = -t.forward;
		}
		if (deploy.place == Deploy.Place.forward)
		{
			result = t.forward;
		}
		if ((bool)deploy.offset && deploy.place == Deploy.Place.up)
		{
			result = deploy.offset.up;
		}
		return result;
	}

	private bool angleCheck(Quaternion rot)
	{
		if (placeOptions.ignoreRules)
		{
			return true;
		}
		if (cs.BPinfo.info.angleLimit > 0f && Mathf.Acos(Mathf.Clamp(Vector3.Dot(Vector3.up, (rot * Vector3.up).normalized), -1f, 1f)) * 57.29578f >= cs.BPinfo.info.angleLimit)
		{
			return false;
		}
		return true;
	}
}
