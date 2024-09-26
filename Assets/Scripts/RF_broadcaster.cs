public class RF_broadcaster : Device
{
	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		getInputPower(0);
		if (inputFrom[0].power >= usage)
		{
			if (!on)
			{
				RF_controller.inst.broadcastChange(value, on: true);
				on = true;
			}
		}
		else if (on)
		{
			RF_controller.inst.broadcastChange(value, on: false);
			on = false;
		}
		sendUsage();
	}

	public override void setValue(int val, bool send)
	{
		if (base.value != val)
		{
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDeviceValue(owner, val);
			}
			int value = base.value;
			base.value = val;
			if (on)
			{
				RF_controller.inst.broadcastChange(value, on: false);
				RF_controller.inst.broadcastChange(base.value, on: true);
			}
			newPowerThru();
		}
	}

	private void OnDestroy()
	{
		if (on)
		{
			RF_controller.inst.broadcastChange(value, on: false);
		}
	}
}
