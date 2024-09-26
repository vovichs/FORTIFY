using UnityEngine;

[AddComponentMenu("Miscellaneous/Info Note")]
public class InfoNote : MonoBehaviour
{
	[TextArea]
	public string TextInfo = "Type your message here and press enter to send";

	private void Awake()
	{
		base.enabled = false;
	}
}
