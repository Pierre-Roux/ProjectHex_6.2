using UnityEngine;

public class CursorView : MonoBehaviour
{
    [SerializeField] public RectTransform cursorRect;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        cursorRect.position = Input.mousePosition;
    }
}