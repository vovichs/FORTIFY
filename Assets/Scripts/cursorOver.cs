using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class cursorOver : MonoBehaviour
{
	public static cursorOver inst;

	public int onCount;

	private Vector3 offset = new Vector3(20f, 10f, 0f);

	public LayerMask usedMask;

	public TextMeshProUGUI text;

	public RawImage img;

	private Transform _transform;

	private bool endCO;

	private void Awake()
	{
		inst = this;
		_transform = base.transform;
	}

	private void Update()
	{
		_transform.position = UnityEngine.Input.mousePosition;
	}

	public void setActive(int index, bool on)
	{
		if (on)
		{
			onCount++;
			_transform.position = UnityEngine.Input.mousePosition;
			base.enabled = true;
			_transform.GetChild(index).gameObject.SetActive(value: true);
			if (index == 1)
			{
				StartCoroutine(iconFadeIn());
			}
		}
		else
		{
			onCount--;
			base.enabled = false;
			if (index == 1)
			{
				img.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, 0);
			}
			_transform.GetChild(index).gameObject.SetActive(value: false);
		}
	}

	public void allOff()
	{
		onCount = 0;
		foreach (Transform item in _transform)
		{
			item.gameObject.SetActive(value: false);
		}
		base.enabled = false;
	}

	private IEnumerator iconFadeIn()
	{
		byte alpha = 0;
		for (int i = 1; i < 17; i++)
		{
			alpha = (byte)(alpha + 15);
			img.color = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, alpha);
			yield return new WaitForFixedUpdate();
		}
	}
}
