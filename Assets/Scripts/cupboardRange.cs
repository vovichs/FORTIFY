using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cupboardRange : MonoBehaviour
{
	private BuilderSystem sys;

	public static cupboardRange inst;

	public int mode;

	public bool show;

	private bool rangesCalc;

	private int strucId;

	private int matColorIndex;

	public List<rangeCircle> rangeList = new List<rangeCircle>();

	private List<BuilderPart> rangeParts = new List<BuilderPart>();

	private List<int> strucIdList = new List<int>();

	public Toggle tog;

	public Toggle showTog;

	public Toggle hideTog;

	private static bool start;

	public Image image1;

	public Image image2;

	public Material[] rangeMats;

	public Material[] rangeMats2d;

	public Color[] rangeColors;

	public LayerMask blockMask;

	public LayerMask groundMask;

	private Coroutine showCR;

	public GameObject cupboardRange3D;

	public GameObject cupboardRange2D;

	private Vector3 scaledUp = new Vector3(1.09f, 1.09f, 1.09f);

	private List<BuilderPart> founders = new List<BuilderPart>();

	private List<BuilderPart> deployables = new List<BuilderPart>();

	private RaycastHit hit;

	private Vector3 fireFrom;

	private int MatColorIndex
	{
		get
		{
			if (matColorIndex == 4)
			{
				matColorIndex = 0;
			}
			else
			{
				matColorIndex++;
			}
			return matColorIndex;
		}
	}

	public bool Show
	{
		get
		{
			return show;
		}
		set
		{
			show = value;
		}
	}

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
	}

	private void Update()
	{
		if (!Input.GetMouseButtonDown(0) || rangesCalc || BuilderUI.inst.mouseOverUI || !Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hit, 100f, 1))
		{
			return;
		}
		BuilderPart component;
		if (hit.transform.tag == "edgeCollider")
		{
			component = hit.transform.parent.GetComponent<BuilderPart>();
		}
		else
		{
			component = hit.transform.GetComponent<BuilderPart>();
			if ((bool)component.deploy)
			{
				return;
			}
		}
		int num = component.strucId;
		if (show)
		{
			foreach (BuilderPart bp in sys.bpList)
			{
				if (bp.found && bp.strucId == num && !rangeParts.Contains(bp))
				{
					rangeParts.Add(bp);
				}
			}
			showRanges(num, -1);
		}
		else
		{
			hideRanges(num);
		}
		if (showTog.isOn)
		{
			showTog.isOn = false;
		}
		if (hideTog.isOn)
		{
			hideTog.isOn = false;
		}
	}

	public void getStructureGroups(bool getDeploys)
	{
		foreach (BuilderPart bp in sys.bpList)
		{
			bp.strucId = 0;
			if (getDeploys && (bool)bp.deploy)
			{
				deployables.Add(bp);
			}
			if (bp.found)
			{
				founders.Add(bp);
			}
		}
		foreach (BuilderPart founder in founders)
		{
			if (founder.strucId <= 0)
			{
				List<block> list = new List<block>
				{
					founder.block
				};
				List<block> list2 = new List<block>
				{
					founder.block
				};
				List<block> list3 = new List<block>();
				strucId++;
				founder.strucId = strucId;
				int num = 0;
				while (list2 != null && list2.Count > 0)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						block block = list2[i];
						for (int j = 0; j < block.sockets.Length; j++)
						{
							socket socket = block.sockets[j];
							if (socket.female)
							{
								for (int k = 0; k < socket.connections.Count; k++)
								{
									block owner = socket.connections[k].owner;
									if (owner.owner.strucId == 0)
									{
										list3.Add(owner);
										owner.owner.strucId = strucId;
									}
									else if (owner.owner.strucId != strucId)
									{
										num = owner.owner.strucId;
									}
								}
							}
						}
					}
					list2 = new List<block>(list3);
					list.AddRange(list3);
					list3.Clear();
				}
				if (num > 0)
				{
					foreach (block item in list)
					{
						item.owner.strucId = num;
					}
				}
			}
		}
		if (getDeploys)
		{
			deployCheck();
		}
		deployables.Clear();
		founders.Clear();
	}

	public void rangeToggle()
	{
		if (tog.isOn)
		{
			if (start)
			{
				popUp.inst.message("select show range");
				popUp.inst.message("then click a structure");
				start = true;
			}
			showAllRanges();
			return;
		}
		if (showTog.isOn)
		{
			showTog.isOn = false;
		}
		if (hideTog.isOn)
		{
			hideTog.isOn = false;
		}
		if (!rangesCalc)
		{
			hideAllRanges();
		}
	}

	public void selectToggle()
	{
		if (showTog.isOn || hideTog.isOn)
		{
			RayPlace.noPlace = true;
			base.enabled = true;
			getStructureGroups(getDeploys: false);
			return;
		}
		base.enabled = false;
		if (!wiring.inst.wireTog.isOn && !BuilderSystem.editMode)
		{
			RayPlace.noPlace = false;
		}
	}

	public void changeMode()
	{
		Sprite sprite = image1.sprite;
		Sprite sprite2 = image2.sprite;
		image1.sprite = sprite2;
		image2.sprite = sprite;
		if (mode == 1)
		{
			mode = 0;
			popUp.inst.message("building privilege range");
			for (int num = rangeList.Count - 1; num >= 0; num--)
			{
				rangeList[num]._transform.localScale = Vector3.one;
			}
		}
		else
		{
			mode = 1;
			popUp.inst.message("TC stacking block range");
			for (int num2 = rangeList.Count - 1; num2 >= 0; num2--)
			{
				rangeList[num2]._transform.localScale = scaledUp;
			}
		}
	}

	private void deployCheck()
	{
		Dictionary<BuilderPart, BuilderPart> dictionary = new Dictionary<BuilderPart, BuilderPart>();
		foreach (BuilderPart deployable in deployables)
		{
			if ((bool)deployable)
			{
				Collider[] array = Physics.OverlapSphere(deployable._transform.position, 0.02f, 1);
				for (int i = 0; i < array.Length; i++)
				{
					BuilderPart component = array[i].GetComponent<BuilderPart>();
					if (!(component == null) && !(component == deployable))
					{
						if ((bool)component.block)
						{
							deployable.strucId = component.strucId;
						}
						else if (component.strucId == 0)
						{
							dictionary.Add(deployable, component);
						}
						else
						{
							deployable.strucId = component.strucId;
						}
						break;
					}
				}
			}
		}
		foreach (KeyValuePair<BuilderPart, BuilderPart> item in dictionary)
		{
			item.Key.strucId = item.Value.strucId;
		}
	}

	private void showRanges(int id, int matIndex)
	{
		rangesCalc = true;
		int num = 0;
		num = ((matIndex <= 0) ? MatColorIndex : matIndex);
		float num2 = 0.01f * (float)num;
		bool flag = false;
		if (sys.currentScene.buildIndex == 1 || sys.currentScene.buildIndex == 5)
		{
			flag = true;
		}
		foreach (BuilderPart rangePart in rangeParts)
		{
			if ((bool)rangePart && rangePart.strucId == id)
			{
				bool flag2 = false;
				for (int i = 0; i < rangePart.block.baseCols.Length; i++)
				{
					if (rangePart.block.baseCols[i].activeSelf)
					{
						flag2 = true;
						break;
					}
				}
				if (flag2)
				{
					if ((bool)rangePart.sObj)
					{
						rangePart.sObj.SetActive(value: true);
					}
					else
					{
						Vector3 position = rangePart.origin.position;
						rangeCircle component;
						if (flag)
						{
							component = UnityEngine.Object.Instantiate(cupboardRange3D, position, Quaternion.identity).GetComponent<rangeCircle>();
							component.rend.sharedMaterial = rangeMats[num];
							component.matIndex = num;
							if (mode == 1)
							{
								component._transform.localScale = scaledUp;
							}
							updateVerts(component, num2);
						}
						else
						{
							float y = 0.05f + num2;
							component = UnityEngine.Object.Instantiate(cupboardRange2D, new Vector3(position.x, y, position.z), Quaternion.identity).GetComponent<rangeCircle>();
							component.rend.sharedMaterial = rangeMats2d[num];
							component.matIndex = num;
							if (mode == 1)
							{
								component._transform.localScale = scaledUp;
							}
						}
						rangePart.sObj = component.gameObject;
						component.bp = rangePart;
						rangeList.Add(component);
					}
				}
				else if ((bool)rangePart.sObj)
				{
					rangeCircle component2 = rangePart.sObj.GetComponent<rangeCircle>();
					rangeList.Remove(component2);
					component2.destroy();
				}
			}
		}
		rangesCalc = false;
	}

	private void hideRanges(int id)
	{
		foreach (BuilderPart rangePart in rangeParts)
		{
			if (rangePart.strucId == id && (bool)rangePart.sObj)
			{
				rangeCircle component = rangePart.sObj.GetComponent<rangeCircle>();
				rangeList.Remove(component);
				component.destroy();
			}
		}
	}

	public void hideAllRanges()
	{
		for (int num = rangeList.Count - 1; num >= 0; num--)
		{
			rangeCircle rangeCircle = rangeList[num];
			if (!rangeCircle)
			{
				rangeList.RemoveAt(num);
			}
			else
			{
				rangeCircle.gameObject.SetActive(value: false);
			}
		}
	}

	public void showAllRanges()
	{
		for (int num = rangeList.Count - 1; num >= 0; num--)
		{
			rangeCircle rangeCircle = rangeList[num];
			if ((bool)rangeCircle)
			{
				if (!rangeCircle.bp)
				{
					rangeList.RemoveAt(num);
					rangeCircle.destroy();
				}
				rangeCircle.gameObject.SetActive(value: true);
			}
		}
	}

	public void removeRanges()
	{
		for (int i = 0; i < rangeList.Count; i++)
		{
			rangeList[i].destroy();
		}
		rangeList.Clear();
	}

	[ContextMenu("update ranges")]
	public void updateRanges()
	{
		for (int num = rangeList.Count - 1; num >= 0; num--)
		{
			if (!rangeList[num].bp)
			{
				rangeList[num].destroy();
				rangeList.RemoveAt(num);
			}
		}
		getStructureGroups(getDeploys: false);
		strucIdList.Clear();
		List<int> list = new List<int>();
		for (int num2 = rangeParts.Count - 1; num2 >= 0; num2--)
		{
			BuilderPart builderPart = rangeParts[num2];
			if (!builderPart)
			{
				rangeParts.RemoveAt(num2);
			}
			else if ((bool)builderPart.sObj && builderPart.sObj.activeSelf && !strucIdList.Contains(builderPart.strucId))
			{
				strucIdList.Add(builderPart.strucId);
				list.Add(builderPart.sObj.GetComponent<rangeCircle>().matIndex);
			}
		}
		foreach (BuilderPart bp in sys.bpList)
		{
			if (bp.found)
			{
				foreach (int strucId2 in strucIdList)
				{
					if (bp.strucId == strucId2 && !rangeParts.Contains(bp))
					{
						rangeParts.Add(bp);
						break;
					}
				}
			}
		}
		for (int i = 0; i < strucIdList.Count; i++)
		{
			showRanges(strucIdList[i], list[i]);
		}
	}

	public void updateVerts(rangeCircle circle, float yOffset)
	{
		Mesh mesh = circle.meshFilter.mesh;
		Vector3[] vertices = mesh.vertices;
		Vector3 a = circle._transform.position + new Vector3(0f, 10f, 0f);
		for (int i = 0; i < vertices.Length; i++)
		{
			fireFrom = a - vertices[i];
			if (Physics.Raycast(fireFrom, Vector3.down, out hit, 20f, groundMask))
			{
				Vector3 position = hit.point + new Vector3(0f, 0.15f + yOffset, 0f);
				vertices[i] = circle._transform.InverseTransformPoint(position);
			}
		}
		mesh.vertices = vertices;
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

	private void selectStructure()
	{
		foreach (BuilderPart rangePart in rangeParts)
		{
			sys.objList.Add(rangePart.gameObject);
			sys.changeSelection(rangePart, add: true);
		}
	}

	private void selectStructureDeploys()
	{
		foreach (BuilderPart deployable in deployables)
		{
			deployable.gameObject.layer = 2;
			Collider[] array = Physics.OverlapSphere(deployable._transform.position, 0.03f, 1);
			for (int i = 0; i < array.Length; i++)
			{
				BuilderPart component = array[i].GetComponent<BuilderPart>();
				if (component != null && (bool)component.block)
				{
					deployable.strucId = component.strucId;
					break;
				}
			}
			deployable.setRaycastLayer();
		}
	}
}
