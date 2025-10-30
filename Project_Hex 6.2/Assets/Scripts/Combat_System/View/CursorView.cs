using UnityEngine;

public class CursorView : MonoBehaviour
{
    [SerializeField] GameObject cursorView;
    [SerializeField] GameObject CursorGameObject;

    public void Start()
    {
        Cursor.visible = false;
    }

    public void Update()
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = -1;
        Vector3 endPos = mouseWorldPos;
        CursorGameObject.transform.position = endPos;
    }
}
