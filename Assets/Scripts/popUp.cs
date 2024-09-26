using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class popUp : MonoBehaviour
{
	public static popUp inst;

	private static int openCount;

	public static GameObject[] messages;

	private void Awake()
	{
		inst = this;
		openCount = 0;
		messages = new GameObject[base.transform.childCount];
		for (int i = 0; i < base.transform.childCount; i++)
		{
			messages[i] = base.transform.GetChild(i).gameObject;
		}
	}

	public void message(string text)
	{
		if (openCount != 3)
		{
			GameObject gameObject = messages[openCount];
			gameObject.SetActive(value: true);
			openCount++;
			gameObject.GetComponent<Text>().text = text;
			StartCoroutine(showMessage(gameObject));
		}
	}

	private IEnumerator showMessage(GameObject obj)
	{
		obj.SetActive(value: true);
		yield return new WaitForSeconds(2.5f);
		obj.SetActive(value: false);
		openCount--;
	}
}
