using UnityEngine;

public class PlayerStartPoint : MonoBehaviour
{
    [SerializeField]
    [Range(1, 10)]
    private int teamNumber;

    public int TeamNumber { get { return teamNumber; } }

    private void OnValidate()
    {
        gameObject.name = "StartPoint Team " + teamNumber;
    }
}