using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Mirror : MonoBehaviour
{
	public static Mirror inst;

	private BuilderSystem sys;

	public Transform mirrorPlane;

	public List<BuilderPart> alignList;

	public Toggle mirrorTog;

	public Toggle positionTog;

	private bool startCheck;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
	}

	private void Update()
	{
		if (BuilderUI.inst.mouseOverUI || !Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out RaycastHit hitInfo, 150f))
		{
			return;
		}
		if (hitInfo.collider.tag == "edgeCollider")
		{
			mirrorPlane.position = hitInfo.transform.position;
			mirrorPlane.rotation = hitInfo.transform.rotation * Quaternion.Euler(Vector3.up * 90f);
		}
		else if (hitInfo.collider.tag == "terrain")
		{
			mirrorPlane.position = hitInfo.point;
			float axis = UnityEngine.Input.GetAxis("Mouse ScrollWheel");
			float num = 0f;
			if (axis > 0f)
			{
				num = -5f;
			}
			else if (axis < 0f)
			{
				num = 5f;
			}
			if (num != 0f)
			{
				mirrorPlane.Rotate(Vector3.up, num);
			}
		}
		if (Input.GetMouseButtonDown(0))
		{
			positionTog.isOn = false;
		}
	}

	public void mirror(bool copy)
	{
		if (sys.objList.Count < 1)
		{
			popUp.inst.message("selection needed");
			return;
		}
		Ray ray = new Ray(mirrorPlane.position, mirrorPlane.right);
		MGMT.inst.undoListInsert();
		for (int num = sys.objList.Count - 1; num >= 0; num--)
		{
			GameObject gameObject = sys.objList[num];
			BuilderPart component = gameObject.GetComponent<BuilderPart>();
			Transform transform = component._transform;
			if (copy)
			{
				sys.objList.Remove(gameObject);
				sys.changeSelection(gameObject, add: false);
				GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, transform.position, transform.rotation);
				gameObject2.name = gameObject.name;
				component = gameObject2.GetComponent<BuilderPart>();
				component.instId = 0;
				if ((bool)component.device)
				{
					component.device.clearConnectedTo();
				}
				transform = component._transform;
				sys.objList.Add(gameObject2);
				sys.changeSelection(gameObject2, add: true);
			}
			else
			{
				MGMT.inst.undoListList[0].Add(new MovedPart(component));
				if ((bool)component.device)
				{
					io[] outputTo = component.device.outputTo;
					for (int i = 0; i < outputTo.Length; i++)
					{
						outputTo[i].sendDisconnect(destroyed: false);
					}
					outputTo = component.device.inputFrom;
					for (int i = 0; i < outputTo.Length; i++)
					{
						outputTo[i].sendDisconnect(destroyed: false);
					}
				}
			}
			Vector3 vector = Vector3.Cross(new Vector3(transform.position.x, mirrorPlane.position.y, transform.position.z) - ray.origin, ray.direction);
			if (vector.y < 0f)
			{
				transform.Translate(mirrorPlane.forward * (vector.magnitude * 2f), Space.World);
			}
			else
			{
				transform.Translate(-mirrorPlane.forward * (vector.magnitude * 2f), Space.World);
			}
			if ((bool)component.block)
			{
				transform.rotation = Quaternion.LookRotation(Vector3.Reflect(-transform.forward, mirrorPlane.forward), Vector3.up);
			}
			else if ((bool)component.deploy && (component.deploy.frameFloor || component.deploy.frameWall || (bool)component.door || component.deploy.windowPart))
			{
				transform.rotation = Quaternion.LookRotation(Vector3.Reflect(-transform.forward, mirrorPlane.forward), Vector3.up);
			}
			else
			{
				transform.rotation = Quaternion.LookRotation(Vector3.Reflect(transform.rotation * Vector3.forward, mirrorPlane.forward), Vector3.Reflect(transform.rotation * Vector3.up, mirrorPlane.forward));
			}
			if (!copy)
			{
				component.moved();
			}
		}
		if (copy)
		{
			foreach (GameObject obj in sys.objList)
			{
				BuilderPart component2 = obj.GetComponent<BuilderPart>();
				sys.PlacedSetup(component2, send: true, notLoaded: false, sound: false, useCodeTog: false);
				MGMT.inst.undoListAddedPart(component2);
			}
		}
		sys.checkAllConditionals(clearNeighbors: true);
	}

	private IEnumerator WaitToCheck()
	{
		yield return null;
		foreach (GameObject obj in sys.objList)
		{
			sys.PlacedSetup(obj.GetComponent<BuilderPart>(), send: true, notLoaded: false, sound: false, useCodeTog: false);
		}
	}

	public void mirrorModeState()
	{
		if (!mirrorPlane.gameObject.activeSelf)
		{
			Symmetry.inst.symTog.isOn = false;
			if (!startCheck)
			{
				startCheck = true;
				positionTog.isOn = true;
			}
			mirrorPlane.gameObject.SetActive(value: true);
			popUp.inst.message("WARNING - wires not included");
		}
		else
		{
			mirrorPlane.gameObject.SetActive(value: false);
			if (positionTog.isOn)
			{
				positionTog.isOn = false;
			}
		}
	}

	private void OnEnable()
	{
		if (sys.inProgress)
		{
			base.enabled = false;
			return;
		}
		sys.inProgress = true;
		BuilderSystem.disableInput = true;
		popUp.inst.message("click to set mirror position");
		sys.SetPlaceColMode(off: false);
	}

	private void OnDisable()
	{
		sys.inProgress = false;
		BuilderSystem.disableInput = false;
		sys.SetPlaceColMode(off: true);
	}
}
