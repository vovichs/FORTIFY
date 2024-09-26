using System.Collections.Generic;
using UnityEngine;

public class autoTurret : Device
{
	[Header("__________________________")]
	public bool interference;

	public int interfCount;

	public string itemName = "empty";

	public Transform _transform;

	public MeshFilter mf;

	public Mesh meshOn;

	public Mesh meshOff;

	public ParticleSystem ps;

	private bool initialized;

	public static List<autoTurret> autoTurretList;

	public List<autoTurret> turretsInRange = new List<autoTurret>();

	public bool[] modes = new bool[3];

	static autoTurret()
	{
		autoTurretList = new List<autoTurret>();
	}

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		if (inputPower >= usage)
		{
			if (!on)
			{
				on = true;
				mf.sharedMesh = meshOn;
				UpdateInterference(rangeCheck: false);
				UpdateInterferenceOnOthers(remove: false);
			}
		}
		else if (on)
		{
			on = false;
			if (interference)
			{
				ps.Stop();
			}
			interference = false;
			mf.sharedMesh = meshOff;
			UpdateInterferenceOnOthers(remove: false);
		}
		for (int i = 0; i < outputTo.Length; i++)
		{
			if (outputTo[i].connectedTo != null)
			{
				if (modes[i] && on && inputPower >= usage + 1)
				{
					outputTo[i].power = 1;
				}
				else
				{
					outputTo[i].power = 0;
				}
				wiring.inst.newPID++;
				PID = wiring.inst.newPID;
				outputPower(i, PID);
			}
		}
		if (circuitEndCheck())
		{
			sendUsage(-1, 0, 0);
		}
	}

	public void UpdateInterference(bool rangeCheck)
	{
		if (on)
		{
			if (rangeCheck)
			{
				GetTurretsInInterferenceRange();
			}
			interfCount = 0;
			foreach (autoTurret item in turretsInRange)
			{
				if ((bool)item && item.gameObject.activeSelf && item.on && !item.interference)
				{
					interfCount++;
				}
			}
			interferenceState(interfCount >= 12);
		}
	}

	public void UpdateInterferenceOnOthers(bool remove)
	{
		foreach (autoTurret item in turretsInRange)
		{
			if ((bool)item)
			{
				if (remove)
				{
					item.RemoveTurretInRange(this);
				}
				if (initialized)
				{
					item.UpdateInterference(rangeCheck: false);
				}
			}
		}
	}

	private void GetTurretsInInterferenceRange()
	{
		turretsInRange.Clear();
		Vector3 position = _transform.position;
		for (int i = 0; i < autoTurretList.Count; i++)
		{
			autoTurret autoTurret = autoTurretList[i];
			if ((bool)autoTurret && !(autoTurret == this) && (position - autoTurret._transform.position).magnitude <= 13.333f)
			{
				turretsInRange.Add(autoTurret);
				autoTurret.AddTurretInRange(this);
			}
		}
	}

	public void AddTurretInRange(autoTurret at)
	{
		if (!turretsInRange.Contains(this))
		{
			turretsInRange.Add(at);
			UpdateInterference(rangeCheck: false);
		}
	}

	public void RemoveTurretInRange(autoTurret at)
	{
		turretsInRange.Remove(at);
		UpdateInterference(rangeCheck: false);
	}

	private void interferenceState(bool state)
	{
		if (state && !interference)
		{
			interference = true;
			mf.sharedMesh = meshOff;
			ps.Play();
		}
		if (!state && interference)
		{
			interference = false;
			mf.sharedMesh = meshOn;
			ps.Stop();
		}
	}

	public void Initialize(bool add)
	{
		initialized = true;
		if (add)
		{
			autoTurretList.Add(this);
		}
		GetTurretsInInterferenceRange();
	}

	public override void setValue(int val, bool send)
	{
		value = val;
		if (send && BuilderSystem.multiplayer)
		{
			Sender.sendDeviceValue(owner, value);
		}
		itemName = RustCopyPaste.inst.GetWeaponNameFromID(value);
	}

	private void OnDestroy()
	{
		if (initialized)
		{
			initialized = false;
			autoTurretList.Remove(this);
			UpdateInterferenceOnOthers(remove: true);
		}
	}

	public override void deviceHideState(bool hide)
	{
		
	}

	public void stabilityExitCheck()
	{
		if (interference)
		{
			ps.Play();
		}
	}
}
