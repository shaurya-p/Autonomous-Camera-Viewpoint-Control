using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetState : MonoBehaviour
{

    public GameObject[] targetObjects;
    public GameObject manipulator;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetGeometricCenter(Vector3 point1, Vector3 point2)
    {
        return ((point1 + point2) / 2.0f);
    }

    public Vector3 Space2Image(Vector3 point)
    {
        return cam.WorldToScreenPoint(point);
    }


    public bool IsVisible(Vector3 screen_point)
    {
        return ((screen_point.x >= 0) && (screen_point.x <= cam.pixelWidth))
                && ((screen_point.y >= 0) && (screen_point.y <= cam.pixelHeight));
    }

}
