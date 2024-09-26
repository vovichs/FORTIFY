using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ioTrigger : MonoBehaviour
{
	public static ioTrigger inst;

	public Transform ioTriggerCol;

	public List<Device> displaying;

	private bool moveWait;

	private void Awake()
	{
		inst = this;
		base.gameObject.SetActive(value: false);
	}

	public void move(Vector3 pos)
	{
		if (!moveWait)
		{
			ioTriggerCol.position = pos;
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (!(col.gameObject.tag != "deploy"))
		{
			BuilderPart component = col.GetComponent<BuilderPart>();
			if (component.device != null && !component.hidden)
			{
				component.device.showIOs(placing: false);
				displaying.Add(component.device);
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (!(col.gameObject.tag != "deploy"))
		{
			BuilderPart component = col.GetComponent<BuilderPart>();
			if (component.device != null)
			{
				component.device.StartCoroutine(component.device.WaitToHideIOs());
				displaying.Remove(component.device);
			}
		}
	}

	public void stopDisplaying()
	{
		foreach (Device item in displaying)
		{
			if ((bool)item)
			{
				item.hideIOs(listRemove: false);
			}
		}
		displaying.Clear();
	}

	public void displayByType(wiring.connected ioType)
	{
		foreach (Device item in displaying)
		{
			if ((bool)item)
			{
				io[] inputFrom = item.inputFrom;
				foreach (io io in inputFrom)
				{
					bool active = wiring.inst.ioMode == io.type && io.ioConnectType() != ioType;
					io.gameObject.SetActive(active);
				}
				inputFrom = item.outputTo;
				foreach (io io2 in inputFrom)
				{
					bool active2 = wiring.inst.ioMode == io2.type && io2.ioConnectType() != ioType;
					io2.gameObject.SetActive(active2);
				}
			}
		}
	}

	public IEnumerator resetPos()
	{
		moveWait = true;
		ioTriggerCol.position = Vector3.zero;
		yield return new WaitForFixedUpdate();
		moveWait = false;
	}
}
