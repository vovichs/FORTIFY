using UnityEngine;
using UnityEngine.UI;

public class wireColorButton : MonoBehaviour
{
	public RectTransform selectRect;

	private Toggle toggle;

	private int colorIndex;

	private void Awake()
	{
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(OnToggleValueChanged);
		colorIndex = int.Parse(toggle.name);
	}

	private void OnToggleValueChanged(bool isOn)
	{
		selectRect.position = base.transform.position;
		wiring.inst.setPlacingWireColor(colorIndex);
	}
}
