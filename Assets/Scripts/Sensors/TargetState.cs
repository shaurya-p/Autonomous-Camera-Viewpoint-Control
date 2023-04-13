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

    /*public bool IsWithinImage(Vector3 point, Camera cam)
    {
        Vector2 camDimensions = new Vector2(cam.pixelWidth, cam.pixelHeight);
        Vector3 imageProjection = cam.WorldToScreenPoint(point);
        if ( - )
        {

        }
    }*/

}
