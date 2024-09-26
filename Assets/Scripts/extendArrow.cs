using UnityEngine;
using UnityEngine.UI;

public class extendArrow : MonoBehaviour
{
	public static extendArrow inst;

	public static int amount = 5;

	public Transform arrow;

	public Toggle tog;

	public InputField extInput;

	private static bool startCheck = false;

	private void Awake()
	{
		if (!startCheck && PlayerPrefs.HasKey("extend"))
		{
			amount = PlayerPrefs.GetInt("extend");
		}
		startCheck = true;
		inst = this;
		extInput.text = amount.ToString();
	}

	public void setExtendAmount()
	{
		string text = extInput.text;
		if (text != "")
		{
			amount = int.Parse(text);
		}
	}

	public void ExtendTogCheck()
	{
		if (!tog.isOn)
		{
			arrow.position = new Vector3(0f, -1000f, 0f);
		}
	}

	public void hideArrow()
	{
		if (tog.isOn)
		{
			arrow.position = new Vector3(0f, -1000f, 0f);
		}
	}

	public void alignWith(Vector3 pos, Vector3 dir)
	{
		if (tog.isOn)
		{
			arrow.SetPositionAndRotation(pos, Quaternion.identity);
			arrow.rotation *= Quaternion.FromToRotation(arrow.right, dir);
		}
	}

	public void alignOnGround(Vector3 pos, Vector3 dir)
	{
		if (tog.isOn)
		{
			arrow.SetPositionAndRotation(pos, Quaternion.identity);
			arrow.rotation *= Quaternion.FromToRotation(arrow.right, dir);
			arrow.Translate(new Vector3(-0.25f, 0.5f, 0f));
		}
	}

	private void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("extend", amount);
	}
}
