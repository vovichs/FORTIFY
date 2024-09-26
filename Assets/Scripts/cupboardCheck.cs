using UnityEngine;

public class cupboardCheck : MonoBehaviour
{
	private bool overlaped;

	private void FixedUpdate()
	{
		if (overlaped)
		{
			overlapCheck.overlap = true;
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (col.gameObject.CompareTag("cupboard"))
		{
			overlaped = true;
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (col.gameObject.CompareTag("cupboard"))
		{
			overlaped = false;
		}
	}
}
