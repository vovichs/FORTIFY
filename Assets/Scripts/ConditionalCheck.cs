using UnityEngine;

public abstract class ConditionalCheck : MonoBehaviour
{
	public block block;

	public abstract void RunCheck();

	public virtual void tierMeshChange(int newTier)
	{
	}

	public virtual void setMesh()
	{
	}
}
