using UnityEngine;

public class IncludePartWith : MonoBehaviour
{
	public static IncludePartWith inst;

	public string windowPart = "window_bars_metal";

	public string doorPart = "door_metal";

	public string wallFramePart = "wf_ddoor_metal";

	public string floorFramePart = "ff_hatch";

	public string floorFrameTriPart = "hatch_tri";

	public GameObject[] rectImages;

	public void Awake()
	{
		inst = this;
	}
}
