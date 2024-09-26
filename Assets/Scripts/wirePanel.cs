using UnityEngine;
using UnityEngine.UI;

public class wirePanel : MonoBehaviour
{
	public static wirePanel inst;

	public GameObject panel;

	public GameObject colorPanelWire;

	public GameObject colorPanelPipe;

	public io io;

	public Text ioText;

	public Toggle circuitColor;

	public Text removeText;

	private string[] names = new string[5]
	{
		"wire",
		"hose",
		"",
		"",
		"pipe"
	};

	private void Awake()
	{
		inst = this;
		panel = base.gameObject;
		panel.SetActive(value: false);
	}

	public void open(io _io)
	{
		base.transform.position = UnityEngine.Input.mousePosition;
		io = _io;
		colorPanelWire.SetActive(_io.type == 0);
		colorPanelPipe.SetActive(_io.type == 4);
		panel.SetActive(value: true);
	}

	public void lockWireHighlight()
	{
		if ((bool)io.wire)
		{
			io.wire.lockGlow = !io.wire.lockGlow;
			if (!io.over)
			{
				io.wire.highlightMat(io.wire.lockGlow);
			}
		}
	}

	public void setColor(int ind)
	{
		bool isOn = circuitColor.isOn;
		if (io != null && io.type != 1)
		{
			io.wire.setMat(ind, isOn);
		}
		circuitColor.isOn = false;
	}

	public void disconnectWire()
	{
		if (io != null)
		{
			wiring.inst.disconnectEnd(io);
		}
		panel.SetActive(value: false);
	}

	public void removeWire()
	{
		if (io != null)
		{
			io.sendDisconnect(destroyed: false);
		}
		panel.SetActive(value: false);
	}
}
