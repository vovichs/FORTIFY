using UnityEngine;

[ExecuteInEditMode]
public class cloudSpawn : MonoBehaviour
{
	public bool reset;

	private GameObject[] clouds;

	public GameObject[] prefabs;

	public int size = 400;

	public int elevationLow = 50;

	public int elevationHigh = 150;

	public int numberOfClouds = 140;

	private void OnEnable()
	{
		if (reset)
		{
			clouds = new GameObject[numberOfClouds];
			InstantiateClouds();
			combineMesh();
		}
	}

	private void InstantiateClouds()
	{
		int num = 0;
		GameObject original = null;
		for (int i = 0; i < numberOfClouds; i++)
		{
			if (num == 0)
			{
				original = prefabs[0];
			}
			if (num == 2)
			{
				original = prefabs[1];
				num = 0;
			}
			num++;
			clouds[i] = UnityEngine.Object.Instantiate(original, randomPos(), UnityEngine.Random.rotation);
			int num2 = UnityEngine.Random.Range(100, 280);
			clouds[i].transform.localScale = new Vector3(num2, num2, num2);
			clouds[i].transform.parent = base.transform;
		}
	}

	private Vector3 randomPos()
	{
		int num = UnityEngine.Random.Range(-size, size);
		int num2 = UnityEngine.Random.Range(elevationLow, elevationHigh);
		int num3 = UnityEngine.Random.Range(-size, size);
		return new Vector3(num, num2, num3);
	}

	private void combineMesh()
	{
		CombineInstance[] array = new CombineInstance[GetComponentsInChildren<MeshFilter>().Length];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			array[i].mesh = base.transform.GetChild(i).GetComponent<MeshFilter>().sharedMesh;
			array[i].transform = base.transform.GetChild(i).localToWorldMatrix;
			base.transform.GetChild(i).gameObject.SetActive(value: false);
		}
		base.transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
		base.transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(array, mergeSubMeshes: true);
		base.transform.GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
		while (base.transform.childCount > 0)
		{
			UnityEngine.Object.DestroyImmediate(base.transform.GetChild(0).gameObject);
		}
	}
}
