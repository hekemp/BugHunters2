using UnityEngine;

public static class TransformExtensions
{
    public static void SetLayerRecursive(this Transform transform, int layer)
    {
        transform.gameObject.layer = layer;
        foreach (Transform child in transform)
        {
            child.SetLayerRecursive(layer);
        }
    }
}