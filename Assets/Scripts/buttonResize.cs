using UnityEngine;

public class buttonResize : MonoBehaviour
{
	private RectTransform rect;

	private RectTransform parentRect;

	private float startSizeX;

	private float sizeY;

	public GameObject toggleObj1;

	public GameObject toggleObj2;

	public void valueChanged(bool state)
	{
		if (!rect)
		{
			rect = GetComponent<RectTransform>();
			startSizeX = rect.sizeDelta.x;
			sizeY = rect.sizeDelta.y;
			parentRect = base.transform.parent.GetComponent<RectTransform>();
		}
		if ((bool)toggleObj1)
		{
			toggleObj1.SetActive(state);
		}
		if ((bool)toggleObj2)
		{
			toggleObj2.SetActive(!state);
		}
		if (state)
		{
			rect.sizeDelta = new Vector2(parentRect.sizeDelta.x, sizeY);
		}
		else
		{
			rect.sizeDelta = new Vector2(startSizeX, sizeY);
		}
	}
}
