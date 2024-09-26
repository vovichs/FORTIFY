using UnityEngine;
using UnityEngine.EventSystems;

public class mouseOverLabel : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public partInfo info;

	public void OnPointerEnter(PointerEventData eventData)
	{
		placeMenuFilter.inst.labelText.text = info.named;
		GameObject label = placeMenuFilter.inst.label;
		label.SetActive(value: true);
		Vector3 position = label.transform.position;
		label.transform.position = new Vector3(position.x, base.transform.position.y, position.z);
		placeMenuFilter.inst.labelText.SetLayoutDirty();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		placeMenuFilter.inst.label.SetActive(value: false);
	}
}
