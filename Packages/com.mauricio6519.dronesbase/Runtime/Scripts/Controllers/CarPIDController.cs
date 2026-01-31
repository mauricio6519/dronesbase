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

// Class to handle PID controlled car dynamics
public class CarPIDController : MonoBehaviour
{

    // -----------------------------------------------------------------------------------------------------
    // Public variables that appear in the Inspector:
    
    // Target object variables
    [Header("Target Position")]
    public Vector3 targetPosition;

    // Margin of error to consider the target reached
    [Header("Target Error Margin")]
    public float targetErrorMargin = 0.5f;
    
    // PID settings for each control axis, may need to be optimized
    [Header("Throttle PID")]
    public PIDSettings throttleSettings = new PIDSettings { Kp = 0.5f, Ki = 0.05f, Kd = 0.2f };

    [Header("Steer PID")]
    public PIDSettings steerSettings = new PIDSettings { Kp = 1.0f, Ki = 0.1f, Kd = 0.5f };

    // -----------------------------------------------------------------------------------------------------
    // Private variables of the class:

    // Required components
    private CarDynamics carDynamics;
    private bool brakingState = false;



    public float distanceToStartBraking = 5f;
    

    public float brakeDistance = 2f; // Distance to start braking
    public float stopDistance = 0.5f; // Distance to stop completely
    public float minSpeedToBrake = 0.1f; // Minimum speed to consider braking
    public float deadzoneForSteeringControl = 10f; // Degrees of deadzone for steering control


    private float maxSpeed;
    private float maxTorque;
    private float maxSteering;

    
    
    public float minSpeedToConsiderStopped = 0.1f;
    
    public float yawErrorToPreventOscillations = 10f;
    
    public float exponentialToSmoothSteering = 1.5f;

    public float progressiveSteeringInTime = 20.0f;


    
    public GameObject targetObj;
    private Rigidbody rb;


    // PID control objects
    private PID throttlePID;
    private PID steerPID;

    // Variables to calculate the error in the PID control
    private float currentSpeed;
    private float speedError;
    private float yawError;
    private float currentDistance;
    private Vector3 directionToTarget;

    // -----------------------------------------------------------------------------------------------------
    // Start is called before the first frame update:

    void Start()
    {
        // Get the Rigidbody component of the car
        rb = GetComponent<Rigidbody>();

        // Get the CarDynamics component
        carDynamics = GetComponent<CarDynamics>();
        
        // Initialize the PID control objects
        throttlePID = new PID();
        steerPID = new PID();

        // Initialize target position
        targetPosition = transform.position;

        // Get car features
        maxSpeed = carDynamics.maxSpeedManufacturer;
        maxTorque = carDynamics.motorTorque;
        maxSteering = carDynamics.maxSteerAngle;
    }

    // -----------------------------------------------------------------------------------------------------
    // FixedUpdate is called at a fixed interval:

    void FixedUpdate()
    {
        // If carDynamics is not assigned, exit
        if(carDynamics == null) return;

        // Calculate direction to target
        directionToTarget = (targetPosition - transform.position).normalized;
        
        // Apply PID controls
        ApplyThrottlePIDControl();
        ApplySteeringPIDControl();
    }

    // -----------------------------------------------------------------------------------------------------
    // Methods to apply PID control for throttle:

    void ApplyThrottlePIDControl()
    {
        // Calculate distance to target only in XZ plane
        Vector3 currentPos = transform.position;
        Vector3 targetPos = targetPosition;
        currentPos.y = 0;
        targetPos.y = 0;
        currentDistance = Vector3.Distance(currentPos, targetPos);

        // Calculate current speed in XZ plane
        Vector3 velocityXZ = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        currentSpeed = velocityXZ.magnitude;

        // Determine if we need to move forward or backward
        float dotProduct = Vector3.Dot(transform.forward, directionToTarget);
        bool shouldMoveForward = dotProduct > 0;

        // Apply brake if very close to target
        if (currentDistance < stopDistance) brakingState = true;

        // If in braking state, apply full brake
        if (brakingState)
        {
            // Cut throttle
            carDynamics.throttle = 0f;

            // Apply full brake
            carDynamics.isBraking = true;

            // Exit braking state if speed is very low
            if (currentSpeed < minSpeedToBrake) 
            {
                brakingState = false; // Exit braking state
                throttlePID.Reset(); // Reset PID controller
            }

            // No further processing needed
            return;
        }

        // Calculate target speed based on distance and time
        float targetSpeed;
        if (currentDistance > distanceToStartBraking)
        {
            targetSpeed = maxSpeed;
        }
        else // Within braking distance, reduce target speed proportionally
        {
            targetSpeed = Mathf.Lerp(0f, maxSpeed, currentDistance / distanceToStartBraking);
        }

        // Calculate speed error
        speedError = targetSpeed - currentSpeed;

        // Calculate throttle using PID control
        float throttle = throttlePID.Compute(speedError, throttleSettings.Kp, throttleSettings.Ki, throttleSettings.Kd);

        // Calculate maximum allowed torque based on distance
        float maxAllowedTorque;
        if (currentDistance > distanceToStartBraking)
        {
            maxAllowedTorque = maxTorque; // When far away, use maximum torque
        }
        else // When close, reduce torque proportionally
        {
            maxAllowedTorque = Mathf.Lerp(0f, maxTorque, currentDistance / distanceToStartBraking);
        }

        // Apply torque limited by distance and direction
        float torque = Mathf.Clamp(throttle, -1f, 1f) * maxAllowedTorque / maxTorque;

        // Invert torque if we need to move backward
        if (!shouldMoveForward) torque = -torque;

        // Apply torque to car controller
        carDynamics.throttle = torque;

        // Apply brake if close to target and speed is higher than desired
        if (currentDistance < brakeDistance && currentSpeed > targetSpeed)
        {
            carDynamics.isBraking = true;
        }
        else // Release brake
        {
            carDynamics.isBraking = false;
        }        

        // Reset PID if error is small
        if (Mathf.Abs(speedError) < targetErrorMargin) throttlePID.Reset();
    }

    // -----------------------------------------------------------------------------------------------------
    // Methods to apply PID control for steering:

    void ApplySteeringPIDControl()
    {
        // Calculate direction to target in local space
        Vector3 localDirectionToTarget = transform.InverseTransformPoint(targetPosition);

        // Calculate target angle in degrees
        float targetAngle = Mathf.Atan2(localDirectionToTarget.x, localDirectionToTarget.z) * Mathf.Rad2Deg;

        // Calculate yaw error
        yawError = targetAngle;
        if (yawError > 180) yawError -= 360;
        if (yawError < -180) yawError += 360;

        // Check if vehicle is stopped using a small velocity threshold
        bool isStopped = rb.velocity.magnitude < minSpeedToConsiderStopped;
        bool isBraking = carDynamics.currentBrakeForce > 0f;

        // If braking, keep wheels straight
        if (isBraking)
        {
            carDynamics.steering = 0f;
            steerPID.Reset();
            return;
        }

        // Ignore small yaw errors to prevent oscillations
        if(Mathf.Abs(yawError) < yawErrorToPreventOscillations || Mathf.Abs(yawError) > (180 - yawErrorToPreventOscillations))
        {
            return;
        }

        // Check if yaw error is outside deadzone
        bool outsideDeadzone = Mathf.Abs(yawError) > deadzoneForSteeringControl;

        // If within deadzone and not stopped, reset steering and PID
        if (!outsideDeadzone)
        {
            carDynamics.steering = 0f;
            if (!isStopped) steerPID.Reset();
            return;
        }

        // If stopped and outside deadzone, keep wheels straight
        if (isStopped && !outsideDeadzone)
        {
            carDynamics.steering = 0f;
            return;
        }

        // If outside deadzone, compute steering using PID
        float steer = steerPID.Compute(yawError, steerSettings.Kp, steerSettings.Ki, steerSettings.Kd);
        
        // Calculate steering factor based on yaw error
        float steeringFactor = Mathf.Clamp01((Mathf.Abs(yawError) - deadzoneForSteeringControl) / (maxSteering - deadzoneForSteeringControl));
        
        // Smooth steering input
        float smoothedSteer = Mathf.Sign(steer) * Mathf.Pow(Mathf.Abs(steer), exponentialToSmoothSteering) * steeringFactor;
        
        // Calculate target steer angle
        float targetSteerAngle = Mathf.Clamp(smoothedSteer, -1f, 1f);
        
        // Apply steering to car controller
        carDynamics.steering = Mathf.Lerp(carDynamics.steering, targetSteerAngle, Time.fixedDeltaTime * progressiveSteeringInTime);
    
        // Reset PID if error is small
        if (Mathf.Abs(yawError) < targetErrorMargin) steerPID.Reset();
    }

}
