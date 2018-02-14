using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(SgtBeltLights))]
public class SgtBeltLights_Editor : SgtEditor<SgtBeltLights>
{
	protected override void OnInspector()
	{
		var updateLights = false;
		var updateApply  = false;

		BeginError(Any(t => t.Belt == null));
			DrawDefault("Belt", ref updateApply);
		EndError();
		DrawDefault("RequireSameLayer", ref updateLights);
		DrawDefault("RequireSameTag", ref updateLights);
		DrawDefault("RequireNameContains", ref updateLights);

		if (updateLights == true) DirtyEach(t => t.UpdateLights());
	}
}
#endif

[ExecuteInEditMode]
[AddComponentMenu(SgtHelper.ComponentMenuPrefix + "Belt Lights")]
public class SgtBeltLights : MonoBehaviour
{
	[Tooltip("The lights will be copied to this")]
	public SgtBelt Belt;

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
		if (Belt != null)
		{
			SgtHelper.GetLights(gameObject, ref Belt.Lights, RequireSameLayer, RequireSameTag, RequireNameContains);
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

			if (Belt == null)
			{
				Belt = GetComponent<SgtBelt>();
			}

			UpdateLights();
		}
	}
}