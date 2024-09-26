using UnityEngine;

[ExecuteInEditMode]
public class TerrainSplats : MonoBehaviour
{
	public bool random;

	private Terrain terrain;

	private TerrainData terrainData;

	private Vector3 terrainPosition = Vector3.zero;

	private float[,,] element;

	private float[,,] map;

	private int mapX;

	private int mapY;

	private float offset;

	[ContextMenu("getSetTerrain")]
	private void getSetTerrain()
	{
		terrain = GetComponent<Terrain>();
		if ((bool)terrain)
		{
			terrainData = terrain.terrainData;
			offset = terrain.transform.position.y - 2f;
			setSplats();
		}
	}

	private void setSplats()
	{
		float[,,] array = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
		for (int i = 0; i < terrainData.alphamapHeight; i++)
		{
			for (int j = 0; j < terrainData.alphamapWidth; j++)
			{
				float num = (float)i / (float)terrainData.alphamapHeight;
				float num2 = (float)j / (float)terrainData.alphamapWidth;
				int x = Mathf.RoundToInt(num * (float)terrainData.heightmapResolution);
				int y = Mathf.RoundToInt(num2 * (float)terrainData.heightmapResolution);
				float num3 = terrainData.GetHeight(x, y) - offset;
				float[] array2 = new float[terrainData.alphamapLayers];
				int num4 = 1;
				if (UnityEngine.Random.Range(0, 2) < 1)
				{
					num4 = 2;
				}
				if (num3 < 2f)
				{
					array2[3] = 1f;
				}
				else if (num3 >= 2f && (double)num3 < 2.1)
				{
					array2[3] = 1f - (array2[0] = (num3 - 2f) / 0.1f);
				}
				else if (num3 >= 2f && num3 < 4f)
				{
					array2[0] = 1f - (array2[num4] = (num3 - 2f) / 2f);
				}
				else
				{
					array2[num4] = 1f;
				}
				for (int k = 0; k < terrainData.alphamapLayers; k++)
				{
					array[j, i, k] = array2[k];
				}
			}
		}
		terrainData.SetAlphamaps(0, 0, array);
	}

	private void randomize()
	{
		float[,,] array = new float[terrain.terrainData.alphamapWidth, terrain.terrainData.alphamapHeight, terrain.terrainData.alphamapLayers];
		element = new float[1, 1, terrain.terrainData.alphamapLayers];
		terrainData = terrain.terrainData;
		for (int i = 0; i < terrainData.alphamapHeight; i++)
		{
			for (int j = 0; j < terrainData.alphamapWidth; j++)
			{
				array[j, i, 0] = 0f;
				if (UnityEngine.Random.Range(0, 2) > 0)
				{
					array[j, i, 1] = 1f;
					array[j, i, 2] = 0f;
				}
				else
				{
					array[j, i, 1] = 0f;
					array[j, i, 2] = 1f;
				}
			}
		}
		terrainData.SetAlphamaps(0, 0, array);
	}

	public void UpdateTerrain(bool dirt, Vector3 pos)
	{
		mapX = (int)((pos.x - terrainPosition.x) / terrainData.size.x * (float)terrainData.alphamapWidth);
		mapY = (int)((pos.z - terrainPosition.z) / terrainData.size.z * (float)terrainData.alphamapHeight);
		if (dirt)
		{
			map[mapY, mapX, 0] = (element[0, 0, 0] = 1f);
			map[mapY, mapX, 1] = (element[0, 0, 1] = 0f);
			map[mapY, mapX, 2] = (element[0, 0, 2] = 0f);
		}
		else
		{
			map[mapY, mapX, 0] = (element[0, 0, 0] = 0f);
			map[mapY, mapX, 1] = (element[0, 0, 1] = 1f);
			map[mapY, mapX, 2] = (element[0, 0, 2] = 0f);
		}
		terrainData.SetAlphamaps(mapX, mapY, element);
	}
}
