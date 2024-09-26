using UnityEngine;

public class ioCopyPaste : Device
{
	public string parentName;

	public bool elevator;

	public override void powerThru(int _PID)
	{
	}

	public override void setValue(int val, bool send)
	{
	}

	public io getParentOutputIO()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position, 0.1f, 1);
		foreach (Collider collider in array)
		{
			if (!(collider.name != parentName))
			{
				return collider.GetComponent<Device>().outputTo[0];
			}
		}
		return null;
	}

	public io getParentInputIO()
	{
		Collider[] array = Physics.OverlapSphere(base.transform.position + new Vector3(0f, 0.9f, 0f), 0.1f, 1);
		foreach (Collider collider in array)
		{
			if (!(collider.name != "elevator"))
			{
				return collider.GetComponent<Device>().inputFrom[2];
			}
		}
		return null;
	}
}
