using UnityEngine;
using UnityEngine.UI;

public class inputFieldLimit : MonoBehaviour
{
	public InputField inputField;

	public void limitInput()
	{
		string text = "";
		string text2 = inputField.text;
		for (int i = 0; i < text2.Length; i++)
		{
			char c = text2[i];
			if (char.IsLetter(c) || char.IsNumber(c) || char.IsWhiteSpace(c) || c == '.' || c == ',' || c == '!' || c == '?' || c == '\'')
			{
				text += text2[i].ToString();
			}
		}
		inputField.text = text;
	}
}
