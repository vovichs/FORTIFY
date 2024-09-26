using UnityEngine;
using UnityEngine.UI;

public class placeOptions : MonoBehaviour
{
	private BuilderSystem cs;

	public static bool allowOverlap = false;

	public Toggle overlapDeploysTog;

	private static bool overlapDeploys;

	public Toggle overlapBlocksTog;

	private static bool overlapBlocks;

	public static bool ignoreRules = false;

	public Toggle ignoreRulesTog;

	public static bool proximityCheck = true;

	public Toggle proximityCheckTog;

	private bool setToggles;

	public Toggle continuousTog;

	public static bool continuous = true;

	private void Awake()
	{
		cs = BuilderSystem.inst;
		setToggles = true;
		allowOverlapSetTogs();
		ignoreRulesTog.isOn = ignoreRules;
		proximityCheckTog.isOn = proximityCheck;
		if (PlayerPrefs.HasKey("clickHoldPlace"))
		{
			if (PlayerPrefs.GetInt("clickHoldPlace") == 0)
			{
				continuous = false;
			}
			else
			{
				continuous = true;
			}
		}
		continuousTog.isOn = continuous;
		setToggles = false;
	}

	public void allowOverlapSetTogs()
	{
		overlapDeploysTog.isOn = overlapDeploys;
		overlapBlocksTog.isOn = overlapBlocks;
		allowOverlapChange();
	}

	public void allowOverlapChange()
	{
		if (!setToggles)
		{
			overlapDeploys = overlapDeploysTog.isOn;
			overlapBlocks = overlapBlocksTog.isOn;
			allowOverlapCheck(cs.BPinfo);
		}
	}

	public static void allowOverlapCheck(BuilderPart bp)
	{
		allowOverlap = false;
		if ((bool)bp.deploy && overlapDeploys)
		{
			allowOverlap = true;
		}
		if ((bool)bp.block && overlapBlocks)
		{
			allowOverlap = true;
		}
		overlapCheck.overlap = false;
	}

	public void ignoreRulesState()
	{
		if (!setToggles)
		{
			ignoreRules = !ignoreRules;
		}
	}

	public void proximityCheckState()
	{
		if (!setToggles)
		{
			proximityCheck = !proximityCheck;
		}
	}

	public void continuousState()
	{
		if (!setToggles)
		{
			continuous = !continuous;
			PlayerPrefs.SetInt("clickHoldPlace", continuous ? 1 : 0);
		}
	}
}
