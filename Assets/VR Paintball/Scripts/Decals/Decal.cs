using UnityEngine;

/// <summary>
/// This class shows the decals in the game.  These are the paint splats.
/// To change the decals, you can change the materials or colors in the inspector.
/// </summary>
internal class Decal : MonoBehaviour
{
    [SerializeField]
    private Material[] materials;
    [SerializeField]
    private Color[] colors;

    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        renderer.material = RandomMaterial();
        renderer.material.color = RandomColor();
    }

    private Color RandomColor()
    {
        return colors[UnityEngine.Random.Range(0, colors.Length)];
    }

    private Material RandomMaterial()
    {
        return materials[UnityEngine.Random.Range(0, materials.Length)];
    }
}