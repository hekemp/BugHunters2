using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtJovianLights))]
public class SgtJovianLights_Editor : SgtEditor<SgtJovianLights>
{
	protected override void OnInspector()
	{
		var updateLights = false;
		var updateApply   = false;

		BeginError(Any(t => t.Jovian == null));
			DrawDefault("Jovian", ref updateApply);
		EndError();
		DrawDefault("RequireSameLayer", ref updateLights);
		DrawDefault("RequireSameTag", ref updateLights);
		DrawDefault("RequireNameContains", ref updateLights);

		if (updateLights == true) DirtyEach(t => t.UpdateLights());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Jovian Lights")]
public class SgtJovianLights : MonoBehaviour
{
	[Tooltip("The lights will be copied to this")]
	public SgtJovian Jovian;

	[Tooltip("Filter all the lights to require the same layer at this GameObject")]
	public bool RequireSameLayer;

	[Tooltip("Filter all the lights to require the same tag at this GameObject")]
	public bool RequireSameTag;

	[Tooltip("Filter all the lights to require a name that contains this")]
	public string RequireNameContains;

	[SerializeField]
	private bool startCalled;

	[ContextMenu("Update Lights")]
	public void UpdateLights()
	{
		if (Jovian != null)
		{
			SgtHelper.GetLights(gameObject, ref Jovian.Lights, RequireSameLayer, RequireSameTag, RequireNameContains);
		}
	}

	protected virtual void OnEnable()
	{
		if (startCalled == true)
		{
			UpdateLights();
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

			UpdateLights();
		}
	}
}