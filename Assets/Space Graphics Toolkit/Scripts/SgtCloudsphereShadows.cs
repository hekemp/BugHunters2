using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtCloudsphereShadows))]
public class SgtCloudsphereShadows_Editor : SgtEditor<SgtCloudsphereShadows>
{
	protected override void OnInspector()
	{
		var updateShadows = false;
		var updateApply   = false;

		BeginError(Any(t => t.Cloudsphere == null));
			DrawDefault("Cloudsphere", ref updateApply);
		EndError();
		DrawDefault("RequireSameLayer", ref updateShadows);
		DrawDefault("RequireSameTag", ref updateShadows);
		DrawDefault("RequireNameContains", ref updateShadows);

		if (updateShadows == true) DirtyEach(t => t.UpdateShadows());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Cloudsphere Shadows")]
public class SgtCloudsphereShadows : MonoBehaviour
{
	[Tooltip("The shadows will be copied to this")]
	public SgtCloudsphere Cloudsphere;

	[Tooltip("Filter all the shadows to require the same layer at this GameObject")]
	public bool RequireSameLayer;

	[Tooltip("Filter all the shadows to require the same tag at this GameObject")]
	public bool RequireSameTag;

	[Tooltip("Filter all the shadows to require a name that contains this")]
	public string RequireNameContains;

	[SerializeField]
	private bool startCalled;

	[ContextMenu("Update Shadows")]
	public void UpdateShadows()
	{
		if (Cloudsphere != null)
		{
			SgtHelper.GetShadows(gameObject, ref Cloudsphere.Shadows, RequireSameLayer, RequireSameTag, RequireNameContains);
		}
	}

	protected virtual void OnEnable()
	{
		if (startCalled == true)
		{
			UpdateShadows();
		}
	}

	protected virtual void Start()
	{
		if (startCalled == false)
		{
			startCalled = true;

			if (Cloudsphere == null)
			{
				Cloudsphere = GetComponent<SgtCloudsphere>();
			}

			UpdateShadows();
		}
	}
}