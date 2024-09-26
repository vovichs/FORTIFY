using UnityEngine;

public class RF_transmitter : MonoBehaviour
{
	private bool state;

	public void buttonTransmitSet()
	{
		state = !state;
		RF_controller.inst.broadcastChange(int.Parse(base.gameObject.name), state);
	}
}
