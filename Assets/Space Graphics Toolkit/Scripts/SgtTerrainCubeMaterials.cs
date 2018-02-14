using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainCubeMaterials))]
public class SgtTerrainCubeMaterials_Editor : SgtEditor<SgtTerrainCubeMaterials>
{
	protected override void OnInspector()
	{
		var updateMaterials = false;

		BeginError(Any(t => t.PositiveX == null));
			DrawDefault("PositiveX", ref updateMaterials);
		EndError();
		BeginError(Any(t => t.PositiveY == null));
			DrawDefault("PositiveY", ref updateMaterials);
		EndError();
		BeginError(Any(t => t.PositiveZ == null));
			DrawDefault("PositiveZ", ref updateMaterials);
		EndError();
		BeginError(Any(t => t.NegativeX == null));
			DrawDefault("NegativeX", ref updateMaterials);
		EndError();
		BeginError(Any(t => t.NegativeY == null));
			DrawDefault("NegativeY", ref updateMaterials);
		EndError();
		BeginError(Any(t => t.NegativeZ == null));
			DrawDefault("NegativeZ", ref updateMaterials);
		EndError();

		if (updateMaterials == true) DirtyEach(t => t.UpdateRenderers());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Cube Materials")]
public class SgtTerrainCubeMaterials : SgtTerrainModifier
{
	public Material NegativeX;
	public Material NegativeY;
	public Material NegativeZ;
	public Material PositiveX;
	public Material PositiveY;
	public Material PositiveZ;

	protected virtual void OnEnable()
	{
		UpdateRenderers();

		terrain.OnCalculateMaterial += CalculateMaterial;
	}

	protected virtual void OnDisable()
	{
		UpdateRenderers();

		terrain.OnCalculateMaterial -= CalculateMaterial;
	}

	private void CalculateMaterial(SgtTerrainFace face, ref Material material)
	{
		switch (face.Side)
		{
			case CubemapFace.NegativeX: material = NegativeX; break;
			case CubemapFace.NegativeY: material = NegativeY; break;
			case CubemapFace.NegativeZ: material = NegativeZ; break;
			case CubemapFace.PositiveX: material = PositiveX; break;
			case CubemapFace.PositiveY: material = PositiveY; break;
			case CubemapFace.PositiveZ: material = PositiveZ; break;
		}
	}
}