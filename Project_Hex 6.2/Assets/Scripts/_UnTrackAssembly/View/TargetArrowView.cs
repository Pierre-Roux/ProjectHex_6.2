using UnityEngine;

public class TargetArrowView : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform arrowHead;

    private Transform source;
    private Transform target;

    public void Setup(Transform source, Transform target)
    {
        this.source = source;
        this.target = target;
    }

    private void LateUpdate()
    {
        if (source == null || target == null)
        {
            Destroy(gameObject);
            return;
        }

        UpdateArrow();
    }

    private void UpdateArrow()
    {
        Vector3 start = source.position;
        Vector3 end = target.position;

        Vector3 direction = end - start;

        if (direction.sqrMagnitude <= 0.001f)
            return;

        direction.Normalize();

        // Position de la ligne
        line.SetPosition(0, start);
        line.SetPosition(1, end);

        // Pointe de la flèche
        arrowHead.position = end;

        // Oriente la pointe vers la cible
        arrowHead.rotation = Quaternion.FromToRotation(Vector3.up, direction) * Quaternion.Euler(0f, 0f, 90f);
    }
}