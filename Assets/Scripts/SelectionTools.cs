using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SelectionTools : MonoBehaviour
{
	public enum SelectMode
	{
		connect,
		scene,
		height
	}

	private BuilderSystem sys;

	public List<BuilderPart> connected;

	public List<BuilderPart> ThisLevel;

	public List<BuilderPart> NextLevel;

	public List<BuilderPart> floors;

	public List<GameObject> deployables;

	public LayerMask usedMask;

	public EventTrigger structET;

	private bool addStruct = true;

	public Toggle upTog;

	public Toggle connectTog;

	public Toggle structTog;

	public Toggle deployTog;

	private string partType;

	public Text mainText;

	public SelectMode selectMode;

	private void Awake()
	{
		sys = BuilderSystem.inst;
	}

	private void Update()
	{
		if (!BuilderSystem.editMode)
		{
			structTog.isOn = false;
		}
		else
		{
			RaycastHit hitInfo;
			if (!Input.GetMouseButtonDown(0) || BuilderUI.inst.mouseOverUI || !Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hitInfo, 150f) || hitInfo.transform.gameObject.layer != 0)
			{
				return;
			}
			BuilderPart component = hitInfo.transform.GetComponent<BuilderPart>();
			if ((bool)component.block)
			{
				cupboardRange.inst.getStructureGroups(deployTog.isOn);
				int strucId = component.strucId;
				if (addStruct)
				{
					foreach (BuilderPart bp in sys.bpList)
					{
						if (bp.strucId == strucId && !bp.selected)
						{
							sys.objList.Add(bp.gameObject);
							sys.changeSelection(bp, add: true);
						}
					}
				}
				else
				{
					foreach (BuilderPart bp2 in sys.bpList)
					{
						if (bp2.strucId == strucId && bp2.selected)
						{
							sys.objList.Remove(bp2.gameObject);
							sys.changeSelection(bp2, add: false);
						}
					}
				}
			}
		}
	}

	public void structMode()
	{
		if (structTog.isOn)
		{
			base.enabled = true;
			structET.enabled = false;
		}
		else
		{
			base.enabled = false;
			structET.enabled = true;
		}
	}

	public void structModeMode(bool state)
	{
		addStruct = state;
	}

	public void LevelSelection(int mode)
	{
		if (sys.objList.Count < 1)
		{
			popUp.inst.message("selection needed");
			return;
		}
		if (mode == 4)
		{
			partType = sys.objList[0].name;
		}
		bool isOn = upTog.isOn;
		if (selectMode == SelectMode.scene)
		{
			sys.objList2.AddRange(sys.bpList);
		}
		else if (selectMode == SelectMode.height)
		{
			foreach (BuilderPart bp in sys.bpList)
			{
				ThisLevel.Add(bp);
			}
			foreach (GameObject obj in sys.objList)
			{
				float y = obj.transform.position.y;
				for (int num = ThisLevel.Count - 1; num >= 0; num--)
				{
					if (isOn)
					{
						if ((double)ThisLevel[num]._transform.position.y >= (double)y - 0.01)
						{
							sys.objList2.Add(ThisLevel[num]);
							ThisLevel.RemoveAt(num);
						}
					}
					else
					{
						float num2 = ThisLevel[num]._transform.position.y - y;
						if ((double)num2 <= 0.6 && (double)num2 > -0.01)
						{
							sys.objList2.Add(ThisLevel[num]);
							ThisLevel.RemoveAt(num);
						}
					}
				}
			}
		}
		else if (selectMode == SelectMode.connect)
		{
			bool flag = mode == 1 && deployTog.isOn;
			cupboardRange.inst.getStructureGroups(flag);
			List<int> list = new List<int>();
			List<float> list2 = new List<float>();
			float num3 = 1000f;
			for (int i = 0; i < sys.objList.Count; i++)
			{
				BuilderPart component = sys.objList[i].GetComponent<BuilderPart>();
				if ((bool)component.deploy)
				{
					sys.objList2.Add(component);
					continue;
				}
				if (!list.Contains(component.strucId))
				{
					list.Add(component.strucId);
				}
				if (isOn)
				{
					if (component.level < num3)
					{
						num3 = component.level;
					}
				}
				else if (!list2.Contains(component.level))
				{
					list2.Add(component.level);
				}
				sys.objList2.Add(component);
			}
			foreach (BuilderPart bp2 in sys.bpList)
			{
				if (!bp2.selected && (!bp2.deploy || flag))
				{
					bool flag2 = false;
					foreach (int item in list)
					{
						if (bp2.strucId == item)
						{
							flag2 = true;
							break;
						}
					}
					if (flag2)
					{
						if (isOn)
						{
							if (bp2.level >= num3)
							{
								sys.objList2.Add(bp2);
							}
						}
						else
						{
							foreach (float item2 in list2)
							{
								if (bp2.level == item2)
								{
									sys.objList2.Add(bp2);
									break;
								}
							}
						}
					}
				}
			}
		}
		sys.objList.Clear();
		ThisLevel.Clear();
		FilterResults(mode);
	}

	private void FilterResults(int mode)
	{
		foreach (BuilderPart item in sys.objList2)
		{
			GameObject gameObject = item.gameObject;
			if (!item.hidden)
			{
				if (mode > 1)
				{
					bool flag = false;
					if (mode == 2)
					{
						if (item.floor || item.found)
						{
							flag = true;
						}
					}
					else if (mode == 3 && item.wall)
					{
						flag = true;
					}
					else if (mode == 4 && gameObject.name.Length == partType.Length && gameObject.name == partType)
					{
						flag = true;
					}
					if (!flag)
					{
						sys.changeSelection(item, add: false);
						continue;
					}
				}
				sys.objList.Add(gameObject);
				sys.changeSelection(item, add: true);
			}
		}
		sys.objList2.Clear();
	}

	private void deployCheck()
	{
		foreach (BuilderPart bp in sys.bpList)
		{
			GameObject gameObject = bp.gameObject;
			if (!bp.hidden && (bool)bp.deploy)
			{
				Vector3 vector = bp._transform.position;
				Vector3 direction = Vector3.up;
				if (bp.deploy.frameFloor)
				{
					direction = -bp._transform.right;
				}
				else if (bp.deploy.frameWall || (bool)bp.door)
				{
					vector += new Vector3(0f, 0.7f, 0f);
				}
				else if (!bp.deploy.windowPart)
				{
					continue;
				}
				gameObject.layer = 2;
				if (Physics.Raycast(vector, direction, out RaycastHit hitInfo, 0.5f, usedMask))
				{
					BuilderPart component = hitInfo.transform.GetComponent<BuilderPart>();
					if (component == null)
					{
						continue;
					}
					if (component.selected)
					{
						sys.changeSelection(gameObject, add: true);
						sys.objList.Add(gameObject);
					}
					else if (bp.selected)
					{
						sys.changeSelection(bp, add: false);
						sys.objList.Remove(gameObject);
					}
				}
				else if (bp.selected)
				{
					sys.changeSelection(bp, add: false);
					sys.objList.Remove(gameObject);
				}
				bp.setRaycastLayer();
			}
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

	public void selectModeChange(int index)
	{
		selectMode = (SelectMode)index;
		mainTextChange();
	}

	public void mainTextChange()
	{
		if (selectMode == SelectMode.scene)
		{
			mainText.text = "scene selection";
		}
		else if (selectMode == SelectMode.height)
		{
			mainText.text = "height selection";
		}
		else
		{
			mainText.text = "level selection";
		}
	}
}
