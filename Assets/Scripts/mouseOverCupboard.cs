using UnityEngine;
using UnityEngine.UI;

public class mouseOverCupboard : MonoBehaviour
{
	public static mouseOverCupboard inst;

	public cupboardRange CR;

	public Vector3 pos;

	public Image image;

	public bool hidden;

	public Transform cupboard;

	private void Awake()
	{
		inst = this;
	}

	private void Update()
	{
		Vector3 vector = CameraCtrl.inst.cam.WorldToScreenPoint(pos);
		if (vector.z > 10f || vector.z < 0.1f)
		{
			if (!hidden)
			{
				image.enabled = false;
				hidden = true;
			}
			return;
		}
		if (hidden)
		{
			image.enabled = true;
			hidden = false;
		}
		base.transform.position = vector;
	}

	public void over(Transform t)
	{
		cupboard = t;
		pos = cupboard.GetComponent<Collider>().bounds.center;
		base.enabled = true;
	}

	public void showRange()
	{
		CR.getStructureGroups(getDeploys: false);
	}

	private void OnEnable()
	{
		hidden = false;
		image.enabled = true;
	}

	private void OnDisable()
	{
		cupboard = null;
		image.enabled = false;
	}
}
