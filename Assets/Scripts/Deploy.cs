using UnityEngine;

public class Deploy : MonoBehaviour
{
	public enum Ground
	{
		norm,
		none,
		noAlign,
		heightAdjust
	}

	public enum Water
	{
		none,
		under,
		on,
		underOnly
	}

	public enum Place
	{
		norm,
		up,
		forward,
		down,
		center,
		top,
		sign,
		none,
		backward,
		snap,
		dropbox
	}

	public enum copyPaste
	{
		none,
		flip,
		up90,
		r90,
		origin
	}

	public Material mat;

	public Material selectMat;

	public GameObject mouseOverObj;

	public bool mouseOverOn;

	public igniteThis ignitable;

	public LockEntity lockEnt;

	public lightCtrl lc;

	public Ground ground;

	public Water water;

	public Place place;

	public copyPaste copypaste;

	public bool frameWall;

	public bool frameFloor;

	public bool windowPart;

	public bool shelf;

	public bool onWall;

	public bool vertical;

	public bool placedOnNoRot;

	public Transform offset;

	public Transform[] corners;

	public Transform origin;

	public Transform[] snapPoints;

	public bool cornerCheck()
	{
		if (placeOptions.ignoreRules)
		{
			return false;
		}
		int num = corners.Length;
		for (int i = 0; i < num; i++)
		{
			if (corners[i] != null && !Physics.Raycast(corners[i].position, -corners[i].up, 0.075f, 1))
			{
				return true;
			}
		}
		return false;
	}

	public Transform getClosestSnap(Vector3 hitPos)
	{
		int num = 0;
		float num2 = 100f;
		for (int i = 0; i < snapPoints.Length; i++)
		{
			float magnitude = (snapPoints[i].position - hitPos).magnitude;
			if (magnitude < num2)
			{
				num2 = magnitude;
				num = i;
			}
		}
		return snapPoints[num];
	}

	public GameObject getParentFloorFrame(BuilderPart bp)
	{
		GameObject result = null;
		GameObject gameObject = base.gameObject;
		LayerMask mask = gameObject.layer;
		gameObject.layer = 2;
		if (Physics.Raycast(bp.center.position, bp._transform.right, out RaycastHit hitInfo, 0.49f, 1) && hitInfo.transform.tag == "block")
		{
			result = hitInfo.transform.gameObject;
		}
		gameObject.layer = mask;
		return result;
	}
}
