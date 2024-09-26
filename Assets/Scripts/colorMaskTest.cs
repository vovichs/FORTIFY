using UnityEngine;

public class colorMaskTest : MonoBehaviour
{
	private void Start()
	{
		Material material = GetComponent<Renderer>().materials[0];
		material.SetColor("_Color", Color.green);
		base.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<Renderer>().material = material;
	}
}
