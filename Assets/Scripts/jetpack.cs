using UnityEngine;

public class jetpack : MonoBehaviour
{
	private Transform t;

	private float randomOffset;

	private void Awake()
	{
		t = base.transform;
	}

	private void Start()
	{
		randomOffset = UnityEngine.Random.Range(0f, 5f);
	}

	private void Update()
	{
		Vector3 localScale = t.localScale;
		localScale.y = Mathf.PerlinNoise(Time.time * 15f, randomOffset) * 1.5f;
		t.localScale = localScale;
	}
}
