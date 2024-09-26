using UnityEngine;

public class ParticleDestroy : MonoBehaviour
{
	public ParticleSystem ps;

	public AudioSource AS;

	public Light flash;

	public void Start()
	{
		AS.pitch = UnityEngine.Random.Range(0.5f, 1f);
		AS.Play();
	}

	public void Update()
	{
		if (flash != null)
		{
			flash.intensity -= Time.deltaTime * 5f;
		}
		if (!ps.IsAlive())
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
