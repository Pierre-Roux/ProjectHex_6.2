using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpinLoader : MonoBehaviour
{
    [SerializeField] private Image loadingImage;
    [SerializeField] private float rotationSpeed = 200f;

    public bool isRunning = false;

    void Update()
    {
        if (isRunning && loadingImage != null)
        {
            loadingImage.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }
    }
}
