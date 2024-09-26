using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class TestSpeed : MonoBehaviour
{
	public BuilderSystem cs;

	public Transform mirrorPlane;

	protected int blocks;

	protected int[] brackets;

	protected float[] bracketRate;

	public Bounds bounds;

	[ContextMenu("bounds")]
	public void boundsExtents()
	{
		MonoBehaviour.print(bounds.extents.magnitude);
	}

	[ContextMenu("selection check")]
	public void objListCheck()
	{
		int num = 0;
		foreach (BuilderPart bp in cs.bpList)
		{
			if (bp.selected)
			{
				num++;
			}
		}
		MonoBehaviour.print("selected -" + num);
		MonoBehaviour.print(cs.objList.Count);
	}

	[ContextMenu("disableColliders")]
	public void disableColliders()
	{
		foreach (BuilderPart bp in cs.bpList)
		{
			if ((bool)bp.block)
			{
				Transform[] edgeCols = bp.block.edgeCols;
				for (int i = 0; i < edgeCols.Length; i++)
				{
					edgeCols[i].gameObject.SetActive(value: false);
				}
				GameObject[] baseCols = bp.block.baseCols;
				for (int i = 0; i < baseCols.Length; i++)
				{
					baseCols[i].SetActive(value: false);
				}
			}
		}
	}

	[ContextMenu("bpListCheck")]
	public void bpListCheck()
	{
		int num = 0;
		foreach (BuilderPart bp in cs.bpList)
		{
			if (!bp)
			{
				num++;
			}
		}
		MonoBehaviour.print(num);
	}

	[ContextMenu("upkeepCalc")]
	public void CalculateBuildingTaxRate()
	{
		int num = blocks;
		float[] array = new float[4];
		float num2 = 0f;
		for (int i = 0; i < brackets.Length; i++)
		{
			array[i] = 0f;
			if (num > 0)
			{
				int num3 = 0;
				num3 = ((i != brackets.Length - 1) ? Mathf.Min(num, brackets[i]) : num);
				num -= num3;
				num2 += (float)num3 * bracketRate[i];
			}
		}
		num2 /= (float)blocks;
		num2 = Mathf.Round(num2 * 1000f) / 1000f;
		MonoBehaviour.print(num2);
	}

	[ContextMenu("findWithTag")]
	public void Test0()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject.FindGameObjectsWithTag("placedPart");
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("findWithinScene")]
	public void Test1()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		List<GameObject> rootGameObjects = new List<GameObject>();
		SceneManager.GetActiveScene().GetRootGameObjects(rootGameObjects);
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("instGetCompTest")]
	public void TestInst()
	{
		GameObject gameObject = wiring.inst.wirePrefab.gameObject;
		wire wirePrefab = wiring.inst.wirePrefab;
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int i = 0; i < 100; i++)
		{
			Object.Instantiate(wirePrefab).colorIndex = 1;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
		stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int j = 0; j < 100; j++)
		{
			Object.Instantiate(gameObject).GetComponent<wire>().colorIndex = 1;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("listAccessPerf")]
	public void ListTest()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int num = cs.bpList.Count - 1; num >= 0; num--)
		{
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
			cs.bpList[num].check = true;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
		stopwatch = new Stopwatch();
		stopwatch.Start();
		for (int num2 = cs.bpList.Count - 1; num2 >= 0; num2--)
		{
			BuilderPart builderPart = cs.bpList[num2];
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
			builderPart.check = true;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("getFromList")]
	public void Test2()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		foreach (BuilderPart bp in cs.bpList)
		{
			GameObject gameObject = bp.gameObject;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("colliderStateOld")]
	public void Test3()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		GameObject[] array = GameObject.FindGameObjectsWithTag("placedPart");
		foreach (GameObject gameObject in array)
		{
			if (!gameObject.GetComponent<Renderer>().enabled)
			{
				continue;
			}
			for (int j = 1; j < gameObject.transform.childCount; j++)
			{
				GameObject gameObject2 = gameObject.transform.GetChild(j).gameObject;
				if (gameObject2.tag == "edgeCollider")
				{
					gameObject2.layer = 16;
				}
			}
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("colliderStateNew")]
	public void Test4()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		foreach (BuilderPart bp in cs.bpList)
		{
			bool flag = bp.block != null;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("collidersOff")]
	public void Test5()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		foreach (BuilderPart bp in cs.bpList)
		{
			if (bp.block != null)
			{
				Transform[] edgeCols = bp.block.edgeCols;
				for (int i = 0; i < edgeCols.Length; i++)
				{
					edgeCols[i].gameObject.SetActive(value: false);
				}
			}
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}

	[ContextMenu("transform")]
	public void Test6()
	{
		Stopwatch stopwatch = new Stopwatch();
		stopwatch.Start();
		foreach (BuilderPart bp in cs.bpList)
		{
			Transform transform = bp.transform;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
		stopwatch.Reset();
		stopwatch.Start();
		foreach (BuilderPart bp2 in cs.bpList)
		{
			Transform transform2 = bp2._transform;
		}
		stopwatch.Stop();
		MonoBehaviour.print(stopwatch.Elapsed);
	}
}
