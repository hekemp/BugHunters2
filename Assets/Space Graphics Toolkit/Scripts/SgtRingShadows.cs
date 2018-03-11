using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtRingShadows))]
public class SgtRingShadows_Editor : SgtEditor<SgtRingShadows>
{
	protected override void OnInspector()
	{
		var updateShadows = false;
		var updateApply   = false;

		BeginError(Any(t => t.Ring == null));
			DrawDefault("Ring", ref updateApply);
		EndError();
		DrawDefault("RequireSameLayer", ref updateShadows);
		DrawDefault("RequireSameTag", ref updateShadows);
		DrawDefault("RequireNameContains", ref updateShadows);

		if (updateShadows == true) DirtyEach(t => t.UpdateShadows());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Ring Shadows")]
public class SgtRingShadows : MonoBehaviour
{
	[Tooltip("The shadows will be copied to this")]
	public SgtRing Ring;

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
		if (Ring != null)
		{
			SgtHelper.GetShadows(gameObject, ref Ring.Shadows, RequireSameLayer, RequireSameTag, RequireNameContains);
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

			if (Ring == null)
			{
				Ring = GetComponent<SgtRing>();
			}

			UpdateShadows();
		}
	}
}