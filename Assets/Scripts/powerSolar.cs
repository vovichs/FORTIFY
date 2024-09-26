public class powerSolar : Device
{
	private void OnEnable()
	{
		if (lightCheck())
		{
			outputTo[0].power = power;
		}
		else
		{
			outputTo[0].power = 0;
		}
	}

	public override void powerThru(int _PID)
	{
		if (outputTo[0].connectedTo != null)
		{
			PID = _PID;
			if (lightCheck())
			{
				outputTo[0].power = power;
			}
			else
			{
				outputTo[0].power = 0;
			}
			outputPower(0, PID);
		}
	}

	private bool lightCheck()
	{
		if (dayNightCtrl.night || sceneSettings.inst.scene == sceneSettings.eScene.caves)
		{
			return false;
		}
		return true;
	}

	public override void setValue(int val, bool send)
	{
	}
}
