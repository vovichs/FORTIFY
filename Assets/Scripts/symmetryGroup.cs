using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class symmetryGroup : MonoBehaviour
{
	public BuilderPart[] group = new BuilderPart[0];

	private Coroutine swapWaitCR;

	private bool wait;

	public void selectionGroupDestroy(BuilderPart owner)
	{
		owner.sg = null;
		BuilderPart[] array = group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart && (bool)builderPart.sg)
			{
				builderPart.removeSymmetry();
				BuilderSystem.inst.objList.Remove(builderPart.gameObject);
				BuilderSystem.inst.destroyPart(builderPart, audio: false, receive: false);
			}
		}
	}

	public void groupDestroy(BuilderPart owner, bool addUndo)
	{
		if (!Symmetry.inst.isOn)
		{
			return;
		}
		owner.sg = null;
		if (group == null)
		{
			return;
		}
		for (int i = 0; i < group.Length; i++)
		{
			BuilderPart builderPart = group[i];
			if ((bool)builderPart && (bool)builderPart.sg && !builderPart.selected)
			{
				builderPart.sg = null;
				if (addUndo)
				{
					MGMT.inst.addDeletedPart(builderPart, undo: true);
				}
				BuilderSystem.inst.destroyPart(builderPart, audio: false, receive: false);
			}
		}
	}

	public void groupSwapOverlapped(BuilderPart owner, BuilderPart bp)
	{
		if (Symmetry.inst.isOn)
		{
			int id = owner.id;
			int id2 = bp.id;
		}
	}

	public void groupTierSet(int newTier)
	{
		if (!Symmetry.inst.isOn)
		{
			return;
		}
		BuilderPart[] array = group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart && (bool)builderPart.block)
			{
				builderPart.block.changeTier(newTier, received: false);
			}
		}
	}

	public void groupStageSet(int stage)
	{
		BuilderPart[] array = group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart && !builderPart.selected)
			{
				builderPart.stage = stage;
				if (BuilderSystem.multiplayer)
				{
					Multiplayer.stageSend(builderPart);
				}
				builderPart.rend[0].sharedMaterial = Staging.inst.stageMats[stage];
			}
		}
	}

	public void groupRotate(float rot)
	{
		BuilderPart[] array = group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart && (bool)builderPart.sg && !builderPart.selected && !builderPart.sg.wait)
			{
				builderPart.rotatePart(rot);
				builderPart.sg.StartCoroutine(builderPart.sg.actionWait());
			}
		}
	}

	private IEnumerator actionWait()
	{
		wait = true;
		yield return null;
		wait = false;
	}

	public void groupMove(BuilderPart bp)
	{
		if (!Symmetry.inst.isOn)
		{
			return;
		}
		float num = 0f;
		int addNum = Symmetry.inst.addNum;
		Vector3 position = Symmetry.inst.symTransform.position;
		Vector3 position2 = bp._transform.position;
		int num2 = 0;
		BuilderPart[] array = bp.sg.group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart)
			{
				num2++;
				if (num2 > addNum)
				{
					break;
				}
				num += Symmetry.inst.angle;
				Vector3 position3 = BuilderSystem.inst.RotatePointAroundPivot(position2, position, num);
				builderPart._transform.SetPositionAndRotation(position3, bp._transform.rotation);
				builderPart._transform.Rotate(Vector3.up, num, Space.World);
				builderPart.moved();
			}
		}
	}

	public void groupVerticalMove(Vector3 dir, float newLvl)
	{
		if (!Symmetry.inst.isOn)
		{
			return;
		}
		BuilderPart[] array = group;
		foreach (BuilderPart builderPart in array)
		{
			if ((bool)builderPart && !builderPart.selected)
			{
				builderPart._transform.position += dir;
				builderPart.moved();
				builderPart.level = newLvl;
			}
		}
	}
}
