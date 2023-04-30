using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPoseEstimation : MonoBehaviour
{
    // Assignables
    public int updateRate = 10;
    public Camera cam;
    public GameObject camera_obj;
    public GameObject target_obj;
    public GameObject manipulator_obj;

    // The line renderer component used to draw the orientation vector
    private LineRenderer lineRenderer;

    // The orientation vector from the source to the target
    private Vector3 orientation;
    private Vector3 baseline;
    private Vector3 mid_point;
    private Vector3 X;
    private float d;  // distance

    private float theta = 1.5708f;
    private float phi = 0.35f;

    // Start is called before the first frame update
    void Start()
    {
        X = new Vector3(1f, 0f, 0f);
        // Get the LineRenderer component attached to this object, or add one if it doesn't exist
        lineRenderer = gameObject.GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
        }

        // Set the properties of the LineRenderer
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the orientation vector from the source to the target
        mid_point = target_obj.transform.position - camera_obj.transform.position;
        orientation = mid_point - camera_obj.transform.position;
        baseline = target_obj.transform.position - manipulator_obj.transform.position;

        mid_point = (target_obj.transform.position + manipulator_obj.transform.position) / 2;

        // Normalize the orientation vector
        orientation.Normalize();

        // Update the line renderer to visualize the orientation vector
        lineRenderer.SetPosition(0, camera_obj.transform.position);
        lineRenderer.SetPosition(1, camera_obj.transform.position + orientation * 5f);

        // Calculate the new coordinate system
        Vector3 newX = baseline - Vector3.Dot(baseline, Vector3.forward) * Vector3.forward;
        newX.Normalize();
        Vector3 newY = Vector3.Cross(Vector3.forward, newX);
        Matrix4x4 transformationMatrix = new Matrix4x4(
            new Vector4(newX.x, newX.y, newX.z, 0),
            new Vector4(newY.x, newY.y, newY.z, 0),
            new Vector4(Vector3.forward.x, Vector3.forward.y, Vector3.forward.z, 0),
            new Vector4(mid_point.x, mid_point.y, mid_point.z, 1)
        );

        // Calculate the camera's new forward direction (z-axis)
        Vector3 newForward = orientation;

        // Calculate the camera's new up direction (y-axis) aligned with world's z-axis
        Vector3 newUp = Vector3.Cross(newForward, Vector3.right).normalized;

        // Compute the new rotation quaternion
        Quaternion transformedRotation = Quaternion.LookRotation(newForward, newUp);

        Matrix4x4 sphericalToWorld = transformationMatrix.inverse;


        Vector3 camera_position_spherical_goal;
        
        Vector3 camera_position_spherical1;
        camera_position_spherical1.x = 0.0f;
        camera_position_spherical1.y = d * Mathf.Cos(phi);
        camera_position_spherical1.z = d * Mathf.Sin(phi);

        Vector3 camera_position_spherical2;
        camera_position_spherical2.x = 0.0f;
        camera_position_spherical2.y = -d * Mathf.Cos(phi);
        camera_position_spherical2.z = d * Mathf.Sin(phi);

        Vector3 camera_position_spherical = transformationMatrix.MultiplyPoint3x4(camera_obj.transform.position);

        if ((camera_position_spherical - camera_position_spherical1).magnitude > (camera_position_spherical - camera_position_spherical2).magnitude)
        {
            camera_position_spherical_goal = camera_position_spherical2;
        }
        else
        {
            camera_position_spherical_goal = camera_position_spherical1;
        }



        // Apply the transformation matrix to the world coordinate system
        Vector3 transformedPosition = sphericalToWorld.MultiplyPoint3x4(camera_position_spherical_goal);
        Debug.Log("Position: " + transformedPosition.ToString());
        Debug.Log("Roatation: " + transformedRotation.ToString());
    }
}
