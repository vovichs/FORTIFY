using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputFieldCtrl : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDeselectHandler
{
	public InputField inputField;

	private bool activated;

	public void OnPointerDown(PointerEventData eventData)
	{
		if (!activated)
		{
			activated = true;
			inputField.ActivateInputField();
			inputField.MoveTextEnd(shift: false);
			BuilderUI.inst.disableInput(state: true);
		}
	}

	public void OnDeselect(BaseEventData eventData)
	{
		activated = false;
		BuilderUI.inst.disableInput(state: false);
	}
}
