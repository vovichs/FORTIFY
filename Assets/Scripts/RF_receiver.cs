using UnityEngine;

public class RF_receiver : Device
{
	[Header("__________________________")]
	private bool added;

	public bool powered;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		if (!added)
		{
			added = true;
			RF_controller.inst.receivers.Add(this);
		}
		int inputPower = getInputPower(0);
		if (inputPower >= usage)
		{
			if (!powered)
			{
				powered = true;
				RF_controller.inst.receiverChange(value, on: true);
			}
			if (on)
			{
				power = inputPower;
				usage = 1;
			}
			else
			{
				power = 0;
				usage = 0;
			}
		}
		else
		{
			power = 0;
			if (powered)
			{
				powered = false;
				RF_controller.inst.receiverChange(value, on: false);
			}
		}
		if (power > 0)
		{
			power--;
		}
		outputTo[0].power = power;
		standardOutput(0);
	}

	public override void setValue(int val, bool send)
	{
		if (value == val)
		{
			return;
		}
		if (added && powered)
		{
			RF_controller.inst.receiverChange(value, on: false);
			RF_controller.inst.receiverChange(val, on: true);
		}
		value = val;
		if (added)
		{
			if (send && BuilderSystem.multiplayer)
			{
				Sender.sendDeviceValue(owner, value);
			}
			on = RF_controller.inst.freqs.ContainsKey(value);
			newPowerThru();
		}
	}

	public void freqReceived(bool state)
	{
		on = state;
		newPowerThru();
	}

	private void OnDestroy()
	{
		if (powered)
		{
			RF_controller.inst.receiverChange(value, on: false);
		}
	}
}
