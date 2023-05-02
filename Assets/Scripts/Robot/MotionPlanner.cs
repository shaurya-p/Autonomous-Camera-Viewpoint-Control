using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionPlanner : MonoBehaviour
{
    // End Effector State
    public GameObject leftEndEffectorLink;
    public GameObject rightEndEffectorLink;
    

    // Targets
    public TargetLocalization targetLocalizer;
    private Vector3[] targets;
    private Vector3 targetPosition;
    private Vector3 manipulatorPosition;
    private Vector3 camPosition;

    // Camera-to-target variables
    private Vector3 orientation;
    private Vector3 baseline;
    private Vector3 centroid;

    // New coordinate system
    private Vector3 newX;
    private Vector3 newY;
    private Vector3 newZ;

    // Camera viewpoint adjustment parameters
    private float deadZone;
    private float alpha;                // FOV between targets
    private float d;                    // Distance to centroid
    private float phi;                  // Vantage point angle

    // Goal pose estimation
    public Vector3 goalPosition;
    public Quaternion goalRotation;  

    // DEBUGGING
    public GameObject DebugSphere;
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
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

        // Set camera viewpoint performance parameters
        deadZone = 0.03f;
        d = 0.7f;
        phi = 0.45f;
        alpha = (float)(30 * Mathf.PI / 180);
    }

    // Update is called once per frame
    void Update()
    {

        if (targetLocalizer == null)
            targetLocalizer = GameObject.FindObjectOfType<TargetLocalization>();

        // Target pose
        targetPosition = targetLocalizer.cube.transform.position;

        // Manipulator pose
        manipulatorPosition = rightEndEffectorLink.transform.position;

        // Cam pose
        camPosition = targetLocalizer.cam.transform.position;

        // Compute centroid of targets
        targets = new Vector3[] {manipulatorPosition, targetPosition};
        // Debug.Log("Count " + targets.Count);

        // Calculate centroid 
        centroid = (manipulatorPosition + targetPosition) / 2f ;

        // Calculate baseline vector between manipulator and centroid
        baseline = centroid - manipulatorPosition;

        // Calculate distance to target
        d = Mathf.Max(0.5f, (baseline).magnitude / Mathf.Tan(alpha / 2));

        Debug.Log("baseline " + baseline.magnitude);

        // Calculate the new coordinate system
        newZ = Vector3.up;
        newX = baseline - Vector3.Dot(baseline, Vector3.up) * Vector3.up;
        newX.Normalize();
        newY = Vector3.Cross(newZ, newX);

        Matrix4x4 spherical2World = new Matrix4x4(
            new Vector4(newX.x, newX.y, newX.z, 0),
            new Vector4(newY.x, newY.y, newY.z, 0),
            new Vector4(newZ.x, newZ.y, newZ.z, 0),
            new Vector4(centroid.x, centroid.y, centroid.z, 1)
        );

        // Define inverse transformation matrix
        Matrix4x4 world2Spherical = spherical2World.inverse;

        // Initialize camera goal position
        Vector3 camera_position_spherical_goal;

        // Calculate "elbow-up" or "elbow-down" candidate camera positions in spherical frame
        Vector3 camera_position_spherical1 = new Vector3(0f, d * Mathf.Cos(phi), d * Mathf.Sin(phi));
        Vector3 camera_position_spherical2 = new Vector3(0f, -d * Mathf.Cos(phi), d * Mathf.Sin(phi));

        // Compute current camera position in spherical frame
        Vector3 camera_position_spherical = world2Spherical * new Vector4(camPosition.x, camPosition.y, camPosition.z, 1);

        // Set new camera position to candidate position closest to the current camera position
        if ((camera_position_spherical - camera_position_spherical1).magnitude > (camera_position_spherical - camera_position_spherical2).magnitude)
            camera_position_spherical_goal = camera_position_spherical2;
        else
            camera_position_spherical_goal = camera_position_spherical1;

        // Transform new camera position to world frame
        Vector3 transformedPosition = spherical2World * new Vector4(camera_position_spherical_goal.x, camera_position_spherical_goal.y, camera_position_spherical_goal.z, 1);

        // Define camera orientation vector
        orientation = centroid - transformedPosition;
        orientation.Normalize();

        // Align camera's viewing direction (y-axis) with orientation
        Vector3 newUp = orientation;

        // Calculate camera's left hand axis (z-axis) constrained to be perpendicular to the world y-axis
        Vector3 newForward = Vector3.Cross(newUp, Vector3.up).normalized;

        // Compute the new rotation quaternion
        Quaternion transformedRotation = Quaternion.LookRotation(newForward, newUp);

        // Assign goal position if greater than some dead-zone threshold
        if ((goalPosition - transformedPosition).magnitude > deadZone)
        {
            goalPosition = transformedPosition;
            goalRotation = transformedRotation;
        }


        // DEBUG
        // Update the line renderer to visualize the orientation vector
        // lineRenderer.SetPosition(0, goalPosition);
        // lineRenderer.SetPosition(1, goalPosition + orientation * 5f);
        //DebugSphere.transform.position = transformedPosition;
        //DebugSphere.transform.rotation = transformedRotation;

    }


    public Vector3 Centroid(Vector3[] targets)
    {
        if (targets.Length == 1)
            return targets[0];
        else
        {
            Vector3 avg = Vector3.zero;
            foreach (Vector3 target in targets)
            {
                avg += target;
            }
            avg /= targets.Length;
            return avg;
        }
    }

    public Vector3[] GetNewCoords()
    {
        return new Vector3[] { newX, newY, newZ };
    }

    public Vector3 GetCentroid()
    {
        return centroid;
    }
}