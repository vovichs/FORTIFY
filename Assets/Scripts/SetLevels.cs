using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevels : MonoBehaviour
{
	private BuilderSystem sys;

	public List<BuilderPart> floors;

	public List<BuilderPart> moreFloors;

	public List<BuilderPart> ThisLevel;

	public List<BuilderPart> NextLevel;

	public List<BuilderPart> deployables;

	public LayerMask usedMask;

	private bool end;

	private void Awake()
	{
		sys = BuilderSystem.inst;
	}

	public void setLevels()
	{
		FirstCheck();
		end = false;
		while (!end)
		{
			StartCoroutine(LevelCheck());
		}
		deployCheck();
		ResetParts();
		ThisLevel.Clear();
		NextLevel.Clear();
		deployables.Clear();
	}

	private void FirstCheck()
	{
		for (int i = 0; i < sys.bpList.Count; i++)
		{
			BuilderPart builderPart = sys.bpList[i];
			if (builderPart.found)
			{
				builderPart.level = 1f;
				FoundCheck(builderPart);
				builderPart.check = true;
				continue;
			}
			if ((bool)builderPart.deploy)
			{
				if (builderPart.deploy.shelf)
				{
					deployables.Insert(0, builderPart);
				}
				else
				{
					deployables.Add(builderPart);
				}
			}
			builderPart.check = false;
		}
	}

	private IEnumerator LevelCheck()
	{
		ThisLevel = new List<BuilderPart>(NextLevel);
		NextLevel.Clear();
		foreach (BuilderPart item in ThisLevel)
		{
			if (item.block.wall || item.block.roof)
			{
				WallRoofCheck(item);
			}
			else if (item.block.stairs)
			{
				StairsCheck(item);
			}
			else if (item.block.pillar)
			{
				PillarCheck(item);
			}
			item.check = true;
		}
		bool flag = false;
		while (!flag)
		{
			foreach (BuilderPart floor in floors)
			{
				FloorCheck(floor);
			}
			floors.Clear();
			if (moreFloors.Count > 0)
			{
				floors = new List<BuilderPart>(moreFloors);
				moreFloors.Clear();
			}
			else
			{
				flag = true;
			}
		}
		if (NextLevel.Count == 0)
		{
			end = true;
		}
		yield return null;
	}

	private void FoundCheck(BuilderPart checkbp)
	{
		for (int i = 0; i < checkbp.block.sockets.Length; i++)
		{
			socket socket = checkbp.block.sockets[i];
			if (!socket.female || (socket.partType != socket.Part.wall && socket.partType != socket.Part.stairs))
			{
				continue;
			}
			for (int j = 0; j < socket.connections.Count; j++)
			{
				BuilderPart owner = socket.connections[j].owner.owner;
				if (!owner.check)
				{
					if (owner.block.halfHeight)
					{
						owner.level = 0.5f;
					}
					else
					{
						owner.level = 1f;
					}
					NextLevel.Add(owner);
					owner.check = true;
				}
			}
		}
	}

	private void WallRoofCheck(BuilderPart checkbp)
	{
		for (int i = 0; i < checkbp.block.sockets.Length; i++)
		{
			socket socket = checkbp.block.sockets[i];
			if (!socket.female)
			{
				continue;
			}
			if (socket.socketType == socket.Type.stability)
			{
				for (int j = 0; j < socket.connections.Count; j++)
				{
					BuilderPart owner = socket.connections[j].owner.owner;
					if (owner.check || !owner.wall)
					{
						continue;
					}
					if (owner.block.halfHeight)
					{
						bool flag = false;
						float num = 0f;
						if (owner._transform.position.y > checkbp._transform.position.y)
						{
							flag = true;
						}
						num = (checkbp.block.halfHeight ? ((!flag) ? 0f : 0.5f) : ((!flag) ? (-0.5f) : 0f));
						owner.level = checkbp.level + num;
					}
					else
					{
						owner.level = checkbp.level;
					}
					NextLevel.Add(owner);
					owner.check = true;
				}
			}
			if (socket.socketType != 0)
			{
				continue;
			}
			for (int k = 0; k < socket.connections.Count; k++)
			{
				BuilderPart owner2 = socket.connections[k].owner.owner;
				if (owner2.wall)
				{
					if (owner2.block.halfHeight)
					{
						owner2.level = checkbp.level + 0.5f;
					}
					else
					{
						owner2.level = checkbp.level + 1f;
					}
					if (!owner2.check)
					{
						NextLevel.Add(owner2);
						owner2.check = true;
					}
				}
				else if (!owner2.check)
				{
					if (owner2.floor)
					{
						floors.Add(owner2);
					}
					owner2.level = checkbp.level + 1f;
					NextLevel.Add(owner2);
					owner2.check = true;
				}
			}
		}
	}

	private void FloorCheck(BuilderPart checkbp)
	{
		for (int i = 0; i < checkbp.block.sockets.Length; i++)
		{
			socket socket = checkbp.block.sockets[i];
			if (!socket.female)
			{
				continue;
			}
			for (int j = 0; j < socket.connections.Count; j++)
			{
				BuilderPart owner = socket.connections[j].owner.owner;
				if (owner.check)
				{
					continue;
				}
				if (owner.wall || owner.block.roof)
				{
					if (owner.block.halfHeight)
					{
						owner.level = checkbp.level - 0.5f;
					}
					else
					{
						owner.level = checkbp.level;
					}
					NextLevel.Add(owner);
					owner.check = true;
				}
				else if (owner.floor)
				{
					moreFloors.Add(owner);
					owner.level = checkbp.level;
					NextLevel.Add(owner);
					owner.check = true;
				}
				else if (owner.block.stairs)
				{
					if (owner.block.spiral)
					{
						owner.level = checkbp.level - 0.5f;
					}
					else
					{
						owner.level = checkbp.level;
					}
					NextLevel.Add(owner);
					owner.check = true;
				}
			}
		}
	}

	private void PillarCheck(BuilderPart checkbp)
	{
		Collider[] array = Physics.OverlapSphere(checkbp._transform.GetChild(1).position, 0.03f, usedMask);
		for (int i = 0; i < array.Length; i++)
		{
			BuilderPart component = array[i].GetComponent<BuilderPart>();
			if (!(component == null) && !component.check)
			{
				if (component.floor)
				{
					floors.Add(component);
				}
				else
				{
					NextLevel.Add(component);
				}
				component.level = checkbp.level + 1f;
				NextLevel.Add(component);
				component.check = true;
			}
		}
	}

	private void StairsCheck(BuilderPart checkbp)
	{
		for (int i = 0; i < checkbp.block.sockets.Length; i++)
		{
			socket socket = checkbp.block.sockets[i];
			if (!socket.female)
			{
				continue;
			}
			for (int j = 0; j < socket.connections.Count; j++)
			{
				BuilderPart owner = socket.connections[j].owner.owner;
				owner.level = checkbp.level;
				if (owner.block.halfHeight)
				{
					owner.level += 0.5f;
				}
				NextLevel.Add(owner);
				owner.check = true;
			}
		}
	}

	private void deployCheck()
	{
		Dictionary<BuilderPart, BuilderPart> dictionary = new Dictionary<BuilderPart, BuilderPart>();
		for (int i = 0; i < deployables.Count; i++)
		{
			BuilderPart builderPart = deployables[i];
			Transform transform = builderPart._transform;
			Vector3 position = transform.position;
			if ((bool)builderPart.deploy.offset)
			{
				position = builderPart.deploy.offset.position;
			}
			if (tools.deployOnCheck(builderPart, position, 0.02f) > 0)
			{
				BuilderPart component = tools.hit[0].transform.root.GetComponent<BuilderPart>();
				if (component != null)
				{
					if ((bool)component.block && component.check)
					{
						builderPart.check = true;
						builderPart.level = component.level;
						if (component.block.wall || component.block.pillar)
						{
							if (component.block.halfHeight)
							{
								if ((double)position.y >= (double)component._transform.position.y + 0.48)
								{
									builderPart.level += 0.5f;
								}
							}
							else if ((double)position.y >= (double)component._transform.position.y + 0.98)
							{
								builderPart.level += 1f;
							}
						}
					}
					if ((bool)component.deploy)
					{
						builderPart.check = true;
						dictionary.Add(builderPart, component);
					}
				}
			}
			if (!builderPart.check)
			{
				builderPart.level = Mathf.CeilToInt(transform.position.y);
			}
		}
		foreach (KeyValuePair<BuilderPart, BuilderPart> item in dictionary)
		{
			item.Key.level = item.Value.level;
		}
	}

	private void ResetParts()
	{
		foreach (BuilderPart bp in sys.bpList)
		{
			if ((bool)bp.block && !bp.check)
			{
				bp.level = Mathf.CeilToInt(bp._transform.position.y);
			}
			bp.check = false;
		}
	}
}
