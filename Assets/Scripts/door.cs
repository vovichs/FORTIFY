using System;
using System.Collections;
using UnityEngine;

public class door : MonoBehaviour
{
	public enum Type
	{
		door,
		hatch,
		garage,
		gate
	}

	[NonSerialized]
	public bool open = true;

	public Type type;

	public GameObject ladderArea;

	public MeshFilter mf;

	public Mesh openMesh;

	public Mesh closeMesh;

	public Collider closeCol;

	public Collider openCol;

	public AudioSource AS;

	public doorController doorCtrl;

	public Transform doorCtrlCheckPos;

	public IEnumerator getDoorParent(BuilderPart bp)
	{
		if (!bp || type == Type.gate)
		{
			yield break;
		}
		yield return null;
		GameObject gameObject = base.gameObject;
		if (type == Type.hatch)
		{
			GameObject parentFloorFrame = bp.deploy.getParentFloorFrame(bp);
			if ((bool)parentFloorFrame)
			{
				parentFloorFrame.GetComponent<BuilderPart>().sObj = gameObject;
			}
			yield break;
		}
		Vector3 origin = bp._transform.position + new Vector3(0f, 0.7f, 0f);
		LayerMask mask = gameObject.layer;
		gameObject.layer = 2;
		if (Physics.Raycast(origin, Vector3.up, out RaycastHit hitInfo, 0.3f, 1))
		{
			GameObject gameObject2 = hitInfo.collider.gameObject;
			if (gameObject2.tag == "block")
			{
				gameObject2.GetComponent<BuilderPart>().sObj = gameObject;
			}
		}
		gameObject.layer = mask;
	}

	public IEnumerator playSound(bool open)
	{
		AS.enabled = true;
		if (type == Type.door || type == Type.hatch)
		{
			if (open)
			{
				AS.clip = AudioPlayer.inst.doorOpen;
			}
			else
			{
				AS.clip = AudioPlayer.inst.doorClose;
			}
		}
		AS.Play();
		while (AS.isPlaying)
		{
			yield return null;
		}
		AS.enabled = false;
	}

	public void controlCheck()
	{
		Vector3 position = doorCtrlCheckPos.position;
		Collider[] array = Physics.OverlapSphere(position, 0.6f, 16777216);
		doorCtrl = null;
		float num = 0.55f;
		for (int i = 0; i < array.Length; i++)
		{
			doorController component = array[i].GetComponent<doorController>();
			if ((bool)component && !component.doorBP && (!component.rotatedForHatch() || type == Type.hatch))
			{
				float magnitude = (component.owner._transform.position - position).magnitude;
				if (!(magnitude > num))
				{
					num = magnitude;
					doorCtrl = component;
				}
			}
		}
		if ((bool)doorCtrl)
		{
			doorCtrl.connect(GetComponent<BuilderPart>());
		}
	}
}
