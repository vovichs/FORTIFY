using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class elevator : Device
{
	[Header("__________________________")]
	public elevator stackHolder;

	public List<elevator> stack = new List<elevator>();

	public bool top;

	public GameObject lift;

	public Transform liftPlatform;

	public Transform liftPoint;

	public Light _light;

	public MeshCollider liftCol;

	public Transform snapTop;

	public BoxCollider bc;

	public bool isRemoving;

	public int liftPower;

	public bool called;

	public bool moving;

	public override void powerThru(int _PID)
	{
		if (PID == _PID)
		{
			return;
		}
		PID = _PID;
		int inputPower = getInputPower(0);
		int inputPower2 = getInputPower(1);
		liftPower = getInputPower(2);
		elevator elevator = getTop();
		if (!elevator)
		{
			return;
		}
		if (top)
		{
			if (liftPower >= usage)
			{
				if (!on)
				{
					on = true;
					_light.enabled = true;
				}
			}
			else if (on)
			{
				on = false;
				_light.enabled = false;
			}
		}
		if (elevator.liftPower >= usage)
		{
			if (inputPower > 0 || inputPower2 > 0)
			{
				if (!called)
				{
					called = true;
					elevator.StartCoroutine(elevator.moveTo(liftPoint.position));
				}
			}
			else
			{
				called = false;
			}
		}
		if (inputFrom[2].connectedTo != null)
		{
			sendUsage();
		}
	}

	public override void setValue(int val, bool send)
	{
	}

	private elevator getTop()
	{
		if (!stackHolder)
		{
			return null;
		}
		elevator elevator = stackHolder.stack[stackHolder.stack.Count - 1];
		if ((bool)elevator && elevator.top && elevator.lift.activeSelf)
		{
			return elevator;
		}
		return null;
	}

	private IEnumerator moveTo(Vector3 pos)
	{
		if (moving)
		{
			yield break;
		}
		Vector3 vector = pos - liftPlatform.position;
		Vector3 startPos = liftPlatform.position;
		if (!(vector.magnitude < 0.9f))
		{
			toggleSound(on: true);
			moving = true;
			float duration = vector.magnitude / 0.333f;
			float runTime = 0f;
			while (runTime < duration && base.gameObject.activeSelf)
			{
				runTime += Time.deltaTime;
				float y = Mathf.Lerp(startPos.y, pos.y, runTime / duration);
				liftPlatform.position = new Vector3(liftPlatform.position.x, y, liftPlatform.position.z);
				yield return null;
			}
			moving = false;
			toggleSound(on: false);
		}
	}

	public void moveStop()
	{
		if (moving)
		{
			StopAllCoroutines();
			moving = false;
			toggleSound(on: false);
		}
		liftPlatform.localPosition = Vector3.zero + Vector3.up * 0.333f;
	}

	public void stackCheck()
	{
		stackHolder = null;
		bool flag = true;
		bool flag2 = true;
		elevator elevator = null;
		Vector3 b = new Vector3(0f, 0.1f, 0f);
		if (Physics.Raycast(owner._transform.position, Vector3.down, out RaycastHit hitInfo, 0.15f, 4, QueryTriggerInteraction.Collide))
		{
			Transform transform = hitInfo.transform;
			if ((bool)transform.parent && transform.parent.name == "elevator")
			{
				elevator component = transform.parent.GetComponent<elevator>();
				if ((bool)component.stackHolder)
				{
					stackHolder = component.stackHolder;
					component.stackHolder.stack.Add(this);
					component.setTopConditional(!component.top);
					flag = false;
				}
			}
		}
		if (Physics.Raycast(snapTop.position - b, Vector3.up, out hitInfo, 0.2f, 4, QueryTriggerInteraction.Collide))
		{
			Transform transform2 = hitInfo.transform;
			if ((bool)transform2.parent && transform2.parent.name == "elevator")
			{
				elevator component2 = transform2.parent.GetComponent<elevator>();
				if ((bool)component2.stackHolder)
				{
					if ((bool)stackHolder)
					{
						foreach (elevator item in component2.stack)
						{
							item.stackHolder = stackHolder;
							if (!stackHolder.stack.Contains(item))
							{
								stackHolder.stack.Add(item);
							}
						}
					}
					else
					{
						component2.stackHolder.stack.Insert(0, this);
						stack = component2.stack;
						foreach (elevator item2 in stack)
						{
							item2.stackHolder = this;
						}
					}
					flag = false;
				}
				else
				{
					flag = true;
					elevator = component2;
				}
				flag2 = false;
			}
		}
		if (flag2 && !top)
		{
			setTopConditional(set: true);
		}
		else if (!flag2 && top)
		{
			setTopConditional(set: false);
		}
		if (flag)
		{
			stackHolder = this;
			stack.Clear();
			stack.Add(this);
			if ((bool)elevator)
			{
				elevator.stackCheck();
			}
		}
	}

	public bool stackLimitCheck()
	{
		if (stackHolder != null)
		{
			return stackHolder.stack.Count < 11;
		}
		return true;
	}

	public void setTopConditional(bool set)
	{
		if (top == set)
		{
			return;
		}
		top = set;
		bc.enabled = !set;
		lift.SetActive(set);
		liftCol.enabled = set;
		liftPower = 0;
		if (top)
		{
			hideObjs[0] = lift;
			return;
		}
		liftPlatform.position = liftPoint.position;
		if (inputFrom[2].connectedTo != null)
		{
			inputFrom[2].sendDisconnect(destroyed: false);
		}
		hideObjs[0] = null;
	}

	private void toggleSound(bool on)
	{
		AudioSource component = liftPlatform.GetComponent<AudioSource>();
		if (on)
		{
			component.enabled = true;
			component.Play();
		}
		else
		{
			component.Stop();
			component.enabled = false;
		}
	}

	private void OnEnable()
	{
		moving = false;
	}

	public void destroyCheck()
	{
		if (!stackHolder || !(stackHolder == this) || stack.Count <= 1)
		{
			return;
		}
		MonoBehaviour.print("wat");
		stack.Remove(this);
		int num = 0;
		while (true)
		{
			if (num < stack.Count)
			{
				if (!stack[num] || !(stack[num].gameObject.tag != "destroy"))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		stack[num].stack = (from item in stack
			where item != null
			select item).ToList();
		foreach (elevator item in stack[num].stack)
		{
			if ((bool)item)
			{
				item.stackHolder = stack[num];
			}
		}
	}
}
