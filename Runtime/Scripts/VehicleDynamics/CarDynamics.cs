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
using UnityEngine; // Library to use in MonoBehaviour classes

// Class to manage the car dynamics such as motor force, brake force, steering angle, and battery consumption
public class CarDynamics : MonoBehaviour
{
    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:

    // Input values for the car's movements
    [Header("Input Values")]
    [Range(-1f, 1f)] public float throttle;
    [Range(-1f, 1f)] public float steering;
    public bool isBraking;

    // Car features
    [Header("Car Features")]
    public float motorTorque = 1500f;
    public float brakeForce = 2000f;
    public float maxSteerAngle = 30f;
    public float unladenWeight = 1500f; // Unladen weight of the car in kg
    public float maxSpeedManufacturer = 50f; // Maximum speed of the car set by the manufacturer
    
    // Aerodynamics coefficients of the car
    [Header("Rigidbody Aerodynamics Coefficients")]
    public float randomDragMin = 0.0f; // Minimum Drag coefficient of the car
    public float randomDragMax = 0.1f; // Maximum Drag coefficient of the car
    public float randomAngularDragMin = 0.0f; // Minimum Angular Drag coefficient of the car
    public float randomAngularDragMax = 0.1f; // Maximum Angular Drag coefficient of the car

    // Forces applied to the car and its current velocity
    [Header("Current velocity and applied forces/torques")]
    public float velocityMagnitude; // Magnitude of the car's velocity
    public float currentMotorTorque;
    public float currentBrakeForce;
    public float currentSteerAngle;

    // Collision detection flag
    [Header("Collision Detection")]
    public bool isColliding = false; // Flag to indicate if the car is colliding with an object
    
    // Wheel Colliders and Transforms
    [Header("Wheel Colliders and Transforms")]
    public WheelCollider wheelFrontLeft;
    public WheelCollider wheelFrontRight;
    public WheelCollider wheelBackLeft;
    public WheelCollider wheelBackRight;
    public Transform frontLeftWheelTransform;
    public Transform frontRightWheelTransform;
    public Transform backLeftWheelTransform;
    public Transform backRightWheelTransform;

    // Steering settings based on speed
    [Header("Steering settings")]
    public AnimationCurve steerBySpeed = AnimationCurve.Linear(0f, 1f, 50f, 0.3f);

    // -----------------------------------------------------------------------------------------------------
    // Private variables of this class:

    // Rigidbody component of the car
    private Rigidbody rb;

    // Allowed steering angle after considering speed-based limitations
    private float allowedSteering;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Get the Rigidbody component of the car
        rb = GetComponent<Rigidbody>();

        // Set the center of mass of the car
        rb.centerOfMass = new Vector3(0, -0.1f, 0);

        // Set the mass and drag properties of the car
        rb.mass = unladenWeight;
        rb.drag = Random.Range(randomDragMin, randomDragMax); // Random drag between min and max for aerodynamics
        rb.angularDrag = Random.Range(randomAngularDragMin, randomAngularDragMax); // Random angular drag between min and max for aerodynamics
        rb.useGravity = true;
    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at a fixed interval:

    void FixedUpdate()
    {
        // Get the forces/torques of the car
        GetForcesTorques();

        // Apply the motor force, steering angle, and brake force
        ApplyHandleMotor();
        ApplyHandleSteering();

        // Update the wheel visuals based on the WheelCollider's position and rotation
        UpdateWheelVisuals(wheelFrontLeft, frontLeftWheelTransform);
        UpdateWheelVisuals(wheelFrontRight, frontRightWheelTransform);
        UpdateWheelVisuals(wheelBackLeft, backLeftWheelTransform);
        UpdateWheelVisuals(wheelBackRight, backRightWheelTransform);

        // Limit the speed of the car
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeedManufacturer);
        velocityMagnitude = rb.velocity.magnitude;
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to get the forces and torques based on input values:

    void GetForcesTorques()
    {
        // Calculate the allowed steering angle based on the current speed
        float maxSteerAllowed = steerBySpeed.Evaluate(rb.velocity.magnitude);

        // Update the allowed steering angle
        allowedSteering = steering * maxSteerAllowed;

        // Apply the motor torque, steering angle, and brake force based on input values
        currentMotorTorque = throttle * motorTorque;
        currentSteerAngle = allowedSteering * maxSteerAngle;
        currentBrakeForce = isBraking ? brakeForce : 0f;
    }

    // -----------------------------------------------------------------------------------------------------
    // Methods to handle the force applied to the wheels:

    void ApplyHandleMotor()
    {
        // Apply the motor torque to the back wheels
        wheelBackLeft.motorTorque = currentMotorTorque;
        wheelBackRight.motorTorque = currentMotorTorque;

        // Apply the brake force to all wheels
        wheelBackLeft.brakeTorque = currentBrakeForce;
        wheelBackRight.brakeTorque = currentBrakeForce;
        wheelFrontLeft.brakeTorque = currentBrakeForce;
        wheelFrontRight.brakeTorque = currentBrakeForce;
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to handle steering angle for the front wheels:

    void ApplyHandleSteering()
    {
        // Apply the steering angle to the front wheels
        wheelFrontLeft.steerAngle = currentSteerAngle;
        wheelFrontRight.steerAngle = currentSteerAngle;
    }

    // -----------------------------------------------------------------------------------------------------
    // Method to update the wheel visuals based on the WheelCollider's position and rotation:

    void UpdateWheelVisuals(WheelCollider collider, Transform wheelTransform)
    {
        // Get the position and rotation of the WheelCollider and apply it to the wheel transform
        Vector3 pos;
        Quaternion rot;
        collider.GetWorldPose(out pos, out rot);
        wheelTransform.position = pos;
        wheelTransform.rotation = rot;
    }

    // -----------------------------------------------------------------------------------------------------
    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider:

    void OnCollisionStay(Collision collision)
    {
        // Set the collision flag to true
        isColliding = true;
    }

    // -----------------------------------------------------------------------------------------------------
    // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigid

    void OnCollisionExit(Collision collision)
    {
        // Set the collision flag to false
        isColliding = false;
    }

}
