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

    // Debugging
    public GameObject debugObj;
    public GameObject debugObj2;
    
    // Timer
    private float deltaTime;
    public int updateRate = 10;

    // Manipulator states
    public GameObject manipulator_GameObj;
    private Transform manipulator_tf;
    private Vector3 manipulator_pos;
    private Vector3 manipulator_vel;

    // Target variables
    private Vector3 predictedPos_m;

    // Camera states
    public GameObject camera_GameObj;
    private Transform camera_tf;
    private Vector3 camera_dir;
    private Vector3 camera_pos;
    
    void Start()    
    {

        // Get transform of objects
        camera_tf = camera_GameObj.transform;
        manipulator_tf = manipulator_GameObj.transform;

        deltaTime = 1f / updateRate;
        InvokeRepeating("ScalarObjective1", 1f, deltaTime);
    
    }

    void Update() {}

    private float ScalarObjective1()
    {
        // Predict manipulator EE position 300 ms in the future
        manipulator_vel = (manipulator_tf.position - manipulator_pos) / deltaTime;
        predictedPos_m = manipulator_pos + 0.3f * manipulator_vel;

        // Update current manipulator position
        manipulator_pos = manipulator_tf.position;

        // Camera arm position and viewing direction
        // [0, 1, 0] is the viewing direction unit vector w.r.t. local camera frame
        camera_dir = camera_tf.TransformDirection(new Vector3(0f, 1f, 0f));    
        camera_pos = camera_tf.position;

        // Calculate shortest normal vector from predictedPosition to viewing direction 
        float shortestDistance = Vector3.Cross(camera_dir.normalized, predictedPos_m - camera_pos).magnitude;

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