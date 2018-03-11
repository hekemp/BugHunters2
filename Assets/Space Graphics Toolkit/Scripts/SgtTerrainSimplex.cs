using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainSimplex))]
public class SgtTerrainSimplex_Editor : SgtEditor<SgtTerrainSimplex>
{
	protected override void OnInspector()
	{
		var updateNoise  = false;
		var dirtyTerrain = false;

		BeginError(Any(t => t.Frequency == 0.0f));
			DrawDefault("Frequency", ref dirtyTerrain);
		EndError();
		BeginError(Any(t => t.Amplitude == 0.0f));
			DrawDefault("Amplitude", ref dirtyTerrain);
		EndError();
		DrawDefault("Octaves", ref dirtyTerrain, ref updateNoise);
		DrawDefault("Seed", ref dirtyTerrain, ref updateNoise);

		if (updateNoise  == true) DirtyEach(t => t.UpdateNoise ());
		if (dirtyTerrain == true) DirtyEach(t => t.Rebuild());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Simplex")]
public class SgtTerrainSimplex : SgtTerrainModifier
{
	[Tooltip("The density/frequency/tiling of the displacement")]
	public double Frequency = 10;

	[Tooltip("The +- strength of the displacement")]
	public double Amplitude = 0.5;

	[Tooltip("The detail of the simplex noise")]
	[Range(1, 20)]
	public int Octaves = 5;

	[Tooltip("The random seed used for the simplex noise")]
	[SgtSeed]
	public int Seed;

	[System.NonSerialized]
	private SgtSimplex[] generators;

	[System.NonSerialized]
	private double scale = 1.0;

	public void UpdateNoise()
	{
		if (generators == null || generators.Length != Octaves)
		{
			generators = new SgtSimplex[Octaves];
		}

		var weight = 1.0;
		var total  = 0.0;

		for (var i = 0; i < Octaves; i++)
		{
			var generator = generators[i];

			if (generator == null)
			{
				generator = generators[i] = new SgtSimplex();
			}

			generator.SetSeed(Seed + i * 999);

			total  += weight;
			weight *= 0.5;
		}

		if (total > 0.0f)
		{
			scale = 1.0 / total;
		}
		else
		{
			scale = 1.0;
		}
	}

	protected virtual void OnEnable()
	{
		Rebuild();

		terrain.OnCalculateHeight += CalculateHeight;

		UpdateNoise();
	}

	protected virtual void OnDisable()
	{
		Rebuild();

		terrain.OnCalculateHeight -= CalculateHeight;
	}

	private void CalculateHeight(SgtVector3D localPosition, ref double height)
	{
		localPosition /= localPosition.magnitude;
		localPosition *= Frequency;

		var contribution = 0.0;
		var weight       = 1.0;

		for (var i = 0; i < Octaves; i++)
		{
			localPosition *= 2.0;
			contribution  += generators[i].Generate((float)localPosition.x, (float)localPosition.y, (float)localPosition.z) * weight;
			weight        *= 0.5;
		}

		// Scale maximum constribution to -1..1
		contribution *= scale;

		// Scale and add
		height += contribution * Amplitude;
	}
}