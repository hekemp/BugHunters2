using UnityEngine;

public class PlayerHead : MonoBehaviour
{
    private Transform headTransform;
    private NetworkPlayer player;
    private Renderer renderer;

    public Vector3 LocalPlayerPosition { get { return headTransform.position; } }
    public Quaternion LocalPlayerRotation { get { return headTransform.rotation; } }
	public static Transform LocalTransform { get; private set; }

    private void Awake()
    {
        player = GetComponentInParent<NetworkPlayer>();
        renderer = GetComponent<Renderer>();
    }
    
    private void Update()
    {
        if (player.isLocalPlayer && headTransform != null)
        {
            transform.position = headTransform.position;
            transform.rotation = headTransform.rotation;
            renderer.enabled = false;
			LocalTransform = headTransform;
		}
    }

    internal void SetTransform(Transform transform)
    {
        this.headTransform = transform;
    }
}