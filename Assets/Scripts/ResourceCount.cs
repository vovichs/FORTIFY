using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceCount : MonoBehaviour
{
	private BuilderSystem sys;

	public static ResourceCount inst;

	public static bool listSetup;

	private bool upkeep;

	private bool clear;

	private bool structSelect;

	private bool selectedOnly;

	private bool filter;

	private List<BuilderPart> structure = new List<BuilderPart>();

	private List<BuilderPart> partGroup = new List<BuilderPart>();

	private string partType;

	private string modeName = "";

	public Toggle resourcCountTog;

	public Toggle structSelectTog;

	public GameObject structClearTog;

	public Text countText;

	private int twig;

	private int wood;

	private int stone;

	private int metal;

	private int hqmetal;

	private int storage;

	private int water;

	private int blocks;

	private int[] brackets = new int[4]
	{
		15,
		50,
		125,
		200
	};

	private float[] bracketRate = new float[4]
	{
		0.1f,
		0.15f,
		0.2f,
		0.333f
	};

	public Text modeText;

	public Text upkeepText;

	public Text twigText;

	public Text woodText;

	public Text stoneText;

	public Text metalText;

	public Text hqmetalText;

	public Text storageText;

	public Text waterText;

	public Text[] resources;

	private int[] counts;

	public TMP_Dropdown weaponsDrop;

	public GameObject partInfoPanel;

	public Text[] partCosts;

	public Toggle[] groupToggles;

	public GameObject[] workBenchTiers;

	public HorizontalLayoutGroup hlg;

	public GameObject pickupImage;

	public Text researchCostText;

	public Text powerUsageText;

	public Text scrapShopText;

	public GameObject scrapShopCircle;

	public GameObject RF_ingredients;

	public Toggle favToggle;

	public Transform partListParent;

	public GameObject buttonPartPrefab;

	public Transform blockParent;

	public Transform deployParent;

	public Transform notIncludedParent;

	public TMP_Text[] partListCounts;

	public Material drawOverMat;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		base.gameObject.SetActive(value: false);
		counts = new int[resources.Length];
	}

	private void Update()
	{
		RaycastHit hitInfo;
		if (!Input.GetMouseButtonDown(0) || BuilderUI.inst.mouseOverUI || !Physics.Raycast(CameraCtrl.inst.cam.ScreenPointToRay(UnityEngine.Input.mousePosition), out hitInfo, 150f) || hitInfo.transform.gameObject.layer != 0)
		{
			return;
		}
		BuilderPart component = hitInfo.transform.GetComponent<BuilderPart>();
		if (!component.block)
		{
			return;
		}
		cupboardRange.inst.getStructureGroups(getDeploys: true);
		int strucId = component.strucId;
		structure.Clear();
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			if (sys.bpList[i].strucId == strucId)
			{
				structure.Add(sys.bpList[i]);
			}
		}
		PartCount(getUpkeep: false);
		structSelectTog.isOn = false;
		structClearTog.SetActive(value: true);
		countText.text = "structure count";
		popUp.inst.message("structure selected");
		popUp.inst.message("used for count until cleared");
	}

	public void reset()
	{
		if (resourcCountTog.isOn)
		{
			resourcCountTog.isOn = false;
		}
		structure.Clear();
		clear = true;
		PartCount(getUpkeep: false);
		clear = true;
	}

	public void togglePanel()
	{
		if (!base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(value: true);
			PartCount(getUpkeep: false);
			return;
		}
		if (base.enabled)
		{
			structSelectTog.isOn = false;
		}
		base.gameObject.SetActive(value: false);
	}

	public void selectedOnlyState()
	{
		selectedOnly = !selectedOnly;
		if (base.isActiveAndEnabled)
		{
			base.enabled = false;
		}
		PartCount(getUpkeep: false);
	}

	public void structSelectState()
	{
		structSelect = !structSelect;
		base.enabled = structSelect;
		if (structSelect)
		{
			popUp.inst.message("select structure for count");
		}
		sys.SetPlaceColMode(structSelect);
	}

	public void structSelectClear()
	{
		countText.text = "total count";
		structure.Clear();
		PartCount(getUpkeep: false);
	}

	public void buildPartsOnlyState()
	{
		filter = !filter;
	}

	public void PartCount(bool getUpkeep)
	{
		upkeep = getUpkeep;
		for (int i = 0; i < 3; i++)
		{
			Transform child = partListParent.GetChild(i);
			for (int num = child.childCount - 1; num >= 1; num--)
			{
				UnityEngine.Object.Destroy(child.GetChild(num).gameObject);
			}
		}
		blocks = 0;
		storage = 0;
		water = 0;
		twig = 0;
		wood = 0;
		stone = 0;
		metal = 0;
		hqmetal = 0;
		int num2 = 0;
		int num3 = 0;
		int num4 = 0;
		bool onPartList = false;
		bool blockPart = false;
		bool flag = false;
		List<BuilderPart> list = new List<BuilderPart>();
		if (clear)
		{
			list.Clear();
		}
		if (structure.Count > 0)
		{
			for (int num5 = structure.Count - 1; num5 > 0; num5--)
			{
				if (!structure[num5])
				{
					structure.Remove(structure[num5]);
				}
			}
			list = (from bp in structure
				orderby bp.name
				select bp).ToList();
		}
		else
		{
			list = (from bp in sys.bpList
				orderby bp.name
				select bp).ToList();
		}
		int count = list.Count;
		if (count > 0)
		{
			for (int j = 0; j < count; j++)
			{
				BuilderPart builderPart = list[j];
				if (!builderPart)
				{
					continue;
				}
				flag = builderPart.deploy;
				int id = builderPart.id;
				int num6 = -1;
				if (j + 1 < count)
				{
					num6 = list[j + 1].id;
				}
				if (num2 == 0)
				{
					includeCheck(builderPart, out blockPart, out onPartList);
				}
				if (!builderPart.hidden && (!selectedOnly || builderPart.selected))
				{
					partGroup.Add(list[j]);
					num2++;
					if ((bool)builderPart.deploy && builderPart.GetCodeString() != "")
					{
						num4++;
					}
				}
				if (id == num6 && num6 != -1)
				{
					continue;
				}
				if (num2 > 0)
				{
					bool flag2 = false;
					if (onPartList)
					{
						if ((bool)builderPart.block)
						{
							blocks += num2;
						}
						if (upkeep)
						{
							if (blockPart)
							{
								if ((bool)builderPart.deploy)
								{
									num3 += num2;
								}
								AddResources();
							}
							else
							{
								flag2 = true;
							}
						}
						else
						{
							if ((bool)builderPart.deploy)
							{
								num3 += num2;
							}
							AddResources();
						}
					}
					else
					{
						flag2 = true;
					}
					Transform transform = null;
					buttonPart component = UnityEngine.Object.Instantiate<GameObject>(parent: flag2 ? notIncludedParent : ((!flag) ? blockParent : deployParent), original: buttonPartPrefab).GetComponent<buttonPart>();
					component.named.text = builderPart.info.named;
					component.count.text = num2.ToString();
					component.id = builderPart.id;
					component.parts = new List<BuilderPart>(partGroup);
					num2 = 0;
				}
				partGroup.Clear();
			}
			if (!upkeep && num4 > 0)
			{
				bool flag3 = true;
				if (filter && (!groupToggles[0].isOn || !groupToggles[1].isOn))
				{
					flag3 = false;
				}
				if (flag3)
				{
					metal += 100 * num4;
					buttonPart component2 = UnityEngine.Object.Instantiate(buttonPartPrefab, deployParent).GetComponent<buttonPart>();
					component2.named.text = "code locks";
					component2.count.text = num4.ToString();
				}
			}
			if (upkeep)
			{
				if (blocks > 0)
				{
					float num7 = upkeepCalc();
					wood = Mathf.RoundToInt((float)wood * num7);
					stone = Mathf.RoundToInt((float)stone * num7);
					metal = Mathf.RoundToInt((float)metal * num7);
					hqmetal = Mathf.RoundToInt((float)hqmetal * num7);
					modeText.text = "Upkeep - " + blocks.ToString() + " blocks at " + (Math.Round(num7, 3) * 100.0).ToString() + "%";
				}
				else
				{
					wood = 0;
					stone = 0;
					metal = 0;
					hqmetal = 0;
					modeText.text = "Upkeep - 0 blocks";
				}
				twig = 0;
				for (int k = 0; k < counts.Length; k++)
				{
					resources[k].text = "0";
					counts[k] = 0;
				}
			}
			else
			{
				modeText.text = modeName + "Upgrade Cost";
				for (int l = 0; l < counts.Length; l++)
				{
					resources[l].text = counts[l].ToString();
					counts[l] = 0;
				}
			}
			partListCounts[0].text = blocks.ToString();
			partListCounts[1].text = num3.ToString();
		}
		else
		{
			modeText.text = modeName + "Upgrade Cost";
		}
		LayoutRebuilder.ForceRebuildLayoutImmediate(partListParent.GetComponent<RectTransform>());
		twigText.text = twig.ToString("n0");
		woodText.text = wood.ToString("n0");
		stoneText.text = stone.ToString("n0");
		metalText.text = metal.ToString("n0");
		hqmetalText.text = hqmetal.ToString("n0");
		storageText.text = "Slots: " + storage.ToString("n0");
		waterText.text = "Water Storage: " + (water * 1000).ToString("n0") + "ml";
	}

	public void includeCheck(BuilderPart bp, out bool blockPart, out bool onPartList)
	{
		blockPart = false;
		if (!bp.info.dontInclude)
		{
			onPartList = true;
			if ((bool)bp.deploy)
			{
				if (filter)
				{
					if (!groupToggles[0].isOn)
					{
						if (groupToggles[1].isOn && (bool)bp.door && bp.door.type != door.Type.gate)
						{
							onPartList = true;
						}
						else if (groupToggles[2].isOn && bp.deploy.windowPart)
						{
							onPartList = true;
						}
						else if (groupToggles[7].isOn && (bool)bp.device)
						{
							onPartList = true;
						}
						else
						{
							onPartList = false;
						}
					}
					else
					{
						if (!groupToggles[1].isOn && (bool)bp.door && bp.door.type != door.Type.gate)
						{
							onPartList = false;
						}
						if (!groupToggles[2].isOn && bp.deploy.windowPart)
						{
							onPartList = false;
						}
						if (!groupToggles[7].isOn && (bool)bp.device)
						{
							onPartList = false;
						}
					}
				}
				if (onPartList && (bp.deploy.windowPart || bp.deploy.frameWall || bp.deploy.frameFloor || ((bool)bp.door && bp.door.type != door.Type.gate)))
				{
					blockPart = true;
				}
				return;
			}
			if (filter)
			{
				if (!groupToggles[3].isOn && bp.block.found)
				{
					onPartList = false;
				}
				if (!groupToggles[4].isOn && bp.block.wall)
				{
					onPartList = false;
				}
				if (!groupToggles[5].isOn && bp.block.floor)
				{
					onPartList = false;
				}
				if (!groupToggles[6].isOn && bp.block.roof)
				{
					onPartList = false;
				}
			}
			if (onPartList)
			{
				blockPart = true;
			}
		}
		else
		{
			onPartList = false;
		}
	}

	public void AddResources()
	{
		partInfo info = partGroup[0].info;
		float multiplier = info.multiplier;
		if (multiplier > 0f)
		{
			for (int i = 0; i < partGroup.Count; i++)
			{
				twig += Mathf.CeilToInt(multiplier * 50f);
				if (upkeep && partGroup[i].tier == 0)
				{
					wood += Mathf.CeilToInt(multiplier * 50f);
				}
				if (partGroup[i].tier == 1)
				{
					wood += Mathf.CeilToInt(multiplier * 200f);
				}
				else if (partGroup[i].tier == 2)
				{
					stone += Mathf.CeilToInt(multiplier * 300f);
				}
				else if (partGroup[i].tier == 3)
				{
					metal += Mathf.CeilToInt(multiplier * 200f);
				}
				else if (partGroup[i].tier == 4)
				{
					hqmetal += Mathf.CeilToInt(multiplier * 25f);
				}
			}
			return;
		}
		if (info.storageSlots > 0)
		{
			storage += info.storageSlots * partGroup.Count;
		}
		if (info.waterStorage > 0)
		{
			water += info.waterStorage * partGroup.Count;
		}
		wood += info.wood * partGroup.Count;
		stone += info.stone * partGroup.Count;
		metal += info.metal * partGroup.Count;
		hqmetal += info.hq_metal * partGroup.Count;
		counts[0] += info.gears * partGroup.Count;
		counts[1] += info.ladders * partGroup.Count;
		counts[2] += info.blades * partGroup.Count;
		counts[3] += info.tarp * partGroup.Count;
		counts[4] += info.fuel * partGroup.Count;
		counts[5] += info.cloth * partGroup.Count;
		counts[6] += info.sewkit * partGroup.Count;
		counts[7] += info.rope * partGroup.Count;
		counts[8] += info.sheetmetal * partGroup.Count;
		counts[9] += info.propane * partGroup.Count;
		counts[10] += info.techTrash * partGroup.Count;
		if (info.shop == partInfo.Shop.craft)
		{
			counts[11] += info.scrap * partGroup.Count;
		}
		counts[12] += info.targetComp * partGroup.Count;
		counts[13] += info.CCTV * partGroup.Count;
	}

	private float upkeepCalc()
	{
		float num = 0f;
		int num2 = blocks;
		for (int i = 0; i < brackets.Length; i++)
		{
			if (num2 > 0)
			{
				int num3 = 0;
				num3 = ((i != brackets.Length - 1) ? Mathf.Min(num2, brackets[i]) : num2);
				num2 -= num3;
				num += (float)num3 * bracketRate[i];
			}
		}
		return num / (float)blocks;
	}

	public void GetResourceInfo(BuilderPart bp)
	{
		for (int i = 0; i < workBenchTiers.Length; i++)
		{
			workBenchTiers[i].SetActive(value: false);
		}
		powerUsageText.transform.parent.gameObject.SetActive(value: false);
		researchCostText.transform.parent.gameObject.SetActive(value: false);
		weaponsDrop.gameObject.SetActive(value: false);
		favToggle.gameObject.SetActive(value: false);
		if (bp.info.multiplier > 0f)
		{
			placeMenuFilter.inst.descText.text = "";
			int tier = BuilderSystem.tier;
			for (int j = 0; j < partCosts.Length; j++)
			{
				partCosts[j].text = "0";
			}
			wood = Mathf.CeilToInt(bp.info.multiplier * 50f);
			switch (tier)
			{
			case 1:
				wood += Mathf.CeilToInt(bp.info.multiplier * 200f);
				break;
			case 2:
				partCosts[1].text = Mathf.CeilToInt(bp.info.multiplier * 300f).ToString();
				break;
			case 3:
				partCosts[2].text = Mathf.CeilToInt(bp.info.multiplier * 200f).ToString();
				break;
			case 4:
				partCosts[3].text = Mathf.CeilToInt(bp.info.multiplier * 25f).ToString();
				break;
			}
		}
		else
		{
			placeMenuFilter.inst.descText.text = bp.info.rustDescription;
			if (!bp.info.buildDeploy)
			{
				favToggle.gameObject.SetActive(value: true);
				favToggle.SetIsOnWithoutNotify(bp.info.favorite);
			}
			if (bp.info.dontInclude)
			{
				for (int k = 0; k < partCosts.Length; k++)
				{
					partCosts[k].text = "0";
				}
				if (bp.info.scrap > 0)
				{
					partCosts[15].text = bp.info.scrap.ToString();
				}
			}
			else
			{
				partCosts[0].text = bp.info.wood.ToString();
				partCosts[1].text = bp.info.stone.ToString();
				partCosts[2].text = bp.info.metal.ToString();
				partCosts[3].text = bp.info.hq_metal.ToString();
				counts[0] = bp.info.gears;
				counts[1] = bp.info.ladders;
				counts[2] = bp.info.blades;
				counts[3] = bp.info.tarp;
				counts[4] = bp.info.fuel;
				counts[5] = bp.info.cloth;
				counts[6] = bp.info.sewkit;
				counts[7] = bp.info.rope;
				counts[8] = bp.info.sheetmetal;
				counts[9] = bp.info.propane;
				counts[10] = bp.info.techTrash;
				counts[11] = bp.info.scrap;
				counts[12] = bp.info.targetComp;
				counts[13] = bp.info.CCTV;
				for (int l = 0; l < counts.Length; l++)
				{
					partCosts[l + 4].text = counts[l].ToString();
					counts[l] = 0;
				}
				if (bp.info.workbench > 0)
				{
					workBenchTiers[bp.info.workbench - 1].SetActive(value: true);
				}
				if (bp.info.researchCost > 0)
				{
					researchCostText.text = bp.info.researchCost.ToString();
					researchCostText.transform.parent.gameObject.SetActive(value: true);
				}
			}
			if ((bool)bp.device)
			{
				if (bp.device is autoTurret)
				{
					weaponsDrop.gameObject.SetActive(value: true);
					if (weaponsDrop.options.Count == 1)
					{
						weaponsDrop.options.Clear();
					}
					foreach (RustCopyPaste.weapon weapons in RustCopyPaste.inst.weaponsList)
					{
						weaponsDrop.options.Add(new TMP_Dropdown.OptionData
						{
							text = weapons.name
						});
					}
				}
				if (bp.device.isElectric() && !(bp.device is battery))
				{
					powerUsageText.text = bp.device.usage.ToString();
					powerUsageText.transform.parent.gameObject.SetActive(value: true);
				}
			}
			RF_ingredients.SetActive(bp.name == "computer_station");
			if (bp.info.scrap > 0)
			{
				if (bp.info.shop == partInfo.Shop.bandit)
				{
					scrapShopText.text = "BC";
				}
				else if (bp.info.shop == partInfo.Shop.outpost)
				{
					scrapShopText.text = "OP";
				}
				else if (bp.info.shop == partInfo.Shop.fishing)
				{
					scrapShopText.text = "FV";
				}
				else
				{
					scrapShopText.text = "";
				}
				scrapShopCircle.SetActive(bp.info.shop != partInfo.Shop.craft);
			}
		}
		pickupImage.SetActive(bp.info.pickup);
		Text[] array = partCosts;
		foreach (Text text in array)
		{
			if (text.text == "0")
			{
				text.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				text.transform.parent.gameObject.SetActive(value: true);
			}
		}
		hlg.enabled = false;
		hlg.enabled = true;
	}

	public int GetWeaponId()
	{
		string weapon = weaponsDrop.options[weaponsDrop.value].text;
		return RustCopyPaste.inst.weaponsList.Single((RustCopyPaste.weapon s) => s.name == weapon).id;
	}

	public IEnumerator highlightStructure()
	{
		foreach (BuilderPart item in structure)
		{
			if ((bool)item)
			{
				item.rend[0].sharedMaterial = sys.checkBlue;
			}
		}
		yield return new WaitForSeconds(0.3f);
		foreach (BuilderPart item2 in structure)
		{
			if ((bool)item2)
			{
				item2.selectMaterial(item2.selected);
			}
		}
	}
}
