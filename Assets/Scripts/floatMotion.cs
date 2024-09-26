using System;
using UnityEngine;

public class floatMotion : MonoBehaviour
{
	private Transform t;

	public float scale = 0.001f;

	public Rigidbody rb;

	private float addMotion;

	private float addMotion2;

	private void Awake()
	{
		t = GetComponent<Transform>();
	}

	private void LateUpdate()
	{
		addMotion = (float)Math.Sin(Time.time * 3f) * scale;
		addMotion2 = (float)Math.Sin(Time.time) * scale;
		Vector3 position = t.position;
		position.x -= addMotion2;
		position.y += addMotion + addMotion2;
		position.z += addMotion2;
		if ((bool)rb)
		{
			rb.MovePosition(position);
		}
		else
		{
			t.position = Vector3.Lerp(t.position, position, 0.5f * Time.deltaTime);
		}
	}
}
