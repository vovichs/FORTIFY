using UnityEngine;

public class BuilderPartChild : MonoBehaviour
{
	private BuilderPart bp;

	private BuilderSystem cs;

	public GameObject parent;

	private void OnMouseEnter()
	{
		if (cs == null)
		{
			cs = BuilderSystem.inst;
			bp = parent.GetComponent<BuilderPart>();
			bp.rend = parent.GetComponentsInChildren<Renderer>();
		}
		if (!BuilderUI.inst.mouseOverUI)
		{
			bp.selectPart();
		}
	}

	private void OnMouseOver()
	{
		if (!BuilderUI.inst.mouseOverUI && !BuilderSystem.disableInput && (UnityEngine.Input.GetKeyDown(KeyCode.Delete) || UnityEngine.Input.GetKeyDown(KeyCode.Backspace) || Input.GetButtonDown("Del")))
		{
			bp.deletePart();
		}
	}

	private void OnMouseDown()
	{
		bp.dragSelection();
	}

	private void OnMouseExit()
	{
		if (!bp.selected)
		{
			bp.selectMaterial(select: false);
		}
	}
}
