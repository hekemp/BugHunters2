using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtJovianShadows))]
public class SgtJovianShadows_Editor : SgtEditor<SgtJovianShadows>
{
	protected override void OnInspector()
	{
		var updateShadows = false;
		var updateApply   = false;

		BeginError(Any(t => t.Jovian == null));
			DrawDefault("Jovian", ref updateApply);
		EndError();
		DrawDefault("RequireSameLayer", ref updateShadows);
		DrawDefault("RequireSameTag", ref updateShadows);
		DrawDefault("RequireNameContains", ref updateShadows);

		if (updateShadows == true) DirtyEach(t => t.UpdateShadows());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Jovian Shadows")]
public class SgtJovianShadows : MonoBehaviour
{
	[Tooltip("The shadows will be copied to this")]
	public SgtJovian Jovian;

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
		if (Jovian != null)
		{
			SgtHelper.GetShadows(gameObject, ref Jovian.Shadows, RequireSameLayer, RequireSameTag, RequireNameContains);
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

			if (Jovian == null)
			{
				Jovian = GetComponent<SgtJovian>();
			}

			UpdateShadows();
		}
	}
}