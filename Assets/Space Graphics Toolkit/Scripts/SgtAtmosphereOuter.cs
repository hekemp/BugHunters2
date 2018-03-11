using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtAtmosphereOuter))]
public class SgtAtmosphereOuter_Editor : SgtEditor<SgtAtmosphereOuter>
{
	protected override void OnInspector()
	{
		BeginDisabled();
			DrawDefault("Atmosphere");
		EndDisabled();
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu("")]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SgtAtmosphereOuter : MonoBehaviour
{
	public class CameraState : SgtCameraState
	{
		public Vector3 LocalPosition;
	}

	[Tooltip("The atmosphere this belongs to")]
	public SgtAtmosphere Atmosphere;

	[System.NonSerialized]
	private MeshFilter meshFilter;

	[System.NonSerialized]
	private MeshRenderer meshRenderer;

	[System.NonSerialized]
	private List<CameraState> cameraStates;

	public void SetMesh(Mesh mesh)
	{
		if (meshFilter == null) meshFilter = gameObject.GetComponent<MeshFilter>();

		if (meshFilter.sharedMesh != mesh)
		{
			meshFilter.sharedMesh = mesh;
		}
	}

	public void SetMaterial(Material material)
	{
		if (meshRenderer == null) meshRenderer = gameObject.GetComponent<MeshRenderer>();

		if (meshRenderer.sharedMaterial != material)
		{
			meshRenderer.sharedMaterial = material;
		}
	}

	public void SetScale(float scale)
	{
		SgtHelper.SetLocalScale(transform, scale);
	}

	public void Save(Camera camera)
	{
		var cameraState = SgtCameraState.Save(ref cameraStates, camera);

		cameraState.LocalPosition = transform.localPosition;
	}

	public void Restore(Camera camera)
	{
		var cameraState = SgtCameraState.Restore(cameraStates, camera);

		if (cameraState != null)
		{
			transform.localPosition = cameraState.LocalPosition;
		}
	}

	public void Revert()
	{
		transform.localPosition = Vector3.zero;
	}

	public static SgtAtmosphereOuter Create(SgtAtmosphere atmosphere)
	{
		var outer = SgtComponentPool<SgtAtmosphereOuter>.Pop(atmosphere.transform, "Outer", atmosphere.gameObject.layer);

		outer.Atmosphere = atmosphere;

		return outer;
	}

	public static void Pool(SgtAtmosphereOuter outer)
	{
		if (outer != null)
		{
			outer.Atmosphere = null;

			SgtComponentPool<SgtAtmosphereOuter>.Add(outer);
		}
	}

	public static void MarkForDestruction(SgtAtmosphereOuter outer)
	{
		if (outer != null)
		{
			outer.Atmosphere = null;

			outer.gameObject.SetActive(true);
		}
	}

	protected virtual void Update()
	{
		if (Atmosphere == null)
		{
			Pool(this);
		}
	}
}