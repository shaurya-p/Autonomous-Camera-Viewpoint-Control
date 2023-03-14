using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Provide util functions to get
///     parameters for optimization-IK
///     of Kinova Gen3 7-DOF robotic arm
/// </summary>

public class OptimizationParams : MonoBehaviour
{
    
    public int updateRate = 10;

    public GameObject cameraToolFrame;
    public GameObject manipulatorToolFrame;
    public GameObject debugObj;
    public GameObject debugObj2;

    private float startTime;
    private float deltaTime;
    
    private Transform camera_tf;
    private Transform manipulator_tf;
    
    private Vector3 position_m;
    private Vector3 predictedPos_m;
    private Vector3 velocity_m;
    private Vector3 direction_c;
    private Vector3 position_c;
    
    void Start()    
    {
        startTime = Time.time;

        // Get EE transform
        camera_tf = cameraToolFrame.transform;
        manipulator_tf = manipulatorToolFrame.transform;

        deltaTime = 1f / updateRate;
        InvokeRepeating("ScalarObjective1", 1f, deltaTime);
    
    }

    void Update() {}

    private float ScalarObjective1()
    {
        // Predict manipulator EE position 300 ms in the future
        velocity_m = (manipulator_tf.position - position_m) / deltaTime;
        predictedPos_m = position_m + 0.3f * velocity_m;

        // Update current manipulator position
        position_m = manipulator_tf.position;

        // Camera arm position and viewing direction
        // [0, 1, 0] is the viewing direction unit vector w.r.t. local camera frame
        direction_c = camera_tf.TransformDirection(new Vector3(0f, 1f, 0f));    
        position_c = camera_tf.position;

        // Calculate shortest normal vector from predictedPosition to viewing direction 
        float shortestDistance = Vector3.Cross(direction_c.normalized, predictedPos_m - position_c).magnitude;

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

}