using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainHeightmap))]
public class SgtTerrainHeightmap_Editor : SgtEditor<SgtTerrainHeightmap>
{
	protected override void OnInspector()
	{
		var dirtyTerrain = false;

		BeginError(Any(t => t.Heightmap == null));
			DrawDefault("Heightmap", ref dirtyTerrain);
		EndError();
		DrawDefault("Encoding", ref dirtyTerrain);
		BeginError(Any(t => t.DisplacementMin >= t.DisplacementMax));
			DrawDefault("DisplacementMin", ref dirtyTerrain);
			DrawDefault("DisplacementMax", ref dirtyTerrain);
		EndError();

		if (dirtyTerrain == true) DirtyEach(t => t.Rebuild());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Heightmap")]
public class SgtTerrainHeightmap : SgtTerrainModifier
{
	public enum EncodingType
	{
		Alpha,
		RedGreen
	}

	[Tooltip("The heightmap texture using a cylindrical (equirectangular) projection")]
	public Texture2D Heightmap;

	[Tooltip("The way the height data is stored in the texture")]
	public EncodingType Encoding = EncodingType.Alpha;

	[Tooltip("The height displacement represented by alpha = 0")]
	public double DisplacementMin = 0.0;

	[Tooltip("The height displacement represented by alpha = 255")]
	public double DisplacementMax = 0.1;

	protected virtual void OnEnable()
	{
		Rebuild();

		terrain.OnCalculateHeight += CalculateHeight;
	}

	protected virtual void OnDisable()
	{
		Rebuild();

		terrain.OnCalculateHeight -= CalculateHeight;
	}
	
	private void CalculateHeight(SgtVector3D localPosition, ref double height)
	{
		if (Heightmap != null)
		{
			var uv    = SgtHelper.CartesianToPolarUV((Vector3)localPosition);
			var color = SampleBilinear(uv);
			var dem   = default(double);

			switch (Encoding)
			{
				case EncodingType.Alpha:
				{
					dem = color.a;
				}
				break;

				case EncodingType.RedGreen:
				{
					dem = (color.r * 255.0 + color.g) / 256.0;
				}
				break;
			}

			height += DisplacementMin + (DisplacementMax - DisplacementMin) * dem;
		}
	}

	private Color SampleBilinear(Vector2 uv)
	{
		return Heightmap.GetPixelBilinear(uv.x, uv.y);
	}
}