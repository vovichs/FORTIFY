using System.Collections.Generic;
using UnityEngine;

public class overlapCheck : MonoBehaviour
{
	public static overlapCheck inst;

	public static bool overlap;

	public static int overlapCount;

	public bool count;

	public bool overfurnace;

	public bool lowWall;

	public bool roof;

	public bool rockBlock;

	[Header("allows storage adapter snap - mainCollider ")]
	public bool deployMainCol;

	[Header("collider enabled after placing")]
	public bool topColHidden;

	[Header("volume should start off")]
	public Collider volume;

	public GameObject volumeObj;

	public List<Collider> colliders = new List<Collider>();

	private void Start()
	{
		inst = this;
	}

	private void FixedUpdate()
	{
		overlap = (!placeOptions.allowOverlap && overlapCount > 0);
	}

	private void OnTriggerEnter(Collider col)
	{
		if (count && triggerCheck(col, exit: false))
		{
			overlapCount++;
		}
		if ((col.tag == "deployIgnoreCollider" || col.tag == "deployCollider" || col.tag == "mainCollider" || col.tag == "blockCollider" || col.tag == "roofCollider") && ((!(base.tag == "deployCollider") && !(base.tag == "deployIgnoreCollider")) || (!(col.tag != "deployCollider") && col.isTrigger)))
		{
			colliders.Add(col);
		}
	}

	public void OnTriggerExit(Collider col)
	{
		if (count && triggerCheck(col, exit: true) && overlapCount > 0)
		{
			overlapCount--;
		}
		if (colliders.Contains(col))
		{
			colliders.Remove(col);
		}
	}

	public bool triggerCheck(Collider col, bool exit)
	{
		if (base.tag == "deployCollider")
		{
			if (rockBlock && col.tag == "rock")
			{
				return true;
			}
			if (col.isTrigger)
			{
				if (col.tag == "deployIgnoreCollider")
				{
					return false;
				}
				if (overfurnace && col.tag == "furnaceCollider")
				{
					return false;
				}
				if (col.tag == "pipeCollider")
				{
					return false;
				}
				if (col.tag == "deployCollider" || col.tag == "furnaceCollider" || col.tag == "blockCollider" || col.tag == "roofCollider")
				{
					return true;
				}
			}
			else if ((col.tag == "block" || (col.tag == "deploy" && col.name != "watchtower_wood")) && col.gameObject.layer != 2)
			{
				return true;
			}
		}
		else
		{
			if (base.tag == "deployIgnoreCollider")
			{
				return col.tag == "deployIgnoreCollider";
			}
			if (col.isTrigger)
			{
				if (col.tag == "pipeCollider")
				{
					return false;
				}
				if (!lowWall)
				{
					if (roof && col.tag == "roofCollider")
					{
						return false;
					}
					if (overfurnace && col.tag == "furnaceCollider")
					{
						return false;
					}
					return true;
				}
				if (col.tag != "deployCollider")
				{
					return true;
				}
			}
			else if (deployMainCol && col.gameObject.layer != 2 && (col.tag == "block" || col.tag == "deploy"))
			{
				return true;
			}
		}
		return false;
	}

	public void DestroyOverlapObj()
	{
		for (int i = 0; i < colliders.Count; i++)
		{
			Collider collider = colliders[i];
			if (collider == null || collider.tag == "blockCollider" || (roof && collider.tag == "roofCollider"))
			{
				continue;
			}
			GameObject gameObject = collider.transform.root.gameObject;
			if (!(gameObject.tag == "destroy"))
			{
				BuilderPart component = gameObject.GetComponent<BuilderPart>();
				if (!(component == null))
				{
					MGMT.inst.addDeletedPart(component, undo: true);
					BuilderSystem.inst.destroyPart(component, audio: false, receive: false);
				}
			}
		}
		overlap = false;
	}

	public bool DestroySelfOverlap(BuilderPart bp, bool send)
	{
		for (int i = 0; i < colliders.Count; i++)
		{
			Collider collider = colliders[i];
			if (!collider)
			{
				continue;
			}
			GameObject gameObject = collider.transform.root.gameObject;
			if (collider.tag == "blockCollider" || collider.tag == "destroy" || gameObject.layer == 2)
			{
				continue;
			}
			if (roof && collider.tag == "roofCollider")
			{
				if (!(Vector3.Distance(gameObject.transform.position, bp._transform.position) > 0.01f))
				{
					return true;
				}
			}
			else if (gameObject.layer != 1)
			{
				if (send && BuilderSystem.multiplayer)
				{
					Multiplayer.destroySend(bp);
				}
				return true;
			}
		}
		return false;
	}

	public void deployVolume()
	{
		overlapCount = 0;
		count = true;
		AddRigidbody();
		if ((bool)volume)
		{
			volume.enabled = true;
		}
		if ((bool)volumeObj)
		{
			volumeObj.SetActive(value: true);
		}
	}

	public void placedVolume()
	{
		if ((bool)volume)
		{
			volume.enabled = false;
		}
		if ((bool)volumeObj)
		{
			volumeObj.SetActive(value: false);
		}
		Rigidbody component = GetComponent<Rigidbody>();
		if ((bool)component)
		{
			UnityEngine.Object.Destroy(component);
		}
		if (topColHidden)
		{
			GetComponent<Collider>().enabled = true;
		}
		UnityEngine.Object.Destroy(this);
	}

	public void AddRigidbody()
	{
		Rigidbody rigidbody = base.gameObject.AddComponent<Rigidbody>();
		rigidbody.useGravity = false;
		rigidbody.isKinematic = true;
		if (topColHidden && (bool)volume)
		{
			volume.enabled = true;
		}
	}

	public void reset()
	{
		colliders.Clear();
		overlapCount = 0;
	}
}
