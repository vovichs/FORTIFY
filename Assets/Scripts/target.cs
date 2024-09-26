using UnityEngine;

public class target : Device
{
	[Header("__________________________")]
	public MeshFilter mf;

	public Mesh mesh;

	public Mesh mesh_dn;

	private bool up = true;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		bool[] array = new bool[3];
		bool flag = false;
		for (int i = 0; i < inputFrom.Length; i++)
		{
			if (inputFrom[i].connectedTo != null)
			{
				if (inputFrom[i].connectedTo.power != inputFrom[i].power)
				{
					array[i] = true;
				}
				inputFrom[i].power = inputFrom[i].connectedTo.power;
			}
			else
			{
				inputFrom[i].power = 0;
			}
		}
		if (inputFrom[1].power > 0 && array[1] && !up)
		{
			up = true;
			mf.sharedMesh = mesh;
			flag = true;
		}
		if (inputFrom[2].power > 0 && array[2] && up)
		{
			up = false;
			mf.sharedMesh = mesh_dn;
			flag = true;
		}
		int num = 0;
		if (inputFrom[0].power > 0)
		{
			if (!up)
			{
				num = inputFrom[0].power - 1;
			}
		}
		else if (!up && (inputFrom[1].power > 0 || inputFrom[2].power > 0))
		{
			num = 1;
		}
		if (num != outputTo[0].power)
		{
			flag = true;
		}
		outputTo[0].power = num;
		if (outputTo[0].connectedTo != null)
		{
			if (flag)
			{
				outputPower(0, PID);
			}
			else
			{
				sendUsage(-1, 0, 0);
			}
		}
		else
		{
			sendUsage(-1, 0, 0);
		}
	}

	public override void setValue(int val, bool send)
	{
	}
}
