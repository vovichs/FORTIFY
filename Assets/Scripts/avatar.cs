using UnityEngine;

public class avatar : MonoBehaviour
{
	public Transform head;

	public GameObject plan;

	public GameObject cannon;

	public ParticleSystem cannonSmoke;

	public void swapPlayerItems(bool fireMode)
	{
		cannon.SetActive(fireMode);
		plan.SetActive(!fireMode);
	}
}
