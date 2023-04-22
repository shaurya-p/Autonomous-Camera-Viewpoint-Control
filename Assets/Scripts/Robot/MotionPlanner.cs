using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionPlanner : MonoBehaviour
{
    // DEBUGGING
    public GameObject DebugSphere;
    
    // End Effector State
    public GameObject leftEndEffectorLink;
    public Vector3 currentPosition;
    public Quaternion currentRotation;

    // Desired pose
    public Vector3 position;                // World Coordinates
    public Quaternion rotation;             // World Coordinates
    public Vector3 offsetPosition;          // World Coordinates
    public Quaternion offsetRotation;          // World Coordinates


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Display current pose
        currentPosition = leftEndEffectorLink.transform.position;
        currentRotation = leftEndEffectorLink.transform.rotation;

        // DEBUGGING - See target IK location in space
        DebugSphere.transform.position = position;
        DebugSphere.transform.rotation = rotation;

        // Desired Pose
        position = currentPosition + offsetPosition;
        rotation = currentRotation * offsetRotation;
    }
}
