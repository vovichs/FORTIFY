using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RF_controller : MonoBehaviour
{
	public static RF_controller inst;

	public List<RF_receiver> receivers;

	public Dictionary<int, int> freqs = new Dictionary<int, int>();

	public Dictionary<int, int> receiveFreqs = new Dictionary<int, int>();

	public GameObject recieveButton;

	public Transform rListParent;

	public GameObject broadcastButton;

	public Transform bListParent;

	private void Awake()
	{
		inst = this;
	}

	public void freqChange(int freq, bool state)
	{
		for (int num = receivers.Count - 1; num >= 0; num--)
		{
			if (receivers[num] == null)
			{
				receivers.RemoveAt(num);
			}
			else if (receivers[num].value == freq)
			{
				receivers[num].freqReceived(state);
			}
		}
	}

	public void broadcastChange(int f, bool on)
	{
		if (on)
		{
			if (freqs.ContainsKey(f))
			{
				Dictionary<int, int> dictionary = freqs;
				dictionary[f]++;
			}
			else
			{
				freqs.Add(f, 1);
				freqChange(f, state: true);
				addBroadcastButton(f);
			}
		}
		else if (freqs.ContainsKey(f))
		{
			Dictionary<int, int> dictionary = freqs;
			dictionary[f]--;
			if (freqs[f] == 0)
			{
				freqChange(f, state: false);
				freqs.Remove(f);
				removeBroadcastButton(f);
			}
		}
	}

	public void receiverChange(int f, bool on)
	{
		if (on)
		{
			if (receiveFreqs.ContainsKey(f))
			{
				Dictionary<int, int> dictionary = receiveFreqs;
				dictionary[f]++;
			}
			else
			{
				receiveFreqs.Add(f, 1);
				addRecieveButton(f);
			}
		}
		else if (receiveFreqs.ContainsKey(f))
		{
			Dictionary<int, int> dictionary = receiveFreqs;
			dictionary[f]--;
			if (receiveFreqs[f] == 0)
			{
				receiveFreqs.Remove(f);
				removeReceiveButton(f);
			}
		}
	}

	public void addRecieveButton(int f)
	{
		string text = f.ToString();
		GameObject gameObject = UnityEngine.Object.Instantiate(recieveButton);
		gameObject.name = text;
		gameObject.GetComponentInChildren<Text>().text = text;
		gameObject.transform.SetParent(rListParent, worldPositionStays: false);
	}

	public void removeReceiveButton(int f)
	{
		if ((bool)rListParent)
		{
			string a = f.ToString();
			foreach (Transform item in rListParent)
			{
				if ((bool)item && a == item.gameObject.name)
				{
					if (item.GetComponent<Toggle>().isOn)
					{
						broadcastChange(f, on: false);
					}
					UnityEngine.Object.Destroy(item.gameObject);
					break;
				}
			}
		}
	}

	public void addBroadcastButton(int f)
	{
		string text = f.ToString();
		GameObject gameObject = UnityEngine.Object.Instantiate(broadcastButton);
		gameObject.name = text;
		gameObject.GetComponentInChildren<Text>().text = text;
		gameObject.transform.SetParent(bListParent, worldPositionStays: false);
	}

	public void removeBroadcastButton(int f)
	{
		string a = f.ToString();
		foreach (Transform item in bListParent)
		{
			if ((bool)item && a == item.gameObject.name)
			{
				UnityEngine.Object.Destroy(item.gameObject);
				break;
			}
		}
	}
}
