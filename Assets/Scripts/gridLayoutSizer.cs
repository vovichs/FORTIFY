using UnityEngine;
using UnityEngine.UI;

public class gridLayoutSizer : MonoBehaviour
{
	public Scrollbar scrollbar;

	private void Awake()
	{
		GetComponent<GridLayoutGroup>();
		float y = base.transform.GetChild(base.transform.childCount - 1).GetComponent<RectTransform>().anchoredPosition.y;
		RectTransform component = GetComponent<RectTransform>();
		component.sizeDelta = new Vector2(component.sizeDelta.x, Mathf.Abs(y));
		scrollbar.value = 1f;
	}
}
