using UnityEngine;

public class powerWind : Device
{
	private int height = -1;

	private int windPower;

	public override void powerThru(int _PID)
	{
		PID = _PID;
		if (outputTo[0].connectedTo != null)
		{
			outputPower(0, PID);
		}
	}

	public void heightWindCheck()
	{
		float num = base.transform.position.y;
		if (Physics.Raycast(base.transform.position, Vector3.down, out RaycastHit hitInfo, 100f, 256))
		{
			num -= hitInfo.point.y;
		}
		if (num < 0f)
		{
			num = 0f;
		}
		float num2 = Mathf.InverseLerp(0f, 16.666f, num) * 0.5f + 0.45f;
		windPower = Mathf.FloorToInt(150f * num2);
		outputTo[0].power = windPower;
	}

	public override void setValue(int val, bool send)
	{
	}
}
