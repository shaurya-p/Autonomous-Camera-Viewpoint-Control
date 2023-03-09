using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Provide util functions to get
///     parameters for optimization-IK
///     of Kinova Gen3 7-DOF robotic arm
/// </summary>

public class OptParameters : MonoBehaviour
{
    
    public int updateRate = 10;
    public GameObject cameraToolFrame;
    public GameObject manipulatorToolFrame;
    private float startTime;
    private float deltaTime;
    private Transform camera_tf;
    private Transform manipulator_tf;
    private Vector3 position_m;
    private Vector3 predictedPos_m;
    private Vector3 velocity_m;
    private Vector3 direction_c;
    private Vector3 position_c;
    private Ray ray_c;
    
    void Start()    
    {
        startTime = Time.time;

        // Get EE transform
        camera_tf = cameraToolFrame.transform;
        manipulator_tf = manipulatorToolFrame.transform;

        deltaTime = 1f / updateRate;
        InvokeRepeating("Objective1", 1f, deltaTime);
    
    }

    void Update() {}

    private float Objective1()
    {
        // Predict manipulator EE position 300 ms in the future
        velocity_m = (manipulator_tf.position - position_m) / deltaTime;
        predictedPos_m = position_m + 0.3f * velocity_m;

        // Update current manipulator position
        position_m = manipulator_tf.position;

        direction_c = camera_tf.TransformDirection(new Vector3(0f, 1f, 0f));
        position_c = camera_tf.position;
        
        //Ray ray_c = new Ray(position_c, direction_c);
        //float distance = Vector3.Cross(ray_c.direction, predictedPoint - viewpoint_ray.origin).magnitude;
        
        float distance = Vector3.Cross(direction_c, predictedPos_m - position_c).magnitude;
        
        return distance;
    }

}