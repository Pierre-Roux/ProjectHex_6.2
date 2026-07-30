using UnityEngine;

public class FlickerMat : MonoBehaviour
{
    public float intensityMin = 0.5f;
    public float intensityMax = 2.0f;

    public float toleranceMin = 0.1f;
    public float toleranceMax = 1.0f;

    public float flickSpeed = 1f;
    public float speedVariation = 0.2f;

    private Material ObjectMaterial;

    private float flickTime;
    private float currentSpeed;
    private bool wasGoingBack;

    void Start()
    {
        ChangeSpeed();
    }

    void Update()
    {
        if (ObjectMaterial == null)
        {
            ObjectMaterial = GetComponent<InstanciateMaterial>().runtimeMaterial;
        }

        flickTime += Time.deltaTime * currentSpeed;

        float flick = Mathf.PingPong(flickTime, 1f);

        bool goingBack = flickTime % 2f > 1f;

        if (goingBack != wasGoingBack)
        {
            ChangeSpeed();
            wasGoingBack = goingBack;
        }

        float intensity = Mathf.Lerp(intensityMin, intensityMax, flick);
        float tolerance = Mathf.Lerp(toleranceMin, toleranceMax, flick);

        ObjectMaterial.SetFloat("_Intensity", intensity);
        ObjectMaterial.SetFloat("_tolerance", tolerance);
    }

    void ChangeSpeed()
    {
        currentSpeed = flickSpeed * Random.Range(1f - speedVariation, 1f + speedVariation);
    }
}

