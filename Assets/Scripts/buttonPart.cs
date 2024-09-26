using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class buttonPart : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
{
	public TMP_Text named;

	public TMP_Text count;

	public int id = -1;

	[HideInInspector]
	public List<BuilderPart> parts;

	public void addToSelection()
	{
		if (id != -1)
		{
			if (!BuilderSystem.editMode)
			{
				BuilderUI.inst.editToggle.isOn = true;
			}
			foreach (BuilderPart part in parts)
			{
				if ((bool)part && !part.selected && !part.hidden)
				{
					BuilderSystem.inst.objList.Add(part.gameObject);
					BuilderSystem.inst.changeSelection(part, add: true);
				}
			}
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (id != -1)
		{
			highlightSelectedPart(show: true);
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (id != -1)
		{
			highlightSelectedPart(show: false);
		}
	}

	public void highlightSelectedPart(bool show)
	{
		foreach (BuilderPart part in parts)
		{
			if ((bool)part && !part.hidden)
			{
				if (show)
				{
					if ((bool)part.deploy)
					{
						Renderer[] rend = part.rend;
						for (int i = 0; i < rend.Length; i++)
						{
							rend[i].sharedMaterial = ResourceCount.inst.drawOverMat;
						}
					}
					else
					{
						part.rend[0].sharedMaterial = ResourceCount.inst.drawOverMat;
					}
				}
				else if (part.selected)
				{
					Renderer[] rend = part.rend;
					for (int i = 0; i < rend.Length; i++)
					{
						rend[i].sharedMaterial = BuilderSystem.inst.highLight;
					}
				}
				else
				{
					part.SetMaterial();
				}
			}
		}
	}
}
