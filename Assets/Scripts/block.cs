using UnityEngine;

public class block : MonoBehaviour
{
	public class UpdateSurroundingsQueue
	{
		protected void RunJob(block block)
		{
			if (!(block == null) && !block.destroyed)
			{
				block.GetNeighborLinks(updateNeighbors: false, clear: false);
				if ((bool)block.conditional)
				{
					block.conditional.RunCheck();
				}
			}
		}
	}

	public BuilderPart owner;

	public ConditionalCheck conditional;

	public Vector4 bounds;

	public Vector3 boundsCenter;

	public bool placing;

	public bool destroyed;

	public int tier;

	public bool continuous;

	public StabilityEntity stabilityEntity;

	public socket[] sockets;

	public Transform[] edgeCols;

	public GameObject[] baseCols;

	public Transform[] proximities;

	public Transform[] terrainChecks;

	public Transform frameCol;

	public bool tri;

	public bool halfHeight;

	public bool wall;

	public bool roof;

	public bool found;

	public bool doorway;

	public bool stairs;

	public bool spiral;

	public bool steps;

	public bool ramp;

	public bool floor;

	public bool frame;

	public bool pillar;

	public static UpdateSurroundingsQueue updateSurroundingsQueue;

	static block()
	{
		updateSurroundingsQueue = new UpdateSurroundingsQueue();
	}

	public void setColliderLayer(int edgeLayerId, int baseLayerId, int frameLayerId)
	{
		if (ramp)
		{
			edgeCols[0].gameObject.layer = baseLayerId;
			return;
		}
		if ((bool)frameCol)
		{
			frameCol.gameObject.layer = frameLayerId;
		}
		Transform[] array = edgeCols;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].gameObject.layer = edgeLayerId;
		}
		GameObject[] array2 = baseCols;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].layer = baseLayerId;
		}
	}

	public void getColliderLayer(int mode)
	{
		int edgeLayerId = 2;
		if (mode == 0)
		{
			edgeLayerId = 16;
		}
		int baseLayerId = 2;
		if (mode == 2)
		{
			baseLayerId = 16;
		}
		int frameLayerId = 2;
		if (mode == 3)
		{
			frameLayerId = 16;
		}
		if (mode == 4)
		{
			edgeLayerId = 16;
			baseLayerId = 16;
		}
		setColliderLayer(edgeLayerId, baseLayerId, frameLayerId);
	}

	public void colliderToggle(int mode)
	{
		bool enabled = true;
		if (mode == 1 || mode == 2 || mode == 4)
		{
			enabled = false;
		}
		Transform[] array = edgeCols;
		foreach (Transform obj in array)
		{
			obj.GetComponent<Collider>().enabled = enabled;
			obj.gameObject.layer = 16;
		}
		if (mode == 4)
		{
			enabled = true;
		}
		if ((bool)frameCol)
		{
			frameCol.GetComponent<Collider>().enabled = enabled;
			frameCol.gameObject.layer = 16;
		}
		if (mode == 3 || mode == 4)
		{
			enabled = false;
		}
		GameObject[] array2 = baseCols;
		foreach (GameObject obj2 in array2)
		{
			obj2.GetComponent<Collider>().enabled = enabled;
			obj2.layer = 16;
		}
	}

	public void GetNeighborLinks(bool updateNeighbors, bool clear)
	{
		if (sockets.Length == 0)
		{
			return;
		}
		if (clear)
		{
			ClearNeighborLinks();
		}
		float w = bounds.w;
		int num = tools.blockNeighborCheck(getBoundsCenter(), w);
		for (int i = 0; i < num; i++)
		{
			block component = tools.hit[i].transform.root.GetComponent<block>();
			if (!(component == this) && !component.placing)
			{
				LinkSockets(component);
				if (updateNeighbors)
				{

				}
			}
		}
	}

	private void LinkSockets(block other)
	{
		if (other.sockets.Length == 0)
		{
			return;
		}
		for (int i = 0; i < sockets.Length; i++)
		{
			socket socket = sockets[i];
			Vector3 position = socket._transform.position;
			for (int j = 0; j < other.sockets.Length; j++)
			{
				socket socket2 = other.sockets[j];
				if (socket.connectCheck(socket2, position))
				{
					if (!socket.connections.Contains(socket2))
					{
						socket.connections.Add(socket2);
					}
					if (!socket2.connections.Contains(socket))
					{
						socket2.connections.Add(socket);
					}
					break;
				}
			}
		}
	}

	public void UpdateNeighborLinks(bool removed)
	{
		if (sockets.Length == 0)
		{
			return;
		}
		for (int i = 0; i < sockets.Length; i++)
		{
			socket socket = sockets[i];
			for (int num = socket.connections.Count - 1; num >= 0; num--)
			{
				socket socket2 = socket.connections[num];
				if (!(socket2 == null) && !socket2.owner.destroyed)
				{
					socket2.connections.Remove(socket);
					if (removed)
					{
						StabilityEntity stabilityEntity = socket2.owner.stabilityEntity;
						if (stabilityEntity.cachedDistanceFromGround >= this.stabilityEntity.cachedDistanceFromGround)
						{
							stabilityEntity.UpdateStability();
						}
					}
				}
			}
		}
		if (!removed)
		{
			ClearNeighborLinks();
		}
	}

	public void UpdateNeighborState()
	{
		if (sockets.Length == 0)
		{
			return;
		}
		for (int i = 0; i < sockets.Length; i++)
		{
			socket socket = sockets[i];
			for (int j = 0; j < socket.connections.Count; j++)
			{
				socket socket2 = socket.connections[j];
				if (socket2 != null)
				{
					
				}
			}
		}
	}

	public socket GetSocketNamed(string named)
	{
		for (int i = 0; i < sockets.Length; i++)
		{
			if (sockets[i].name == named)
			{
				return sockets[i];
			}
		}
		return null;
	}

	public void ClearNeighborLinks()
	{
		for (int i = 0; i < sockets.Length; i++)
		{
			sockets[i].connections.Clear();
		}
	}

	public bool checkForPipes()
	{
		if (placeOptions.allowOverlap)
		{
			return false;
		}
		if (frame || roof)
		{
			return false;
		}
		Collider[] array = Physics.OverlapBox(halfExtents: owner.GetComponent<MeshFilter>().sharedMesh.bounds.size / 2f, center: owner._transform.position, orientation: owner._transform.rotation, layerMask: 4, queryTriggerInteraction: QueryTriggerInteraction.Collide);
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i].tag == "pipeCollider")
			{
				return true;
			}
		}
		return false;
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(base.transform.TransformPoint(new Vector3(bounds.x, bounds.y, bounds.z)), bounds.w);
	}

	[ContextMenu("update socket list")]
	private void UpdateSocketList()
	{
		socket[] array = sockets = GetComponentsInChildren<socket>(includeInactive: true);
	}

	public bool OpenEdgeCheck()
	{
		if (found)
		{
			GameObject[] array = baseCols;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].activeSelf)
				{
					return true;
				}
			}
			return false;
		}
		for (int j = 0; j < sockets.Length; j++)
		{
			socket socket = sockets[j];
			if (socket.partType == socket.Part.floor)
			{
				foreach (socket connection in socket.connections)
				{
					if (connection != null)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public Transform getClosestEdgeCol(Vector3 hitPos)
	{
		int num = 0;
		float num2 = 100f;
		for (int i = 0; i < edgeCols.Length; i++)
		{
			float magnitude = (edgeCols[i].position - hitPos).magnitude;
			if (magnitude < num2)
			{
				num2 = magnitude;
				num = i;
			}
		}
		return edgeCols[num];
	}

	public bool heightCheck()
	{
		BuilderSystem.inst.redblock = false;
		if (placeOptions.ignoreRules)
		{
			return false;
		}
		RaycastHit hitInfo;
		if (found)
		{
			for (int i = 0; i < terrainChecks.Length; i++)
			{
				Vector3 position = terrainChecks[i].position;
				if (i == 0)
				{
					if (!Physics.Raycast(position, Vector3.down, 1f, 256))
					{
						return true;
					}
					continue;
				}
				if (!Physics.Raycast(new Ray(position + Vector3.up * 100f, Vector3.down), 100f, 256))
				{
					return true;
				}
				if (Physics.Raycast(position + Vector3.up * 1f, Vector3.down, out hitInfo, 100f, 256) && hitInfo.distance > 1f)
				{
					return true;
				}
			}
			return false;
		}
		if (ramp)
		{
			if (rampCenterCheck())
			{
				return false;
			}
			for (int j = 0; j < terrainChecks.Length; j++)
			{
				Vector3 position2 = terrainChecks[j].position;
				if (!Physics.Raycast(new Ray(position2 + Vector3.up * 100f, Vector3.down), 100f, 256))
				{
					return true;
				}
				if (Physics.Raycast(position2 + Vector3.up * 1f, Vector3.down, out hitInfo, 100f, 256) && hitInfo.distance > 1f)
				{
					return true;
				}
			}
		}
		else if (Physics.Raycast(owner._transform.GetChild(1).position, Vector3.down, out hitInfo, 50f, BuilderSystem.inst.groundMask) && hitInfo.distance > 0.07f)
		{
			return true;
		}
		return false;
	}

	public bool rampCenterCheck()
	{
		if (Physics.Raycast(owner.center.position, Vector3.down, out RaycastHit hitInfo, 0.51f, 1) && hitInfo.transform.tag == "block")
		{
			BuilderPart component = hitInfo.transform.GetComponent<BuilderPart>();
			if (!component.tri && (component.found || component.floor))
			{
				return true;
			}
		}
		return false;
	}

	public void changeTier(int newTier, bool received)
	{
		tier = newTier;
		if (found || ramp)
		{
			conditional.setMesh();
		}
		if (roof)
		{
			UpdateNeighborLinks(removed: false);
		}
		if (!owner.selected)
		{
			owner.SetMaterial();
		}
		if (!received && BuilderSystem.multiplayer)
		{
			Multiplayer.tierSend(owner);
		}
	}

	public Vector3 getBoundsCenter()
	{
		return owner._transform.TransformPoint(new Vector3(bounds.x, bounds.y, bounds.z));
	}
}
