using UnityEngine;

public class camAlignStatic : MonoBehaviour
{
	public static camAlignStatic inst;

	public static Quaternion camRot;

	private Transform camT;

	private void Awake()
	{
		inst = this;
		camT = BuilderSystem.inst._transform;
	}

	private void Update()
	{
		camRot = Quaternion.LookRotation(camT.forward, Vector3.up);
	}
}
