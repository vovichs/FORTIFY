using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class io : MonoBehaviour
{
	public int power;

	public int type;

	public bool noUsage;

	public int index;

	public int id;

	public Device dev;

	public io connectedTo;

	public wire wire;

	public Renderer rend;

	public Text text;

	public string label;

	public bool over;

	private string mouseOverText;

	public bool blockPanel;

	private void OnMouseEnter()
	{
		if (BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		if (mouseOverText == null)
		{
			mouseOverText = dev.owner.info.named + "\n" + label + "\n";
		}
		if (wiring.inst.devPanel.panel == null)
		{
			wiring.inst.devPanel.moveToMouse();
			wiring.inst.devPanel.text.enabled = true;
			StartCoroutine(overMat());
			if (type == 0)
			{
				StartCoroutine(powerDisplayLoop());
			}
			else
			{
				wiring.inst.devPanel.text.text = mouseOverText;
			}
		}
	}

	private void OnMouseOver()
	{
		if (connectedTo == null || BuilderUI.inst.mouseOverUI)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if (blockPanel)
			{
				return;
			}
			if (wiring.inst.wire == null)
			{
				wirePanel.inst.open(this);
			}
			if (connectedTo != null)
			{
				wirePanel.inst.ioText.text = connectedTo.dev.name + "\n" + connectedTo.label;
			}
		}
		if ((UnityEngine.Input.GetKeyDown(KeyCode.Delete) || Input.GetButtonDown("Del")) && !BuilderSystem.disableInput && wire != null)
		{
			MGMT.inst.undoListInsert();
			MGMT.inst.undoListList[0].Add(MGMT.inst.newDeletedWire(wire));
			sendDisconnect(destroyed: false);
		}
	}

	private void OnMouseExit()
	{
		if (over)
		{
			wiring.inst.devPanel.text.enabled = false;
		}
		over = false;
	}

	private IEnumerator overMat()
	{
		over = true;
		rend.sharedMaterial = wiring.inst.ioOverMat;
		if ((bool)wire)
		{
			wire.highlightMat(state: true);
		}
		yield return new WaitUntil(() => !over);
		if (connectedTo == null)
		{
			rend.sharedMaterial = wiring.inst.ioMat;
		}
		else
		{
			rend.sharedMaterial = wiring.inst.ioConnectMat;
		}
		if ((bool)wire)
		{
			wire.highlightMat(state: false);
		}
	}

	private IEnumerator powerDisplayLoop()
	{
		int savedPower = -1;
		while (over)
		{
			if (power != savedPower)
			{
				savedPower = power;
				wiring.inst.devPanel.text.text = mouseOverText + power;
			}
			yield return new WaitForSeconds(0.5f);
		}
	}

	public void sendDisconnect(bool destroyed)
	{
		if (!(connectedTo != null))
		{
			return;
		}
		connectedTo.disconnect();
		connectedTo = null;
		if (!destroyed)
		{
			disconnect();
			if (BuilderSystem.multiplayer)
			{
				Sender.sendWireRemove(this);
			}
		}
	}

	public void disconnect()
	{
		if (dev.owner.tag == "destroy")
		{
			return;
		}
		if (base.gameObject.tag == "output")
		{
			if (wire != null)
			{
				UnityEngine.Object.Destroy(wire.gameObject);
				wire = null;
			}
			if (dev is branch || dev is splitter || dev is memoryCell || dev is autoTurret || dev is samSite || dev is power)
			{
				dev.outUsage[index] = 0;
				dev.sendUsage(0, index, 0);
			}
			else
			{
				dev.savedUsage = 0;
			}
		}
		connectedTo = null;
		if (type == 0)
		{
			dev.newPowerThru();
		}
		rend.sharedMaterial = wiring.inst.ioMat;
	}

	public wiring.connected ioConnectType()
	{
		if (base.gameObject.tag == "output")
		{
			return wiring.connected.output;
		}
		return wiring.connected.input;
	}

	private void OnDisable()
	{
		if (over && (bool)wire)
		{
			wire.highlightMat(state: false);
		}
	}
}
