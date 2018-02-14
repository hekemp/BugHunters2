using UnityEngine;
using System.Collections.Generic;

public abstract class SgtShadow : MonoBehaviour
{
	// All active and enabled shadows in the scene
	public static List<SgtShadow> AllShadows = new List<SgtShadow>();

	[Tooltip("The light that this shadow is being cast away from")]
	public Light Light;

	public abstract Texture GetTexture();

	protected virtual void OnEnable()
	{
		AllShadows.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllShadows.Remove(this);
	}

	public virtual bool CalculateShadow(ref Matrix4x4 matrix, ref float ratio)
	{
		if (SgtHelper.Enabled(Light) == true && Light.intensity > 0.0f)
		{
			return true;
		}

		return false;
	}
}