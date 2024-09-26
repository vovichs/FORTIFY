using UnityEngine;

public class electricFurnace : Device
{
	[Header("__________________________")]
	public BuilderPart bp;

	public Material[] mats;

	public override void powerThru(int _PID)
	{
		if (PID != _PID)
		{
			PID = _PID;
			getInputPower(0);
			setState();
			sendUsage(-1, 0, 0);
		}
	}

	private void setState()
	{
		if (inputFrom[0].power >= usage)
		{
			if (!on)
			{
				on = true;
				bp.rend[0].sharedMaterial = mats[1];
				bp.deploy.mat = mats[1];
			}
		}
		else if (on)
		{
			on = false;
			bp.rend[0].sharedMaterial = mats[0];
			bp.deploy.mat = mats[0];
		}
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(bp, val);
		}
		if (PID > 0 && inputFrom[0].power > 0)
		{
			newPowerThru();
		}
	}
}
