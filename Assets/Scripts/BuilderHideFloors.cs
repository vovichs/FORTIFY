using UnityEngine;
using UnityEngine.UI;

public class BuilderHideFloors : MonoBehaviour
{
	private BuilderSystem sys;

	private wire[] wires;

	public GameObject panel;

	public Toggle options;

	public Text floorLvl;

	public Toggle halfLevel;

	public Toggle roofPlus;

	private bool stageMode;

	private float floorNum = 1f;

	private float stageNum = 9f;

	public Text textFloors;

	public Text textStages;

	public Image image1;

	public Image image2;

	public Toggle useHeight;

	public Toggle hideBlocksOnly;

	private bool hideDeploys;

	public bool HideDeploys
	{
		get
		{
			return hideDeploys;
		}
		set
		{
			hideDeploys = !hideDeploys;
			if (hideDeploys)
			{
				foreach (BuilderPart bp in sys.bpList)
				{
					if ((bool)bp.deploy && !bp.hidden)
					{
						HidePart(bp);
					}
				}
			}
			else if (sys.hideMode)
			{
				HideFloors(0, send: false);
			}
			else
			{
				foreach (BuilderPart bp2 in sys.bpList)
				{
					if ((bool)bp2.deploy && bp2.hidden)
					{
						ShowPart(bp2);
					}
				}
			}
		}
	}

	private void Awake()
	{
		sys = BuilderSystem.inst;
	}

	public void stageModeToggle()
	{
		stageMode = !stageMode;
		Sprite sprite = image1.sprite;
		image1.sprite = image2.sprite;
		image2.sprite = sprite;
		textFloors.gameObject.SetActive(!stageMode);
		textStages.gameObject.SetActive(stageMode);
		if (sys.hideMode)
		{
			HideFloors(0, send: true);
		}
	}

	public void HideFloorsState()
	{
		sys.hideMode = !sys.hideMode;
		if (!sys.hideMode)
		{
			foreach (BuilderPart bp in sys.bpList)
			{
				if ((!bp.deploy || !hideDeploys) && bp.hidden)
				{
					ShowPart(bp);
				}
			}
			if (!Stability.inst.raidMode)
			{
				foreach (wire wire in BuilderSystem.wireList)
				{
					if ((bool)wire && !wire.lr.enabled)
					{
						wire.showWire(state: true);
					}
				}
			}
			if (options.isOn)
			{
				options.isOn = false;
			}
		}
		else
		{
			HideFloors(0, send: false);
		}
	}

	public void HideFloors(int change, bool send)
	{
		if (!sys.hideMode)
		{
			return;
		}
		ioTrigger.inst.ioTriggerCol.position = Vector3.zero;
		bool flag = !stageMode && roofPlus.isOn;
		bool isOn = halfLevel.isOn;
		bool isOn2 = hideBlocksOnly.isOn;
		float num;
		switch (change)
		{
		case -2:
			num = 1f;
			break;
		case -3:
		{
			if (stageMode)
			{
				num = 9f;
				break;
			}
			float num2 = 1f;
			for (int i = 0; i < sys.bpList.Count; i++)
			{
				if (sys.bpList[i].level > num2)
				{
					num2 = sys.bpList[i].level;
				}
			}
			num = ((!isOn) ? ((float)Mathf.CeilToInt(num2)) : num2);
			break;
		}
		default:
			if (!stageMode)
			{
				num = floorNum;
				if (isOn)
				{
					switch (change)
					{
					case -1:
						num -= 0.5f;
						break;
					case 1:
						num += 0.5f;
						break;
					}
				}
				else
				{
					num += (float)change;
				}
				if (num < 0f)
				{
					return;
				}
			}
			else
			{
				num = stageNum;
				num += (float)change;
				if (num < 1f || num > 9f)
				{
					return;
				}
			}
			break;
		}
		if (stageMode)
		{
			stageNum = num;
		}
		else
		{
			floorNum = num;
		}
		floorLvl.text = ">" + num.ToString();
		foreach (BuilderPart bp in sys.bpList)
		{
			if (!bp.deploy || !hideDeploys)
			{
				if (floorNum < 1f)
				{
					if (!bp.found)
					{
						if (!((bool)bp.deploy && isOn2) && !bp.hidden)
						{
							HidePart(bp);
						}
					}
					else if (bp.hidden && (float)bp.stage < stageNum)
					{
						ShowPart(bp);
					}
				}
				else
				{
					float num3 = (!useHeight.isOn) ? bp.level : (bp._transform.position.y + 0.25f);
					if (flag && (bool)bp.block && bp.block.roof)
					{
						num3 += 1f;
					}
					if (num3 > floorNum || (float)bp.stage > stageNum)
					{
						if (!((bool)bp.deploy && isOn2) && !bp.hidden)
						{
							HidePart(bp);
						}
					}
					else if (bp.hidden)
					{
						ShowPart(bp);
					}
				}
			}
		}
		if (!Stability.inst.raidMode && !isOn2)
		{
			foreach (wire wire in BuilderSystem.wireList)
			{
				if ((bool)wire)
				{
					if (wire.output.dev.owner.hidden && wire.input.dev.owner.hidden)
					{
						wire.showWire(state: false);
					}
					else
					{
						wire.showWire(state: true);
					}
				}
			}
		}
	}

	private void HidePart(BuilderPart bp)
	{
		if (bp == null)
		{
			return;
		}
		GameObject gameObject = bp.gameObject;
		if ((bool)bp.block)
		{
			bp.block.setColliderLayer(2, 2, 2);
		}
		else
		{
			if ((bool)bp.device)
			{
				bp.device.deviceHideState(hide: false);
			}
			GameObject mouseOverObj = bp.deploy.mouseOverObj;
			if ((bool)mouseOverObj && mouseOverObj.activeSelf)
			{
				mouseOverObj.SetActive(value: false);
			}
		}
		Renderer[] rend = bp.rend;
		for (int i = 0; i < rend.Length; i++)
		{
			rend[i].enabled = false;
		}
		gameObject.layer = 2;
		bp.hidden = true;
	}

	private void ShowPart(BuilderPart bp)
	{
		if (!(bp == null))
		{
			if ((bool)bp.device)
			{
				bp.device.deviceHideState(hide: true);
			}
			Renderer[] rend = bp.rend;
			for (int i = 0; i < rend.Length; i++)
			{
				rend[i].enabled = true;
			}
			if ((bool)bp.block && !wiring.inst.on && !BuilderSystem.editMode && !BuilderSystem.placeMode)
			{
				bp.block.getColliderLayer((int)sys.placeColMode);
			}
			bp.setRaycastLayer();
			bp.hidden = false;
		}
	}

	public void hideBlocksOnlyState()
	{
		if (hideBlocksOnly.isOn)
		{
			foreach (BuilderPart bp in sys.bpList)
			{
				if ((bool)bp.deploy && bp.hidden)
				{
					ShowPart(bp);
				}
			}
			if (!Stability.inst.raidMode)
			{
				foreach (wire wire in BuilderSystem.wireList)
				{
					if ((bool)wire && !wire.lr.enabled)
					{
						wire.showWire(state: true);
					}
				}
			}
		}
		else
		{
			HideFloors(0, send: false);
		}
	}

	public void HideFloorsControl(int change)
	{
		HideFloors(change, send: true);
	}

	public void roofPlusPressed()
	{
		if (roofPlus.isOn)
		{
			popUp.inst.message("hide roofs one level earlier");
		}
		else
		{
			popUp.inst.message("hide roofs one level earlier disabled");
		}
	}
}
