using UnityEngine;

public class panelResize : MonoBehaviour
{
	public RectTransform rt;

	private bool expanded;

	public void toggled(int change)
	{
		Vector2 sizeDelta = rt.sizeDelta;
		if (expanded)
		{
			rt.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y - (float)change);
		}
		else
		{
			rt.sizeDelta = new Vector2(sizeDelta.x, sizeDelta.y + (float)change);
		}
		expanded = !expanded;
	}
}
