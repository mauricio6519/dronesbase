/*******************************************************************************
* Copyright 2025 INTRIG
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*     http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
******************************************************************************/

// Libraries
using UnityEngine; // Unity Engine library to use in MonoBehaviour classes

// Main class to control the drone's movements:
public class DroneDynamics : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Input values for the drone's movements
    [Header("Input Values")]
    [Range(0f, 1f)] public float throttle = 0f; // Throttle value between 0 and 1
    [Range(-1f, 1f)] public float pitch = 0f; // Pitch value between -1 and 1
    [Range(-1f, 1f)] public float roll = 0f; // Roll value between -1 and 1
    [Range(-1f, 1f)] public float yaw = 0f; // Yaw value between -1 and 1

    // Drone variables
    [Header("Drone Features")]
    public float unladenWeight = 10; // Unladen weight of the drone in kg
    public float maxThrust = 196; // Maximum thrust of the drone. It is configured with twice the weight due to gravity.
    public float thrustUsedDuringPitchRoll = 0.2f; // Thrust percentage used during pitch and roll movements
    public float thrustUsedDuringYaw = 0.02f; // Thrust percentage used during yaw movements
    public float maximumTiltAngle = 15; // Maximum tilt angle of the drone
    public float maxSpeedManufacturer = 50; // Maximum speed of the drone set by the manufacturer
    public float minimumThrottleToStop = 0.1f; // Minimum throttle to apply forces to the drone or stop it
    
    // Aerodynamics coefficients of the drone
    [Header("Rigidbody Aerodynamics Coefficients")]
    public float randomDragMin = 0.8f; // Minimum Drag coefficient of the drone
    public float randomDragMax = 1.3f; // Maximum Drag coefficient of the drone
    public float randomAngularDragMin = 1.0f; // Minimum Angular Drag coefficient of the drone
    public float randomAngularDragMax = 2.0f; // Maximum Angular Drag coefficient of the drone

    // Forces applied to the drone
    [Header("Total Force Applied")]
    public Vector3 totalForce; // Total force applied to the drone. Public for using it in other classes
    public float velocityMagnitude; // Magnitude of the drone's velocity

    // Collision detection flag
    [Header("Collision Detection")]
    public bool isColliding = false; // Flag to indicate if the drone is colliding with an object
    
    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:
    
    // Variables for the drone's torques
    private Vector3 totalTorque;
    private Vector3 yawTorque;

    // Rigidbody component of the drone
    private Rigidbody rb;

    // Variables for the drone's forces and torques
    private Vector3 upwardForce;
    private Vector3 forwardForce;
    private Vector3 lateralForce;
    private Vector3 forwardTorque;
    private Vector3 lateralTorque;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Get the Rigidbody component of the drone
        rb = GetComponent<Rigidbody>();

        // Set the center of mass of the drone
        rb.centerOfMass = new Vector3(0, 0, 0);

        // Set the mass and drag properties of the drone
        rb.mass = unladenWeight;
        rb.drag = Random.Range(randomDragMin, randomDragMax); // Random drag between min and max for aerodynamics
        rb.angularDrag = Random.Range(randomAngularDragMin, randomAngularDragMax); // Random angular drag between min and max for aerodynamics
        rb.useGravity = true;
    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at fixed time intervals, ideal for physical changes in objects:

    void FixedUpdate()
    {
        // Apply the forces to the motors of the drone
        ApplyForcesTorques();
        
        // Limit the speed of the drone
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeedManufacturer);
        velocityMagnitude = rb.velocity.magnitude;
    }

    // -----------------------------------------------------------------------------------------------------
    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider:

    void OnCollisionStay(Collision collision)
    {
        // Set the collision flag to true
        isColliding = true;

        // If the drone collides with the Ground or DronePads, stop the drone's movements:
        if (collision.gameObject.CompareTag("DronePad") || collision.gameObject.CompareTag("Ground"))
        {
            pitch = 0f; // Stop the pitch movement
            roll = 0f; // Stop the roll movement
            yaw = 0f; // Stop the yaw movement
        }
    }

    // -----------------------------------------------------------------------------------------------------
    // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigid

    void OnCollisionExit(Collision collision)
    {
        // Set the collision flag to false
        isColliding = false;
    }
    
    // -----------------------------------------------------------------------------------------------------
    // Method to apply the forces to the motors of the drone:
    
    void ApplyForcesTorques()
    {
        // Input scaling
        float throttleScaled = Mathf.Clamp01(throttle); // between 0 and 1
        float pitchScaled = Mathf.Clamp(pitch, -1f, 1f); // between -maximumTiltAngle and maximumTiltAngle
        float rollScaled  = Mathf.Clamp(roll, -1f, 1f); // between -maximumTiltAngle and maximumTiltAngle
        float yawScaled   = Mathf.Clamp(yaw, -1f, 1f); // between -maximumTiltAngle and maximumTiltAngle

        // Forces calculation
        upwardForce = Vector3.up * throttleScaled * maxThrust;
        forwardForce = transform.forward * pitchScaled * maxThrust * thrustUsedDuringPitchRoll;
        lateralForce = transform.right * rollScaled * maxThrust * thrustUsedDuringPitchRoll;
        
        // Total force application
        totalForce = upwardForce + forwardForce + lateralForce;
        if(totalForce.magnitude > maxThrust) totalForce = totalForce.normalized * maxThrust;
        if(throttleScaled < minimumThrottleToStop) totalForce = Vector3.zero;

        // Apply forces to the Rigidbody
        rb.AddForce(totalForce, ForceMode.Force);

        // Torques calculation
        forwardTorque = Vector3.right * pitchScaled;
        lateralTorque = - Vector3.forward * rollScaled;
        totalTorque = forwardTorque + lateralTorque;
        
        // Apply torques to the Rigidbody 
        rb.AddTorque(totalTorque);

        // Yaw torque calculation
        yawTorque = Vector3.up * yawScaled * maxThrust * thrustUsedDuringYaw;

        // Apply yaw torque to the Rigidbody
        rb.AddTorque(yawTorque);
    }

}
