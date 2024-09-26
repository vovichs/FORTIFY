using Steamworks;
using UnityEngine;
using UnityEngine.UI;

public class OtherPlayer : MonoBehaviour
{
	public CSteamID steamid;

	public Transform _transform;

	public Transform sittingPos;

	public GameObject meshChild;

	public Transform head;

	public Transform body;

	public Transform bodySit;

	public GameObject jetPack;

	public GameObject miniCopter;

	private int colorIndex = -1;

	public Text nameText;

	public Transform part;

	private float maxAngularSpeed = 180f;

	private float closingSpeed;

	public float minSmoothSpeed = 4f;

	public float catchupMulti = 2f;

	public Vector3 newPos;

	public Quaternion newRot;

	public float headTiltX;

	private Vector3 offset = new Vector3(1.5f, 0f, 1.5f);

	private void Awake()
	{
		newPos = _transform.position;
		newRot = _transform.rotation;
	}

	public void Update()
	{
		Vector3 a = newPos - _transform.position;
		float magnitude = a.magnitude;
		if (magnitude > 0f)
		{
			closingSpeed = Mathf.Max(closingSpeed, Mathf.Max(minSmoothSpeed, magnitude * catchupMulti));
			float num = Mathf.Min(Time.deltaTime * closingSpeed, magnitude);
			a *= num / magnitude;
			_transform.position += a;
		}
		else
		{
			closingSpeed = 0f;
		}
		float num2 = Quaternion.Angle(_transform.rotation, newRot);
		if (num2 > 0f)
		{
			float t = Mathf.Min(Time.deltaTime * maxAngularSpeed, num2) / num2;
			_transform.rotation = Quaternion.Slerp(_transform.rotation, newRot, t);
		}
		if ((bool)head && (headTiltX < 15f || headTiltX > 320f))
		{
			head.localRotation = Quaternion.Euler(new Vector3(headTiltX, 0f, 0f));
		}
	}

	public void playerSetup(CSteamID id)
	{
		steamid = id;
		foreach (Text playerName in Multiplayer.inst.playerNames)
		{
			if (!playerName.enabled)
			{
				nameText = playerName;
				break;
			}
		}
		nameText.enabled = true;
		nameText.text = SteamFriends.GetFriendPersonaName(id);
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load("foundation"), new Vector3(0f, -1000f, 0f), Quaternion.identity) as GameObject;
		part = gameObject.transform;
		part.GetChild(0).gameObject.SetActive(value: false);
	}

	public void avatarMeshSetup(int avatarId)
	{
		if (!meshChild)
		{
			GameObject original = MGMT.inst.avatars[avatarId];
			meshChild = UnityEngine.Object.Instantiate(original, _transform.position, _transform.rotation, _transform);
			Transform transform = meshChild.transform;
			head = transform.Find("head");
			body = transform.Find("body");
			bodySit = transform.Find("bodySit");
			jetPack = transform.Find("jetpack").gameObject;
		}
	}

	public void setColor(int _colorIndex)
	{
		if (_colorIndex != colorIndex)
		{
			colorIndex = _colorIndex;
			Material material = body.GetComponent<Renderer>().materials[0];
			material.SetColor("_Color", Multiplayer.inst.colors[colorIndex]);
			head.GetComponent<Renderer>().material = material;
			body.GetComponent<Renderer>().material = material;
			bodySit.GetComponent<Renderer>().material = material;
			if (nameText != null)
			{
				nameText.color = Multiplayer.inst.colors[colorIndex];
				part.GetComponent<Renderer>().sharedMaterial = Multiplayer.inst._Receiver.partColors[colorIndex];
			}
		}
	}

	public void miniCopterState(bool mount)
	{
		if ((bool)meshChild && mount != miniCopter.activeSelf)
		{
			if (mount)
			{
				meshChild.transform.localPosition = sittingPos.localPosition;
				body.gameObject.SetActive(value: false);
				bodySit.gameObject.SetActive(value: true);
				jetPack.SetActive(value: false);
				miniCopter.SetActive(value: true);
				meshChild.GetComponent<floatMotion>().enabled = false;
			}
			else
			{
				meshChild.transform.localPosition = Vector3.zero;
				body.gameObject.SetActive(value: true);
				bodySit.gameObject.SetActive(value: false);
				jetPack.SetActive(value: true);
				miniCopter.SetActive(value: false);
				meshChild.GetComponent<floatMotion>().enabled = true;
			}
		}
	}

	public void DestroyPlayer()
	{
		if ((bool)part)
		{
			UnityEngine.Object.Destroy(part.gameObject);
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	[ContextMenu("miniCopterTest1")]
	public void miniCopterStateTest1()
	{
		avatarMeshSetup(0);
		miniCopterState(mount: true);
	}

	[ContextMenu("miniCopterTest2")]
	public void miniCopterStateTest2()
	{
		miniCopterState(mount: false);
	}
}
