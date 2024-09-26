using UnityEngine;

public class viewClearCollider : MonoBehaviour
{
	public Transform cam;

	public float zoom;

	private int enterCount;

	private void OnTriggerEnter(Collider col)
	{
		if (col.tag == "mainCollider" && !col.transform.parent.GetComponent<BuilderPart>().hidden)
		{
			if (enterCount == 0)
			{
				cam.Translate(Vector3.forward * zoom);
			}
			enterCount++;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.tag == "mainCollider" && !col.transform.parent.GetComponent<BuilderPart>().hidden)
		{
			if (enterCount == 1)
			{
				cam.Translate(Vector3.forward * (0f - zoom));
			}
			enterCount--;
		}
	}
}
