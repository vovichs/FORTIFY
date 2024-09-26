using UnityEngine;

public class DontDestroy : MonoBehaviour
{
	private bool start;

	private void Awake()
	{
		if (start)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		start = true;
		Object.DontDestroyOnLoad(this);
	}
}
