using UnityEngine;
using UnityEngine.UI;

public class locks : MonoBehaviour
{
	public static locks inst;

	private BuilderSystem cs;

	public InputField codePanelInput;

	public Material invisMat;

	public Material codeLock;

	public bool includeCodeLocks = true;

	private void Awake()
	{
		inst = this;
		cs = BuilderSystem.inst;
	}

	public void codeLockMode()
	{
		includeCodeLocks = !includeCodeLocks;
		if (!includeCodeLocks)
		{
			popUp.inst.message("auto codelocks off");
		}
		UnityEngine.Object.Destroy(cs.BPinfo.gameObject);
		cs.InstPart(cs.BPinfo.id);
	}

	public void addCodelock(BuilderPart bp, string code)
	{
		LockEntity lockEnt = bp.deploy.lockEnt;
		if (lockEnt.code == "")
		{
			Material[] sharedMaterials = bp.rend[0].sharedMaterials;
			sharedMaterials[1] = codeLock;
			bp.rend[0].sharedMaterials = sharedMaterials;
		}
		lockEnt.code = code;
	}

	public void removeCodelock(BuilderPart bp)
	{
		LockEntity lockEnt = bp.deploy.lockEnt;
		if (lockEnt.code != "")
		{
			Material[] sharedMaterials = bp.rend[0].sharedMaterials;
			sharedMaterials[1] = invisMat;
			bp.rend[0].sharedMaterials = sharedMaterials;
		}
		lockEnt.code = "";
	}

	public void toggleDoorState(bool state)
	{
		bool flag = false;
		for (int i = 0; i < cs.objList.Count; i++)
		{
			BuilderPart component = cs.objList[i].GetComponent<BuilderPart>();
			if (component.selected && (bool)component.door)
			{
				flag = true;
				if (component.door.open != state)
				{
					StartCoroutine(component.changeDoorMeshState(state, audio: false, send: true));
				}
			}
		}
		if (!flag)
		{
			popUp.inst.message("select doors first");
		}
	}

	public void toggleDoorLocks(bool add)
	{
		if (cs.objList.Count < 1)
		{
			return;
		}
		for (int i = 0; i < cs.objList.Count; i++)
		{
			BuilderPart component = cs.objList[i].GetComponent<BuilderPart>();
			if (component.selected && (bool)component.deploy && component.deploy.lockEnt != null)
			{
				if (add)
				{
					addCodelock(component, "0");
				}
				else
				{
					removeCodelock(component);
				}
			}
		}
	}

	public void changeCode(bool add)
	{
		if (codePanelInput.text.Length < 4)
		{
			return;
		}
		string text = codePanelInput.text;
		bool flag = false;
		if (cs.objList.Count > 0)
		{
			for (int i = 0; i < cs.objList.Count; i++)
			{
				BuilderPart component = cs.objList[i].GetComponent<BuilderPart>();
				if ((bool)component.deploy && !(component.deploy.lockEnt == null))
				{
					if (add)
					{
						flag = true;
						addCodelock(component, text);
					}
					else
					{
						removeCodelock(component);
					}
				}
			}
		}
		if (flag)
		{
			popUp.inst.message("code set");
		}
		else
		{
			popUp.inst.message("no codelocks in selection");
		}
	}
}
