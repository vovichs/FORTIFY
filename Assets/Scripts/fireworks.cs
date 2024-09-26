using System.Collections;
using UnityEngine;

public class fireworks : MonoBehaviour
{
	public Color color;

	public AnimationCurve lightCurve;

	public ParticleSystem ps_sparks;

	public ParticleSystem ps_explo;

	public Light flash;

	public Rigidbody rb;

	private bool boom;

	private float timer;

	private float boomTime;

	private void Awake()
	{
		rb.AddForce(offsetDir(base.transform.up) * 15000f);
		boomTime = UnityEngine.Random.Range(1.5f, 2f);
		flash.color = color;
	}

	private void Update()
	{
		if (timer < boomTime)
		{
			timer += Time.deltaTime;
		}
		else if (!boom)
		{
			ps_sparks.Stop();
			rb.velocity = Vector3.zero;
			rb.angularVelocity = Vector3.zero;
			rb.useGravity = false;
			ps_explo.gameObject.SetActive(value: true);
			flash.intensity = 3f;
			boom = true;
			if (ps_explo.subEmitters.enabled)
			{
				StartCoroutine(timedSubEmit());
			}
		}
		else
		{
			flash.intensity -= Time.deltaTime * 4f;
			if (!ps_explo.IsAlive())
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	private IEnumerator timedSubEmit()
	{
		yield return new WaitForSeconds(1.5f);
		ps_explo.TriggerSubEmitter(0);
	}

	private Vector3 offsetDir(Vector3 initial)
	{
		float num = 0.05f;
		float x = UnityEngine.Random.Range(0f - num, num);
		float y = UnityEngine.Random.Range(0f - num, num);
		float z = UnityEngine.Random.Range(0f - num, num);
		return (new Vector3(x, y, z) + initial).normalized;
	}
}
