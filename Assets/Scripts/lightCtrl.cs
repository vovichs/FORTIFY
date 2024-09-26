using UnityEngine;

public class lightCtrl : MonoBehaviour
{
	public Light _light;

	private void Start()
	{
		if (dayNightCtrl.night)
		{
			_light.enabled = true;
		}
	}
}
