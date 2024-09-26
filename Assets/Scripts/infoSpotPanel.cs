using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class infoSpotPanel : MonoBehaviour
{
	public static infoSpotPanel inst;

	public InputField infoField;

	public GameObject infoPanel;

	public Button reposTog;

	public CanvasGroup cg;

	public RawImage icon;

	public static infoSpot activeSpot;

	public GameObject spotPrefab;

	public List<infoSpot> spots;

	public bool show = true;

	private void Awake()
	{
		inst = this;
		infoPanel = base.gameObject;
		infoField = base.transform.GetChild(0).GetComponent<InputField>();
		infoPanel.SetActive(value: false);
		reposTog.image.enabled = false;
	}

	public void addSpot()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(spotPrefab);
		activeSpot = gameObject.GetComponent<infoSpot>();
		spots.Add(activeSpot);
		gameObject.transform.SetParent(base.transform.parent);
		activeSpot.savedPos = BuilderSystem.inst._transform.position;
		activeSpot.savedRot = BuilderSystem.inst._transform.rotation;
		activeSpot.moveTo(justAdded: true);
	}

	public void delete()
	{
		spots.Remove(activeSpot);
		UnityEngine.Object.Destroy(activeSpot);
		activeSpot = null;
		infoField.text = "";
		infoPanel.SetActive(value: false);
	}

	public void exit()
	{
		activeSpot.icon.uvRect = new Rect(0f, 0.5f, 0.5f, 0.5f);
		activeSpot.infoString = infoField.text;
		infoField.text = "";
		if (reposTog.image.isActiveAndEnabled)
		{
			repositionSave(camCtrl: true);
		}
		infoPanel.SetActive(value: false);
	}

	public void reposition()
	{
		if (!reposTog.image.isActiveAndEnabled)
		{
			CameraCtrl.inst.enabled = true;
			reposTog.image.enabled = true;
		}
		else
		{
			repositionSave(camCtrl: false);
		}
	}

	public void repositionSave(bool camCtrl)
	{
		reposTog.image.enabled = false;
		if (activeSpot != null)
		{
			activeSpot.savedPos = BuilderSystem.inst._transform.position;
			activeSpot.savedRot = BuilderSystem.inst._transform.rotation;
		}
		CameraCtrl.inst.enabled = camCtrl;
	}

	public void nextSpot(bool up)
	{
		if (spots.Count <= 1)
		{
			return;
		}
		if (reposTog.image.isActiveAndEnabled)
		{
			repositionSave(camCtrl: false);
		}
		activeSpot.infoString = infoField.text;
		int num = spots.IndexOf(activeSpot);
		if (up)
		{
			num++;
			if (num < spots.Count)
			{
				activeSpot = spots[num];
			}
			else
			{
				activeSpot = spots[0];
			}
		}
		else
		{
			num--;
			if (num >= 0)
			{
				activeSpot = spots[num];
			}
			else
			{
				activeSpot = spots[spots.Count - 1];
			}
		}
		if (!(activeSpot == null))
		{
			BuilderSystem.inst._transform.parent.position = activeSpot.savedPos;
			BuilderSystem.inst._transform.rotation = activeSpot.savedRot;
			infoField.text = activeSpot.infoString;
		}
	}

	public void toggleSpots(bool hide)
	{
		show = !show;
		if (hide)
		{
			show = false;
		}
		if (show)
		{
			icon.uvRect = new Rect(0f, 0f, 0.5f, 0.5f);
		}
		else
		{
			icon.uvRect = new Rect(0.5f, 0f, 0.5f, 0.5f);
		}
		foreach (infoSpot spot in spots)
		{
			spot.gameObject.SetActive(show);
		}
	}

	public void destroySpots()
	{
		foreach (infoSpot spot in spots)
		{
			UnityEngine.Object.Destroy(spot.gameObject);
		}
		spots.Clear();
		activeSpot = null;
		show = true;
	}

	private void OnEnable()
	{
		toggleSpots(hide: true);
		BuilderSystem.disableInput = true;
		cg.interactable = false;
		CameraCtrl.inst.enabled = false;
	}

	private void OnDisable()
	{
		reposTog.image.enabled = false;
		toggleSpots(hide: false);
		BuilderSystem.disableInput = false;
		if (cg != null)
		{
			cg.interactable = true;
			BuilderSystem.inst.GetComponent<CameraCtrl>().enabled = true;
		}
		activeSpot = null;
	}
}
