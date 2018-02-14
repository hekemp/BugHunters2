#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class VRUIItem : MonoBehaviour
{
    private BoxCollider boxCollider;
    private RectTransform rectTransform;

    private void OnRectTransformDimensionsChange()
    {
        ValidateCollider();
    }

    [ContextMenu("Rebuild Collider")]
    public void ValidateCollider()
    {
        rectTransform = GetComponent<RectTransform>();

        boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        boxCollider.size = rectTransform.rect.size;

        var yOffset = Mathf.Lerp(boxCollider.size.y / 2f, boxCollider.size.y / -2f, rectTransform.pivot.y);
        boxCollider.center = new Vector3(0, yOffset, 0);
    }

    
}

#if UNITY_EDITOR
[CustomEditor(typeof(VRUIItem))]
public class VRUIItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Click"))
        {
            EventSystem.current.SetSelectedGameObject((target as Component).gameObject);
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }

        if (GUILayout.Button("Rebuild Collider"))
        {
            (target as VRUIItem).ValidateCollider();
        }
    }
}
#endif