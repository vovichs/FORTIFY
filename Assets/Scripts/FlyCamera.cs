using UnityEngine;

public class FlyCamera : MonoBehaviour
{
	private float mainSpeed = 10f;

	private float shiftAdd = 250f;

	private float maxShift = 1000f;

	private float camSens = 0.25f;

	private Vector3 lastMouse = new Vector3(255f, 255f, 255f);

	private float totalRun = 1f;

	private void Update()
	{
		lastMouse = UnityEngine.Input.mousePosition - lastMouse;
		lastMouse = new Vector3((0f - lastMouse.y) * camSens, lastMouse.x * camSens, 0f);
		lastMouse = new Vector3(base.transform.eulerAngles.x + lastMouse.x, base.transform.eulerAngles.y + lastMouse.y, 0f);
		base.transform.eulerAngles = lastMouse;
		lastMouse = UnityEngine.Input.mousePosition;
		Vector3 vector = GetBaseInput();
		if (UnityEngine.Input.GetKey(KeyCode.LeftShift))
		{
			totalRun += Time.deltaTime;
			vector = vector * totalRun * shiftAdd;
			vector.x = Mathf.Clamp(vector.x, 0f - maxShift, maxShift);
			vector.y = Mathf.Clamp(vector.y, 0f - maxShift, maxShift);
			vector.z = Mathf.Clamp(vector.z, 0f - maxShift, maxShift);
		}
		else
		{
			totalRun = Mathf.Clamp(totalRun * 0.5f, 1f, 1000f);
			vector *= mainSpeed;
		}
		vector *= Time.deltaTime;
		Vector3 position = base.transform.position;
		if (UnityEngine.Input.GetKey(KeyCode.Space))
		{
			base.transform.Translate(vector);
			position.x = base.transform.position.x;
			position.z = base.transform.position.z;
			base.transform.position = position;
		}
		else
		{
			base.transform.Translate(vector);
		}
	}

	private Vector3 GetBaseInput()
	{
		Vector3 vector = default(Vector3);
		if (UnityEngine.Input.GetKey(KeyCode.W))
		{
			vector += new Vector3(0f, 0f, 1f);
		}
		if (UnityEngine.Input.GetKey(KeyCode.S))
		{
			vector += new Vector3(0f, 0f, -1f);
		}
		if (UnityEngine.Input.GetKey(KeyCode.A))
		{
			vector += new Vector3(-1f, 0f, 0f);
		}
		if (UnityEngine.Input.GetKey(KeyCode.D))
		{
			vector += new Vector3(1f, 0f, 0f);
		}
		return vector;
	}
}
