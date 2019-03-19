using UnityEngine;
using System.Collections;

public class ExampleClass : MonoBehaviour
{

    void Start()
    {
        LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.SetPosition(0, new Vector3(-0.3f, 2.29f, 0));
        lineRenderer.SetPosition(1, new Vector3(2f, 4.29f, 0));
    }
}