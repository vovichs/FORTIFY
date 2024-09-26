using UnityEngine;

public class randomOffset : MonoBehaviour
{
	public Animation anim;

	private void Start()
	{
		anim[anim.clip.name].normalizedTime = UnityEngine.Random.Range(0f, 1f);
	}
}
