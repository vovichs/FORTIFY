using UnityEngine;

public class uvFlow : MonoBehaviour
{
	private Vector2 uvOffset = Vector2.zero;

	public float uvOffsetRate = 0.5f;

	private string property = "_DetailAlbedoMap";

	public Material mat;

	public static bool started;

	private void Update()
	{
		uvOffset += new Vector2(uvOffsetRate, 0f) * Time.deltaTime;
		mat.SetTextureOffset(property, uvOffset);
	}
}
