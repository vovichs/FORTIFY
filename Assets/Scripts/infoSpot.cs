using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class infoSpot : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	private bool hidden;

	public RawImage icon;

	public string infoString = "";

	public Vector3 savedPos;

	public Quaternion savedRot;

	private Transform _transfrom;

	private void Start()
	{
		_transfrom = base.transform;
	}

	private void Update()
	{
		Vector3 vector = CameraCtrl.inst.cam.WorldToScreenPoint(savedPos);
		if (vector.z > 50f || vector.z < 0.5f)
		{
			if (!hidden)
			{
				icon.raycastTarget = false;
				hidden = true;
				icon.color = new Vector4(1f, 1f, 1f, 0f);
			}
			return;
		}
		if (hidden)
		{
			icon.raycastTarget = true;
			hidden = false;
		}
		_transfrom.position = vector;
		float w = 1f - vector.z / 50f;
		icon.color = new Vector4(1f, 1f, 1f, w);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		icon.uvRect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		icon.uvRect = new Rect(0f, 0.5f, 0.5f, 0.5f);
	}

	public void moveTo(bool justAdded)
	{
		if (!justAdded)
		{
			if (CameraCtrl.inst.mode != 0)
			{
				CameraCtrl.inst.changeMode(0);
			}
			infoSpotPanel.activeSpot = this;
			Transform transform = BuilderSystem.inst._transform;
			transform.parent.position = savedPos;
			transform.parent.rotation = Quaternion.Euler(0f, savedRot.eulerAngles.y, 0f);
			transform.rotation = savedRot;
			CameraCtrl.setRotation();
		}
		if (infoString != "")
		{
			infoSpotPanel.inst.infoField.text = infoString;
		}
		infoSpotPanel.inst.infoPanel.SetActive(value: true);
	}
}
