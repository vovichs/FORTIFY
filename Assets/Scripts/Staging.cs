using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Staging : MonoBehaviour
{
	public static Staging inst;

	public bool stageMode;

	public Toggle stageTog;

	public BuilderSystem sys;

	public Text stageText;

	public Image image;

	public static int stage = 1;

	public Material[] stageMats;

	private void Awake()
	{
		inst = this;
		sys = BuilderSystem.inst;
		image.color = stageMats[stage].GetColor("_Color");
	}

	public void PickStage(int num)
	{
		stage = num;
		stageText.text = stage.ToString();
		image.color = stageMats[stage].GetColor("_Color");
		base.transform.Find("stagePicker").gameObject.SetActive(value: false);
	}

	public void SetStage()
	{
		if (sys.objList.Count > 0)
		{
			StartCoroutine(showStage());
		}
		else
		{
			popUp.inst.message("selection needed");
		}
	}

	private IEnumerator showStage()
	{
		stage = int.Parse(stageText.text);
		List<BuilderPart> rendList = new List<BuilderPart>();
		for (int i = 0; i < sys.objList.Count; i++)
		{
			BuilderPart component = sys.objList[i].GetComponent<BuilderPart>();
			component.stage = stage;
			if (Symmetry.inst.isOn && (bool)component.sg)
			{
				component.sg.groupStageSet(stage);
			}
			if (BuilderSystem.multiplayer)
			{
				Multiplayer.stageSend(component);
			}
			component.rend[0].sharedMaterial = stageMats[stage];
			rendList.Add(component);
		}
		yield return new WaitForSeconds(0.3f);
		foreach (BuilderPart item in rendList)
		{
			if ((bool)item)
			{
				item.selectMaterial(select: true);
			}
		}
	}

	public void SelectStage()
	{
		sys.objList.Clear();
		foreach (BuilderPart bp in sys.bpList)
		{
			if (!(bp == null))
			{
				if (bp.stage == stage)
				{
					sys.changeSelection(bp, add: true);
					sys.objList.Add(bp.gameObject);
				}
				else if (bp.selected)
				{
					sys.changeSelection(bp, add: false);
				}
			}
		}
	}

	public void stageToggle()
	{
		stageMode = stageTog.isOn;
		if (stageMode && Stability.inst.colorView)
		{
			Stability.inst.colorView = false;
			Stability.inst.stabViewToggle.SetIsOnWithoutNotify(value: false);
			Stability.inst.stabViewToggle.GetComponent<toggleColor>().OnToggleValueChanged(isOn: false);
			Stability.inst.colors.SetActive(value: false);
		}
		foreach (BuilderPart bp in sys.bpList)
		{
			if ((bool)bp)
			{
				bp.SetMaterial();
			}
		}
	}
}
