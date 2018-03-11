using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtTerrain))]
public class SgtTerrain_Editor : SgtEditor<SgtTerrain>
{
	protected override void OnInspector()
	{
		var rebuild         = false;
		var updateStates    = false;
		var updateRenderer  = false;
		var updateColliders = false;

		DrawDefault("Material", ref updateRenderer);
		DrawDefault("Atmosphere", ref updateRenderer);

		Separator();

		BeginError(Any(t => t.Targets == null || t.Targets.Count == 0));
			DrawDefault("Targets", ref updateStates);
		EndError();
		BeginError(Any(t => t.Radius <= 0.0));
			DrawDefault("Radius", ref rebuild);
		EndError();
		DrawDefault("Subdivisions", ref rebuild);
		DrawDefault("Normals", ref rebuild);
		DrawDefault("Tangents", ref rebuild);
		DrawDefault("CenterBounds", ref rebuild);
		BeginError(Any(t => t.MaxColliderDepth < 0 || (t.Distances != null && t.MaxColliderDepth > t.Distances.Count)));
			DrawDefault("MaxColliderDepth", ref updateColliders);
		EndError();
		BeginError(Any(t => DistancesValid(t.Distances) == false));
			DrawDefault("Distances", ref updateStates);
		EndError();

		Separator();

		if (Button("Add Distance") == true)
		{
			Each(t => AddDistance(ref t.Distances));
		}

		if (rebuild         == true) DirtyEach(t => { serializedObject.ApplyModifiedProperties(); t.Rebuild(); });
		if (updateStates    == true) DirtyEach(t => t.UpdateStates   ());
		if (updateRenderer  == true) DirtyEach(t => t.UpdateRenderers());
		if (updateColliders == true) DirtyEach(t => t.UpdateColliders());
	}

	private static void AddDistance(ref List<double> distances)
	{
		if (distances == null)
		{
			distances = new List<double>();
		}

		var lastDistance = 2.0;

		if (distances.Count > 0)
		{
			var distance = distances[distances.Count - 1];

			if (distance > 0.0)
			{
				lastDistance = distance;
			}
		}

		distances.Add(lastDistance * 0.5);
	}

	private static bool DistancesValid(List<double> distances)
	{
		if (distances == null)
		{
			return false;
		}

		var lastDistance = double.PositiveInfinity;

		for (var i = 0; i < distances.Count; i++)
		{
			var distance = distances[i];

			if (distance <= 0.0 || distance >= lastDistance)
			{
				return false;
			}

			lastDistance = distance;
		}

		return true;
	}
}
#endif

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Terrain")]
public partial class SgtTerrain : MonoBehaviour
{
	public enum NormalsType
	{
		Normalized,
		Hierarchical
	}

	public delegate void CalculateHeightDelegate(SgtVector3D localPosition, ref double height);

	public delegate void CalculateColorDelegate(SgtVector3D localPosition, double height, ref Color color);

	public delegate void CalculateMaterialDelegate(SgtTerrainFace face, ref Material material);

	public delegate void CalculateFaceDelegate(SgtTerrainFace face);

	// All active and enabled terrains in the scene
	public static List<SgtTerrain> AllTerrains = new List<SgtTerrain>();

	[Tooltip("The base material for the terrain (can be overirdden by each face)")]
	public Material Material;

	[Tooltip("The atmosphere applied to this terrain")]
	public SgtAtmosphere Atmosphere;

	[Tooltip("The transforms the LOD will be based around (e.g. your main camera)")]
	public List<Transform> Targets;

	[Tooltip("The base radius of the terrain")]
	public double Radius = 1.0f;

	[Tooltip("The detail of each LOD level")]
	[Range(0, 3)]
	public int Subdivisions;

	[Tooltip("Normal generation strategy")]
	public NormalsType Normals;

	[Tooltip("Generate tangent data?")]
	public bool Tangents;

	[Tooltip("Should the bounds match the size of the terrain?")]
	public bool CenterBounds;

	[Tooltip("The maximum LOD depth that colliders will be generated for (0 = none)")]
	public int MaxColliderDepth;

	[Tooltip("The LOD distances in local space, these should be sorted from high to low")]
	public List<double> Distances;

	[System.NonSerialized]
	public bool DelayRebuild;

	[System.NonSerialized]
	public bool DelayUpdateRenderers;

	[System.NonSerialized]
	public bool DelayUpdateColliders;

	[System.NonSerialized]
	public List<SgtVector3D> LocalPoints = new List<SgtVector3D>();

	[System.NonSerialized]
	public List<double> LocalDistances = new List<double>();

	// Each face handles a cube face
	public SgtTerrainFace NegativeX;
	public SgtTerrainFace NegativeY;
	public SgtTerrainFace NegativeZ;
	public SgtTerrainFace PositiveX;
	public SgtTerrainFace PositiveY;
	public SgtTerrainFace PositiveZ;

	// Called when the displacement of a specific point is being calculated
	public CalculateHeightDelegate OnCalculateHeight;

	// Called when a face is spawned
	public CalculateFaceDelegate OnSpawnFace;

	public CalculateFaceDelegate OnDespawnFace;

	// Called when a level's material needs to be set
	public CalculateMaterialDelegate OnCalculateMaterial;

	// Called when a vertex color needs to be calculated
	public CalculateColorDelegate OnCalculateColor;

	public CalculateFaceDelegate OnPostProcessVertices;

	private void CalculateLocalValues()
	{
		LocalPoints.Clear();

		if (Targets != null)
		{
			for (var i = Targets.Count - 1; i >= 0; i--)
			{
				var target = Targets[i];

				if (target != null)
				{
					var point = new SgtVector3D(transform.InverseTransformPoint(target.position));

					LocalPoints.Add(point);
				}
			}
		}

		LocalDistances.Clear();

		for (var i = 0; i < 32; i++)
		{
			if (Distances != null && i < Distances.Count)
			{
				var distance = Distances[i];

				LocalDistances.Add(distance * distance);
			}
			else
			{
				LocalDistances.Add(double.NegativeInfinity);
			}
		}
	}

	// Gets the surface height under the input point in local space
	public double GetLocalHeight(SgtVector3D localPoint)
	{
		var height = Radius;

		if (OnCalculateHeight != null)
		{
			OnCalculateHeight(localPoint.normalized * Radius, ref height);
		}

		return height;
	}

	public float GetWorldHeight(Vector3 worldPoint)
	{
		var surfacePoint = GetWorldPoint(worldPoint);

		return Vector3.Distance(transform.position, surfacePoint);
	}

	public SgtVector3D GetLocalPoint(SgtVector3D localCube)
	{
		return localCube.normalized * GetLocalHeight(localCube);
	}

	public Vector3 GetWorldPoint(Vector3 position)
	{
		var localPosition = transform.InverseTransformPoint(position);
		var localPoint    = new SgtVector3D(localPosition);

		localPoint = GetLocalPoint(localPoint);

		return transform.TransformPoint((Vector3)localPoint);
	}

	public SgtVector3D GetLocalNormal(SgtVector3D point, SgtVector3D right, SgtVector3D up)
	{
		var localPoint  = GetLocalPoint(point);
		var localPointR = GetLocalPoint(localPoint + right);
		var localPointU = GetLocalPoint(localPoint - up);
		var vectorR     = localPointR - localPoint;
		var vectorU     = localPointU - localPoint;

		return SgtVector3D.Cross(vectorR, vectorU).normalized;
	}

	public Vector3 GetWorldNormal(Vector3 point, Vector3 right, Vector3 up)
	{
		var worldPoint  = GetWorldPoint(point);
		var worldPointR = GetWorldPoint(point + right);
		var worldPointU = GetWorldPoint(point + up);
		var vectorR     = worldPointR - worldPoint;
		var vectorU     = worldPointU - worldPoint;

		return Vector3.Cross(vectorR, vectorU).normalized;
	}

	public Vector3 GetWorldNormal(Vector3 point)
	{
		return Vector3.Normalize(point - transform.position);
	}

	[ContextMenu("Rebuild")]
	public void Rebuild()
	{
		DelayRebuild = false;
		
		ValidateFaces();

		NegativeX.UpdateInvalid();
		NegativeY.UpdateInvalid();
		NegativeZ.UpdateInvalid();
		PositiveX.UpdateInvalid();
		PositiveY.UpdateInvalid();
		PositiveZ.UpdateInvalid();
	}

	[ContextMenu("Update States")]
	public void UpdateStates()
	{
		ValidateFaces();
		CalculateLocalValues();

		if (NegativeX != null) NegativeX.UpdateStates();
		if (NegativeY != null) NegativeY.UpdateStates();
		if (NegativeZ != null) NegativeZ.UpdateStates();
		if (PositiveX != null) PositiveX.UpdateStates();
		if (PositiveY != null) PositiveY.UpdateStates();
		if (PositiveZ != null) PositiveZ.UpdateStates();
	}

	[ContextMenu("Update Renderers")]
	public void UpdateRenderers()
	{
		DelayUpdateRenderers = false;

		if (NegativeX != null) NegativeX.UpdateRenderers();
		if (NegativeY != null) NegativeY.UpdateRenderers();
		if (NegativeZ != null) NegativeZ.UpdateRenderers();
		if (PositiveX != null) PositiveX.UpdateRenderers();
		if (PositiveY != null) PositiveY.UpdateRenderers();
		if (PositiveZ != null) PositiveZ.UpdateRenderers();
	}

	[ContextMenu("Update Colliders")]
	public void UpdateColliders()
	{
		DelayUpdateColliders = false;

		if (NegativeX != null) NegativeX.UpdateColliders();
		if (NegativeY != null) NegativeY.UpdateColliders();
		if (NegativeZ != null) NegativeZ.UpdateColliders();
		if (PositiveX != null) PositiveX.UpdateColliders();
		if (PositiveY != null) PositiveY.UpdateColliders();
		if (PositiveZ != null) PositiveZ.UpdateColliders();
	}

	public void RunFaces(CalculateFaceDelegate method)
	{
		RunFaces(NegativeX, method);
		RunFaces(NegativeY, method);
		RunFaces(NegativeZ, method);
		RunFaces(PositiveX, method);
		RunFaces(PositiveY, method);
		RunFaces(PositiveZ, method);
	}

	public void RunFaces(SgtTerrainFace face, CalculateFaceDelegate method)
	{
		if (face != null)
		{
			method(face);

			if (face.Split == true)
			{
				RunFaces(face.ChildBL, method);
				RunFaces(face.ChildBR, method);
				RunFaces(face.ChildTL, method);
				RunFaces(face.ChildTR, method);
			}
		}
	}

	public void ValidateFaces()
	{
		if (NegativeX == null) NegativeX = CreateFace(CubemapFace.NegativeX, new SgtVector3D(-1.0, -1.0,  1.0), new SgtVector3D(-1.0, -1.0, -1.0), new SgtVector3D(-1.0,  1.0,  1.0), new SgtVector3D(-1.0,  1.0, -1.0));
		if (NegativeY == null) NegativeY = CreateFace(CubemapFace.NegativeY, new SgtVector3D( 1.0, -1.0, -1.0), new SgtVector3D(-1.0, -1.0, -1.0), new SgtVector3D( 1.0, -1.0,  1.0), new SgtVector3D(-1.0, -1.0,  1.0));
		if (NegativeZ == null) NegativeZ = CreateFace(CubemapFace.NegativeZ, new SgtVector3D(-1.0, -1.0, -1.0), new SgtVector3D( 1.0, -1.0, -1.0), new SgtVector3D(-1.0,  1.0, -1.0), new SgtVector3D( 1.0,  1.0, -1.0));
		if (PositiveX == null) PositiveX = CreateFace(CubemapFace.PositiveX, new SgtVector3D( 1.0, -1.0, -1.0), new SgtVector3D( 1.0, -1.0,  1.0), new SgtVector3D( 1.0,  1.0, -1.0), new SgtVector3D( 1.0,  1.0,  1.0));
		if (PositiveY == null) PositiveY = CreateFace(CubemapFace.PositiveY, new SgtVector3D( 1.0,  1.0,  1.0), new SgtVector3D(-1.0,  1.0,  1.0), new SgtVector3D( 1.0,  1.0, -1.0), new SgtVector3D(-1.0,  1.0, -1.0));
		if (PositiveZ == null) PositiveZ = CreateFace(CubemapFace.PositiveZ, new SgtVector3D( 1.0, -1.0,  1.0), new SgtVector3D(-1.0, -1.0,  1.0), new SgtVector3D( 1.0,  1.0,  1.0), new SgtVector3D(-1.0,  1.0,  1.0));

		NegativeX.NeighbourL.Set(PositiveZ, 0, 0, 2, 1, 1, 3, 1);
		NegativeX.NeighbourR.Set(NegativeZ, 1, 1, 3, 0, 0, 2, 0);
		NegativeX.NeighbourB.Set(NegativeY, 2, 0, 1, 1, 3, 1, 5);
		NegativeX.NeighbourT.Set(PositiveY, 3, 2, 3, 1, 1, 3, 1);

		NegativeY.NeighbourL.Set(PositiveX, 0, 0, 2, 2, 0, 1, 2);
		NegativeY.NeighbourR.Set(NegativeX, 1, 1, 3, 2, 1, 0, 6);
		NegativeY.NeighbourB.Set(NegativeZ, 2, 0, 1, 2, 1, 0, 6);
		NegativeY.NeighbourT.Set(PositiveZ, 3, 2, 3, 2, 0, 1, 2);

		NegativeZ.NeighbourL.Set(NegativeX, 0, 0, 2, 1, 1, 3, 1);
		NegativeZ.NeighbourR.Set(PositiveX, 1, 1, 3, 0, 0, 2, 0);
		NegativeZ.NeighbourB.Set(NegativeY, 2, 0, 1, 2, 1, 0, 6);
		NegativeZ.NeighbourT.Set(PositiveY, 3, 2, 3, 3, 3, 2, 7);

		PositiveX.NeighbourL.Set(NegativeZ, 0, 0, 2, 1, 1, 3, 1);
		PositiveX.NeighbourR.Set(PositiveZ, 1, 1, 3, 0, 0, 2, 0);
		PositiveX.NeighbourB.Set(NegativeY, 2, 0, 1, 0, 0, 2, 0);
		PositiveX.NeighbourT.Set(PositiveY, 3, 2, 3, 0, 2, 0, 4);

		PositiveY.NeighbourL.Set(PositiveX, 0, 0, 2, 3, 3, 2, 7);
		PositiveY.NeighbourR.Set(NegativeX, 1, 1, 3, 3, 2, 3, 3);
		PositiveY.NeighbourB.Set(PositiveZ, 2, 0, 1, 3, 2, 3, 3);
		PositiveY.NeighbourT.Set(NegativeZ, 3, 2, 3, 3, 3, 2, 7);

		PositiveZ.NeighbourL.Set(PositiveX, 0, 0, 2, 1, 1, 3, 1);
		PositiveZ.NeighbourR.Set(NegativeX, 1, 1, 3, 0, 0, 2, 0);
		PositiveZ.NeighbourB.Set(NegativeY, 2, 0, 1, 3, 2, 3, 3);
		PositiveZ.NeighbourT.Set(PositiveY, 3, 2, 3, 2, 0, 1, 2);
	}

	public static SgtTerrain CreateTerrain(int layer = 0, Transform parent = null)
	{
		return CreateTerrain(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtTerrain CreateTerrain(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		var gameObject = SgtHelper.CreateGameObject("Terrain", layer, parent, localPosition, localRotation, localScale);
		var terrain    = gameObject.AddComponent<SgtTerrain>();

		return terrain;
	}

#if UNITY_EDITOR
	[MenuItem(SgtHelper.GameObjectMenuPrefix + "Terrain", false, 10)]
	public static void CreateTerrainMenuItem()
	{
		var parent  = SgtHelper.GetSelectedParent();
		var terrain = CreateTerrain(parent != null ? parent.gameObject.layer : 0, parent);

		SgtHelper.SelectAndPing(terrain);
	}
#endif

	protected virtual void OnEnable()
	{
		AllTerrains.Add(this);

		if (NegativeX != null) NegativeX.gameObject.SetActive(true);
		if (NegativeY != null) NegativeY.gameObject.SetActive(true);
		if (NegativeZ != null) NegativeZ.gameObject.SetActive(true);
		if (PositiveX != null) PositiveX.gameObject.SetActive(true);
		if (PositiveY != null) PositiveY.gameObject.SetActive(true);
		if (PositiveZ != null) PositiveZ.gameObject.SetActive(true);

		UpdateStates();
	}

	protected virtual void Update()
	{
		if (DelayRebuild == true)
		{
			Rebuild();
		}

		if (DelayUpdateRenderers == true)
		{
			UpdateRenderers();
		}

		if (DelayUpdateColliders == true)
		{
			UpdateColliders();
		}

		UpdateStates();
	}

	protected virtual void OnDisable()
	{
		AllTerrains.Remove(this);

		if (NegativeX != null) NegativeX.gameObject.SetActive(false);
		if (NegativeY != null) NegativeY.gameObject.SetActive(false);
		if (NegativeZ != null) NegativeZ.gameObject.SetActive(false);
		if (PositiveX != null) PositiveX.gameObject.SetActive(false);
		if (PositiveY != null) PositiveY.gameObject.SetActive(false);
		if (PositiveZ != null) PositiveZ.gameObject.SetActive(false);
	}

	protected virtual void OnDestroy()
	{
		SgtTerrainFace.MarkForDestruction(NegativeX);
		SgtTerrainFace.MarkForDestruction(NegativeY);
		SgtTerrainFace.MarkForDestruction(NegativeZ);
		SgtTerrainFace.MarkForDestruction(PositiveX);
		SgtTerrainFace.MarkForDestruction(PositiveY);
		SgtTerrainFace.MarkForDestruction(PositiveZ);
	}

	private SgtTerrainFace CreateFace(CubemapFace side, SgtVector3D cornerBL, SgtVector3D cornerBR, SgtVector3D cornerTL, SgtVector3D cornerTR)
	{
		var face = SgtTerrainFace.Create(side.ToString(), gameObject.layer, transform);

		face.Terrain  = this;
		face.Side     = side;
		face.Depth    = 0;
		face.CornerBL = cornerBL;
		face.CornerBR = cornerBR;
		face.CornerTL = cornerTL;
		face.CornerTR = cornerTR;
		face.CoordBL  = new SgtVector2D(0.0, 0.0);
		face.CoordBR  = new SgtVector2D(1.0, 0.0);
		face.CoordTL  = new SgtVector2D(0.0, 1.0);
		face.CoordTR  = new SgtVector2D(1.0, 1.0);

		return face;
	}
}