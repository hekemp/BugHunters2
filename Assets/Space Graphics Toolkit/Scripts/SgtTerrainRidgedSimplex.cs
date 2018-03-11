using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrainRidgedSimplex))]
public class SgtTerrainRidgedSimplex_Editor : SgtEditor<SgtTerrainRidgedSimplex>
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
		DrawDefault("Invert", ref dirtyTerrain, ref updateNoise);
		DrawDefault("Seed", ref dirtyTerrain, ref updateNoise);

		if (updateNoise  == true) DirtyEach(t => t.UpdateNoise ());
		if (dirtyTerrain == true) DirtyEach(t => t.Rebuild());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain Ridged Simplex")]
public class SgtTerrainRidgedSimplex : SgtTerrainModifier
{
	[Tooltip("The density/frequency/tiling of the displacement")]
	public double Frequency = 10;

	[Tooltip("The +- strength of the displacement")]
	public double Amplitude = 0.5;

	[Tooltip("The detail of the simplex noise")]
	[Range(1, 20)]
	public int Octaves = 5;

	[Tooltip("Invert the ridges?")]
	public bool Invert;

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

		if (total > 0.0)
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
			contribution  += generators[i].Generate((float)localPosition.x, (float)localPosition.y, (float)localPosition.z) * weight;
			localPosition *= 2.0;
			weight        *= 0.5;
		}

		// Scale maximum constribution to -2..2
		contribution *= scale * 2.0;

		// Abs to 0 .. 2
		if (contribution < 0.0)
		{
			contribution = -contribution;
		}

		if (Invert == true)
		{
			contribution = 2.0 - contribution;
		}

		// Scale and add
		height += contribution * Amplitude;
	}
}