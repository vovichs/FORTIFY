using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Symmetry : MonoBehaviour
{
	public static Symmetry inst;

	private BuilderSystem sys;

	public bool isOn;

	public bool centerSet;

	public float angle = 90f;

	public int addNum = 3;

	public float[] angles;

	public List<BuilderPart> groupList;

	public GameObject panel;

	public Transform symTransform;

	public Toggle symTog;

	public Toggle setCenterTog;

	public Toggle deployTog;

	private bool wait;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		panel.SetActive(value: false);
		setPanelMode();
	}

	private void Update()
	{
		if (BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		Ray ray = CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition);
		BuilderPart builderPart = null;
		if (!Physics.Raycast(ray, out RaycastHit hitInfo, 150f))
		{
			return;
		}
		string tag = hitInfo.collider.tag;
		if (tag == "edgeCollider")
		{
			builderPart = hitInfo.transform.root.GetComponent<BuilderPart>();
			Transform transform = hitInfo.transform;
			symTransform.position = transform.position;
			if (angle != 180f)
			{
				symTransform.rotation = transform.rotation * Quaternion.Euler(Vector3.up * 90f);
			}
			else
			{
				symTransform.rotation = transform.rotation;
			}
			Vector3 lhs = hitInfo.point - transform.position;
			Vector3 a = symTransform.InverseTransformDirection(transform.forward);
			if (lhs.magnitude > 0.2f)
			{
				if (Vector3.Dot(lhs, transform.forward) > 0f)
				{
					symTransform.Translate(a * 0.5f, Space.Self);
				}
				else
				{
					symTransform.Translate(a * -0.5f, Space.Self);
				}
			}
		}
		else if (tag == "block")
		{
			rotateSymmetry(overBlock: true);
			builderPart = hitInfo.transform.GetComponent<BuilderPart>();
			Vector3 boundsCenter = builderPart.block.getBoundsCenter();
			if (Vector3.Distance(boundsCenter, hitInfo.point) < 0.6f)
			{
				symTransform.SetPositionAndRotation(boundsCenter, builderPart._transform.rotation);
			}
		}
		else if (tag == "terrain")
		{
			symTransform.position = hitInfo.point;
			rotateSymmetry(overBlock: false);
		}
		if (Input.GetMouseButtonDown(0))
		{
			centerSet = true;
			setCenterTog.isOn = false;
			Vector3 position = symTransform.position;
			position.y = Terrain.activeTerrain.SampleHeight(symTransform.position) + Terrain.activeTerrain.transform.position.y;
			symTransform.position = position;
			BuilderSystem.inst.rpOverCheck();
		}
	}

	public IEnumerator setupGroup(List<BuilderPart> addList)
	{
		if (!isOn)
		{
			yield break;
		}
		yield return new WaitForFixedUpdate();
		BuilderPart[] array = addList.ToArray();
		int count = addList.Count;
		for (int i = 0; i < count; i++)
		{
			BuilderPart builderPart = addList[i];
			if (!builderPart)
			{
				continue;
			}
			builderPart.check = false;
			if (!builderPart.sg)
			{
				builderPart.sg = builderPart.gameObject.AddComponent<symmetryGroup>();
			}
			if (!builderPart.sg)
			{
				builderPart.sg = builderPart.gameObject.GetComponent<symmetryGroup>();
			}
			builderPart.sg.group = (BuilderPart[])array.Clone();
			groupList.Add(builderPart);
			for (int j = 0; j < count; j++)
			{
				if (builderPart.sg.group[j] == builderPart)
				{
					builderPart.sg.group[j] = null;
					break;
				}
			}
		}
	}

	public void selectionAddSymmetry()
	{
		if (wait)
		{
			return;
		}
		StartCoroutine(startWait());
		if (!BuilderSystem.editMode)
		{
			popUp.inst.message("edit mode selection needed");
		}
		else if (BuilderSystem.inst.objList.Count == 0)
		{
			popUp.inst.message("nothing selected");
		}
		else
		{
			if (base.enabled)
			{
				return;
			}
			MGMT.inst.undoListInsert();
			bool flag = deployTog.isOn;
			for (int i = 0; i < BuilderSystem.inst.objList.Count; i++)
			{
				if (!(BuilderSystem.inst.objList[i] == null))
				{
					BuilderSystem.inst.objList[i].GetComponent<BuilderPart>().removeSymmetry();
				}
			}
			for (int j = 0; j < BuilderSystem.inst.objList.Count; j++)
			{
				if (!(BuilderSystem.inst.objList[j] == null))
				{
					BuilderPart component = BuilderSystem.inst.objList[j].GetComponent<BuilderPart>();
					if ((!component.deploy || flag) && !component.sg && !component.check)
					{
						BuilderSystem.inst.AddSymmetry(component, addSelection: true);
					}
				}
			}
		}
	}

	private void rotateSymmetry(bool overBlock)
	{
		float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
		int num = 5;
		if (overBlock)
		{
			num = 90;
		}
		float num2 = 0f;
		if (axis > 0f)
		{
			num2 = -num;
		}
		else if (axis < 0f)
		{
			num2 = num;
		}
		if (num2 != 0f)
		{
			symTransform.Rotate(Vector3.up, num2);
		}
	}

	public void modeState()
	{
		isOn = !isOn;
		if (isOn)
		{
			if (!centerSet)
			{
				setCenterTog.isOn = true;
			}
			else
			{
				CameraCtrl.lookAt(symTransform.position);
				CameraCtrl.setRotation();
			}
			panel.SetActive(value: true);
			symTransform.gameObject.SetActive(value: true);
		}
		else
		{
			if (setCenterTog.isOn)
			{
				setCenterTog.isOn = false;
			}
			symTransform.gameObject.SetActive(value: false);
			panel.SetActive(value: false);
		}
	}

	public void setCenterState()
	{
		if (setCenterTog.isOn)
		{
			if (groupList.Count > 0)
			{
				clearGroupList();
			}
			base.enabled = true;
			if (sys.inProgress)
			{
				base.enabled = false;
				setCenterTog.SetIsOnWithoutNotify(value: false);
				return;
			}
			BuilderSystem.inst.SetPlaceColMode(off: false);
			sys.inProgress = true;
			RayPlace.noPlace = true;
			BuilderSystem.disableInput = true;
			popUp.inst.message("click to set center");
			popUp.inst.message("snaps to blocks");
			sys.SetPlaceColMode(off: false);
		}
		else
		{
			base.enabled = false;
			sys.inProgress = false;
			BuilderSystem.disableInput = false;
			if (!BuilderSystem.editMode)
			{
				sys.SetPlaceColMode(off: false);
				RayPlace.noPlace = false;
			}
		}
	}

	public void changeSymType(int index)
	{
		if (angle != angles[index])
		{
			angle = angles[index];
			addNum = (int)(360f / angle) - 1;
			clearGroupList();
			int childCount = symTransform.childCount;
			for (int i = 0; i < childCount; i++)
			{
				symTransform.GetChild(i).gameObject.SetActive(i == index);
			}
		}
	}

	public int getSymType()
	{
		int childCount = symTransform.childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (symTransform.GetChild(i).gameObject.activeSelf)
			{
				return i;
			}
		}
		return 1;
	}

	public void selectionFilter()
	{
		if (!isOn)
		{
			return;
		}
		List<GameObject> list = new List<GameObject>(BuilderSystem.inst.objList);
		for (int i = 0; i < list.Count; i++)
		{
			if (list[i] == null)
			{
				continue;
			}
			BuilderPart component = list[i].GetComponent<BuilderPart>();
			if (!component.sg || !component.selected)
			{
				continue;
			}
			BuilderPart[] group = component.sg.group;
			foreach (BuilderPart builderPart in group)
			{
				if ((bool)builderPart && builderPart.selected)
				{
					BuilderSystem.inst.objList.Remove(builderPart.gameObject);
					BuilderSystem.inst.changeSelection(builderPart, add: false);
				}
			}
		}
	}

	public bool selectionCheck()
	{
		if (!isOn)
		{
			return true;
		}
		for (int i = 0; i < BuilderSystem.inst.objList.Count; i++)
		{
			if (BuilderSystem.inst.objList[i] == null)
			{
				continue;
			}
			BuilderPart component = BuilderSystem.inst.objList[i].GetComponent<BuilderPart>();
			if (!component.sg || !component.selected)
			{
				continue;
			}
			BuilderPart[] group = component.sg.group;
			foreach (BuilderPart builderPart in group)
			{
				if ((bool)builderPart && builderPart.selected)
				{
					popUp.inst.message("can't include mulitple symmetry sides");
					return false;
				}
			}
		}
		return true;
	}

	private void clearGroupList()
	{
		for (int i = 0; i < groupList.Count; i++)
		{
			if (!(groupList[i] == null))
			{
				UnityEngine.Object.Destroy(groupList[i].sg);
				groupList[i].removeSymmetry();
			}
		}
		groupList.Clear();
	}

	public void deployStateChange()
	{
		if (deployTog.isOn)
		{
			return;
		}
		for (int i = 0; i < groupList.Count; i++)
		{
			if (!(groupList[i] == null) && !groupList[i].block)
			{
				UnityEngine.Object.Destroy(groupList[i].sg);
				groupList[i].removeSymmetry();
			}
		}
	}

	public void setPanelMode()
	{
		RectTransform component = panel.GetComponent<RectTransform>();
		if (BuilderSystem.placeMode)
		{
			panel.transform.GetChild(0).gameObject.SetActive(value: true);
			component.anchoredPosition = new Vector2(-16.48f, component.anchoredPosition.y);
		}
		else
		{
			panel.transform.GetChild(0).gameObject.SetActive(value: false);
			component.anchoredPosition = new Vector2(0f, component.anchoredPosition.y);
		}
	}

	private Vector2 XZ(Vector3 v3)
	{
		return new Vector2(v3.x, v3.z);
	}

	private IEnumerator startWait()
	{
		wait = true;
		yield return new WaitForSeconds(2f);
		wait = false;
	}
}
