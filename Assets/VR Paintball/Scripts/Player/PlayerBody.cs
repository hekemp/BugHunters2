using UnityEngine;

public class PlayerBody : MonoBehaviour
{
    [SerializeField]
    private PlayerHead playerHead;

    private void Update()
    {
        float height = playerHead.transform.position.y - 0.5f;
        transform.position = new Vector3(playerHead.transform.position.x, height, playerHead.transform.position.z);
    }

    private void OnValidate()
    {
        if (playerHead == null)
        {
            playerHead = transform.parent.GetComponentInChildren<PlayerHead>();
        }
    }
}