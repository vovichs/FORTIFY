using UnityEngine;
using UnityEngine.UI;

public class inputFieldFloatLimit : MonoBehaviour
{
	public InputField inputField;

	public void checkInput()
	{
		if (!(inputField.text == ".") && float.Parse(inputField.text) < 0.25f)
		{
			inputField.text = "0.25";
		}
	}
}
