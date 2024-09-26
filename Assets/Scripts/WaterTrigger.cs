using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if ((bool)MiniCopter.inst && MiniCopter.inst.flying)
		{
			MiniCopter.inst.engineThrustMax = 0f;
		}
	}
}
