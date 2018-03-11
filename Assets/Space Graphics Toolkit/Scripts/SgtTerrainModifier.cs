using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SgtTerrain))]
public abstract class SgtTerrainModifier : MonoBehaviour
{
	[System.NonSerialized]
	protected SgtTerrain terrain;

	public void Rebuild()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		terrain.DelayRebuild = true;
	}

	public void UpdateRenderers()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		terrain.DelayUpdateRenderers = true;
	}

	public void UpdateColliders()
	{
		if (terrain == null) terrain = GetComponent<SgtTerrain>();

		terrain.DelayUpdateColliders = true;
	}
}