using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionPlanner : MonoBehaviour
{
    // Targets
    public TargetLocalization targetLocalizer;
    private Vector3 targetPosition;
    public GameObject rightEndEffectorLink;
    private Vector3 manipulatorPosition;
    private Vector3 camPosition;

    // DEBUGGING
    public GameObject DebugSphere;

    // End Effector State
    public GameObject leftEndEffectorLink;
    public Vector3 currentPosition;
    public Quaternion currentRotation;

    // Desired pose
    public Vector3 desiredPosition;         // World Coordinates
    public Quaternion rotation;             // World Coordinates
    public Vector3 offsetVector;            // Local Coordinates
    public Quaternion offsetRotation;       // World Coordinates

    // Goal pose estimation
    public GameObject camera_obj;
    public GameObject target_obj;
    public GameObject manipulator_obj;

    // The line renderer component used to draw the orientation vector
    private LineRenderer lineRenderer;

    // The orientation vector from the source to the target
    private Vector3 orientation;
    private Vector3 baseline;
    public Vector3 mid_point;

    private float d;  // TBC
    private float phi;

    // Coordinate system0
    public Vector3 newX;
    public Vector3 newY;
    public Vector3 newZ;

    // External
    public Vector3 goalPosition;
    public Quaternion goalRotation;


    // Start is called before the first frame update
    void Start()
    {
        offsetVector = new Vector3(0f, 0f, 0f);
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
        d = 0.7f;
        phi = 0.35f;

        if (targetLocalizer == null)
            targetLocalizer = GameObject.FindObjectOfType<TargetLocalization>();

        // Target pose
        targetPosition = targetLocalizer.cube.transform.position;

        // Manipulator pose
        manipulatorPosition = rightEndEffectorLink.transform.position;

        // Cam pose
        camPosition = targetLocalizer.cam.transform.position;

        // Calculate the orientation vector from the source to the target
        mid_point = (targetPosition + manipulatorPosition) / 2;
        orientation = mid_point - camPosition;
        baseline = targetPosition - manipulatorPosition;

        // Calculate the new coordinate system
        newZ = Vector3.up;
        newX = baseline - Vector3.Dot(baseline, Vector3.up) * Vector3.up;
        newX.Normalize();
        newY = Vector3.Cross(newZ, newX);

        Matrix4x4 spherical2World = new Matrix4x4(
            new Vector4(newX.x, newX.y, newX.z, 0),
            new Vector4(newY.x, newY.y, newY.z, 0),
            new Vector4(newZ.x, newZ.y, newZ.z, 0),
            new Vector4(mid_point.x, mid_point.y, mid_point.z, 1)
        );

        

        Matrix4x4 world2Spherical = spherical2World.inverse;

        Vector3 camera_position_spherical_goal;

        Vector3 camera_position_spherical1 = new Vector3(0f, d * Mathf.Cos(phi), d * Mathf.Sin(phi));
        //camera_position_spherical1.x = 0.0f;
        //camera_position_spherical1.y = d * Mathf.Cos(phi);
        //camera_position_spherical1.z = d * Mathf.Sin(phi);

        Vector3 camera_position_spherical2;
        camera_position_spherical2.x = 0.0f;
        camera_position_spherical2.y = -d * Mathf.Cos(phi);
        camera_position_spherical2.z = d * Mathf.Sin(phi);

        
        //DebugSphere.transform.position = spherical2World * new Vector4(camera_position_spherical1.x, camera_position_spherical1.y, camera_position_spherical1.z, 1);
        //Debug.Log(camera_position_spherical1);

        Vector3 camera_position_spherical = world2Spherical * new Vector4(camPosition.x, camPosition.y, camPosition.z, 1);
        // Debug.Log(camera_position_spherical);

        if ((camera_position_spherical - camera_position_spherical1).magnitude > (camera_position_spherical - camera_position_spherical2).magnitude)
            camera_position_spherical_goal = camera_position_spherical2;
        else
            camera_position_spherical_goal = camera_position_spherical1;

        // Apply the transformation matrix to the world coordinate system
        Vector3 transformedPosition = spherical2World * new Vector4(camera_position_spherical_goal.x, camera_position_spherical_goal.y, camera_position_spherical_goal.z, 1);

        // Define orientation vector
        orientation = mid_point - transformedPosition;
        orientation.Normalize();

        // Update the line renderer to visualize the orientation vector
        lineRenderer.SetPosition(0, goalPosition);
        lineRenderer.SetPosition(1, goalPosition + orientation * 5f);

        // Calculate the camera's new forward direction (z-axis)
        Vector3 newUp = orientation;

        // Calculate the camera's new up direction (y-axis) aligned with world's z-axis
        //Vector3 newUp = Vector3.Cross(newForward, Vector3.right).normalized;
        //Vector3 newForward = Vector3.Cross(newUp, -leftEndEffectorLink.transform.right).normalized;
        Vector3 newForward = Vector3.Cross(newUp, Vector3.up).normalized;

        // Compute the new rotation quaternion
        Quaternion transformedRotation = Quaternion.LookRotation(newForward, newUp);
        
        // Out
        goalPosition = transformedPosition;
        goalRotation = transformedRotation;

        DebugSphere.transform.position = goalPosition;
        DebugSphere.transform.rotation = goalRotation;

        //Debug.Log("Pos: " + goalPosition + "Rot: " + goalRotation);
        //Debug.Log("Roatation: " + transformedRotation.ToString());



        /*
         */

        // Debug.Log(leftEndEffectorLink.GetComponent<ArticulationBody>().transform.position);
        //(Vector3 endEffectorPosition, Quaternion endEffectorRotation) = kinematicSolver.GetPose(kinematicSolver.numJoint);
        //Debug.Log("EE pos " + endEffectorPosition);

        // Debug.Log("Visible? " + rightEndEffectorLink.GetComponent<Renderer>().isVisible);
        // targetLocalizer.IsVisible(manipulatorPosition) && targetLocalizer.IsVisible(targetPosition);

        // Display current pose
        // KEY currentPosition = leftEndEffectorLink.transform.position;
        // KEY currentRotation = leftEndEffectorLink.transform.rotation;

        // DEBUGGING - See target IK location in space
        // DebugSphere.transform.position = desiredPosition;
        // DebugSphere.transform.rotation = rotation;

        // Desired Pose
        // KEY desiredPosition = currentPosition + targetLocalizer.camera_tf.TransformDirection(offsetVector);
        // Debug.Log("current " + currentPosition + "desired " + desiredPosition);
        // KEY rotation = currentRotation;
        // rotation = currentRotation * offsetRotation;

        // Debug.Log(position);
        // Debug.Log("my pos " + DebugSphere.transform.position);
        // DebugSphere.transform.position = desiredPosition;
        // 
    }

}