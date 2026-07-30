using UnityEngine;

public class InstanciateMaterial : MonoBehaviour
{
private Renderer rend;
[HideInInspector] public Material runtimeMaterial;

    void Start()
    {
        rend = GetComponent<Renderer>();
        runtimeMaterial = new Material(rend.material);
        rend.material = runtimeMaterial;
    }

    void OnDestroy()
    {
        if (runtimeMaterial != null)
        {
            Destroy(runtimeMaterial);
        }
    }
}
