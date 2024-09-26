using System.Collections.Generic;
using UnityEngine;

public class getStructureIds : MonoBehaviour
{
	private BuilderSystem cs;

	public LayerMask usedMask;

	public List<BuilderPart> founders;

	public List<BuilderPart> deployables;

	public List<BuilderPart> idGroup = new List<BuilderPart>();

	public int lastId;

	private void Awake()
	{
		cs = BuilderSystem.inst;
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0) && Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 150f, 1))
		{
			int strucId = hitInfo.transform.GetComponent<BuilderPart>().strucId;
			if (strucId != 0 && strucId != lastId)
			{
				lastId = strucId;
				idGroup.Clear();
				foreach (BuilderPart bp in cs.bpList)
				{
					if (bp.found && bp.strucId == strucId)
					{
						idGroup.Add(bp);
					}
				}
				selectStructure();
			}
		}
	}

	private void selectStructure()
	{
		foreach (BuilderPart item in idGroup)
		{
			cs.objList.Add(item.gameObject);
			cs.changeSelection(item, add: true);
		}
	}

	[ContextMenu("getIds")]
	public void getIds()
	{
		founders.Clear();
		foreach (BuilderPart bp in cs.bpList)
		{
			bp.strucId = 0;
			if ((bool)bp.block)
			{
				if (bp.found && bp.block.edgeCols != null)
				{
					founders.Add(bp);
				}
			}
			else if (bp.deploy.windowPart || bp.deploy.frameWall || bp.deploy.frameFloor || (bool)bp.door)
			{
				deployables.Add(bp);
			}
		}
		int num = 0;
		foreach (BuilderPart founder in founders)
		{
			if (founder.strucId <= 0)
			{
				List<Transform> list = new List<Transform>(founder.block.edgeCols);
				List<BuilderPart> list2 = new List<BuilderPart>();
				List<Transform> list3 = new List<Transform>();
				int num2 = 0;
				num = (founder.strucId = num + 1);
				founder.gameObject.layer = 2;
				list2.Add(founder);
				while (list != null && list.Count > 0)
				{
					foreach (Transform item in list)
					{
						Collider[] array = Physics.OverlapSphere(item.position, 0.01f, usedMask);
						for (int i = 0; i < array.Length; i++)
						{
							BuilderPart component = array[i].gameObject.GetComponent<BuilderPart>();
							if ((bool)component.block)
							{
								if (component.strucId == 0)
								{
									list2.Add(component);
									component.gameObject.layer = 2;
									component.strucId = num;
									if (component.wall)
									{
										list3.Add(component.block.edgeCols[0]);
									}
									else
									{
										list3.AddRange(component.block.edgeCols);
									}
								}
								else if (component.strucId != num)
								{
									num2 = component.strucId;
								}
							}
						}
					}
					list = new List<Transform>(list3);
					list3.Clear();
				}
				foreach (BuilderPart item2 in list2)
				{
					item2.gameObject.layer = 0;
					if (num2 > 0)
					{
						item2.strucId = num2;
					}
				}
				deployCheck();
			}
		}
	}

	private void deployCheck()
	{
		foreach (BuilderPart deployable in deployables)
		{
			deployable.gameObject.layer = 2;
			Collider[] array = Physics.OverlapSphere(deployable._transform.position, 0.03f, usedMask);
			for (int i = 0; i < array.Length; i++)
			{
				BuilderPart component = array[i].gameObject.GetComponent<BuilderPart>();
				if (!(component == null) && (bool)component.block)
				{
					deployable.strucId = component.strucId;
				}
			}
			deployable.setRaycastLayer();
		}
	}

	private bool connectCheck(Vector3 pos, block b)
	{
		Transform[] edgeCols = b.edgeCols;
		foreach (Transform transform in edgeCols)
		{
			if (tools.FlatDistance(pos, transform.position) < 0.005f)
			{
				return true;
			}
		}
		return false;
	}
}
