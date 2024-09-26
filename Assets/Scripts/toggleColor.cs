using UnityEngine;
using UnityEngine.UI;

public class toggleColor : MonoBehaviour
{
	[Header("uses normal and pressed colors")]
	public Toggle toggle;

	private Color normal;

	private Color pressed;

	private void Awake()
	{
		if (!toggle)
		{
			toggle = GetComponent<Toggle>();
		}
		toggle.onValueChanged.AddListener(OnToggleValueChanged);
		ColorBlock colors = toggle.colors;
		normal = colors.normalColor;
		pressed = colors.pressedColor;
		if (toggle.isOn)
		{
			colors.normalColor = pressed;
			toggle.colors = colors;
		}
	}

	public void OnToggleValueChanged(bool isOn)
	{
		ColorBlock colors = toggle.colors;
		if (isOn)
		{
			colors.normalColor = pressed;
		}
		else
		{
			colors.normalColor = normal;
		}
		toggle.colors = colors;
	}
}
