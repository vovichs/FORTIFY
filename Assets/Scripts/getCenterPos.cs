using UnityEngine;

public class getCenterPos : MonoBehaviour
{
	[ExecuteInEditMode]
	[ContextMenu("get bounds center")]
	private void getCenter()
	{
		UnityEngine.Debug.Log(GetComponent<MeshFilter>().sharedMesh.bounds.center);
	}
}
