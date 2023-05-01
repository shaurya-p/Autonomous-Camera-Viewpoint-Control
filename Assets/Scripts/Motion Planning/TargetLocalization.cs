using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetLocalization : MonoBehaviour
{
    // Debugging
    private GameObject debugObj;
    private GameObject debugObj2;

    // Screen visibility
    private int majorRadius;

    // Targets
    public GameObject cube;
    public GameObject cylinder;
    public GameObject ellipsoid;

    // Assignables
    public int updateRate = 10;
    public Camera cam;
    public GameObject camera_GameObj;
    public GameObject manipulator_GameObj;

    // Timer
    private float deltaTime;
    private float lookAheadTime = 0.3f;      // how much time forward we want to predict

    // Manipulator states
    private Transform manipulator_tf;
    private Vector3 manipulator_pos;
    private Vector3 manipulator_vel;
    
    // Camera states    
    public Transform camera_tf;
    private Vector3 camera_dir;
    private Vector3 camera_pos;
    
    // Target variables
    private Vector3 target_pos;

    // Start is called before the first frame update
    void Start()
    {
        // Get transform of objects
        camera_tf = camera_GameObj.transform;
        manipulator_tf = manipulator_GameObj.transform;

        deltaTime = 1f / updateRate;
        //InvokeRepeating("GetDirectionError", 1f, deltaTime);
        // Set major viewing radius
        majorRadius = cam.pixelHeight / 2 - 20;
    }

    void Update()
    {
        //DEBUG
    }


    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(cam.transform.position, majorRadius);
    }*/

    private float GetDirectionError()
    {
        // Set target_pos variable
        // target_pos = FindTarget();
        
        // Get camera arm position and viewing direction
        // [0, 1, 0] is the viewing direction unit vector w.r.t. local camera frame
        camera_dir = camera_tf.TransformDirection(new Vector3(0f, 1f, 0f));
        camera_pos = camera_tf.position;

        // Calculate shortest normal vector from target to viewing direction vector
        float shortestDistance = Vector3.Cross(camera_dir.normalized, target_pos - camera_pos).magnitude;

        return shortestDistance;

        // Debugging
        // Create a small 3D sphere and set its transform.position = predictedPos_m;
        // float intersectionDistance = Vector3.Dot(direction_c.normalized, predictedPos_m - position_c);
        // Vector3 intersectionPoint = position_c + direction_c.normalized * intersectionDistance;
        // Viz the different vectors
        // Camera view direction
        // Debug.DrawRay(position_c, direction_c * 10f, Color.green, 1f);
        // Camera to Manipulator vector
        // Debug.DrawRay(position_c, predictedPos_m - position_c, Color.cyan, 1f);
        // Shortest normal vector from manipulator predicted position to camera viewing vector
        // Debug.DrawRay(predictedPos_m, intersection - predictedPos_m, Color.yellow, 1f);

    }
    
    private Vector3 MakePrediction(string mode = "")
    {
        // Predict target position by extrapolating manipulator e.e. position 300 ms in the future
        manipulator_vel = (manipulator_tf.position - manipulator_pos) / deltaTime;
        Vector3 prediction = manipulator_pos + lookAheadTime * manipulator_vel;

        if (mode == "screen")
            prediction = cam.WorldToScreenPoint(prediction);

        // Update current manipulator e.e. position
        manipulator_pos = manipulator_tf.position;

        return prediction;
    }

    public bool IsVisible(Vector3 point)
    {
        // Checks if a world point projected onto camera space
        // is within the major viewing circle.
        // The major viewing circle has a radius of (cam.pixelHeight / 2 - threshold)

        Vector3 screenPoint = cam.WorldToViewportPoint(point);
        bool inCameraView = IsWithin01(screenPoint.x) && IsWithin01(screenPoint.y);
        bool inFront = screenPoint.z > 0;

        RaycastHit depthCheck;
        bool blocked = false;

        Vector3 dir2point = point - cam.transform.position;

        float distance = Vector3.Distance(cam.transform.position, point);

        if (Physics.Raycast(cam.transform.position, dir2point, out depthCheck, distance + 0.05f))
            if (depthCheck.transform.position != point)
                blocked = true;
        
        // Debug.Log("inCam " + inCameraView + "inFront " + inFront + "viz " + !blocked );
        return inCameraView && inFront && !blocked;

    }

    public bool IsWithin01(float val)
    {
        return val > 0 && val < 1;
    }
}