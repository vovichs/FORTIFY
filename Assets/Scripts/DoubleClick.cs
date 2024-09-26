using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClick : MonoBehaviour, IPointerClickHandler, IEventSystemHandler
{
	public void OnPointerClick(PointerEventData pointerEventData)
	{
		if (pointerEventData.button == PointerEventData.InputButton.Right)
		{
			UnityEngine.Debug.Log(base.name + " Game UnityEngine.Object Right Clicked!");
		}
		if (pointerEventData.button == PointerEventData.InputButton.Left)
		{
			UnityEngine.Debug.Log(base.name + " Game UnityEngine.Object Left Clicked!");
		}
		if (pointerEventData.clickCount == 2)
		{
			UnityEngine.Debug.Log("double click");
		}
	}
}
