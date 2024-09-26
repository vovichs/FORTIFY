using System.Collections;
using UnityEngine;

public class doorController : Device
{
	[Header("__________________________")]
	public BuilderPart doorBP;

	private bool open;

	public Renderer[] lights;

	public LayerMask mask;

	private bool toggleOpen;

	private bool toggleClose;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		int inputPower3 = getInputPower(2);
		if (inputPower > 0)
		{
			if (!on)
			{
				on = true;
				value = 1;
				lights[1].sharedMaterial = wiring.inst.lightGreen;
			}
			outputTo[0].power = inputPower - usage;
		}
		else
		{
			if (on)
			{
				on = false;
				value = 0;
				lights[1].sharedMaterial = wiring.inst.lightOff;
			}
			outputTo[0].power = 0;
		}
		if (inputPower > 0)
		{
			if (inputPower2 > 0)
			{
				if (!toggleOpen)
				{
					toggleOpen = true;
					value = 1;
				}
			}
			else
			{
				toggleOpen = false;
			}
			if (inputPower3 > 0)
			{
				if (!toggleClose)
				{
					toggleClose = true;
					value = 0;
				}
			}
			else
			{
				toggleClose = false;
			}
		}
		if (value == 1)
		{
			if (!open)
			{
				open = true;
				if (doorBP != null)
				{
					StartCoroutine(doorBP.changeDoorMeshState(state: true, audio: true, send: false));
				}
			}
		}
		else if (open)
		{
			open = false;
			if (doorBP != null)
			{
				StartCoroutine(doorBP.changeDoorMeshState(state: false, audio: true, send: false));
			}
		}
		standardOutput(0);
	}

	public IEnumerator connectToDoor(bool manual)
	{
		if ((bool)doorBP)
		{
			yield break;
		}
		yield return new WaitForFixedUpdate();
		Vector3 position = owner._transform.position;
		Collider[] array = Physics.OverlapSphere(position, 0.6f, mask, QueryTriggerInteraction.Collide);
		door door = null;
		float num = 0.55f;
		if (rotatedForHatch())
		{
			num = 0.1f;
		}
		for (int i = 0; i < array.Length; i++)
		{
			door component = array[i].transform.root.GetComponent<door>();
			if ((bool)component && !component.doorCtrl)
			{
				float magnitude = (component.doorCtrlCheckPos.position - position).magnitude;
				if (!(magnitude > num))
				{
					num = magnitude;
					door = component;
				}
			}
		}
		if ((bool)door)
		{
			connect(door.GetComponent<BuilderPart>());
		}
		else if (manual)
		{
			popUp.inst.message("door connect failed");
		}
	}

	public void connect(BuilderPart bp)
	{
		bp.door.doorCtrl = this;
		doorBP = bp;
		lights[0].sharedMaterial = wiring.inst.lightGreen;
	}

	public void disconnect()
	{
		doorBP = null;
		lights[0].sharedMaterial = wiring.inst.lightOff;
	}

	public bool rotatedForHatch()
	{
		return Mathf.Abs(Vector3.Dot(owner._transform.forward, Vector3.up)) > 0.99f;
	}

	public override void setValue(int val, bool send)
	{
	}
}
