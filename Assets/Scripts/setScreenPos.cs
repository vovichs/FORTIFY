using UnityEngine;
using UnityEngine.UI;

public class setScreenPos : MonoBehaviour
{
	public Vector3 pos;

	public Image image;

	public bool hidden;

	private void OnEnable()
	{
		hidden = false;
		image.enabled = true;
	}

	private void OnDisable()
	{
		image.enabled = false;
	}
}
