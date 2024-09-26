using UnityEngine;
using UnityEngine.UI;

public class inputFieldClipboard : MonoBehaviour
{
	public InputField inputField;

	public void inputFieldPaste()
	{
		inputField.text = GUIUtility.systemCopyBuffer;
	}
}
