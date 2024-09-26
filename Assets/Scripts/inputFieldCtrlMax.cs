using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputFieldCtrlMax : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IDeselectHandler
{
	public InputField inputField;

	public int max;

	private bool activated;

	private void Start()
	{
		inputField.onEndEdit.AddListener(delegate
		{
			OnEndEdit();
		});
	}

	private void OnEndEdit()
	{
		if (inputField.text == "")
		{
			inputField.text = "1";
			return;
		}
		int num = int.Parse(inputField.text);
		if (num > max)
		{
			inputField.text = max.ToString();
		}
		if (num == 0)
		{
			inputField.text = "1";
		}
	}

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
