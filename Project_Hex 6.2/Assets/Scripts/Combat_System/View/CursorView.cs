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
        Plane plane = new Plane(Vector3.forward, Vector3.zero); // plan Z=0
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            CursorGameObject.transform.position = worldPos;
        }
    }
}
